using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NationalClothingStore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesAndCustomerEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLine2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmailOptIn = table.Column<bool>(type: "boolean", nullable: false),
                    SmsOptIn = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OriginalTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    ChangeGiven = table.Column<decimal>(type: "numeric", nullable: false),
                    LoyaltyPointsEarned = table.Column<int>(type: "integer", nullable: false),
                    LoyaltyPointsRedeemed = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_SalesTransactions_OriginalTransactionId",
                        column: x => x.OriginalTransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLoyalty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoyaltyCardNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PointsBalance = table.Column<int>(type: "integer", nullable: false),
                    TotalPointsEarned = table.Column<int>(type: "integer", nullable: false),
                    PointsRedeemed = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PointsToNextTier = table.Column<int>(type: "integer", nullable: false),
                    TierDiscountPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpgradeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLoyalty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerLoyalty_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductVariationId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    SerialNumbers = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesTransactionItems_Inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesTransactionItems_ProductVariations_ProductVariationId",
                        column: x => x.ProductVariationId,
                        principalTable: "ProductVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SalesTransactionItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesTransactionItems_SalesTransactions_SalesTransactionId",
                        column: x => x.SalesTransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesTransactionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CardLastFour = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    CardType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    GiftCardNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AuthorizationCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesTransactionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesTransactionPayments_SalesTransactions_SalesTransactionId",
                        column: x => x.SalesTransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerLoyaltyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SalesTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_CustomerLoyalty_CustomerLoyaltyId",
                        column: x => x.CustomerLoyaltyId,
                        principalTable: "CustomerLoyalty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_SalesTransactions_SalesTransactionId",
                        column: x => x.SalesTransactionId,
                        principalTable: "SalesTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLoyalty_CustomerId",
                table: "CustomerLoyalty",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLoyalty_LoyaltyCardNumber",
                table: "CustomerLoyalty",
                column: "LoyaltyCardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_CustomerLoyaltyId",
                table: "LoyaltyTransactions",
                column: "CustomerLoyaltyId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_SalesTransactionId",
                table: "LoyaltyTransactions",
                column: "SalesTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactionItems_InventoryId",
                table: "SalesTransactionItems",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactionItems_ProductId",
                table: "SalesTransactionItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactionItems_ProductVariationId",
                table: "SalesTransactionItems",
                column: "ProductVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactionItems_SalesTransactionId",
                table: "SalesTransactionItems",
                column: "SalesTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactionPayments_SalesTransactionId",
                table: "SalesTransactionPayments",
                column: "SalesTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_BranchId",
                table: "SalesTransactions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_CompletedAt",
                table: "SalesTransactions",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_CreatedAt",
                table: "SalesTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_CustomerId",
                table: "SalesTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_OriginalTransactionId",
                table: "SalesTransactions",
                column: "OriginalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_TransactionNumber",
                table: "SalesTransactions",
                column: "TransactionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesTransactions_UserId",
                table: "SalesTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoyaltyTransactions");

            migrationBuilder.DropTable(
                name: "SalesTransactionItems");

            migrationBuilder.DropTable(
                name: "SalesTransactionPayments");

            migrationBuilder.DropTable(
                name: "CustomerLoyalty");

            migrationBuilder.DropTable(
                name: "SalesTransactions");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}