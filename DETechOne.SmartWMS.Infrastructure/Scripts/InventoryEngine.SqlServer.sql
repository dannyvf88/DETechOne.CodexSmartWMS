-- Fase 10.4 - Inventory Engine / SQL Server reference script
-- Este script es una base técnica. En SAP Business One productivo debe alinearse con UDT/UDO y el Metadata Installer.

CREATE TABLE DTO_SW_INV_BALANCE
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ItemCode NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(20) NOT NULL,
    LocationCode NVARCHAR(50) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    OnHandQuantity DECIMAL(19, 6) NOT NULL,
    ReservedQuantity DECIMAL(19, 6) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAtUtc DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE UNIQUE INDEX UX_DTO_SW_INV_BALANCE_KEY
ON DTO_SW_INV_BALANCE(ItemCode, WarehouseCode, LocationCode, LotNumber)
WHERE DeletedAtUtc IS NULL;

CREATE TABLE DTO_SW_INV_RESERVATION
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ItemCode NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(20) NOT NULL,
    LocationCode NVARCHAR(50) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    Quantity DECIMAL(19, 6) NOT NULL,
    ReferenceType NVARCHAR(50) NOT NULL,
    ReferenceNumber NVARCHAR(50) NOT NULL,
    Status INT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAtUtc DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE TABLE DTO_SW_INV_TRANSACTION
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ItemCode NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(20) NOT NULL,
    LocationCode NVARCHAR(50) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    Quantity DECIMAL(19, 6) NOT NULL,
    TransactionType INT NOT NULL,
    ReasonCode NVARCHAR(50) NOT NULL,
    ReferenceType NVARCHAR(50) NULL,
    ReferenceNumber NVARCHAR(50) NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAtUtc DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);
