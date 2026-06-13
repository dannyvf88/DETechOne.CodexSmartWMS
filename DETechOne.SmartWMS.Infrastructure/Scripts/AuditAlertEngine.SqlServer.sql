IF OBJECT_ID('dbo.DTO_SW_AUDIT', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DTO_SW_AUDIT
    (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Module NVARCHAR(100) NOT NULL,
        Action NVARCHAR(100) NOT NULL,
        EntityType NVARCHAR(100) NOT NULL,
        EntityId NVARCHAR(100) NOT NULL,
        UserName NVARCHAR(150) NOT NULL,
        DeviceCode NVARCHAR(100) NULL,
        CorrelationId NVARCHAR(100) NULL,
        Description NVARCHAR(1000) NULL,
        Payload NVARCHAR(MAX) NULL,
        OccurredAtUtc DATETIME2 NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL
    );
END;

IF OBJECT_ID('dbo.DTO_SW_ALERT', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DTO_SW_ALERT
    (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Severity NVARCHAR(50) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        Source NVARCHAR(100) NOT NULL,
        Code NVARCHAR(100) NOT NULL,
        Title NVARCHAR(250) NOT NULL,
        Message NVARCHAR(1000) NOT NULL,
        EntityType NVARCHAR(100) NULL,
        EntityId NVARCHAR(100) NULL,
        UserName NVARCHAR(150) NULL,
        DeviceCode NVARCHAR(100) NULL,
        CorrelationId NVARCHAR(100) NULL,
        CreatedAtUtc DATETIME2 NOT NULL,
        AcknowledgedAtUtc DATETIME2 NULL,
        AcknowledgedBy NVARCHAR(150) NULL,
        ResolvedAtUtc DATETIME2 NULL,
        ResolvedBy NVARCHAR(150) NULL
    );
END;
