IF NOT EXISTS (
    SELECT 1
    FROM sys.databases
    WHERE name = N'HealthPlus'
)
BEGIN
    CREATE DATABASE HealthPlus;
END
GO