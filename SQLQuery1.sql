-- 1. Roles Table
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE -- Admin, Technician, Employee
);

-- 2. Users / Employees Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(120) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Department VARCHAR(50) NOT NULL,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleId),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 3. Asset Categories Table
CREATE TABLE AssetCategories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL, -- Laptop, Server, Router, Monitor
    ExpectedLifespanMonths INT DEFAULT 36
);

-- 4. Assets Inventory Table
CREATE TABLE Assets (
    AssetId INT IDENTITY(1,1) PRIMARY KEY,
    AssetTag VARCHAR(50) NOT NULL UNIQUE, -- E.g., AST-2026-001
    AssetName VARCHAR(100) NOT NULL,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES AssetCategories(CategoryId),
    SerialNumber VARCHAR(100) NOT NULL UNIQUE,
    PurchaseDate DATE NOT NULL,
    WarrantyExpiryDate DATE NOT NULL,
    Status VARCHAR(20) DEFAULT 'Active', -- Active, Under Repair, Retired, Lost
    AssignedToUserId INT NULL FOREIGN KEY REFERENCES Users(UserId),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 5. SLA Policies Table
CREATE TABLE SLAPolicies (
    PolicyId INT IDENTITY(1,1) PRIMARY KEY,
    PriorityLevel VARCHAR(20) NOT NULL UNIQUE, -- Low, Medium, High, Critical
    ResponseTimeHours INT NOT NULL,
    ResolutionTimeHours INT NOT NULL
);

-- 6. Service / Incident Tickets Table
CREATE TABLE ServiceTickets (
    TicketId INT IDENTITY(1,1) PRIMARY KEY,
    TicketNumber VARCHAR(50) NOT NULL UNIQUE, -- E.g., TCK-2026-8801
    Subject VARCHAR(150) NOT NULL,
    Description TEXT NOT NULL,
    AssetId INT NOT NULL FOREIGN KEY REFERENCES Assets(AssetId),
    ReportedByUserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    AssignedTechnicianId INT NULL FOREIGN KEY REFERENCES Users(UserId),
    PolicyId INT NOT NULL FOREIGN KEY REFERENCES SLAPolicies(PolicyId),
    Status VARCHAR(20) DEFAULT 'Open', -- Open, In Progress, On Hold, Resolved, Closed
    DueDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ResolvedAt DATETIME NULL
);

-- 7. Ticket Updates & Comments Table
CREATE TABLE TicketComments (
    CommentId INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL FOREIGN KEY REFERENCES ServiceTickets(TicketId) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CommentText TEXT NOT NULL,
    IsInternalNote BIT DEFAULT 0, -- 1 for tech-only notes, 0 for public
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 8. Asset Maintenance History Log
CREATE TABLE MaintenanceLogs (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    AssetId INT NOT NULL FOREIGN KEY REFERENCES Assets(AssetId),
    PerformedByUserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ServiceDetails TEXT NOT NULL,
    Cost DECIMAL(10, 2) DEFAULT 0.00,
    MaintenanceDate DATETIME DEFAULT GETDATE()
);


USE ITAssetManagementDB;
GO

-- 1. Fix AssetId constraint to allow NULL for non-hardware tickets
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceTickets')
BEGIN
    ALTER TABLE ServiceTickets ALTER COLUMN AssetId INT NULL;
END
GO

-- 2. Seed Default Roles
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin')
BEGIN
    INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Technician'), ('Employee');
END
GO

-- 3. Seed Default SLA Policies
IF NOT EXISTS (SELECT 1 FROM SLAPolicies WHERE PriorityLevel = 'Critical')
BEGIN
    SET IDENTITY_INSERT SLAPolicies ON;
    INSERT INTO SLAPolicies (PolicyId, PriorityLevel, ResponseTimeHours, ResolutionTimeHours) VALUES 
    (1, 'Critical', 1, 2),
    (2, 'High', 2, 8),
    (3, 'Medium', 4, 24),
    (4, 'Low', 8, 48);
    SET IDENTITY_INSERT SLAPolicies OFF;
END
GO


USE ITAssetManagementDB;
GO

-- 1. Populate Roles
IF NOT EXISTS (SELECT 1 FROM Roles)
BEGIN
    INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Technician'), ('Employee');
END
GO

-- 2. Populate Test Users
IF NOT EXISTS (SELECT 1 FROM Users)
BEGIN
    DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'Admin');
    DECLARE @TechRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'Technician');
    DECLARE @EmpRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'Employee');

    INSERT INTO Users (FullName, Email, PasswordHash, Department, RoleId, IsActive) VALUES
    ('System Admin', 'admin@domain.com', 'admin123', 'IT Management', @AdminRoleId, 1),
    ('John Tech', 'tech@domain.com', 'tech123', 'IT Support', @TechRoleId, 1),
    ('Ayan Shah', 'employee@domain.com', 'emp123', 'Engineering', @EmpRoleId, 1);
END
GO

-- 3. Populate Asset Categories
IF NOT EXISTS (SELECT 1 FROM AssetCategories)
BEGIN
    INSERT INTO AssetCategories (CategoryName, ExpectedLifespanMonths) VALUES
    ('Laptop', 36),
    ('Peripheral', 24),
    ('Accessory', 12),
    ('Server', 60);
END
GO

-- 4. Populate SLA Policies
IF NOT EXISTS (SELECT 1 FROM SLAPolicies WHERE PriorityLevel = 'Critical')
BEGIN
    SET IDENTITY_INSERT SLAPolicies ON;
    INSERT INTO SLAPolicies (PolicyId, PriorityLevel, ResponseTimeHours, ResolutionTimeHours) VALUES 
    (1, 'Critical', 1, 2),
    (2, 'High', 2, 8),
    (3, 'Medium', 4, 24),
    (4, 'Low', 8, 48);
    SET IDENTITY_INSERT SLAPolicies OFF;
END
GO

-- 5. Populate Initial Sample Hardware
IF NOT EXISTS (SELECT 1 FROM Assets)
BEGIN
    DECLARE @EmpId INT = (SELECT UserId FROM Users WHERE Email = 'employee@domain.com');
    DECLARE @LaptopCat INT = (SELECT CategoryId FROM AssetCategories WHERE CategoryName = 'Laptop');
    DECLARE @PeripheralCat INT = (SELECT CategoryId FROM AssetCategories WHERE CategoryName = 'Peripheral');

    INSERT INTO Assets (AssetTag, AssetName, CategoryId, SerialNumber, PurchaseDate, WarrantyExpiryDate, Status, AssignedToUserId) VALUES
    ('AST-10042', 'Dell Latitude 7420', @LaptopCat, 'SN-7420-99A', '2024-01-15', '2026-12-31', 'Active', @EmpId),
    ('AST-20991', 'Dell 27-inch Monitor', @PeripheralCat, 'SN-27MON-12', '2024-02-01', '2027-02-01', 'Active', @EmpId);
END
GO