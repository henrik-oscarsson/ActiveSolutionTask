IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ActiveSolutionDB')
    CREATE DATABASE ActiveSolutionDB;
GO

PRINT 'Use database "ActiveSolutionDB"'
USE ActiveSolutionDB
GO

IF NOT EXISTS
    (SELECT *
     FROM master.sys.server_principals
     WHERE name = 'admin')
BEGIN
        PRINT 'Create login "admin"'
        CREATE LOGIN admin WITH PASSWORD = 'Qwerty123!'
END
GO

IF NOT EXISTS(SELECT *
              from sys.database_principals
              WHERE name = 'admin')
BEGIN
        PRINT 'Create user "admin"'
        CREATE USER admin FOR LOGIN admin WITH DEFAULT_SCHEMA = dbo
END
GO

ALTER ROLE [db_owner] ADD MEMBER [admin];
GO

