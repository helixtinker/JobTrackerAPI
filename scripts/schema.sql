-- SQL Script to create JobTracker database schema
-- This is the initial schema setup

CREATE DATABASE JobTracker;
GO
USE JobTracker;
GO

-- Lookup Tables

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

CREATE TABLE dbo.QuestionType (
    QuestionTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO dbo.QuestionType (TypeName)
VALUES
('Behavioral'),
('Technical'),
('Experience');

CREATE TABLE dbo.RecruiterStatus (
    RecruiterStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO dbo.RecruiterStatus (StatusName)
VALUES
('Active'),
('Inactive'),
('Do Not Contact');

-- Main Tables

CREATE TABLE dbo.Recruiters (
    RecruiterId INT IDENTITY(1,1) PRIMARY KEY,
    RecruiterName NVARCHAR(200) NOT NULL,
    Company NVARCHAR(200) NULL,
    Email NVARCHAR(255) NULL,
    Phone NVARCHAR(20) NULL,
    LinkedInUrl NVARCHAR(500) NULL,
    DateContacted DATE NOT NULL,
    RecruiterStatusId INT NULL DEFAULT 1,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Recruiters_Status
        FOREIGN KEY (RecruiterStatusId) REFERENCES dbo.RecruiterStatus(RecruiterStatusId)
);

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
    RecruiterId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Applications_Status
        FOREIGN KEY (StatusId) REFERENCES dbo.ApplicationStatus(StatusId),
    CONSTRAINT FK_Applications_Recruiters
        FOREIGN KEY (RecruiterId) REFERENCES dbo.Recruiters(RecruiterId)
);

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

-- Indexes

CREATE INDEX IX_Applications_RecruiterId ON dbo.Applications(RecruiterId);
CREATE INDEX IX_Recruiters_Company ON dbo.Recruiters(Company);
CREATE INDEX IX_Recruiters_DateContacted ON dbo.Recruiters(DateContacted);

PRINT 'JobTracker database schema created successfully!';
