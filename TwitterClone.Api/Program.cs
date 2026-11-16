using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TwitterClone.Application;
using TwitterClone.Infrastructure;
using TwitterClone.Infrastructure.Auth;
using TwitterClone.Infrastructure.Persistence;

// Program.cs দুই ভাগে কাজ করে:
//   ভাগ ১ (builder): অ্যাপ চালু হওয়ার আগে সব service DI container-এ রেজিস্টার করা।
//   ভাগ ২ (app):     প্রতিটি request কোন কোন middleware-এর ভেতর দিয়ে যাবে তা সাজানো।
// বিস্তারিত বাংলা ব্যাখ্যা: PROJECT_GUIDE.md

var builder = WebApplication.CreateBuilder(args);

// ======================= ভাগ ১: Services =======================

// ১. Controllers: [ApiController] ক্লাসগুলো খুঁজে বের করে route হিসেবে চালু করে।
builder.Services.AddControllers();

// ২. Swagger: /swagger পেজে API টেস্ট করা যায়। "Authorize" বাটনে token বসানো যায়।
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste the accessToken returned by /api/auth/login."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ৩. আমাদের নিজেদের লেয়ার:
//    Application    -> Services (business logic)
//    Infrastructure -> DbContext (EF Core + PostgreSQL), Repositories, JWT token, password hasher
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ৪. Authentication: request-এর "Authorization: Bearer <token>" header পড়ে token যাচাই করা।
//    token বানানোর সময় যে Key/Issuer/Audience ব্যবহার হয় (JwtTokenService), যাচাইও সেগুলো দিয়েই হয়।
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // claim-এর নাম token-এ যেমন আছে ("sub", "email") তেমনই রাখো।
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Email
        };
    });

// ৫. Authorization: [Authorize] লাগানো endpoint-এ ঢোকার অনুমতি আছে কিনা তা চেক করা।
builder.Services.AddAuthorization();

var app = builder.Build();

// ======================= ভাগ ২: Middleware pipeline =======================
// প্রতিটি request উপর থেকে নিচে এই ধাপগুলো পার হয়ে controller-এ পৌঁছায়।

// ৬. শুধু Development-এ Swagger UI চালু।
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ৭. চালু হওয়ার সময় বাকি থাকা EF Core migration ডাটাবেজে apply করা (appsettings থেকে on/off)।
if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TwitterCloneDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();

// ৮. আগে Authentication (তুমি কে?), তারপর Authorization (তোমার অনুমতি আছে?)। ক্রম বদলানো যাবে না।
app.UseAuthentication();
app.UseAuthorization();

// ৯. request-কে সঠিক controller action-এ পাঠানো।
app.MapControllers();

app.Run();
