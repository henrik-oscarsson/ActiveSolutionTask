USE ActiveSolutionDB
GO

IF object_id('Vehicle', 'U') is null
BEGIN
CREATE TABLE Vehicle
(
    Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
    [RegistrationNumber] NVARCHAR(100) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    MeterSetting INT NOT NULL DEFAULT 0,
    CONSTRAINT AK_RegistrationNumber UNIQUE (RegistrationNumber)
    )
END;
GO

IF object_id('Customer', 'U') is null
BEGIN
CREATE TABLE Customer
(
    Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
    [SSN] NVARCHAR(50) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    CONSTRAINT AK_SSN UNIQUE (SSN)
    )
END;
GO

IF object_id('Setting', 'U') is null
BEGIN
CREATE TABLE Setting
(
    Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
    [BaseRent] FLOAT NOT NULL,
    [BaseKilometerPrice] FLOAT NOT NULL,
    [DefaultNumberOfDays] INT NOT NULL,
    [DefaultNumberOfKilometers] INT NOT NULL,
    )
END;
GO

IF object_id('Booking', 'U') is null
BEGIN
CREATE TABLE Booking
(
    Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
    [ScheduledPickUpDate] DATETIME2 NOT NULL,
    [ScheduledReturnDate] DATETIME2 NOT NULL,
    [ActualPickupDate] DATETIME2 NULL,
    [ActualReturnDate] DATETIME2 NULL,
    [IsPickedUp] BIT NOT NULL DEFAULT 0,
    [IsReturned] BIT NOT NULL DEFAULT 0,
    [PickupMeterSetting] INT NULL,
    [ReturnMeterSetting] INT NULL,
    VehicleId INT NOT NULL,
    CustomerId INT NOT NULL,
    CONSTRAINT FK_Booking_VehicleId FOREIGN KEY(VehicleId) REFERENCES Vehicle(Id),
    CONSTRAINT FK_Booking_CustomerId FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
    )
END;
GO
