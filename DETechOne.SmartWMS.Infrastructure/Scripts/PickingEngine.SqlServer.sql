/*
    SmartWMS 2.0 - Fase 10.6 Picking Engine
    Script base SQL Server.

    Este script es una primera aproximación para persistencia relacional.
    En SAP Business One productivo, el Metadata Installer deberá mapear estas
    estructuras a UDT/UDO equivalentes cuando aplique.
*/

CREATE TABLE DTO_SW_PICK_HDR
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PickingNumber NVARCHAR(50) NOT NULL,
    SourceDocumentType NVARCHAR(30) NOT NULL,
    SourceDocumentNumber NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(30) NOT NULL,
    RequestedBy NVARCHAR(100) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    StartedAtUtc DATETIME2 NULL,
    StartedBy NVARCHAR(100) NULL,
    CompletedAtUtc DATETIME2 NULL,
    CompletedBy NVARCHAR(100) NULL,
    CancelledAtUtc DATETIME2 NULL,
    CancelledBy NVARCHAR(100) NULL,
    CancelReason NVARCHAR(500) NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL
);

CREATE TABLE DTO_SW_PICK_LINE
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    PickingId UNIQUEIDENTIFIER NOT NULL,
    LineNumber INT NOT NULL,
    ItemCode NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(30) NOT NULL,
    LocationCode NVARCHAR(50) NULL,
    RequiredQuantity DECIMAL(19, 6) NOT NULL,
    PickedQuantity DECIMAL(19, 6) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    UomCode NVARCHAR(30) NULL,
    Status NVARCHAR(30) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAtUtc DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    CONSTRAINT FK_DTO_SW_PICK_LINE_HDR FOREIGN KEY (PickingId) REFERENCES DTO_SW_PICK_HDR(Id)
);

CREATE UNIQUE INDEX UX_DTO_SW_PICK_HDR_Number ON DTO_SW_PICK_HDR(PickingNumber);
CREATE UNIQUE INDEX UX_DTO_SW_PICK_LINE ON DTO_SW_PICK_LINE(PickingId, LineNumber);
