# PowerShell script to set up PostgreSQL database for National Clothing Store

Write-Host "Setting up PostgreSQL database for National Clothing Store..." -ForegroundColor Green

# Check if psql is available
try {
    $psql_version = & psql --version
    Write-Host "Found PostgreSQL client: $psql_version" -ForegroundColor Green
} 
catch {
    Write-Host "Error: PostgreSQL client (psql) is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install PostgreSQL and ensure psql is available in your PATH" -ForegroundColor Red
    exit 1
}

# Define database parameters
$databaseName = "NationalClothingStore_Dev"
$username = "postgres"
$password = "password"
$host = "localhost"
$port = "5432"

Write-Host "`nConnecting to PostgreSQL server at $host`:$port..." -ForegroundColor Yellow

# Attempt to connect and create database
try {
    # Test connection first
    $testConn = "Host=$host;Port=$port;Username=$username;Password=$password;Database=postgres"
    $connString = "postgresql://$username`:$password@$host`:$port/postgres"
    
    # Create database if it doesn't exist
    $createDbSql = @"
SELECT 'CREATE DATABASE ""$databaseName""' 
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$databaseName');
"@
    
    Write-Host "`nChecking if database '$databaseName' exists..." -ForegroundColor Yellow
    
    # Execute SQL to create database if it doesn't exist
    $result = echo $createDbSql | & psql -h $host -p $port -U $username --set=ON_ERROR_STOP=1 -t -A 2>$null
    
    if ($result -match "CREATE DATABASE") {
        Write-Host "Creating database '$databaseName'..." -ForegroundColor Yellow
        & psql -h $host -p $port -U $username -c "CREATE DATABASE ""$databaseName"";" --set=ON_ERROR_STOP=1 2>$null
        Write-Host "Database '$databaseName' created successfully." -ForegroundColor Green
    } else {
        Write-Host "Database '$databaseName' already exists." -ForegroundColor Green
    }
    
    # Create/update user if needed (this might fail if user already exists, which is OK)
    Write-Host "`nEnsuring user '$username' has proper permissions..." -ForegroundColor Yellow
    & psql -h $host -p $port -U $username -c "ALTER USER $username PASSWORD '$password';" --set=ON_ERROR_STOP=1 2>$null
    & psql -h $host -p $port -U $username -c "GRANT ALL PRIVILEGES ON DATABASE ""$databaseName"" TO $username;" --set=ON_ERROR_STOP=1 2>$null
    
    Write-Host "`nDatabase setup completed successfully!" -ForegroundColor Green
    Write-Host "Database: $databaseName" -ForegroundColor Cyan
    Write-Host "User: $username" -ForegroundColor Cyan
    Write-Host "You can now run the application which should connect successfully." -ForegroundColor Green
    
} 
catch {
    Write-Host "Error setting up database: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure PostgreSQL is running and credentials are correct." -ForegroundColor Red
    exit 1
}