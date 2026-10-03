🚀 ASP.NET Core 8 Clean Architecture Setup Guide
নতুন প্রজেক্ট তৈরি করার সময় Terminal-এ MyProjectName-এর জায়গায় আপনার নিজের প্রজেক্টের নাম (যেমন: TwitterClone বা CarShop) বসিয়ে দিবেন।

১. প্রজেক্ট ফোল্ডার ও Solution (.sln) তৈরি
💡 Solution (.sln) কী? সলিউশন হলো এমন একটি মেইন ফোল্ডার বা কন্টেইনার, যা আপনার প্রজেক্টের সমস্ত ছোট ছোট প্রজেক্ট বা লেয়ারকে (Domain, Application, Api) একসাথে বেঁধে রাখে।

Bash
# প্রজেক্টের নামের ফোল্ডার তৈরি ও প্রবেশ
mkdir MyProjectName
cd MyProjectName

# মূল Solution ফাইল তৈরি

# dotnet new sln -n MyProjectName

২. আর্কিটেকচারের সব লেয়ার (Projects) তৈরি
💡 লেয়ারগুলোর কাজ কী?

Domain: আপনার প্রজেক্টের মূল ডাটা মডেল/এন্টিটি (Entities/Models) থাকবে। এটি কারো উপর নির্ভর করে না।

Application: বিজনেস লজিক, Interface ও DTOs (Data Transfer Objects) থাকবে।

Infrastructure: ডাটাবেজ কনেকশন (Entity Framework, DbContext) ও External Services থাকবে।

Api: আপনার মূল Web API (Controllers, Program.cs)।

ConsoleApp: টেস্ট বা ছোটখাটো স্ক্রিপ্ট রান করার জন্য (অপশনাল)।

Bash
# Domain Layer

# dotnet new classlib -n MyProjectName.Domain -f net8.0

# Application Layer
# dotnet new classlib -n MyProjectName.Application -f net8.0

# Infrastructure Layer
# dotnet new classlib -n MyProjectName.Infrastructure -f net8.0

# Web API Layer
# dotnet new webapi -n MyProjectName.Api -f net8.0

# Console App Layer (Optional)
# dotnet new console -n MyProjectName.ConsoleApp -f net8.0
৩. সলিউশন (.sln)-এ সব প্রজেক্ট রেজিস্টার করা
💡 কেন এটি দরকার? প্রজেক্টগুলো তৈরি করলেই সলিউশন ফাইল তাদের চেনে না। dotnet sln add করলে VS Code এবং .NET কম্পাইলার বুঝতে পারে যে এরা সবাই একই পরিবারের অংশ।

Bash
# dotnet sln add MyProjectName.Domain/MyProjectName.Domain.csproj
# dotnet sln add MyProjectName.Application/MyProjectName.Application.csproj
# dotnet sln add MyProjectName.Infrastructure/MyProjectName.Infrastructure.csproj
# dotnet sln add MyProjectName.Api/MyProjectName.Api.csproj
# dotnet sln add MyProjectName.ConsoleApp/MyProjectName.ConsoleApp.csproj
৪. প্রজেক্ট কানেকশন বা রেফারেন্স (Reference Add) তৈরি
💡 Clean Architecture-এর মূল নিয়ম:

Application চিনবে Domain-কে।

Infrastructure চিনবে Application ও Domain-কে।

API চিনবে Application, Infrastructure ও Domain-কে।

Bash
# ১. Application Layer-কে Domain-এর সাথে যুক্ত করা

# dotnet add MyProjectName.Application/MyProjectName.Application.csproj reference MyProjectName.Domain/MyProjectName.Domain.csproj

# ২. Infrastructure Layer-কে Application ও Domain-এর সাথে যুক্ত করা

# dotnet add MyProjectName.Infrastructure/MyProjectName.Infrastructure.csproj reference MyProjectName.Application/MyProjectName.Application.csproj
# dotnet add MyProjectName.Infrastructure/MyProjectName.Infrastructure.csproj reference MyProjectName.Domain/MyProjectName.Domain.csproj

# ৩. Web API Layer-কে Application, Infrastructure ও Domain-এর সাথে যুক্ত করা

# dotnet add MyProjectName.Api/MyProjectName.Api.csproj reference MyProjectName.Application/MyProjectName.Application.csproj
# dotnet add MyProjectName.Api/MyProjectName.Api.csproj reference MyProjectName.Infrastructure/MyProjectName.Infrastructure.csproj
# dotnet add MyProjectName.Api/MyProjectName.Api.csproj reference MyProjectName.Domain/MyProjectName.Domain.csproj

# ৪. Console App-কে কানেক্ট করা (যদি ব্যবহার করেন)

# dotnet add MyProjectName.ConsoleApp/MyProjectName.ConsoleApp.csproj reference MyProjectName.Domain/MyProjectName.Domain.csproj
# dotnet add MyProjectName.ConsoleApp/MyProjectName.ConsoleApp.csproj reference MyProjectName.Application/MyProjectName.Application.csproj
৫. প্রজেক্ট বিল্ড ও পরীক্ষা
সবকিছু কানেক্ট করার পর কোডে কোনো ভুল বা ব্রোকেন রেফারেন্স আছে কিনা দেখতে বিল্ড চেক দিন:

Bash
# dotnet build
৬. প্রজেক্ট রান করার উপায়
Web API রান করা:
Bash
# dotnet run --project MyProjectName.Api
ব্রাউজারে খুলুন: http://localhost:5185/swagger (পোর্ট নাম্বার আপনার টার্মিনালে দেখানো অনুযায়ী পরিবর্তন হতে পারে)।

Console App রান করা:
Bash
# dotnet run --project MyProjectName.ConsoleApp
🛠️ VS Code-এ মিথ্যা লাল দাগ (Red Error Lines) দেখা দিলে কী করবেন?
কখনো কখনো Terminal-এ dotnet build সফল হলেও VS Code-এর ইন্টেলিসেন্স ক্যাশের কারণে কোডে লাল দাগ দেখিয়ে বলে Namespace or Type not found (যেমন: CS0246 বা CS0234)।

সেক্ষেত্রে নিচের ৩টির যেকোনো একটি সমাধান প্রয়োগ করুন:

VS Code-এ Ctrl + Shift + P চেপে লিখুন: Developer: Reload Window এবং Enter দিন।

Ctrl + Shift + P চেপে লিখুন: C#: Restart Language Server এবং Enter দিন।

টার্মিনালে ক্যাশ ফোল্ডার মুছে রি-বিল্ড দিন:

Bash
rm -rf */bin */obj
dotnet restore
dotnet build