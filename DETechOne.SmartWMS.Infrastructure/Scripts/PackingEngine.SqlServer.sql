/*
    SmartWMS 2.0 - Fase 10.7 Packing Engine
    SQL Server reference script.
    These tables mirror the initial in-memory model and will be refined when persistence moves to production UDT/UDO integration.
*/

CREATE TABLE DTO_SW_PACKING_HDR
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PackingNumber NVARCHAR(50) NOT NULL,
    PickingId UNIQUEIDENTIFIER NOT NULL,
    PickingNumber NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(50) NOT NULL,
    RequestedBy NVARCHAR(100) NOT NULL,
    Status INT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    StartedAtUtc DATETIME2 NULL,
    StartedBy NVARCHAR(100) NULL,
    CompletedAtUtc DATETIME2 NULL,
    CompletedBy NVARCHAR(100) NULL,
    CancelledAtUtc DATETIME2 NULL,
    CancelledBy NVARCHAR(100) NULL,
    CancelReason NVARCHAR(500) NULL
);

CREATE TABLE DTO_SW_PACKING_LIN
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PackingId UNIQUEIDENTIFIER NOT NULL,
    LineNumber INT NOT NULL,
    ItemCode NVARCHAR(100) NOT NULL,
    WarehouseCode NVARCHAR(50) NOT NULL,
    LocationCode NVARCHAR(100) NULL,
    PickedQuantity DECIMAL(19, 6) NOT NULL,
    PackedQuantity DECIMAL(19, 6) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    UomCode NVARCHAR(50) NULL,
    PackageCode NVARCHAR(100) NULL,
    Status INT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    CONSTRAINT FK_DTO_SW_PACKING_LIN_HDR FOREIGN KEY (PackingId) REFERENCES DTO_SW_PACKING_HDR(Id)
);

CREATE UNIQUE INDEX UX_DTO_SW_PACKING_HDR_NUMBER ON DTO_SW_PACKING_HDR(PackingNumber);
CREATE INDEX IX_DTO_SW_PACKING_HDR_STATUS ON DTO_SW_PACKING_HDR(Status);
CREATE INDEX IX_DTO_SW_PACKING_LIN_ITEM ON DTO_SW_PACKING_LIN(ItemCode, WarehouseCode, LocationCode);
