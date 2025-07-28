-- =================================================================================
-- TỆP SCRIPT SQL HOÀN CHỈNH ĐỂ TẠO CƠ SỞ DỮ LIỆU TRAVELCARDSDB
-- Tác giả: Gemini
-- Ngày tạo: 25/07/2025
-- Phiên bản: 2.0
-- Mô tả: Script này tạo toàn bộ cấu trúc cho ứng dụng TravelCards,
-- bao gồm quản lý người dùng, địa điểm, vật phẩm, nhiệm vụ, huy hiệu
-- và theo dõi sức khỏe tinh thần.
-- =================================================================================

-- -----------------------------------------------------------------
-- BƯỚC 1: TẠO VÀ CẤU HÌNH CƠ SỞ DỮ LIỆU
-- -----------------------------------------------------------------
USE [master];
GO

-- Tạo database nếu nó chưa tồn tại để tránh lỗi khi chạy lại script
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TravelCardsDB')
BEGIN
    -- Đường dẫn file có thể cần được điều chỉnh tùy theo cấu hình SQL Server của bạn
    CREATE DATABASE [TravelCardsDB]
     CONTAINMENT = NONE
     ON  PRIMARY 
    ( NAME = N'TravelCardsDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TravelCardsDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
     LOG ON 
    ( NAME = N'TravelCardsDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\TravelCardsDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB );
    
    PRINT 'Database [TravelCardsDB] created.';
END
ELSE
BEGIN
    PRINT 'Database [TravelCardsDB] already exists.';
END
GO

-- Áp dụng các cấu hình cho database
ALTER DATABASE [TravelCardsDB] SET COMPATIBILITY_LEVEL = 150;
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TravelCardsDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TravelCardsDB] SET ANSI_NULL_DEFAULT OFF;
GO
ALTER DATABASE [TravelCardsDB] SET ANSI_NULLS OFF;
GO
ALTER DATABASE [TravelCardsDB] SET ANSI_PADDING OFF;
GO
ALTER DATABASE [TravelCardsDB] SET ANSI_WARNINGS OFF;
GO
ALTER DATABASE [TravelCardsDB] SET ARITHABORT OFF;
GO
ALTER DATABASE [TravelCardsDB] SET AUTO_CLOSE OFF;
GO
ALTER DATABASE [TravelCardsDB] SET AUTO_SHRINK OFF;
GO
ALTER DATABASE [TravelCardsDB] SET AUTO_UPDATE_STATISTICS ON;
GO
ALTER DATABASE [TravelCardsDB] SET CURSOR_CLOSE_ON_COMMIT OFF;
GO
ALTER DATABASE [TravelCardsDB] SET CURSOR_DEFAULT  GLOBAL;
GO
ALTER DATABASE [TravelCardsDB] SET CONCAT_NULL_YIELDS_NULL OFF;
GO
ALTER DATABASE [TravelCardsDB] SET NUMERIC_ROUNDABORT OFF;
GO
ALTER DATABASE [TravelCardsDB] SET QUOTED_IDENTIFIER OFF;
GO
ALTER DATABASE [TravelCardsDB] SET RECURSIVE_TRIGGERS OFF;
GO
ALTER DATABASE [TravelCardsDB] SET  ENABLE_BROKER;
GO
ALTER DATABASE [TravelCardsDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF;
GO
ALTER DATABASE [TravelCardsDB] SET DATE_CORRELATION_OPTIMIZATION OFF;
GO
ALTER DATABASE [TravelCardsDB] SET TRUSTWORTHY OFF;
GO
ALTER DATABASE [TravelCardsDB] SET ALLOW_SNAPSHOT_ISOLATION OFF;
GO
ALTER DATABASE [TravelCardsDB] SET PARAMETERIZATION SIMPLE;
GO
ALTER DATABASE [TravelCardsDB] SET READ_COMMITTED_SNAPSHOT OFF;
GO
ALTER DATABASE [TravelCardsDB] SET HONOR_BROKER_PRIORITY OFF;
GO
ALTER DATABASE [TravelCardsDB] SET RECOVERY FULL;
GO
ALTER DATABASE [TravelCardsDB] SET  MULTI_USER;
GO
ALTER DATABASE [TravelCardsDB] SET PAGE_VERIFY CHECKSUM;
GO
ALTER DATABASE [TravelCardsDB] SET DB_CHAINING OFF;
GO
ALTER DATABASE [TravelCardsDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF );
GO
ALTER DATABASE [TravelCardsDB] SET TARGET_RECOVERY_TIME = 60 SECONDS;
GO
ALTER DATABASE [TravelCardsDB] SET DELAYED_DURABILITY = DISABLED;
GO
ALTER DATABASE [TravelCardsDB] SET QUERY_STORE = ON;
GO
ALTER DATABASE [TravelCardsDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON);
GO

-- -----------------------------------------------------------------
-- BƯỚC 2: CHUYỂN SANG NGỮ CẢNH DATABASE VÀ TẠO BẢNG
-- -----------------------------------------------------------------
USE [TravelCardsDB];
GO

-- Bảng hồ sơ người dùng
CREATE TABLE dbo.Users (
    UserID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DisplayName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    ProfilePictureURL NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Bảng đăng nhập bằng mật khẩu
CREATE TABLE dbo.LocalAuthentications (
    UserID UNIQUEIDENTIFIER PRIMARY KEY,
    PasswordHash NVARCHAR(255) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE
);
GO

-- Bảng đăng nhập qua mạng xã hội
CREATE TABLE dbo.SocialAuthentications (
    SocialAuthID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    ProviderName NVARCHAR(50) NOT NULL,
    ProviderUserID NVARCHAR(255) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE,
    CONSTRAINT UQ_SocialAuth_Provider_UserID UNIQUE (ProviderName, ProviderUserID)
);
GO

-- Bảng vai trò
CREATE TABLE dbo.Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Bảng gán vai trò cho người dùng
CREATE TABLE dbo.UserRoles (
    UserID UNIQUEIDENTIFIER NOT NULL,
    RoleID INT NOT NULL,
    PRIMARY KEY (UserID, RoleID),
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RoleID) REFERENCES dbo.Roles(RoleID) ON DELETE CASCADE
);
GO

-- Bảng lưu trữ các địa điểm (Landmarks)
CREATE TABLE dbo.Landmarks (
    LandmarkID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Latitude DECIMAL(9, 6) NOT NULL,
    Longitude DECIMAL(9, 6) NOT NULL,
    Address NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Bảng ghi lại lịch sử người dùng ghé thăm các địa điểm
CREATE TABLE dbo.LandmarkVisits (
    VisitID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    LandmarkID UNIQUEIDENTIFIER NOT NULL,
    VisitTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Note NVARCHAR(MAX) NULL,
    ImageURL NVARCHAR(255) NULL,
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (LandmarkID) REFERENCES dbo.Landmarks(LandmarkID)
);
GO

-- Bảng định nghĩa các loại "kho báu" hoặc "thẻ bài"
CREATE TABLE dbo.Treasures (
    TreasureID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ImageURL NVARCHAR(255),
    Rarity NVARCHAR(50),
    LandmarkID UNIQUEIDENTIFIER NULL, 
    FOREIGN KEY (LandmarkID) REFERENCES dbo.Landmarks(LandmarkID)
);
GO

-- Bảng ghi lại các kho báu mà người dùng đã thu thập
CREATE TABLE dbo.UserTreasures (
    UserTreasureID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    TreasureID UNIQUEIDENTIFIER NOT NULL,
    CollectedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    VisitID UNIQUEIDENTIFIER NULL, 
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (TreasureID) REFERENCES dbo.Treasures(TreasureID),
    FOREIGN KEY (VisitID) REFERENCES dbo.LandmarkVisits(VisitID)
);
GO

-- Bảng định nghĩa các nhiệm vụ
CREATE TABLE dbo.Missions (
    MissionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CompletionCriteria NVARCHAR(MAX) NULL 
);
GO

-- Bảng theo dõi tiến trình thực hiện nhiệm vụ của người dùng
CREATE TABLE dbo.UserMissions (
    UserMissionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    MissionID UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- 'InProgress', 'Completed'
    StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (MissionID) REFERENCES dbo.Missions(MissionID)
);
GO

-- Bảng định nghĩa các huy hiệu
CREATE TABLE dbo.Badges (
    BadgeID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ImageURL NVARCHAR(255) NOT NULL
);
GO

-- Bảng ghi lại các huy hiệu mà người dùng đã đạt được
CREATE TABLE dbo.UserBadges (
    UserBadgeID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    BadgeID UNIQUEIDENTIFIER NOT NULL,
    EarnedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (BadgeID) REFERENCES dbo.Badges(BadgeID)
);
GO

-- Bảng ghi lại các bản ghi theo dõi sức khỏe tinh thần của người dùng
CREATE TABLE dbo.StressLogs (
    LogID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserID UNIQUEIDENTIFIER NOT NULL,
    StressLevel INT NOT NULL, -- Thang điểm từ 1 đến 10
    LogTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Notes NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserID) REFERENCES dbo.Users(UserID) ON DELETE CASCADE
);
GO

-- -----------------------------------------------------------------
-- BƯỚC 3: TẠO CHỈ MỤC (INDEXES) ĐỂ TỐI ƯU HÓA HIỆU SUẤT
-- -----------------------------------------------------------------
CREATE INDEX IX_Landmarks_Coordinates ON dbo.Landmarks(Latitude, Longitude);
GO
CREATE INDEX IX_LandmarkVisits_User_Landmark ON dbo.LandmarkVisits(UserID, LandmarkID);
GO
CREATE INDEX IX_UserTreasures_User ON dbo.UserTreasures(UserID);
GO
CREATE INDEX IX_UserMissions_User_Mission ON dbo.UserMissions(UserID, MissionID);
GO
CREATE INDEX IX_UserBadges_User_Badge ON dbo.UserBadges(UserID, BadgeID);
GO
CREATE INDEX IX_StressLogs_User_Time ON dbo.StressLogs(UserID, LogTime);
GO

-- -----------------------------------------------------------------
-- BƯỚC 4: HOÀN TẤT
-- -----------------------------------------------------------------
PRINT '================================================================================';
PRINT 'SUCCESS: Database [TravelCardsDB] and all its objects have been created successfully.';
PRINT '================================================================================';
GO
