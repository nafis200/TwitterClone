🚀 Complete Project Setup Guide (Clean Architecture)
📌 Architecture Map (কোন লেয়ার কার সাথে যুক্ত?)
┌────────────────────────────────────────────────────────┐
│                   TwitterClone.Api                     │
└─────────────────────────┬──────────────────────────────┘
                          │
         ┌────────────────┴────────────────┐
         ▼                                 ▼
┌─────────────────┐               ┌──────────────────┐
│   Application   │               │  Infrastructure  │
└────────┬────────┘               └────────┬─────────┘
         │                                 │
         └────────────────┬────────────────┘
                          ▼
                 ┌─────────────────┐
                 │     Domain      │
                 └─────────────────┘
নিয়ম (Dependency Rule):

Domain: কাউকেই চেনে না (সম্পূর্ণ স্বাধীন Core)।

Application: শুধু Domain-কে চেনে।

Infrastructure: Application এবং Domain দুটিকেই চেনে।

Api: Application এবং Infrastructure দুটিকে চেনে।

Utilities (Optional): প্রয়োজন অনুযায়ী Application, Infrastructure বা Api-তে যোগ করা হয়।

🛠️ Step-by-Step Terminal Commands
১. ফোল্ডার ও সলিউশন তৈরি (Folder & Solution Setup)
Bash
# প্রজেক্টের নামে ফোল্ডার তৈরি ও প্রবেশ
mkdir TwitterClone
cd TwitterClone

# Solution ফাইল তৈরি
dotnet new sln -n TwitterClone
২. সব লেয়ার বা প্রজেক্ট তৈরি করা (Create Layers)
Bash
# Core Layers
dotnet new classlib -n TwitterClone.Domain -f net8.0
dotnet new classlib -n TwitterClone.Application -f net8.0
dotnet new classlib -n TwitterClone.Infrastructure -f net8.0
dotnet new classlib -n TwitterClone.Utilities -f net8.0

# Entry Points (Presentation/App)
dotnet new webapi -n TwitterClone.Api -f net8.0
dotnet new console -n TwitterClone.ConsoleApp -f net8.0
৩. প্রজেক্টগুলো সলিউশনে যুক্ত করা (Add Projects to Solution)
Bash
dotnet sln add TwitterClone.Domain/TwitterClone.Domain.csproj
dotnet sln add TwitterClone.Application/TwitterClone.Application.csproj
dotnet sln add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj
dotnet sln add TwitterClone.Utilities/TwitterClone.Utilities.csproj
dotnet sln add TwitterClone.Api/TwitterClone.Api.csproj
dotnet sln add TwitterClone.ConsoleApp/TwitterClone.ConsoleApp.csproj
৪. লেয়ারগুলোর মধ্যে সঠিক রেফারেন্স দেওয়া (Add Project References)
Bash
# 1. Application references Domain & Utilities
dotnet add TwitterClone.Application/TwitterClone.Application.csproj reference TwitterClone.Domain/TwitterClone.Domain.csproj
dotnet add TwitterClone.Application/TwitterClone.Application.csproj reference TwitterClone.Utilities/TwitterClone.Utilities.csproj

# 2. Infrastructure references Application, Domain & Utilities
dotnet add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj reference TwitterClone.Application/TwitterClone.Application.csproj
dotnet add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj reference TwitterClone.Domain/TwitterClone.Domain.csproj
dotnet add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj reference TwitterClone.Utilities/TwitterClone.Utilities.csproj

# 3. API references Application, Infrastructure & Utilities
dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Application/TwitterClone.Application.csproj
dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj
dotnet add TwitterClone.Api/TwitterClone.Api.csproj reference TwitterClone.Utilities/TwitterClone.Utilities.csproj

# 4. Console App references Domain (Optional)
dotnet add TwitterClone.ConsoleApp/TwitterClone.ConsoleApp.csproj reference TwitterClone.Domain/TwitterClone.Domain.csproj
৫. ডিপেন্ডেন্সি ইনজেকশন (DI) প্যাকেজ ইন্সটল করা
Class Library প্রজেক্টগুলোতে ডিফল্টভাবে IServiceCollection থাকে না, তাই এই প্যাকেজটি লাগে:

Bash
dotnet add TwitterClone.Application/TwitterClone.Application.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2
dotnet add TwitterClone.Infrastructure/TwitterClone.Infrastructure.csproj package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.2
৬. Git & Cleanup Setup
Bash
# .gitignore ফাইল তৈরি (bin/ obj/ ফোল্ডার গিট-এ যাওয়া বন্ধ করতে)
dotnet new gitignore

# যদি bin/ বা obj/ আগেই গিট-এ ট্র্যাক হয়ে থাকে, তা ক্যাশ থেকে সরানোর জন্য:
git rm -r --cached TwitterClone.Api/bin TwitterClone.Api/obj TwitterClone.Application/bin TwitterClone.Application/obj TwitterClone.Domain/bin TwitterClone.Domain/obj TwitterClone.Infrastructure/bin TwitterClone.Infrastructure/obj TwitterClone.Utilities/bin TwitterClone.Utilities/obj TwitterClone.ConsoleApp/bin TwitterClone.ConsoleApp/obj