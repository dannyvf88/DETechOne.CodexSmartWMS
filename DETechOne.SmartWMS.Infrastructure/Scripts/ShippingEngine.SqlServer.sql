-- Fase 10.8 - Shipping Engine
-- Script inicial SQL Server para tablas futuras del motor de embarque.
-- Este script es intencionalmente conservador: la implementación actual usa repositorio in-memory.

CREATE TABLE DTO_SW_SHIPPING_HDR
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ShippingNumber NVARCHAR(50) NOT NULL,
    PackingId UNIQUEIDENTIFIER NOT NULL,
    PackingNumber NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(50) NOT NULL,
    CustomerCode NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(200) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    DeliveryDocEntry INT NULL,
    DeliveryDocNum INT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    UpdatedAtUtc DATETIME2 NULL
);

CREATE TABLE DTO_SW_SHIPPING_LIN
(
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    ShippingId UNIQUEIDENTIFIER NOT NULL,
    LineNumber INT NOT NULL,
    ItemCode NVARCHAR(50) NOT NULL,
    WarehouseCode NVARCHAR(50) NOT NULL,
    LocationCode NVARCHAR(50) NULL,
    PackedQuantity DECIMAL(19, 6) NOT NULL,
    LotNumber NVARCHAR(100) NULL,
    UomCode NVARCHAR(30) NULL,
    Status NVARCHAR(30) NOT NULL,
    CONSTRAINT FK_DTO_SW_SHIPPING_LIN_HDR FOREIGN KEY (ShippingId) REFERENCES DTO_SW_SHIPPING_HDR(Id)
);
