CREATE DATABASE JobTracker;
GO
USE JobTracker;
GO

CREATE TABLE dbo.ApplicationStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO dbo.ApplicationStatus (StatusName)
VALUES
('Applied'),
('Screening'),
('Interviewing'),
('Offer'),
('Rejected'),
('Withdrawn'),
('Accepted'),
('On Hold');

CREATE TABLE dbo.Applications (
    ApplicationId INT IDENTITY(1,1) PRIMARY KEY,
    AppliedDate DATE NULL,
    JobTitle NVARCHAR(200) NULL,
    CompanyName NVARCHAR(200) NULL,
    Location NVARCHAR(200) NULL,
    JobPostUrl NVARCHAR(1000) NULL,
    StatusId INT NULL,
    CompanyWebsite NVARCHAR(1000) NULL,
    NetworkContacts NVARCHAR(1000) NULL,
    CompanyResearchKeyPoints NVARCHAR(MAX) NULL,
    Notes NVARCHAR(MAX) NULL,
    TechFocus NVARCHAR(500) NULL,
    JobPublishedDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Applications_Status
        FOREIGN KEY (StatusId) REFERENCES dbo.ApplicationStatus(StatusId)
);

CREATE TABLE dbo.QuestionType (
    QuestionTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO dbo.QuestionType (TypeName)
VALUES
('Behavioral'),
('Technical'),
('Experience');

CREATE TABLE dbo.Questions (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    QuestionText NVARCHAR(MAX) NOT NULL,
    AnswerText NVARCHAR(MAX) NULL,
    QuestionTypeId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Questions_QuestionType
        FOREIGN KEY (QuestionTypeId) REFERENCES dbo.QuestionType(QuestionTypeId)
);
