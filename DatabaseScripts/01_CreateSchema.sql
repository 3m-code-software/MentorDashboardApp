-- 01_CreateSchema.sql
-- إنشاء الجداول الأساسية لنظام (Mentor Dashboard) متوافقة مع استضافة MonsterASP.NET و SQL Server

-- 1. جدول المستخدمين (Users)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(50) DEFAULT 'User', -- 'Admin', 'User', 'Trainer'
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- 2. جدول الكورسات (Courses)
CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    TrainerId INT NULL, -- رابط مع جدول المستخدمين للمدرب
    ImageUrl NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Courses_Trainer FOREIGN KEY (TrainerId) REFERENCES Users(UserId)
);

-- 3. جدول تسجيل الطلاب في الكورسات (Enrollments)
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    UserId INT NOT NULL,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Active', -- 'Active', 'Completed', 'Cancelled'
    CONSTRAINT FK_Enrollments_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    CONSTRAINT FK_Enrollments_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- 4. جدول إيرادات التقارير (Payments - للوحة تحكم NiceAdmin)
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentMethod NVARCHAR(50), -- 'CreditCard', 'PayPal', 'Cash'
    CONSTRAINT FK_Payments_Enrollment FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(EnrollmentId)
);

-- إضافة مستخدم افتراضي كمدير (Admin)
INSERT INTO Users (FullName, Email, PasswordHash, Role)
VALUES ('Admin User', 'admin@mentor.com', 'HASH_PASSWORD_HERE', 'Admin');
