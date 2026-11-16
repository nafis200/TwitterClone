# TwitterClone — সম্পূর্ণ প্রজেক্ট গাইড (বাংলা)

এই ফাইলে তিনটি জিনিস আছে:

1. **এই প্রজেক্ট কীভাবে কাজ করে** — একটা request API-তে ঢোকা থেকে শুরু করে ডাটাবেজ (EF Core + PostgreSQL) হয়ে response ফিরে যাওয়া পর্যন্ত, ধাপে ধাপে।
2. **Program.cs লাইন ধরে ব্যাখ্যা।**
3. **নতুন প্রজেক্ট একদম শুরু থেকে বানানোর রেসিপি** — Auth (Register/Login/JWT), Authorization, আর GET/POST/PUT/DELETE route সহ।

> টেকনিক্যাল শব্দগুলো (Controller, DbContext, Token...) ইংরেজিতেই রাখা হয়েছে, কারণ কোডে এগুলোই লিখতে হবে।

---

## সূচিপত্র

- [পর্ব ১: বড় ছবি — লেয়ার ও ফোল্ডার](#পর্ব-১-বড়-ছবি--লেয়ার-ও-ফোল্ডার)
- [পর্ব ২: Program.cs বোঝা](#পর্ব-২-programcs-বোঝা)
- [পর্ব ৩: EF Core — ডাটাবেজের সাথে কথা বলা](#পর্ব-৩-ef-core--ডাটাবেজের-সাথে-কথা-বলা)
- [পর্ব ৪: Request ১ — Register (পুরো চক্র)](#পর্ব-৪-request-১--register-পুরো-চক্র)
- [পর্ব ৫: Request ২ — Login ও Token পাঠানো](#পর্ব-৫-request-২--login-ও-token-পাঠানো)
- [পর্ব ৬: Request ৩ — লগইন করা ইউজারের যেকোনো request (Tweet POST/GET/PUT/DELETE)](#পর্ব-৬-request-৩--লগইন-করা-ইউজারের-যেকোনো-request)
- [পর্ব ৭: Authentication বনাম Authorization](#পর্ব-৭-authentication-বনাম-authorization)
- [পর্ব ৮: Refresh Token ও Logout (ঐচ্ছিক)](#পর্ব-৮-refresh-token-ও-logout-ঐচ্ছিক)
- [পর্ব ৯: নতুন প্রজেক্ট শুরু থেকে — ধাপে ধাপে](#পর্ব-৯-নতুন-প্রজেক্ট-শুরু-থেকে--ধাপে-ধাপে)
- [পর্ব ১০: নতুন ফিচার যোগ করার চেকলিস্ট](#পর্ব-১০-নতুন-ফিচার-যোগ-করার-চেকলিস্ট)
- [পর্ব ১১: সাধারণ ভুল ও সমাধান](#পর্ব-১১-সাধারণ-ভুল-ও-সমাধান)
- [পর্ব ১২: এই প্রজেক্টের বর্তমান অবস্থা ও পরের কাজ](#পর্ব-১২-এই-প্রজেক্টের-বর্তমান-অবস্থা-ও-পরের-কাজ)

---

## পর্ব ১: বড় ছবি — লেয়ার ও ফোল্ডার

প্রজেক্টটা **Clean Architecture**-এ ভাগ করা। প্রতিটা লেয়ারের একটাই দায়িত্ব:

```
        Client (Browser / Postman / Swagger / .http file)
                         │  HTTP request (JSON)
                         ▼
┌──────────────────────────────────────────────────────────┐
│ TwitterClone.Api            ← দরজা                       │
│   Program.cs, Controllers/, Auth/ClaimsPrincipalExt...   │
└───────────────┬──────────────────────────────────────────┘
                │ Service ডাকে (interface দিয়ে)
                ▼
┌──────────────────────────────────────────────────────────┐
│ TwitterClone.Application    ← মাথা (business logic)      │
│   Services/, Dtos/, Interfaces/, Mappings/               │
└───────────────┬──────────────────────────────────────────┘
                │ Repository interface ডাকে
                ▼
┌──────────────────────────────────────────────────────────┐
│ TwitterClone.Infrastructure ← হাত-পা (বাইরের দুনিয়া)     │
│   Persistence/ (DbContext, Configurations, Migrations)   │
│   Repositories/, Auth/ (JWT, PasswordHasher)             │
└───────────────┬──────────────────────────────────────────┘
                │ EF Core → SQL
                ▼
          PostgreSQL ডাটাবেজ

TwitterClone.Domain  ← হৃদয়: Entities/ (User, Tweet, RefreshToken ...)
                       সবাই Domain ব্যবহার করে, Domain কাউকে চেনে না।
```

| লেয়ার | কী থাকে | কেন আলাদা |
|---|---|---|
| **Domain** | Entity class (`User`, `Tweet`) — ডাটাবেজের এক একটা টেবিলের রূপ | কোনো framework-এর উপর নির্ভর করে না |
| **Application** | DTO, Interface, Service (নিয়মকানুন: "একই email দুইবার হবে না") | ডাটাবেজ কোনটা জানে না, শুধু interface জানে |
| **Infrastructure** | EF Core DbContext, Repository, JWT বানানো, Password hash | Application-এর interface-গুলো এখানে বাস্তবায়ন হয় |
| **Api** | Controller (route), Program.cs (সব জোড়া লাগানো) | শুধু HTTP-এর কাজ: request নেওয়া, response দেওয়া |

**Dependency নিয়ম:** Api → Application → Domain, আর Infrastructure → Application + Domain।
Application কখনো Infrastructure-কে সরাসরি চেনে না — সে `IUserRepository` চেনে, আর Program.cs (DI) বলে দেয় ওটার আসল class `UserRepository`।

### তিন ধরনের "মডেল" — গুলিয়ে ফেলবে না

| নাম | উদাহরণ | কাজ |
|---|---|---|
| **Entity** | `Domain/Entities/User.cs` | ডাটাবেজের টেবিলের সারি। `PasswordHash` এখানে থাকে। |
| **DTO** (Data Transfer Object) | `Application/Dtos/CreateUserDto.cs`, `UserDto.cs` | Client-এর সাথে আদান-প্রদানের আকার। `UserDto`-তে password নেই — তাই কখনো লিক হয় না। |
| **Mapping** | `Application/Mappings/MappingExtensions.cs` | `user.ToDto()` — Entity থেকে DTO বানায়। |

---

## পর্ব ২: Program.cs বোঝা

ফাইল: [TwitterClone.Api/Program.cs](TwitterClone.Api/Program.cs)

Program.cs হলো অ্যাপের **শুরুর জায়গা**। এর দুইটা ভাগ:

```
var builder = WebApplication.CreateBuilder(args);   ← appsettings.json, environment পড়ে
   ভাগ ১: builder.Services.Add...   ← "কী কী জিনিস লাগবে" (DI container-এ রেজিস্টার)
var app = builder.Build();
   ভাগ ২: app.Use...                ← "প্রতিটা request কোন পথে যাবে" (middleware pipeline)
app.Run();                           ← সার্ভার চালু, request-এর অপেক্ষা
```

### DI (Dependency Injection) কী?

ধরো `AuthController`-এর `IAuthService` লাগবে। আমরা নিজে `new AuthService(...)` লিখি না। Program.cs-এ একবার বলে দিই:

```csharp
services.AddScoped<IAuthService, AuthService>();   // "IAuthService চাইলে AuthService দিও"
```

তারপর controller-এর constructor-এ `IAuthService` চাইলেই ASP.NET নিজে বানিয়ে দেয় — এবং `AuthService`-এর যা যা লাগে (repository, hasher...) সেগুলোও নিজে বানিয়ে দেয়।

**Lifetime তিন রকম:**

| Lifetime | মানে | এই প্রজেক্টে |
|---|---|---|
| `AddSingleton` | পুরো অ্যাপে একটাই object | `JwtSettings` |
| `AddScoped` | **প্রতি request-এ একটা** নতুন object, request শেষে নষ্ট | সব Service, Repository, `DbContext` |
| `AddTransient` | যতবার চাইবে ততবার নতুন | ব্যবহার হয়নি |

### ভাগ ১: Services (ধাপ ১–৫)

| ধাপ | কোড | কী করে |
|---|---|---|
| ১ | `AddControllers()` | `Controllers/` ফোল্ডারের সব controller খুঁজে বের করে |
| ২ | `AddSwaggerGen(...)` | `/swagger` পেজ বানায়; "Authorize" বাটন যোগ করে যাতে token বসিয়ে টেস্ট করা যায় |
| ৩ | `AddApplication()` | [Application/DependencyInjection.cs](TwitterClone.Application/DependencyInjection.cs) — সব Service রেজিস্টার |
| ৩ | `AddInfrastructure(config)` | [Infrastructure/DependencyInjection.cs](TwitterClone.Infrastructure/DependencyInjection.cs) — `DbContext` (PostgreSQL connection string দিয়ে), সব Repository, `JwtSettings`, `JwtTokenService`, `PasswordHasher` |
| ৪ | `AddAuthentication().AddJwtBearer(...)` | বলে দেয়: "request-এর `Authorization: Bearer xxx` header থেকে JWT পড়ো এবং এই Key/Issuer/Audience দিয়ে যাচাই করো" |
| ৫ | `AddAuthorization()` | `[Authorize]` attribute কাজ করার জন্য |

ধাপ ৪-এর `TokenValidationParameters` মানে:

```csharp
ValidateIssuer = true,  ValidIssuer = "TwitterClone"        // token কে বানিয়েছে? আমরা কিনা
ValidateAudience = true, ValidAudience = "TwitterClone.Api" // token কার জন্য? আমাদের API-র জন্য কিনা
ValidateIssuerSigningKey = true, IssuerSigningKey = Key     // signature আমাদের গোপন Key দিয়ে কিনা (কেউ বদলায়নি তো?)
ValidateLifetime = true, ClockSkew = TimeSpan.Zero          // মেয়াদ (exp) শেষ কিনা, একদম ঠিক সময়ে
MapInboundClaims = false                                     // "sub", "email" নাম যেমন আছে তেমন রাখো
```

এই একই মানগুলো দিয়েই [JwtTokenService.cs](TwitterClone.Infrastructure/Auth/JwtTokenService.cs) token **বানায়**। বানানো আর যাচাই — দুই জায়গায় একই `appsettings` → `JwtSettings` ব্যবহার হয়।

### ভাগ ২: Middleware pipeline (ধাপ ৬–৯)

প্রতিটা request **উপর থেকে নিচে** এই ধাপগুলো পার হয়, response উল্টো পথে ফেরে:

```
Request ──► [Swagger (শুধু Dev)] ──► [HttpsRedirection] ──► [Authentication] ──► [Authorization] ──► [Controller]
Response ◄──────────────────────────────────────────────────────────────────────────────────────────────┘
```

| ধাপ | কোড | কী করে |
|---|---|---|
| ৬ | `UseSwagger()`, `UseSwaggerUI()` | Development-এ `/swagger` পেজ |
| ৭ | `Database.MigrateAsync()` | অ্যাপ চালু হওয়ার সময় বাকি থাকা migration ডাটাবেজে চালায় (`appsettings.Development.json` → `Database:ApplyMigrationsOnStartup`) |
| — | `UseHttpsRedirection()` | http → https এ পাঠায় |
| ৮ | `UseAuthentication()` | **"তুমি কে?"** — token থাকলে যাচাই করে `HttpContext.User` ভরে দেয়। token না থাকলে বা ভুল হলে User = anonymous (এখানেই error দেয় না) |
| ৮ | `UseAuthorization()` | **"তোমার ঢোকার অনুমতি আছে?"** — action-এ `[Authorize]` থাকলে আর User anonymous হলে → **401** |
| ৯ | `MapControllers()` | URL + HTTP method দেখে সঠিক controller method-এ পাঠায় |

> ⚠️ `UseAuthentication()` সবসময় `UseAuthorization()`-এর **আগে**। উল্টো করলে সবাই 401 পাবে।

---

## পর্ব ৩: EF Core — ডাটাবেজের সাথে কথা বলা

EF Core হলো **ORM** — C# class ↔ ডাটাবেজ টেবিল অনুবাদক। তুমি C# লেখো, EF Core SQL বানিয়ে PostgreSQL-এ পাঠায়।

### মূল অংশগুলো

| অংশ | ফাইল | কাজ |
|---|---|---|
| **Entity** | `Domain/Entities/Tweet.cs` | একটা class = একটা টেবিল, একটা property = একটা column |
| **DbContext** | [Persistence/TwitterCloneDbContext.cs](TwitterClone.Infrastructure/Persistence/TwitterCloneDbContext.cs) | ডাটাবেজের সাথে একটা "সেশন"। প্রতি request-এ একটা (Scoped) |
| **DbSet** | `public DbSet<Tweet> Tweets { get; set; }` | একটা টেবিল। এর উপর LINQ লিখলে SQL হয় |
| **Configuration** | [Persistence/Configurations/TweetConfiguration.cs](TwitterClone.Infrastructure/Persistence/Configurations/TweetConfiguration.cs) | টেবিলের নিয়ম: primary key, max length, unique index, foreign key |
| **Migration** | `Persistence/Migrations/` | Entity বদলালে ডাটাবেজ বদলানোর C# স্ক্রিপ্ট (CREATE TABLE, ADD COLUMN...) |
| **Repository** | [Repositories/TweetRepository.cs](TwitterClone.Infrastructure/Repositories/TweetRepository.cs) | DbContext ব্যবহার করে CRUD; Service শুধু এটাকেই ডাকে |

`OnModelCreating`-এর `ApplyConfigurationsFromAssembly(...)` লাইনটা `Configurations/` ফোল্ডারের সব `IEntityTypeConfiguration<T>` নিজে খুঁজে লাগিয়ে দেয়।

### C# → SQL (Change Tracking)

EF Core প্রতিটা object-এর **state** মনে রাখে। `SaveChanges()` ডাকলে তবেই SQL যায়:

| C# কোড | State | `SaveChanges()`-এ যে SQL যায় |
|---|---|---|
| `_context.Tweets.Add(tweet)` | Added | `INSERT INTO "Tweets" (...) VALUES (...)` |
| `_context.Tweets.Find(id)` | (সাথে সাথে চলে) | `SELECT ... FROM "Tweets" WHERE "Id" = @id LIMIT 1` |
| `_context.Tweets.Where(t => t.UserId == x).ToList()` | (`.ToList()`-এ চলে) | `SELECT ... WHERE "UserId" = @x` |
| `tweet.Content = "new"` + `Update(tweet)` | Modified | `UPDATE "Tweets" SET "Content" = ... WHERE "Id" = ...` |
| `_context.Tweets.Remove(tweet)` | Deleted | `DELETE FROM "Tweets" WHERE "Id" = ...` |

> মনে রাখো: `Add`/`Remove`/`Update` শুধু **লিস্ট বানায়**; `SaveChanges()` না ডাকলে ডাটাবেজে কিছুই যায় না।
> `Where(...)` লিখলেও query চলে না — `.ToList()`, `.FirstOrDefault()`, `.Find()` ইত্যাদিতে চলে।

### Migration কমান্ড (প্রজেক্ট root থেকে)

```bash
# Entity/Configuration বদলানোর পর নতুন migration বানাও
dotnet ef migrations add AddTweets \
  --project TwitterClone.Infrastructure \
  --startup-project TwitterClone.Api \
  --output-dir Persistence/Migrations

# ডাটাবেজে apply করো (এই প্রজেক্টে অ্যাপ চালু হলেও নিজে apply হয় — Program.cs ধাপ ৭)
dotnet ef database update --project TwitterClone.Infrastructure --startup-project TwitterClone.Api

# শেষ migration ভুল হলে (এখনো apply না করে থাকলে) মুছে ফেলো
dotnet ef migrations remove --project TwitterClone.Infrastructure --startup-project TwitterClone.Api
```

`--project` = যেখানে DbContext আছে, `--startup-project` = যেখানে `appsettings` (connection string) আছে।
ডাটাবেজের `__EFMigrationsHistory` টেবিলে লেখা থাকে কোন কোন migration চলেছে।

---

## পর্ব ৪: Request ১ — Register (পুরো চক্র)

**Client পাঠায়:**

```http
POST http://localhost:5185/api/auth/register
Content-Type: application/json

{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@example.com",
  "password": "password123"
}
```

### ধাপে ধাপে কী ঘটে

```
Client ─► Kestrel ─► Middleware ─► AuthController.Register ─► AuthService.Register
                                                                  │
                                         ┌────────────────────────┤
                                         ▼                        ▼
                                UserService.CreateUser      IssueTokens()
                                  │   │                       │       │
                    UserRepository   PasswordHasher   JwtTokenService  RefreshTokenRepository
                         │                                                    │
                    EF Core ──► INSERT "Users"                    EF Core ──► INSERT "RefreshTokens"
                                                                  
◄── 201 Created + JSON { accessToken, refreshToken, user } ◄──────────────────┘
```

**ধাপ ১ — সার্ভারে পৌঁছানো।** Kestrel (ASP.NET-এর web server) request নেয় এবং একটা `HttpContext` বানায় (request + response + User সব এর ভেতরে)। এই request-এর জন্য একটা নতুন **DI scope** খোলে।

**ধাপ ২ — Middleware।**
- `UseAuthentication()`: `Authorization` header নেই → User = anonymous। কোনো error না।
- `UseAuthorization()`: `Register` method-এ `[Authorize]` নেই → ঢুকতে দাও।
- `MapControllers()`: `POST` + `/api/auth/register` → `AuthController.Register()`।

**ধাপ ৩ — Controller বানানো (DI)।** ASP.NET `AuthController` বানাতে গিয়ে দেখে এর `IAuthService` আর `IUserService` লাগবে → `AuthService` বানায় → তার লাগে `IUserRepository`, `IRefreshTokenRepository`, `IPasswordHasher`, `ITokenService` → Repository-গুলোর লাগে `TwitterCloneDbContext` → এই request-এর জন্য **একটাই** DbContext বানিয়ে সবাইকে দেয়।

**ধাপ ৪ — Model binding + Validation।** JSON body → `CreateUserDto` object। তারপর `[ApiController]` attribute নিজে থেকে [CreateUserDto.cs](TwitterClone.Application/Dtos/CreateUserDto.cs)-এর নিয়ম চেক করে:
`[Required]`, `[EmailAddress]`, `[StringLength(100, MinimumLength = 8)]`।
কোনোটা ভাঙলে method-এ ঢোকেই না → সরাসরি **400 Bad Request** (কোন field-এ কী ভুল লেখা থাকে)।

**ধাপ ৫ — Controller → Service।** [AuthController.cs](TwitterClone.Api/Controllers/AuthController.cs):
```csharp
var authResponse = _authService.Register(createUserDto);
```

**ধাপ ৬ — Email আগে আছে কিনা।** [UserService.CreateUser](TwitterClone.Application/Services/UserService.cs) → `_userRepository.GetUserByEmail(email)` → EF Core:
```sql
SELECT ... FROM "Users" WHERE lower("Email") = 'ada@example.com' LIMIT 1;
```
পাওয়া গেলে `null` ফেরে → Controller **409 Conflict** দেয়।

**ধাপ ৭ — Password hash।** `_passwordHasher.Hash("password123")` → [PasswordHasher.cs](TwitterClone.Infrastructure/Auth/PasswordHasher.cs) (ASP.NET Identity-র PBKDF2 + random salt) → `"AQAAAAIAAYagAAAAE..."`।
আসল password **কখনো** ডাটাবেজে যায় না। Hash একমুখী — উল্টো করে password বের করা যায় না।

**ধাপ ৮ — User save।** `new User { FirstName, LastName, Email (lowercase), PasswordHash }` → `UserRepository.AddUser` → `_context.Users.Add(user)` + `SaveChanges()`:
```sql
INSERT INTO "Users" ("Id","FirstName","LastName","Email","PasswordHash","CreatedAt",...) VALUES (...);
```

**ধাপ ৯ — Token বানানো** (`AuthService.IssueTokens`):
1. **Access token (JWT)** — [JwtTokenService.GetToken](TwitterClone.Infrastructure/Auth/JwtTokenService.cs): user-এর তথ্য দিয়ে **claims** বানায় —
   `sub` = user Id, `email`, `given_name`, `family_name`, `jti` (token-এর নিজস্ব id)। সাথে `iss`, `aud`, `exp` (এখন + 20 মিনিট)। তারপর `appsettings`-এর গোপন `Key` দিয়ে **HMAC-SHA256 signature** দেয়।
2. **Refresh token** — 64 byte random string (JWT না)। এর **SHA-256 hash** ডাটাবেজে রাখা হয়:
   ```sql
   INSERT INTO "RefreshTokens" ("Id","UserId","TokenHash","ExpiresAt",...) VALUES (...);
   ```

**ধাপ ১০ — Response।** Controller `Created(...)` → JSON (camelCase) → **201 Created**:
```json
{
  "tokenType": "Bearer",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "accessTokenExpiresAt": "2026-11-17T10:20:00Z",
  "refreshToken": "q8Xw...random...",
  "refreshTokenExpiresAt": "2026-11-24T10:00:00Z",
  "user": { "id": "ea1d...", "firstName": "Ada", "lastName": "Lovelace", "email": "ada@example.com" }
}
```

**ধাপ ১১ — পরিষ্কার।** Request শেষ → DI scope বন্ধ → DbContext সহ সব Scoped object নষ্ট (database connection pool-এ ফেরত যায়)।

---

## পর্ব ৫: Request ২ — Login ও Token পাঠানো

```http
POST http://localhost:5185/api/auth/login
Content-Type: application/json

{ "email": "ada@example.com", "password": "password123" }
```

ধাপ ১–৪ Register-এর মতোই (anonymous, validation `LoginDto`-তে)। তারপর [AuthService.Login](TwitterClone.Application/Services/AuthService.cs):

1. `GetUserByEmail(email)` → `SELECT ... FROM "Users" WHERE lower("Email") = ...`
2. User না পেলে **অথবা** `_passwordHasher.Verify(password, user.PasswordHash)` false হলে → `null` → **401 Unauthorized** `"Invalid email or password."`
   (কোনটা ভুল — email না password — বলি না, যাতে কেউ বুঝতে না পারে কোন email রেজিস্টার্ড।)
3. ঠিক থাকলে → `IssueTokens(user)` → Register-এর মতো একই JSON, **200 OK**।

### Token কীভাবে পাঠাতে হয়

Login-এর পর client (frontend/মোবাইল অ্যাপ) `accessToken` সংরক্ষণ করে রাখে। পরের **প্রতিটা** protected request-এ header-এ পাঠায়:

```http
GET http://localhost:5185/api/auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

- `Bearer` শব্দ + একটা space + token। এভাবেই লিখতে হবে।
- **Swagger-এ:** উপরে ডানে **Authorize** বাটন → শুধু token পেস্ট করো (`Bearer` লিখতে হবে না) → Authorize।
- **VS Code `.http` ফাইলে:** [TwitterClone.Api.http](TwitterClone.Api/TwitterClone.Api.http)-এ `@accessToken = ` এর পরে পেস্ট করো।

### JWT দেখতে কেমন?

তিন অংশ, ডট দিয়ে আলাদা: `header.payload.signature`

```
eyJhbGciOiJIUzI1NiJ9 . eyJzdWIiOiJlYTFkLi4uIiwiZW1haWwiOi... . rV6CCJmV02NAH5M4hft...
   header (algorithm)        payload (claims) — Base64, encrypted না!         signature
```

- Payload যে কেউ পড়তে পারে (jwt.io-তে পেস্ট করে দেখো)। তাই **password বা গোপন তথ্য কখনো claim-এ রাখবে না**।
- কিন্তু কেউ payload বদলালে (যেমন `sub` অন্য কারো Id করে দিলে) signature আর মিলবে না → সার্ভার reject করবে। কারণ সঠিক signature বানাতে গোপন `Key` লাগে, যা শুধু সার্ভারের কাছে আছে।
- সার্ভার token ডাটাবেজে খোঁজে না — শুধু signature আর মেয়াদ চেক করে। তাই JWT যাচাই খুব দ্রুত।

---

## পর্ব ৬: Request ৩ — লগইন করা ইউজারের যেকোনো request

উদাহরণ: **নতুন tweet বানানো।** [TweetsController.cs](TwitterClone.Api/Controllers/TweetsController.cs)

```http
POST http://localhost:5185/api/tweets
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{ "content": "My first tweet!" }
```

লক্ষ্য করো: body-তে `userId` **নেই**। লেখক কে, সেটা token থেকে নেওয়া হয় — নাহলে যে কেউ অন্যের নামে tweet করতে পারত।

### ধাপে ধাপে

**ধাপ ১ — Authentication middleware (JwtBearer handler):**
1. `Authorization` header পড়ে, `Bearer ` বাদ দিয়ে token নেয়।
2. Signature যাচাই (`Key` দিয়ে) → মিলল না? → User anonymous।
3. `iss` = "TwitterClone"? `aud` = "TwitterClone.Api"? `exp` পার হয়নি? → কোনোটা ভুল হলে User anonymous।
4. সব ঠিক → claims দিয়ে `ClaimsPrincipal` বানিয়ে `HttpContext.User`-এ রাখে। এখন `User.Identity.IsAuthenticated == true`।

**ধাপ ২ — Authorization middleware:** `TweetsController`-এর উপরে `[Authorize]` আছে।
- User anonymous → এখানেই থামে → **401 Unauthorized** (controller-এ ঢোকেই না)।
- User authenticated → এগিয়ে যাও।

**ধাপ ৩ — Validation:** `CreateTweetDto` → `[Required]` + `TweetContentValidator` (খালি না, 200 অক্ষরের বেশি না)। ভুল হলে **400**।

**ধাপ ৪ — Controller:**
```csharp
var createdTweet = _tweetService.CreateTweet(User.GetUserId(), createTweetDto);
```
`User.GetUserId()` → [ClaimsPrincipalExtensions.cs](TwitterClone.Api/Auth/ClaimsPrincipalExtensions.cs) → token-এর `sub` claim পড়ে `Guid` বানায়।

**ধাপ ৫ — Service:** [TweetService.CreateTweet](TwitterClone.Application/Services/TweetService.cs)
- `_userRepository.GetUserById(userId)` → `SELECT ... FROM "Users" WHERE "Id" = ...` (account মুছে গেলে null → 400)
- `new Tweet(user.Id, content.Trim())`

**ধাপ ৬ — Repository + EF Core:** `TweetRepository.AddTweet` →
```sql
INSERT INTO "Tweets" ("Id","UserId","Content","CreatedAt",...) VALUES (...);
```

**ধাপ ৭ — Response:** `tweet.ToDto()` → `CreatedAtAction(...)` → **201 Created**, সাথে `Location: /api/tweets/{id}` header:
```json
{ "id": "e38a...", "userId": "567a...", "content": "My first tweet!", "createdAt": "...", "modifiedAt": null }
```

### বাকি তিনটা method — কী আলাদা

| Method | Route | বিশেষ কী | ফলাফল |
|---|---|---|---|
| **GET** | `/api/tweets` বা `/api/tweets?userId=...` | শুধু পড়া: `SELECT`। নতুন থেকে পুরনো সাজানো | 200 + list |
| **GET** | `/api/tweets/{id}` | `Find(id)` | 200, না পেলে 404 |
| **PUT** | `/api/tweets/{id}` + body `{ "content": "..." }` | **মালিক কিনা চেক** → তারপর `UPDATE` | 200 / 403 / 404 |
| **DELETE** | `/api/tweets/{id}` | **মালিক কিনা চেক** → তারপর `DELETE` | 204 / 403 / 404 |

PUT/DELETE-এর **মালিকানা চেক** (এটাই আসল *authorization*):

```csharp
var tweet = _tweetService.GetTweetById(id);
if (tweet == null) return NotFound();                    // 404: tweet নেই
if (tweet.UserId != User.GetUserId()) return Forbid();   // 403: লগইন আছে, কিন্তু tweet তোমার না
_tweetService.DeleteTweet(id);
return NoContent();                                      // 204: মুছে ফেলা হয়েছে
```

### Status code সারাংশ

| Code | কখন |
|---|---|
| 200 OK | সফল (data সহ) |
| 201 Created | নতুন কিছু তৈরি হয়েছে |
| 204 No Content | সফল, ফেরত দেওয়ার কিছু নেই (DELETE) |
| 400 Bad Request | ইনপুট ভুল (validation) |
| 401 Unauthorized | token নেই / ভুল / মেয়াদ শেষ — **"তুমি কে জানি না"** |
| 403 Forbidden | token ঠিক, কিন্তু অনুমতি নেই — **"তোমাকে চিনি, কিন্তু এটা তোমার না"** |
| 404 Not Found | জিনিসটা নেই |
| 409 Conflict | ইতিমধ্যে আছে (একই email) |

---

## পর্ব ৭: Authentication বনাম Authorization

| | Authentication | Authorization |
|---|---|---|
| প্রশ্ন | **তুমি কে?** | **তুমি কী করতে পারো?** |
| কোথায় | `UseAuthentication()` + JwtBearer | `UseAuthorization()` + `[Authorize]`, আর controller-এর ভেতরের চেক |
| ব্যর্থ হলে | 401 | 401 (লগইন নেই) বা 403 (অনুমতি নেই) |

### এই প্রজেক্টে route-গুলো কীভাবে protect করা

| Attribute | কোথায় | মানে |
|---|---|---|
| (কিছু নেই) | `AuthController`-এর register, login, refresh | সবাই ঢুকতে পারে |
| `[Authorize]` method-এ | `AuthController`-এর me, logout, change-password | শুধু ওই method-এ token লাগবে |
| `[Authorize]` class-এ | `TweetsController`, `UsersController`, `LikesController`... | ওই controller-এর সব method-এ token লাগবে |
| `[AllowAnonymous]` | `UsersController.GetUsers` | class-এ `[Authorize]` থাকলেও এই method খোলা |
| `if (... != User.GetUserId()) return Forbid();` | Users-এর PUT/PATCH/DELETE, Tweets-এর PUT/DELETE | শুধু নিজের জিনিস বদলাতে পারবে |

> পরে চাইলে **Role** যোগ করতে পারো: token-এ `new Claim("role", "Admin")` দাও, তারপর `[Authorize(Roles = "Admin")]`।

---

## পর্ব ৮: Refresh Token ও Logout (ঐচ্ছিক)

Access token 20 মিনিটে মেয়াদ শেষ। প্রতি 20 মিনিটে আবার password চাওয়া বিরক্তিকর, তাই **refresh token** (7 দিন):

```http
POST /api/auth/refresh
{ "refreshToken": "q8Xw..." }
```

1. পাঠানো token-এর SHA-256 hash বানাও → `RefreshTokens` টেবিলে খোঁজো।
2. না পেলে, বা revoked/expired হলে → 401 (আবার login করতে হবে)।
3. পেলে → নতুন access + নতুন refresh token দাও, **পুরনোটা revoke** করো (একটা refresh token একবারই চলে)।

**Logout** (`POST /api/auth/logout` + header-এ access token + body-তে refresh token): ওই refresh token revoke করে।
**Change password**: password বদলায়, সব পুরনো refresh token revoke করে (সব ডিভাইস থেকে logout), নতুন token দেয়।

> **নতুন প্রজেক্টে পরামর্শ:** প্রথমে শুধু Register + Login + JWT বানাও। ওটা পুরোপুরি বুঝলে তারপর refresh token যোগ করো।

---

## পর্ব ৯: নতুন প্রজেক্ট শুরু থেকে — ধাপে ধাপে

নিচে `MyApp` নাম ব্যবহার করা হয়েছে — নিজের নাম বসাও। প্রতিটা ধাপের পূর্ণ কোড এই প্রজেক্টের সংশ্লিষ্ট ফাইলে দেখে নিতে পারো।

### ধাপ ০ — যা লাগবে

```bash
dotnet --version                         # .NET 8 SDK
dotnet tool install --global dotnet-ef --version 8.*   # EF Core CLI (একবারই)
# PostgreSQL (লোকাল বা Neon-এর মতো cloud)
```

### ধাপ ১ — Solution ও চারটা প্রজেক্ট

```bash
mkdir MyApp && cd MyApp
dotnet new sln -n MyApp
dotnet new classlib -n MyApp.Domain         -f net8.0
dotnet new classlib -n MyApp.Application    -f net8.0
dotnet new classlib -n MyApp.Infrastructure -f net8.0
dotnet new webapi   -n MyApp.Api            -f net8.0 --use-controllers
dotnet sln add MyApp.Domain MyApp.Application MyApp.Infrastructure MyApp.Api
```

### ধাপ ২ — কে কাকে চিনবে (reference)

```bash
dotnet add MyApp.Application    reference MyApp.Domain
dotnet add MyApp.Infrastructure reference MyApp.Application MyApp.Domain
dotnet add MyApp.Api            reference MyApp.Application MyApp.Infrastructure MyApp.Domain
```

### ধাপ ৩ — NuGet package

```bash
# Application (DI extension method লেখার জন্য)
dotnet add MyApp.Application package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2

# Infrastructure
dotnet add MyApp.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.2
dotnet add MyApp.Infrastructure package Microsoft.Extensions.Identity.Core --version 8.0.*        # PasswordHasher
dotnet add MyApp.Infrastructure package Microsoft.IdentityModel.JsonWebTokens --version 8.*      # JWT বানানো
dotnet add MyApp.Infrastructure package Microsoft.Extensions.Configuration.Binder --version 8.0.2

# Api
dotnet add MyApp.Api package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.*
dotnet add MyApp.Api package Microsoft.EntityFrameworkCore.Design --version 8.0.2              # migration-এর জন্য
dotnet add MyApp.Api package Swashbuckle.AspNetCore --version 6.6.2
```

### ধাপ ৪ — Domain: Entity

`MyApp.Domain/Entities/BaseEntity.cs`, `User.cs`, `Tweet.cs` — দেখো [BaseEntity.cs](TwitterClone.Domain/Entities/BaseEntity.cs)। সহজ রূপ:

```csharp
public class User : BaseEntity
{
    public User() : base(Guid.NewGuid()) { }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public class Tweet : BaseEntity
{
    private Tweet() : base(Guid.NewGuid()) { }          // EF Core ডাটাবেজ থেকে পড়ার সময় এটা ব্যবহার করে
    public Tweet(Guid userId, string content) : base(Guid.NewGuid())
    {
        UserId = userId;
        Content = content;
    }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
```

### ধাপ ৫ — Application: DTO, Interface, Service

**DTO** (validation attribute সহ):
```csharp
public class CreateUserDto
{
    [Required, StringLength(50)] public required string FirstName { get; set; }
    [Required, StringLength(50)] public required string LastName { get; set; }
    [Required, EmailAddress]     public required string Email { get; set; }
    [Required, StringLength(100, MinimumLength = 8)] public required string Password { get; set; }
}
public class LoginDto { [Required, EmailAddress] public required string Email { get; set; }
                        [Required] public required string Password { get; set; } }
public class CreateTweetDto { [Required, StringLength(200)] public required string Content { get; set; } }
// UserDto, TweetDto, AuthResponseDto { AccessToken, AccessTokenExpiresAt, User }
```

**Interface** (Application শুধু এগুলো চেনে; বাস্তবায়ন Infrastructure-এ):
```csharp
public interface IUserRepository  { User AddUser(User u); User? GetUserById(Guid id); User? GetUserByEmail(string email); ... }
public interface ITweetRepository { Tweet AddTweet(Tweet t); Tweet? GetTweetById(Guid id); List<Tweet> GetTweets(); Tweet UpdateTweet(Tweet t); bool DeleteTweet(Tweet t); }
public interface IPasswordHasher  { string Hash(string password); bool Verify(string password, string passwordHash); }
public interface ITokenService    { (string Token, DateTime ExpiresAt) GetToken(UserDto user); }
public interface IAuthService     { AuthResponseDto? Register(CreateUserDto dto); AuthResponseDto? Login(LoginDto dto); }
public interface ITweetService    { TweetDto? CreateTweet(Guid userId, CreateTweetDto dto); ... }
```

**Service** (নিয়মকানুন):
```csharp
public AuthResponseDto? Register(CreateUserDto dto)
{
    if (_userRepository.GetUserByEmail(dto.Email) != null) return null;   // email আগেই আছে

    var user = _userRepository.AddUser(new User
    {
        FirstName = dto.FirstName.Trim(),
        LastName  = dto.LastName.Trim(),
        Email     = dto.Email.Trim().ToLowerInvariant(),
        PasswordHash = _passwordHasher.Hash(dto.Password)
    });
    return CreateResponse(user.ToDto());
}

public AuthResponseDto? Login(LoginDto dto)
{
    var user = _userRepository.GetUserByEmail(dto.Email);
    if (user is null || !_passwordHasher.Verify(dto.Password, user.PasswordHash)) return null;
    return CreateResponse(user.ToDto());
}

private AuthResponseDto CreateResponse(UserDto user)
{
    var (token, expiresAt) = _tokenService.GetToken(user);
    return new AuthResponseDto { AccessToken = token, AccessTokenExpiresAt = expiresAt, User = user };
}
```

**DI** — `MyApp.Application/DependencyInjection.cs`:
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<ITweetService, TweetService>();
    return services;
}
```

### ধাপ ৬ — Infrastructure: EF Core, Repository, JWT, Hasher

**DbContext:**
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Tweet> Tweets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
```

**Configuration** — দেখো [UserConfiguration.cs](TwitterClone.Infrastructure/Persistence/Configurations/UserConfiguration.cs) (email unique) ও [TweetConfiguration.cs](TwitterClone.Infrastructure/Persistence/Configurations/TweetConfiguration.cs) (foreign key)।

**Repository** — দেখো [TweetRepository.cs](TwitterClone.Infrastructure/Repositories/TweetRepository.cs)। নিয়ম: `Add/Update/Remove` → তারপর `SaveChanges()`।

**PasswordHasher** — হুবহু [PasswordHasher.cs](TwitterClone.Infrastructure/Auth/PasswordHasher.cs)।

**JwtSettings + JwtTokenService** — হুবহু [JwtSettings.cs](TwitterClone.Infrastructure/Auth/JwtSettings.cs) ও [JwtTokenService.cs](TwitterClone.Infrastructure/Auth/JwtTokenService.cs) (`GetToken` method টাই মূল)। মূল অংশ:
```csharp
var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),   // পরে User.GetUserId() এটাই পড়বে
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
};
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
var descriptor = new SecurityTokenDescriptor
{
    Issuer = _settings.Issuer, Audience = _settings.Audience,
    Subject = new ClaimsIdentity(claims),
    Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryTimeInMinutes),
    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
};
return (new JsonWebTokenHandler().CreateToken(descriptor), descriptor.Expires!.Value);
```

**DI** — `MyApp.Infrastructure/DependencyInjection.cs`:
```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<AppDbContext>(o => o.UseNpgsql(config.GetConnectionString("DefaultConnection")));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<ITweetRepository, TweetRepository>();

    var jwt = config.GetSection("JwtSettings").Get<JwtSettings>()!;
    services.AddSingleton(jwt);
    services.AddScoped<ITokenService, JwtTokenService>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    return services;
}
```

### ধাপ ৭ — Api: appsettings, Program.cs, Controller

**`appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=myapp;Username=postgres;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "Issuer": "MyApp",
    "Audience": "MyApp.Api",
    "Key": "কমপক্ষে-৩২-অক্ষরের-লম্বা-গোপন-random-string-এখানে",
    "ExpiryTimeInMinutes": 20
  },
  "Database": { "ApplyMigrationsOnStartup": true }
}
```
Key বানাতে: `openssl rand -base64 48`

**Program.cs** — হুবহু এই প্রজেক্টের [Program.cs](TwitterClone.Api/Program.cs) কপি করো (namespace বদলে)। ক্রম মনে রাখো:
`AddControllers` → `AddSwaggerGen` → `AddApplication` → `AddInfrastructure` → `AddAuthentication().AddJwtBearer` → `AddAuthorization` → `Build` → `UseAuthentication` → `UseAuthorization` → `MapControllers` → `Run`।

**`Auth/ClaimsPrincipalExtensions.cs`** — হুবহু [এটা](TwitterClone.Api/Auth/ClaimsPrincipalExtensions.cs)।

**AuthController** — `register` ও `login` ([AuthController.cs](TwitterClone.Api/Controllers/AuthController.cs)), আর একটা `[Authorize]` দেওয়া `me` endpoint যাতে token কাজ করছে কিনা টেস্ট করা যায়।

**TweetsController** — হুবহু [TweetsController.cs](TwitterClone.Api/Controllers/TweetsController.cs): class-এ `[Authorize]`, POST-এ `User.GetUserId()`, PUT/DELETE-এ মালিকানা চেক।

### ধাপ ৮ — Migration ও ডাটাবেজ

```bash
dotnet ef migrations add InitialCreate --project MyApp.Infrastructure --startup-project MyApp.Api --output-dir Persistence/Migrations
dotnet ef database update            --project MyApp.Infrastructure --startup-project MyApp.Api
```

### ধাপ ৯ — চালাও ও টেস্ট করো

```bash
dotnet build
dotnet run --project MyApp.Api
```
ব্রাউজারে `http://localhost:<port>/swagger` খোলো (port টার্মিনালে দেখাবে)। এই ক্রমে টেস্ট করো:

1. `POST /api/auth/register` → 201, `accessToken` কপি করো
2. `GET /api/auth/me` token **ছাড়া** → 401 ✅ (protection কাজ করছে)
3. **Authorize** বাটনে token বসাও → `GET /api/auth/me` → 200 ✅
4. `POST /api/tweets` → 201, `id` কপি করো
5. `GET /api/tweets` → list-এ tweet আছে ✅ (ডাটাবেজে save হয়েছে)
6. দ্বিতীয় user register করো, তার token দিয়ে প্রথম user-এর tweet `PUT`/`DELETE` → 403 ✅
7. নিজের token দিয়ে `DELETE` → 204, আবার `GET /api/tweets/{id}` → 404 ✅

---

## পর্ব ১০: নতুন ফিচার যোগ করার চেকলিস্ট

যেকোনো নতুন জিনিস (যেমন **Like**) যোগ করতে **ভেতর থেকে বাইরে** এই ক্রমে যাও:

| # | লেয়ার | কাজ |
|---|---|---|
| 1 | Domain | `Entities/Like.cs` (`UserId`, `TweetId`) |
| 2 | Infrastructure | `Configurations/LikeConfiguration.cs` (key, foreign key, unique index `(UserId, TweetId)`) |
| 3 | Infrastructure | DbContext-এ `public DbSet<Like> Likes { get; set; }` |
| 4 | Application | `Dtos/CreateLikeDto.cs`, `LikeDto.cs` + `MappingExtensions`-এ `ToDto()` |
| 5 | Application | `Interfaces/ILikeRepository.cs`, `ILikeService.cs` |
| 6 | Infrastructure | `Repositories/LikeRepository.cs` (DbContext দিয়ে) |
| 7 | Application | `Services/LikeService.cs` (নিয়ম: একই tweet দুইবার like না) |
| 8 | DI | `AddScoped<ILikeRepository, LikeRepository>()` ও `AddScoped<ILikeService, LikeService>()` |
| 9 | Api | `Controllers/LikesController.cs` — `[Authorize]`, userId সবসময় `User.GetUserId()` থেকে |
| 10 | EF | `dotnet ef migrations add AddLikes ...` |
| 11 | Test | `.http` ফাইলে request লিখে টেস্ট |

---

## পর্ব ১১: সাধারণ ভুল ও সমাধান

| সমস্যা | কারণ | সমাধান |
|---|---|---|
| সবসময় 401, token দেওয়ার পরও | `Bearer ` লেখা নেই / token মেয়াদ শেষ / `UseAuthentication` নেই বা `UseAuthorization`-এর পরে | header ঠিক করো, নতুন login করো, middleware-এর ক্রম দেখো |
| 401, কিন্তু token নতুন | বানানো আর যাচাইয়ের Issuer/Audience/Key আলাদা | দুই জায়গাতেই একই `JwtSettings` ব্যবহার করো |
| `User.GetUserId()` exception | `MapInboundClaims = false` নেই, তাই `sub` অন্য নামে বদলে গেছে | Program.cs-এ `options.MapInboundClaims = false` |
| `relation "Tweets" does not exist` | migration বানানো/চালানো হয়নি, বা migration-এর পরে build হয়নি | `dotnet ef migrations add ...` → `dotnet build` → `database update` |
| ডাটা পরের request-এ হারিয়ে যায় | Repository `List<>`-এ রাখে আর `AddScoped` — প্রতি request-এ নতুন খালি list | EF Core DbContext দিয়ে ডাটাবেজে রাখো |
| `Unable to resolve service for type 'IXxx'` | DI-তে রেজিস্টার করতে ভুলে গেছো | `DependencyInjection.cs`-এ `AddScoped<IXxx, Xxx>()` |
| `JwtSettings:Key must be at least 32 bytes` | Key ছোট | ৩২+ অক্ষরের random Key দাও |
| 400 কিন্তু কোডে কোনো ভুল নেই | DTO validation ব্যর্থ | response body-র `errors` দেখো |

---

## পর্ব ১২: এই প্রজেক্টের বর্তমান অবস্থা ও পরের কাজ

**ডাটাবেজে (EF Core) আছে:** `Users`, `RefreshTokens`, `Tweets`।

**এখনো memory-তে (`List<>`) আছে:** Likes, Bookmarks, Follows, Messages, Notifications, Retweets। এই repository-গুলো `AddScoped`, তাই প্রতি request-এ নতুন খালি list তৈরি হয় — **ডাটা টিকে থাকে না**। অনুশীলন হিসেবে প্রতিটাকে Tweet-এর মতো EF Core-এ সরাও (পর্ব ১০-এর চেকলিস্ট ধরে)। [TweetRepository.cs](TwitterClone.Infrastructure/Repositories/TweetRepository.cs) ও [TweetConfiguration.cs](TwitterClone.Infrastructure/Persistence/Configurations/TweetConfiguration.cs) নমুনা হিসেবে দেখো।

**নিরাপত্তা নোট:** `appsettings.Development.json`-এ ডাটাবেজ password আর JWT Key git-এ commit করা আছে। আসল প্রজেক্টে এগুলো **User Secrets**-এ রাখো:
```bash
cd TwitterClone.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Password=..."
dotnet user-secrets set "JwtSettings:Key" "$(openssl rand -base64 48)"
```
তারপর json ফাইল থেকে মানগুলো মুছে দাও, আর ডাটাবেজ password বদলে ফেলো (কারণ পুরনোটা git history-তে রয়ে গেছে)।
