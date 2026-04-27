MentorDashboardApp
منصة تعليم إلكتروني متكاملة (LMS) مبنية باستخدام ASP.NET Core MVC مع دعم كامل للعربية والإنجليزية.

نظرة عامة
MentorDashboardApp هي منصة تعليم إلكتروني شاملة توفر:

واجهة عامة للطلاب لاستعراض الكورسات والتسجيل فيها
لوحة تحكم للمدراء لإدارة الكورسات والمستخدمين والمدربين
مشغل دروس متقدم (LMS Player) مع تتبع تقدم الطالب
منشئ منهج دراسي بنظام السحب والإفلات (Drag & Drop)
نظام دردشة ذكي للإجابة على أسئلة الطلاب
دعم ثنائي اللغة (عربي/إنجليزي) مع واجهات RTL/LTR
المتطلبات
المتطلب
الإصدار
.NET SDK	9.0+
SQL Server	2019+ (أو Azure SQL)
Visual Studio 2022 / VS Code	أحدث إصدار
Node.js (للموارد الأمامية)	18+

التثبيت والتشغيل
1. استنساخ المشروع
bash

git clone https://github.com/3m-code-software/MentorDashboardApp.git
cd MentorDashboardApp
2. إعداد قاعدة البيانات
أنشئ قاعدة بيانات SQL Server جديدة ثم حدّث سلسلة الاتصال في ملف appsettings.json:

json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MentorDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
3. تشغيل المشروع
bash

# استعادة الحزم
dotnet restore

# بناء المشروع
dotnet build

# تشغيل المشروع
dotnet run
سيتم إنشاء الجداول تلقائياً عند أول تشغيل للمشروع.

4. الوصول للتطبيق
الواجهة العامة: https://localhost:5001
لوحة التحكم: https://localhost:5001/Admin
تسجيل الدخول: https://localhost:5001/Account/Login
حساب الدخول الافتراضي
الدور
البريد الإلكتروني
كلمة المرور
مدير النظام	admin@3m.com	admin123

تنبيه: يجب تغيير كلمة مرور المدير فوراً في بيئة الإنتاج.

هيكل المشروع
text

MentorDashboardApp/
├── Controllers/              # وحدات التحكم (11 كنترولر)
│   ├── AccountController.cs        # إدارة الحسابات (تسجيل/دخول)
│   ├── AdminController.cs          # لوحة تحكم المدير
│   ├── CoursesController.cs        # إدارة الكورسات (CRUD)
│   ├── CurriculumController.cs     # بناء المنهج الدراسي
│   ├── EnrollmentController.cs     # التسجيل والدفع
│   ├── LMSController.cs            # مشغل الدروس وتقدم الطالب
│   ├── PublicCoursesController.cs  # عرض الكورسات العامة
│   ├── TrainersController.cs       # عرض بيانات المدربين
│   ├── ChatbotController.cs        # نظام الدردشة الذكية
│   ├── LanguageController.cs       # تبديل اللغة
│   └── HomeController.cs           # الصفحة الرئيسية
│
├── Models/                   # نماذج البيانات (8 موديل)
│   ├── User.cs                    # المستخدم (طالب/مدرب/مدير)
│   ├── Course.cs                   # الكورس
│   ├── CourseModule.cs             # وحدة/قسم في الكورس
│   ├── CourseLesson.cs             # درس داخل الوحدة
│   ├── Enrollment.cs               # تسجيل طالب في كورس
│   ├── Payment.cs                  # عملية الدفع
│   └── StudentLessonProgress.cs    # تقدم الطالب في الدرس
│
├── Views/                    # واجهات Razor (~25 فيو)
│   ├── Shared/                     # القوالب المشتركة وال.layouts
│   ├── Home/                       # الصفحة الرئيسية
│   ├── Admin/                      # لوحة تحكم المدير
│   ├── Account/                    # تسجيل الدخول والتسجيل
│   ├── Courses/                    # إدارة الكورسات
│   ├── LMS/                        # مشغل الدروس
│   ├── PublicCourses/              # كتالوج الكورسات
│   ├── Curriculum/                 # بناء المنهج
│   ├── Enrollment/                 # التسجيل والدفع
│   └── Trainers/                   # بيانات المدربين
│
├── Data/
│   └── ApplicationDbContext.cs     # سياق قاعدة البيانات (EF Core)
│
├── Resources/                # ملفات الترجمة
│   ├── SharedResource.ar-EG.resx   # المصادر العربية
│   └── SharedResource.en-US.resx   # المصادر الإنجليزية
│
├── ViewModels/               # نماذج العرض
│   └── DashboardViewModel.cs
│
├── wwwroot/                  # الملفات الثابتة
│   ├── admin/                     # قالب NiceAdmin (لوحة التحكم)
│   └── frontend/                  # قالب eLearning (الواجهة العامة)
│
├── Program.cs                # نقطة الدخول وإعداد التطبيق
├── MentorDashboardApp.csproj # ملف المشروع
└── appsettings.json          # إعدادات التطبيق
التقنيات المستخدمة
الخلفية (Backend)
ASP.NET Core 9.0 - إطار العمل الرئيسي
Entity Framework Core 9.0 - التعامل مع قاعدة البيانات
SQL Server - نظام إدارة قواعد البيانات
Cookie Authentication - نظام المصادقة
الواجهة الأمامية (Frontend)
Bootstrap 5.3 - إطار التصميم
NiceAdmin - قالب لوحة التحكم
eLearning - قالب الواجهة العامة
Plyr - مشغل الفيديو
SortableJS - السحب والإفلات
ApexCharts - الرسوم البيانية
AOS - حركات التمرير
Cairo / Open Sans - الخطوط
الميزات الرئيسية
للمدراء (Admin)
لوحة تحكم مع إحصائيات ورسوم بيانية
إدارة كورسات كاملة (إنشاء/تعديل/حذف/نسخ)
بناء منهج دراسي بالسحب والإفلات
إدارة المستخدمين والمدربين
إعدادات النظام
للطلاب (Students)
تصفح كتالوج الكورسات مع بحث وفلترة
عرض تفاصيل الكورس والمنهج الدراسي
التسجيل والدفع
مشغل دروس متقدم مع تتبع التقدم
لوحة الكورسات المسجل فيها
للمدربين (Trainers)
ملف تعريفي عام
إدارة الكورسات المرتبطة
تعدد اللغات
يدعم المشروع اللغتين العربية والإنجليزية:

واجهات RTL كاملة للعربية مع خط Cairo
واجهات LTR للإنجليزية مع خط Open Sans
محتوى ثنائي اللغة في قاعدة البيانات (Title/TitleAr)
تبديل اللغة عبر زر في الشريط العلوي
الترخيص
هذا المشروع ملك لـ 3M Code Software .

المساهمة
Fork المشروع
أنشئ فرع جديد (git checkout -b feature/new-feature)
أرسل التعديلات (git commit -m 'Add new feature')
ارفع الفرع (git push origin feature/new-feature)
أنشئ Pull Request
