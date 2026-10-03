
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


