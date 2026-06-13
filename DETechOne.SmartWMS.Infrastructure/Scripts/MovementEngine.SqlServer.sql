/*
    SmartWMS 2.0 - Fase 10.5 Movement Engine
    Base SQL Server draft for future physical persistence.
    The current implementation uses InMemoryMovementRepository while the repository contract is stabilized.
*/

CREATE TABLE DTO_SW_MOVEMENT_HDR
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    MovementNumber NVARCHAR(50) NOT NULL,
    MovementType NVARCHAR(30) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    ReferenceType NVARCHAR(50) NOT NULL,
    ReferenceNumber NVARCHAR(100) NULL,
    RequestedBy NVARCHAR(100) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    ConfirmedAtUtc DATETIME2 NULL,
    ConfirmedBy NVARCHAR(100) NULL,
    CancelledAtUtc DATETIME2 NULL,
    CancelledBy NVARCHAR(100) NULL,
    CancelReason NVARCHAR(250) NULL
);

CREATE TABLE DTO_SW_MOVEMENT_LINE
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    MovementId UNIQUEIDENTIFIER NOT NULL,
    LineNumber INT NOT NULL,
    ItemCode NVARCHAR(50) NOT NULL,
    FromWarehouseCode NVARCHAR(20) NOT NULL,
    FromLocationCode NVARCHAR(50) NOT NULL,
    ToWarehouseCode NVARCHAR(20) NOT NULL,
    ToLocationCode NVARCHAR(50) NOT NULL,
    Quantity DECIMAL(19, 6) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    UomCode NVARCHAR(20) NULL,
    CONSTRAINT FK_DTO_SW_MOVEMENT_LINE_HDR FOREIGN KEY (MovementId) REFERENCES DTO_SW_MOVEMENT_HDR(Id)
);
