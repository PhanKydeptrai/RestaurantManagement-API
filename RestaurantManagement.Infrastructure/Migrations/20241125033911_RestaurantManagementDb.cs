using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantManagementDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(200)", nullable: true),
                    CategoryStatus = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "TableTypes",
                columns: table => new
                {
                    TableTypeId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TableTypeName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    TableCapacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(250)", nullable: true),
                    TablePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableTypes", x => x.TableTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Password = table.Column<string>(type: "varchar(64)", nullable: true),
                    Phone = table.Column<string>(type: "varchar(10)", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(255)", nullable: true),
                    Gender = table.Column<string>(type: "varchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    VoucherId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    VoucherName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    VoucherCode = table.Column<string>(type: "varchar(50)", nullable: false),
                    VoucherType = table.Column<string>(type: "varchar(20)", nullable: false),
                    PercentageDiscount = table.Column<int>(type: "int", nullable: true),
                    MaximumDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VoucherConditions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.VoucherId);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    MealId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    MealName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(255)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    SellStatus = table.Column<string>(type: "varchar(50)", nullable: false),
                    MealStatus = table.Column<string>(type: "varchar(50)", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.MealId);
                    table.ForeignKey(
                        name: "FK_Meals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    TableId = table.Column<int>(type: "int", nullable: false),
                    TableTypeId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TableStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    ActiveStatus = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.TableId);
                    table.ForeignKey(
                        name: "FK_Tables_TableTypes_TableTypeId",
                        column: x => x.TableTypeId,
                        principalTable: "TableTypes",
                        principalColumn: "TableTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillLogs",
                columns: table => new
                {
                    BillLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillLogs", x => x.BillLogId);
                    table.ForeignKey(
                        name: "FK_BillLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingLogs",
                columns: table => new
                {
                    BookingLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingLogs", x => x.BookingLogId);
                    table.ForeignKey(
                        name: "FK_BookingLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryLogs",
                columns: table => new
                {
                    CategoryLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryLogs", x => x.CategoryLogId);
                    table.ForeignKey(
                        name: "FK_CategoryLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLogs",
                columns: table => new
                {
                    CustomerLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLogs", x => x.CustomerLogId);
                    table.ForeignKey(
                        name: "FK_CustomerLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    CustomerStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    CustomerType = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerificationTokens",
                columns: table => new
                {
                    EmailVerificationTokenId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    Temporary = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationTokens", x => x.EmailVerificationTokenId);
                    table.ForeignKey(
                        name: "FK_EmailVerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLogs",
                columns: table => new
                {
                    EmployeeLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLogs", x => x.EmployeeLogId);
                    table.ForeignKey(
                        name: "FK_EmployeeLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    EmployeeStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    Role = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealLogs",
                columns: table => new
                {
                    MealLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLogs", x => x.MealLogId);
                    table.ForeignKey(
                        name: "FK_MealLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    Paragraph = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLogs",
                columns: table => new
                {
                    OrderLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLogs", x => x.OrderLogId);
                    table.ForeignKey(
                        name: "FK_OrderLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableLogs",
                columns: table => new
                {
                    TableLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableLogs", x => x.TableLogId);
                    table.ForeignKey(
                        name: "FK_TableLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableTypeLogs",
                columns: table => new
                {
                    TableTypeLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableTypeLogs", x => x.TableTypeLogId);
                    table.ForeignKey(
                        name: "FK_TableTypeLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLog",
                columns: table => new
                {
                    UserLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLog", x => x.UserLogId);
                    table.ForeignKey(
                        name: "FK_UserLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherLogs",
                columns: table => new
                {
                    VoucherLogId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    LogDetails = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherLogs", x => x.VoucherLogId);
                    table.ForeignKey(
                        name: "FK_VoucherLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    BookingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BookingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    BookingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    BookingStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    NumberOfCustomers = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_Bookings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerVouchers",
                columns: table => new
                {
                    CustomerVoucherId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    VoucherId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerVouchers", x => x.CustomerVoucherId);
                    table.ForeignKey(
                        name: "FK_CustomerVouchers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerVouchers_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "VoucherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(26)", nullable: true),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_Orders_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    BookingDetailId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.BookingDetailId);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings_BookId",
                        column: x => x.BookId,
                        principalTable: "Bookings",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    BillId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(26)", nullable: true),
                    BookId = table.Column<string>(type: "nvarchar(26)", nullable: true),
                    VoucherId = table.Column<string>(type: "nvarchar(26)", nullable: true),
                    PaymentType = table.Column<string>(type: "varchar(20)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsVoucherUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.BillId);
                    table.ForeignKey(
                        name: "FK_Bills_Bookings_BookId",
                        column: x => x.BookId,
                        principalTable: "Bookings",
                        principalColumn: "BookId");
                    table.ForeignKey(
                        name: "FK_Bills_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_Bills_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "VoucherId");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    MealId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "MealId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    PayerName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    PayerEmail = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(26)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillLogs_UserId",
                table: "BillLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_BookId",
                table: "Bills",
                column: "BookId",
                unique: true,
                filter: "[BookId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_OrderId",
                table: "Bills",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_VoucherId",
                table: "Bills",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_BookId",
                table: "BookingDetails",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_TableId",
                table: "BookingDetails",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingLogs_UserId",
                table: "BookingLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerId",
                table: "Bookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLogs_UserId",
                table: "CategoryLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLogs_UserId",
                table: "CustomerLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerVouchers_CustomerId",
                table: "CustomerVouchers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerVouchers_VoucherId",
                table: "CustomerVouchers",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLogs_UserId",
                table: "EmployeeLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealLogs_UserId",
                table: "MealLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_CategoryId",
                table: "Meals",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MealId",
                table: "OrderDetails",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLogs_UserId",
                table: "OrderLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TableLogs_UserId",
                table: "TableLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableTypeId",
                table: "Tables",
                column: "TableTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TableTypeLogs_UserId",
                table: "TableTypeLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLog_UserId",
                table: "UserLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherLogs_UserId",
                table: "VoucherLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillLogs");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "BookingLogs");

            migrationBuilder.DropTable(
                name: "CategoryLogs");

            migrationBuilder.DropTable(
                name: "CustomerLogs");

            migrationBuilder.DropTable(
                name: "CustomerVouchers");

            migrationBuilder.DropTable(
                name: "EmailVerificationTokens");

            migrationBuilder.DropTable(
                name: "EmployeeLogs");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "MealLogs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderLogs");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "TableLogs");

            migrationBuilder.DropTable(
                name: "TableTypeLogs");

            migrationBuilder.DropTable(
                name: "UserLog");

            migrationBuilder.DropTable(
                name: "VoucherLogs");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TableTypes");
        }
    }
}
