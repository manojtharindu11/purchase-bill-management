IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Location_Details] (
    [Location_Code] nvarchar(50) NOT NULL,
    [Location_Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Location_Details] PRIMARY KEY ([Location_Code])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260917053410_AddLocationDetails', N'8.0.31');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Purchase_Bills] (
    [Id] int NOT NULL IDENTITY,
    [BillNumber] nvarchar(50) NOT NULL,
    [CompanyCode] nvarchar(max) NOT NULL,
    [UserCode] nvarchar(max) NOT NULL,
    [UserDisplayName] nvarchar(max) NOT NULL,
    [BatchLocationName] nvarchar(200) NOT NULL,
    [TotalItems] int NOT NULL,
    [TotalQuantity] decimal(18,3) NOT NULL,
    [TotalCost] decimal(18,2) NOT NULL,
    [TotalSelling] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Purchase_Bills] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Purchase_Bill_Items] (
    [Id] int NOT NULL IDENTITY,
    [PurchaseBillId] int NOT NULL,
    [ItemName] nvarchar(150) NOT NULL,
    [BatchLocationName] nvarchar(200) NOT NULL,
    [StandardCost] decimal(18,2) NOT NULL,
    [StandardPrice] decimal(18,2) NOT NULL,
    [Quantity] decimal(18,3) NOT NULL,
    [DiscountPercent] decimal(5,2) NOT NULL,
    [TotalCost] decimal(18,2) NOT NULL,
    [TotalSelling] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Purchase_Bill_Items] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Purchase_Bill_Items_Purchase_Bills_PurchaseBillId] FOREIGN KEY ([PurchaseBillId]) REFERENCES [Purchase_Bills] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Purchase_Bill_Items_PurchaseBillId] ON [Purchase_Bill_Items] ([PurchaseBillId]);
GO

CREATE UNIQUE INDEX [IX_Purchase_Bills_BillNumber] ON [Purchase_Bills] ([BillNumber]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260917061704_AddPurchaseBillTables', N'8.0.31');
GO

COMMIT;
GO

