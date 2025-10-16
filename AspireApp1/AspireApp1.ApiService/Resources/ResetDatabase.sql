USE ActiveSolutionDB
GO

IF object_id('Booking', 'U') is not null
    DROP TABLE Booking;
GO

IF object_id('Vehicle', 'U') is not null
    DROP TABLE Vehicle;
GO

IF object_id('Setting', 'U') is not null
DROP TABLE Setting;
GO

IF object_id('Customer', 'U') is not null
DROP TABLE Customer;
GO

