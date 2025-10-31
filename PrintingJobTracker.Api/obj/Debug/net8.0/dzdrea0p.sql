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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE TABLE [AuditLogTable] (
        [Id] uniqueidentifier NOT NULL,
        [EntityName] nvarchar(150) NOT NULL,
        [EntityId] nvarchar(100) NOT NULL,
        [ActionType] nvarchar(50) NOT NULL,
        [NewRecord] nvarchar(max) NULL,
        [PreviousRecord] nvarchar(max) NULL,
        [OcurredOn] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
    DECLARE @defaultSchema AS sysname;
    SET @defaultSchema = SCHEMA_NAME();
    DECLARE @description AS sql_variant;
    SET @description = N'Stores all audit trail entries recording create, update, and delete operations across entities.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable';
    SET @description = N'Unique identifier for the audit record.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'Id';
    SET @description = N'Name of the entity that was affected (e.g., Job, Client).';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'EntityName';
    SET @description = N'Identifier of the entity instance that was modified.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'EntityId';
    SET @description = N'Type of action performed: Create, Update, or Delete.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'ActionType';
    SET @description = N'Serialized JSON of the entity state after the operation.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'NewRecord';
    SET @description = N'Serialized JSON of the entity state before the operation.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'PreviousRecord';
    SET @description = N'UTC timestamp indicating when the operation occurred.';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'AuditLogTable', 'COLUMN', N'OcurredOn';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE TABLE [ClientTable] (
        [Id] int NOT NULL IDENTITY,
        [IdentityCard] nvarchar(50) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NULL,
        [SecondLastName] nvarchar(100) NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_Clients] PRIMARY KEY ([Id])
    );
    DECLARE @defaultSchema1 AS sysname;
    SET @defaultSchema1 = SCHEMA_NAME();
    DECLARE @description1 AS sql_variant;
    SET @description1 = N'Stores information about clients who request printing jobs.';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable';
    SET @description1 = N'Unique identifier for the client.';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable', 'COLUMN', N'Id';
    SET @description1 = N'National ID or business identification number for the client.';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable', 'COLUMN', N'IdentityCard';
    SET @description1 = N'Client''s first name or company legal name.';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable', 'COLUMN', N'FirstName';
    SET @description1 = N'Client''s last name (if applicable).';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable', 'COLUMN', N'LastName';
    SET @description1 = N'Client''s second last name (optional).';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable', 'COLUMN', N'SecondLastName';
    SET @description1 = N'Indicates whether the client record is logically deleted.';
    EXEC sp_addextendedproperty 'MS_Description', @description1, 'SCHEMA', @defaultSchema1, 'TABLE', N'ClientTable', 'COLUMN', N'IsDeleted';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE TABLE [JobTable] (
        [Id] int NOT NULL IDENTITY,
        [ClientId] int NOT NULL,
        [JobName] nvarchar(200) NOT NULL,
        [Quantity] int NOT NULL,
        [Carrier] nvarchar(50) NOT NULL,
        [CurrentStatus] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [MailDeadline] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Jobs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Jobs_Clients] FOREIGN KEY ([ClientId]) REFERENCES [ClientTable] ([Id]) ON DELETE NO ACTION
    );
    DECLARE @defaultSchema2 AS sysname;
    SET @defaultSchema2 = SCHEMA_NAME();
    DECLARE @description2 AS sql_variant;
    SET @description2 = N'Stores information about printing jobs and their current status in the production process.';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable';
    SET @description2 = N'Unique identifier for the job.';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'Id';
    SET @description2 = N'Unique identifier of the client associated with the job.';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'ClientId';
    SET @description2 = N'Descriptive name of the printing job.';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'JobName';
    SET @description2 = N'Number of items or copies to be produced for this job.';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'Quantity';
    SET @description2 = N'Name of the carrier handling the delivery (e.g., USPS, UPS, FedEx).';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'Carrier';
    SET @description2 = N'Current processing status of the job (Received, Printing, Inserting, etc.).';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'CurrentStatus';
    SET @description2 = N'UTC date and time when the job was created.';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'CreatedAt';
    SET @description2 = N'Deadline by which the job must be mailed according to the SLA (Service Level Agreement).';
    EXEC sp_addextendedproperty 'MS_Description', @description2, 'SCHEMA', @defaultSchema2, 'TABLE', N'JobTable', 'COLUMN', N'MailDeadline';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE TABLE [JobStatusHistoryTable] (
        [Id] int NOT NULL IDENTITY,
        [JobId] int NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [Note] nvarchar(500) NULL,
        [ChangedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_JobStatusHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JobStatusHistory_Job] FOREIGN KEY ([JobId]) REFERENCES [JobTable] ([Id]) ON DELETE NO ACTION
    );
    DECLARE @defaultSchema3 AS sysname;
    SET @defaultSchema3 = SCHEMA_NAME();
    DECLARE @description3 AS sql_variant;
    SET @description3 = N'Stores the historical records of job status changes over time.';
    EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'JobStatusHistoryTable';
    SET @description3 = N'Unique identifier for the job status history record.';
    EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'JobStatusHistoryTable', 'COLUMN', N'Id';
    SET @description3 = N'Foreign key referencing the related Job entity.';
    EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'JobStatusHistoryTable', 'COLUMN', N'JobId';
    SET @description3 = N'Represents the job status at this point in time (e.g., Received, Printing, Mailed, etc.).';
    EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'JobStatusHistoryTable', 'COLUMN', N'Status';
    SET @description3 = N'Optional note explaining details or exceptions related to the status change.';
    EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'JobStatusHistoryTable', 'COLUMN', N'Note';
    SET @description3 = N'UTC timestamp when this status change occurred.';
    EXEC sp_addextendedproperty 'MS_Description', @description3, 'SCHEMA', @defaultSchema3, 'TABLE', N'JobStatusHistoryTable', 'COLUMN', N'ChangedAt';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityName_EntityId] ON [AuditLogTable] ([EntityName], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_OcurredOn] ON [AuditLogTable] ([OcurredOn]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE INDEX [IX_Clients_FirstName_LastName] ON [ClientTable] ([FirstName], [LastName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clients_IdentityCard] ON [ClientTable] ([IdentityCard]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE INDEX [IX_Clients_IsDeleted] ON [ClientTable] ([IsDeleted]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ClientTable_Id] ON [ClientTable] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE INDEX [IX_JobStatusHistory_JobId_Status_ChangedAt] ON [JobStatusHistoryTable] ([JobId], [Status], [ChangedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JobStatusHistoryTable_Id] ON [JobStatusHistoryTable] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE INDEX [IX_JobTable_ClientId] ON [JobTable] ([ClientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JobTable_Id] ON [JobTable] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251031040654_202510302206'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251031040654_202510302206', N'9.0.10');
END;

COMMIT;
GO

