-- =============================================
-- User Management Database Schema
-- =============================================

-- Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastLoginAt DATETIME2 NULL,
    RefreshToken NVARCHAR(500) NULL,
    RefreshTokenExpiry DATETIME2 NULL
);

-- Roles table
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Permissions table
CREATE TABLE Permissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    Category NVARCHAR(50) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- UserRoles junction table (Many-to-Many)
CREATE TABLE UserRoles (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    AssignedBy UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (AssignedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    UNIQUE (UserId, RoleId)
);

-- RolePermissions junction table (Many-to-Many)
CREATE TABLE RolePermissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PermissionId UNIQUEIDENTIFIER NOT NULL,
    GrantedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    GrantedBy UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE,
    FOREIGN KEY (GrantedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    UNIQUE (RoleId, PermissionId)
);

-- UserSessions table for tracking user sessions
CREATE TABLE UserSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    SessionToken NVARCHAR(500) NOT NULL UNIQUE,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    StartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiresAt DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- UserLoginHistory table for tracking login attempts
CREATE TABLE UserLoginHistory (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    LoginProvider NVARCHAR(50) NULL,
    ProviderKey NVARCHAR(255) NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    LoginTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsSuccessful BIT NOT NULL,
    FailureReason NVARCHAR(255) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX IX_Users_UserName ON Users(UserName);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);

CREATE INDEX IX_Roles_Name ON Roles(Name);
CREATE INDEX IX_Roles_IsActive ON Roles(IsActive);
CREATE INDEX IX_Roles_CreatedAt ON Roles(CreatedAt);

CREATE INDEX IX_Permissions_Name ON Permissions(Name);
CREATE INDEX IX_Permissions_Category ON Permissions(Category);
CREATE INDEX IX_Permissions_IsActive ON Permissions(IsActive);

CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId);
CREATE INDEX IX_UserRoles_AssignedAt ON UserRoles(AssignedAt);

CREATE INDEX IX_RolePermissions_RoleId ON RolePermissions(RoleId);
CREATE INDEX IX_RolePermissions_PermissionId ON RolePermissions(PermissionId);
CREATE INDEX IX_RolePermissions_GrantedAt ON RolePermissions(GrantedAt);

CREATE INDEX IX_UserSessions_UserId ON UserSessions(UserId);
CREATE INDEX IX_UserSessions_SessionToken ON UserSessions(SessionToken);
CREATE INDEX IX_UserSessions_ExpiresAt ON UserSessions(ExpiresAt);
CREATE INDEX IX_UserSessions_IsActive ON UserSessions(IsActive);

CREATE INDEX IX_UserLoginHistory_UserId ON UserLoginHistory(UserId);
CREATE INDEX IX_UserLoginHistory_LoginTime ON UserLoginHistory(LoginTime);
CREATE INDEX IX_UserLoginHistory_IsSuccessful ON UserLoginHistory(IsSuccessful);

-- Insert default roles
INSERT INTO Roles (Name, Description, CreatedAt, UpdatedAt) VALUES
('Administrator', 'System administrator with full access', GETUTCDATE(), GETUTCDATE()),
('Manager', 'Store manager with limited administrative access', GETUTCDATE(), GETUTCDATE()),
('Employee', 'Regular employee with basic access', GETUTCDATE(), GETUTCDATE()),
('Customer', 'Customer account with limited access', GETUTCDATE(), GETUTCDATE());

-- Insert default permissions
INSERT INTO Permissions (Name, Description, Category, CreatedAt, UpdatedAt) VALUES
-- User Management
('user.create', 'Create new user accounts', 'User Management', GETUTCDATE(), GETUTCDATE()),
('user.read', 'View user information', 'User Management', GETUTCDATE(), GETUTCDATE()),
('user.update', 'Update user information', 'User Management', GETUTCDATE(), GETUTCDATE()),
('user.delete', 'Delete user accounts', 'User Management', GETUTCDATE(), GETUTCDATE()),
('user.manage.roles', 'Assign roles to users', 'User Management', GETUTCDATE(), GETUTCDATE()),

-- Role Management
('role.create', 'Create new roles', 'Role Management', GETUTCDATE(), GETUTCDATE()),
('role.read', 'View role information', 'Role Management', GETUTCDATE(), GETUTCDATE()),
('role.update', 'Update role information', 'Role Management', GETUTCDATE(), GETUTCDATE()),
('role.delete', 'Delete roles', 'Role Management', GETUTCDATE(), GETUTCDATE()),
('role.manage.permissions', 'Assign permissions to roles', 'Role Management', GETUTCDATE(), GETUTCDATE()),

-- Product Management
('product.create', 'Create new products', 'Product Management', GETUTCDATE(), GETUTCDATE()),
('product.read', 'View product information', 'Product Management', GETUTCDATE(), GETUTCDATE()),
('product.update', 'Update product information', 'Product Management', GETUTCDATE(), GETUTCDATE()),
('product.delete', 'Delete products', 'Product Management', GETUTCDATE(), GETUTCDATE()),
('product.manage.inventory', 'Manage product inventory', 'Product Management', GETUTCDATE(), GETUTCDATE()),

-- Inventory Management
('inventory.read', 'View inventory information', 'Inventory Management', GETUTCDATE(), GETUTCDATE()),
('inventory.update', 'Update inventory levels', 'Inventory Management', GETUTCDATE(), GETUTCDATE()),
('inventory.adjust', 'Adjust inventory levels', 'Inventory Management', GETUTCDATE(), GETUTCDATE()),

-- Sales Management
('sale.create', 'Create new sales', 'Sales Management', GETUTCDATE(), GETUTCDATE()),
('sale.read', 'View sales information', 'Sales Management', GETUTCDATE(), GETUTCDATE()),
('sale.update', 'Update sales information', 'Sales Management', GETUTCDATE(), GETUTCDATE()),
('sale.delete', 'Delete sales records', 'Sales Management', GETUTCDATE(), GETUTCDATE()),

-- Reporting
('report.view', 'View system reports', 'Reporting', GETUTCDATE(), GETUTCDATE()),
('report.export', 'Export system data', 'Reporting', GETUTCDATE(), GETUTCDATE()),

-- System Administration
('system.audit', 'View audit logs', 'System Administration', GETUTCDATE(), GETUTCDATE()),
('system.settings', 'Manage system settings', 'System Administration', GETUTCDATE(), GETUTCDATE()),
('system.health', 'View system health status', 'System Administration', GETUTCDATE(), GETUTCDATE());

-- Assign permissions to default roles
-- Administrator gets all permissions
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
SELECT r.Id, p.Id, GETUTCDATE()
FROM Roles r
CROSS JOIN Permissions p ON 1=1
WHERE r.Name = 'Administrator';

-- Manager gets most permissions (except system administration)
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
SELECT r.Id, p.Id, GETUTCDATE()
FROM Roles r
CROSS JOIN Permissions p ON 1=1
WHERE r.Name = 'Manager' AND p.Category NOT IN ('System Administration');

-- Employee gets basic permissions
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
SELECT r.Id, p.Id, GETUTCDATE()
FROM Roles r
CROSS JOIN Permissions p ON 1=1
WHERE r.Name = 'Employee' AND p.Category IN ('Product Management', 'Inventory Management', 'Sales Management', 'Reporting');

-- Customer gets minimal permissions
INSERT INTO RolePermissions (RoleId, PermissionId, GrantedAt)
SELECT r.Id, p.Id, GETUTCDATE()
FROM Roles r
CROSS JOIN Permissions p ON 1=1
WHERE r.Name = 'Customer' AND p.Name IN ('product.read', 'sale.read', 'report.view');
