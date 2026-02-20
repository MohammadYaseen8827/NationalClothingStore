-- Apply inventory management schema migration
-- This script applies the 003_CreateInventoryManagementSchema.sql migration

\echo 'Applying migration 003_CreateInventoryManagementSchema.sql...'
psql -h localhost -U postgres -d NationalClothingStore_Dev -W password -f "src\Infrastructure\Data\Migrations\003_CreateInventoryManagementSchema.sql"

if [ $? -eq 0 ]; then
    echo 'Migration applied successfully!'
else
    echo 'Migration failed with exit code: $?'
    exit 1
fi
