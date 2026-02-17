-- SQL Script to add Recruiters table and link to Applications
-- Run this in SSMS against the JobTracker database

USE JobTracker;
GO

-- Create RecruiterStatus lookup table (optional but recommended)
CREATE TABLE dbo.RecruiterStatus (
    RecruiterStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO dbo.RecruiterStatus (StatusName)
VALUES
('Active'),
('Inactive'),
('Do Not Contact');

GO

-- Create Recruiters table
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

GO

-- Add RecruiterId foreign key to Applications table
ALTER TABLE dbo.Applications
ADD RecruiterId INT NULL;

ALTER TABLE dbo.Applications
ADD CONSTRAINT FK_Applications_Recruiters
    FOREIGN KEY (RecruiterId) REFERENCES dbo.Recruiters(RecruiterId);

GO

-- Create indexes for better query performance
CREATE INDEX IX_Applications_RecruiterId ON dbo.Applications(RecruiterId);
CREATE INDEX IX_Recruiters_Company ON dbo.Recruiters(Company);
CREATE INDEX IX_Recruiters_DateContacted ON dbo.Recruiters(DateContacted);

GO

PRINT 'Recruiters table and relationships created successfully!';
