
# setup project

# ১. প্রজেক্টের নামে ফোল্ডার বানান এবং ঢুকুন
# mkdir MyProjectName
# cd MyProjectName

# ২. Solution ফাইল তৈরি করুন
# dotnet new sln -n MyProjectName

Step 2: প্রয়োজন অনুযায়ী প্রজেক্ট (Layers) তৈরি করুন
আপনার প্রজেক্টের ধরন অনুসারে যে অংশগুলো লাগবে সেগুলো তৈরি করবেন:

#### Class Library (Core / Domain / Infrastructure-এর জন্য):


# dotnet new classlib -n MyProjectName.Domain -f net8.0


#### Web API (Backend Server-এর জন্য):


# dotnet new webapi -n MyProjectName.Api -f net8.0


# Step 3: প্রজেক্টগুলোকে Solution-এ যুক্ত করুন

# সব তৈরি করা প্রজেক্ট Solution-এ কানেক্ট করুন
dotnet sln add MyProjectName.Domain/MyProjectName.Domain.csproj
dotnet sln add MyProjectName.Api/MyProjectName.Api.csproj


#### Step 4: একটি প্রজেক্টকে অন্য প্রজেক্টের সাথে যুক্ত করুন (Reference Add)
যেমন: Web API যেন Domain-এর কোড ব্যবহার করতে পারে:

# dotnet add MyProjectName.Api/MyProjectName.Api.csproj reference MyProjectName.Domain/MyProjectName.Domain.csproj


Step 5: VS Code-এ চালু ও বিল্ড করা

# VS Code-এ প্রজেক্ট খুলুন
code .

# প্রজেক্ট ঠিকঠাক আছে কিনা দেখতে বিল্ড করুন
dotnet build



ধাপ ১: Console App তৈরি করা

# dotnet new console -n TwitterClone.ConsoleApp

ধাপ ২: Console App-কে Solution (.sln) ফাইলের সাথে যুক্ত করা

# dotnet sln add TwitterClone.ConsoleApp/TwitterClone.ConsoleApp.csproj

ধাপ ৩: Domain প্রজেক্টকে Console App-এর সাথে কানেক্ট করা (Reference করা)

# dotnet add TwitterClone.ConsoleApp/TwitterClone.ConsoleApp.csproj reference TwitterClone.Domain/TwitterClone.Domain.csproj

ধাপ ৪: প্রজেক্ট রান (Run) করা

# cd TwitterClone.ConsoleApp
# dotnet run


ধাপ ১: Web API প্রজেক্ট তৈরি করা
আপনার প্রজেক্টের মূল ফোল্ডারে (Root Directory) টার্মিনাল খুলে নিচের কমান্ডটি দিন:


# dotnet new webapi -n TwitterClone.Api -f net8.0


ধাপ ২: Web API-কে Solution (.sln) ফাইলের সাথে যুক্ত করা

# dotnet sln add TwitterClone.Api/TwitterClone.Api.csproj


ধাপ ৩: Domain প্রজেক্টকে Web API-এর সাথে Reference করা
Web API যেন Domain লেয়ারের Model/Entity ব্যবহার করতে পারে:


# dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Domain/TwitterClone.Domain.csproj

# Api folder above click then build with right click


# Project Run command

# dotnet run --project TwitterClone.Api


http://localhost:5185/swagger


ধাপ ১: Application Class Library প্রজেক্ট তৈরি করা

# dotnet new classlib -n TwitterClone.Application -f net8.0

ধাপ ২: Application প্রজেক্টকে Solution (.sln) ফাইলের সাথে যুক্ত করা

# dotnet sln add TwitterClone.Application/TwitterClone.Application.csproj

ধাপ ৩: Application প্রজেক্টে Domain প্রজেক্টের Reference যুক্ত করা

# dotnet add TwitterClone.Application/TwitterClone.Application.csproj reference TwitterClone.Domain/TwitterClone.Domain.csproj

# dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Application/TwitterClone.Application.csproj

# dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Application/TwitterClone.Application.csproj


Ulities class create

# dotnet new classlib -n TwitterClone.Utilities

# dotnet sln add TwitterClone.Utilities/TwitterClone.Utilities.csproj

# dotnet add TwitterClone.Application/TwitterClone.Application.csproj reference TwitterClone.Utilities/TwitterClone.Utilities.csproj

# dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Utilities/TwitterClone.Utilities.csproj

# dotnet add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj reference TwitterClone.Utilities/TwitterClone.Utilities.csproj


https://github.com/MehrajShakil/TwitterClone/pull/8


DependencyInjection.cs যোগ করা (Program.cs থেকে service registration সরিয়ে আলাদা ফাইলে)

ধাপ ১: Application আর Infrastructure প্রজেক্টে DI package যোগ করা
(classlib প্রজেক্টে IServiceCollection নিজে থেকে থাকে না, তাই এই package লাগে)

# dotnet add TwitterClone.Application/TwitterClone.Application.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2

# dotnet add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2

ধাপ ২: দুটো ফাইল তৈরি করা
- TwitterClone.Application/DependencyInjection.cs  -> AddApplication() (সব Service, Scoped)
- TwitterClone.Infrastructure/DependencyInjection.cs -> AddInfrastructure() (সব Repository, Singleton)

সতর্কতা: IDE-র "Generate interface IServiceCollection" quick-fix এ ক্লিক করবেন না।
এটা একটা ভুয়া IServiceCollection বানিয়ে দেয়, যার ফলে build ভেঙে যায়।
এর বদলে উপরের package যোগ করুন, আর ফাইলের উপরে লিখুন: using Microsoft.Extensions.DependencyInjection;

ধাপ ৩: Program.cs এ namespace যোগ করে method দুটো call করা

using TwitterClone.Application;
using TwitterClone.Infrastructure;

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

ধাপ ৪: Build করে দেখা

# dotnet build


.gitignore যোগ করা (bin/ আর obj/ যেন git এ না যায়)

# dotnet new gitignore

আগে থেকে commit হয়ে থাকা bin/ obj/ কে git থেকে বের করতে (ফাইল ডিস্ক থেকে মুছবে না, শুধু git tracking বন্ধ হবে):

# git rm -r --cached TwitterClone.Api/bin TwitterClone.Api/obj TwitterClone.Application/bin TwitterClone.Application/obj TwitterClone.Domain/bin TwitterClone.Domain/obj TwitterClone.Infrastructure/bin TwitterClone.Infrastructure/obj TwitterClone.ConsoleApp/bin TwitterClone.ConsoleApp/obj


Step 1: PostgreSQL-এর জন্য NuGet প্যাকেজ ইন্সটল করুন
আপনার প্রজেক্টের টার্মিনালে (Terminal) গিয়ে নিচের কমান্ডগুলো রান করুন:

১. Infrastructure প্রজেক্টে PostgreSQL Provider যোগ করতে:


# dotnet add TwitterClone.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.2
(এখানে প্রজেক্টের নাম আপনার মতো TwitterClone.Infrastructure বা আপনার প্রজেক্টের নাম হতে পারে)

২. Api প্রজেক্টে EF Core Design প্যাকেজ যোগ করতে:

# dotnet add TwitterClone.Api package Microsoft.EntityFrameworkCore.Design --version 8.0.2

৩. Entity Framework Tool (যদি আগে ইন্সটল করা না থাকে):

# dotnet tool install --global dotnet-ef --version 8.0.2