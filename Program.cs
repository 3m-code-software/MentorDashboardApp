using Microsoft.EntityFrameworkCore;
using MentorDashboardApp.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(); // Enable View Localization

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Enrollment/Checkout";
        options.LogoutPath = "/Home/Logout";
        options.Cookie.Name = "MentorAuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

// Register ApplicationDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

var app = builder.Build();

// Configure Localization
var supportedCultures = new[] { "en-US", "ar-EG" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Auto-create database tables on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated(); // This will create the database & tables if they don't exist
        
        // Dynamically add new columns to Courses table if they don't exist
        var addColumnsSql = @"
            IF COL_LENGTH('Courses', 'VideoUrl') IS NULL ALTER TABLE Courses ADD VideoUrl nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'VideoPlatform') IS NULL ALTER TABLE Courses ADD VideoPlatform nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'Status') IS NULL ALTER TABLE Courses ADD Status nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'Category') IS NULL ALTER TABLE Courses ADD Category nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'Level') IS NULL ALTER TABLE Courses ADD Level nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'Duration') IS NULL ALTER TABLE Courses ADD Duration nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'Prerequisites') IS NULL ALTER TABLE Courses ADD Prerequisites nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'Tags') IS NULL ALTER TABLE Courses ADD Tags nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'DiscountPrice') IS NULL ALTER TABLE Courses ADD DiscountPrice decimal(18,2) NULL;
            IF COL_LENGTH('Courses', 'SeoTitle') IS NULL ALTER TABLE Courses ADD SeoTitle nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'SeoDescription') IS NULL ALTER TABLE Courses ADD SeoDescription nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'HasCertificate') IS NULL ALTER TABLE Courses ADD HasCertificate bit NOT NULL DEFAULT 0;
            IF COL_LENGTH('Courses', 'IsFeatured') IS NULL ALTER TABLE Courses ADD IsFeatured bit NOT NULL DEFAULT 0;
            IF COL_LENGTH('Courses', 'TotalLessons') IS NULL ALTER TABLE Courses ADD TotalLessons int NULL;
            IF COL_LENGTH('Courses', 'Language') IS NULL ALTER TABLE Courses ADD Language nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'StartDate') IS NULL ALTER TABLE Courses ADD StartDate datetime2 NULL;
            IF COL_LENGTH('Courses', 'EndDate') IS NULL ALTER TABLE Courses ADD EndDate datetime2 NULL;
            IF COL_LENGTH('Courses', 'MaxCapacity') IS NULL ALTER TABLE Courses ADD MaxCapacity int NULL;
            IF COL_LENGTH('Courses', 'WhatYouWillLearn') IS NULL ALTER TABLE Courses ADD WhatYouWillLearn nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'TargetAudience') IS NULL ALTER TABLE Courses ADD TargetAudience nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'SyllabusUrl') IS NULL ALTER TABLE Courses ADD SyllabusUrl nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'CommunityLink') IS NULL ALTER TABLE Courses ADD CommunityLink nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'PaymentType') IS NULL ALTER TABLE Courses ADD PaymentType nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'AcceptsCoupons') IS NULL ALTER TABLE Courses ADD AcceptsCoupons bit NOT NULL DEFAULT 1;
            
            IF COL_LENGTH('Courses', 'TitleAr') IS NULL ALTER TABLE Courses ADD TitleAr nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'DescriptionAr') IS NULL ALTER TABLE Courses ADD DescriptionAr nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'WhatYouWillLearnAr') IS NULL ALTER TABLE Courses ADD WhatYouWillLearnAr nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'TargetAudienceAr') IS NULL ALTER TABLE Courses ADD TargetAudienceAr nvarchar(max) NULL;
            IF COL_LENGTH('Courses', 'PrerequisitesAr') IS NULL ALTER TABLE Courses ADD PrerequisitesAr nvarchar(max) NULL;
            
            -- Make Title nullable to support Arabic-only or English-only data entry
            ALTER TABLE Courses ALTER COLUMN Title nvarchar(max) NULL;

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CourseModules' and xtype='U')
            BEGIN
                CREATE TABLE CourseModules (
                    ModuleId INT IDENTITY(1,1) PRIMARY KEY,
                    CourseId INT NOT NULL FOREIGN KEY REFERENCES Courses(CourseId) ON DELETE CASCADE,
                    Title NVARCHAR(MAX) NULL,
                    TitleAr NVARCHAR(MAX) NULL,
                    OrderIndex INT NOT NULL
                );
            END

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CourseLessons' and xtype='U')
            BEGIN
                CREATE TABLE CourseLessons (
                    LessonId INT IDENTITY(1,1) PRIMARY KEY,
                    ModuleId INT NOT NULL FOREIGN KEY REFERENCES CourseModules(ModuleId) ON DELETE CASCADE,
                    Title NVARCHAR(MAX) NULL,
                    TitleAr NVARCHAR(MAX) NULL,
                    LessonType NVARCHAR(MAX) NULL,
                    ContentUrl NVARCHAR(MAX) NULL,
                    TextContent NVARCHAR(MAX) NULL,
                    OrderIndex INT NOT NULL,
                    DurationMinutes INT NULL,
                    IsFreePreview BIT NOT NULL DEFAULT 0,
                    IsPublished BIT NOT NULL DEFAULT 1
                );
            END

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StudentLessonProgresses' and xtype='U')
            BEGIN
                CREATE TABLE StudentLessonProgresses (
                    ProgressId INT IDENTITY(1,1) PRIMARY KEY,
                    UserId INT NOT NULL,
                    CourseId INT NOT NULL,
                    LessonId INT NOT NULL FOREIGN KEY REFERENCES CourseLessons(LessonId) ON DELETE CASCADE,
                    IsCompleted BIT NOT NULL DEFAULT 0,
                    CompletedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                );
            END

            -- Add Trainer Fields to Users Table
            IF COL_LENGTH('Users', 'ProfileImageUrl') IS NULL ALTER TABLE Users ADD ProfileImageUrl nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'Specialty') IS NULL ALTER TABLE Users ADD Specialty nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'SpecialtyAr') IS NULL ALTER TABLE Users ADD SpecialtyAr nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'Bio') IS NULL ALTER TABLE Users ADD Bio nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'BioAr') IS NULL ALTER TABLE Users ADD BioAr nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'FacebookUrl') IS NULL ALTER TABLE Users ADD FacebookUrl nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'TwitterUrl') IS NULL ALTER TABLE Users ADD TwitterUrl nvarchar(max) NULL;
            IF COL_LENGTH('Users', 'LinkedInUrl') IS NULL ALTER TABLE Users ADD LinkedInUrl nvarchar(max) NULL;
        ";
        db.Database.ExecuteSqlRaw(addColumnsSql);

        // Seed Admin User
        if (!db.Users.Any(u => u.Role == "Admin"))
        {
            db.Users.Add(new MentorDashboardApp.Models.User
            {
                FullName = "System Administrator",
                Email = "admin@3m.com",
                PasswordHash = "admin123", // In a real app, hash the password
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
