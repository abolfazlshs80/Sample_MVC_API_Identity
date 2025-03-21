معرفی پروژه
این پروژه یک API تحت وب با استفاده از ASP.NET Core طراحی شده است که تمامی ویژگی‌های ضروری برای مدیریت احراز هویت، لاگین و ثبت‌نام کاربران، و مدیریت توکن‌های امنیتی را پوشش می‌دهد. این پروژه شامل موارد زیر است:

احراز هویت با ASP.NET Identity: مدیریت کاربران و نقش‌ها با استفاده از Identity Framework.
توکن‌های JWT (Json Web Token): ایجاد توکن‌های دسترسی (Access Token) و توکن‌های رفرش (Refresh Token) برای احراز هویت کاربران.
مدیریت توکن‌های رفرش: ذخیره‌سازی و اعتبارسنجی توکن‌های رفرش در پایگاه داده.
لیست بلاک توکن‌ها: امکان بلاک کردن توکن‌های نامعتبر یا منقضی.
لاگین و ثبت‌نام: امکان لاگین و ثبت‌نام کاربران با اعتبارسنجی قوی.
مستندسازی API با Swagger: ارائه مستندات کامل و تعاملی برای API با استفاده از Swagger.
آزمایش API با RestSharp: امکان تست و مصرف API با استفاده از کتابخانه RestSharp.
ویژگی‌های کلیدی
1. احراز هویت و مدیریت کاربران
استفاده از ASP.NET Identity برای مدیریت کاربران، نقش‌ها و هش‌کردن رمز عبور.
امکان لاگین و ثبت‌نام کاربران با اعتبارسنجی ورودی‌ها.
2. توکن‌های JWT
ایجاد توکن‌های دسترسی (Access Token) برای احراز هویت کوتاه‌مدت.
استفاده از توکن‌های رفرش (Refresh Token) برای ایجاد توکن‌های جدید بدون نیاز به لاگین مجدد.
ذخیره‌سازی توکن‌های رفرش در پایگاه داده برای امنیت بیشتر.
3. لیست بلاک توکن‌ها
امکان بلاک کردن توکن‌های نامعتبر یا منقضی.
مدیریت لیست بلاک با ذخیره‌سازی توکن‌های بلاک شده در پایگاه داده.
4. مستندسازی API با Swagger
ارائه مستندات تعاملی و کامل برای API با استفاده از Swagger.
امکان تست API مستقیماً از طریق رابط کاربری Swagger.
5. آزمایش API با RestSharp
امکان تست و مصرف API با استفاده از کتابخانه RestSharp.
نمونه کدهایی برای ارسال درخواست‌های HTTP و دریافت پاسخ‌ها.
راه‌اندازی پروژه
پیش‌نیازها
.NET SDK (حداقل نسخه 6)
SQL Server یا هر پایگاه داده مورد نظر
Postman یا RestSharp برای تست API
مراحل راه‌اندازی
Clone پروژه:
bash
Copy
1
2
git clone https://github.com/yourusername/MyAuthApi.git
cd MyAuthApi
نصب بسته‌های NuGet:
bash
Copy
1
dotnet restore
تنظیمات پایگاه داده:
فایل appsettings.json را باز کنید و اطلاعات اتصال به پایگاه داده خود را وارد کنید:
json
Copy
1
2
3
⌄
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
اجرای Migration:
bash
Copy
1
dotnet ef database update
اجرای پروژه:
bash
Copy
1
dotnet run
دسترسی به Swagger:
پس از اجرای پروژه، مستندات API را می‌توانید در آدرس زیر مشاهده کنید:
Copy
1
http://localhost:5000/swagger
API Endpoints
1. ثبت‌نام کاربر
Endpoint: POST /api/auth/register
ورودی:
json
Copy
1
2
3
4
5
⌄
{
  "username": "string",
  "email": "string",
  "password": "string"
}
خروجی: پیام موفقیت‌آمیز یا خطای اعتبارسنجی.
2. لاگین کاربر
Endpoint: POST /api/auth/login
ورودی:
json
Copy
1
2
3
4
⌄
{
  "username": "string",
  "password": "string"
}
خروجی:
json
Copy
1
2
3
4
⌄
{
  "accessToken": "string",
  "refreshToken": "string"
}
3. دریافت توکن جدید با توکن رفرش
Endpoint: POST /api/auth/refresh-token
ورودی:
json
Copy
1
2
3
⌄
{
  "refreshToken": "string"
}
خروجی:
json
Copy
1
2
3
4
⌄
{
  "accessToken": "string",
  "refreshToken": "string"
}
4. خروج کاربر (بلاک کردن توکن)
Endpoint: POST /api/auth/logout
ورودی:
json
Copy
1
2
3
⌄
{
  "refreshToken": "string"
}
خروجی: پیام موفقیت‌آمیز یا خطای اعتبارسنجی.
تست API با RestSharp
برای تست API با RestSharp، می‌توانید از کد زیر استفاده کنید:

csharp
Copy
1
2
3
4
5
6
var client = new RestClient("http://localhost:5000");
var request = new RestRequest("/api/auth/login", Method.Post);
request.AddJsonBody(new { username = "testuser", password = "testpass" });

var response = client.Execute(request);
Console.WriteLine(response.Content);
لایسنس
این پروژه تحت لایسنس MIT منتشر شده است. برای اطلاعات بیشتر، فایل LICENSE را مشاهده کنید.

مشارکت‌کنندگان
نام شما
تماس با ما
برای ارتباط و ارسال بازخورد، می‌توانید از طریق ایمیل یا ایشو GitHub با ما در تماس باشید:

ایمیل: your.email@example.com
GitHub Issues: Issues
این توضیحات به صورت حرفه‌ای و ساختارمند طراحی شده‌اند تا بتوانید پروژه خود را به بهترین شکل معرفی کنید. اگر نیاز به تغییر یا اضافه کردن بخش‌های دیگر دارید، لطفاً اطلاع دهید! 🚀
