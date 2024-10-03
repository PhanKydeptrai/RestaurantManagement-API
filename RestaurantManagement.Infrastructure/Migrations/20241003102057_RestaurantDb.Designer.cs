﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantManagement.Infrastructure.Persistence;

#nullable disable

namespace RestaurantManagement.Infrastructure.Migrations
{
    [DbContext(typeof(RestaurantManagementDbContext))]
    [Migration("20241003102057_RestaurantDb")]
    partial class RestaurantDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Bill", b =>
                {
                    b.Property<string>("BillId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("BookId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("PaymentTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("VoucherId")
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("BillId");

                    b.HasIndex("BookId")
                        .IsUnique()
                        .HasFilter("[BookId] IS NOT NULL");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.HasIndex("PaymentTypeId");

                    b.HasIndex("VoucherId");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Booking", b =>
                {
                    b.Property<string>("BookId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<decimal>("BookingPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime");

                    b.HasKey("BookId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.BookingChangeLog", b =>
                {
                    b.Property<string>("BookingChangeLogId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("BookId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime");

                    b.Property<string>("LogMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("BookingChangeLogId");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("BookingChangeLogs");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.BookingDetail", b =>
                {
                    b.Property<string>("BookingDetailId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("BookId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("TableId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("BookingDetailId");

                    b.HasIndex("BookId");

                    b.HasIndex("TableId");

                    b.ToTable("BookingDetails");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Category", b =>
                {
                    b.Property<string>("CategoryId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<byte[]>("CategoryImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("CategoryStatus")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Customer", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("CustomerStatus")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("CustomerType")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("CustomerId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.CustomerVoucher", b =>
                {
                    b.Property<string>("CustomerVoucherId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("VoucherId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("CustomerVoucherId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("VoucherId");

                    b.ToTable("CustomerVouchers");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Employee", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("EmployeeStatus")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("EmployeeId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Meal", b =>
                {
                    b.Property<string>("MealId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("MealName")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("MealStatus")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SellStatus")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("MealId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Meals");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Notification", b =>
                {
                    b.Property<string>("NotificationId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Paragraph")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Order", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("OrderTime")
                        .HasColumnType("datetime");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("TableId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("TableId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.OrderChangeLog", b =>
                {
                    b.Property<string>("OrderChangeLogId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime");

                    b.Property<string>("LogMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("OrderChangeLogId");

                    b.HasIndex("OrderId");

                    b.HasIndex("UserId");

                    b.ToTable("OrderChangeLogs");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.OrderDetail", b =>
                {
                    b.Property<string>("OrderDetailId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("MealId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderDetailId");

                    b.HasIndex("MealId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.PaymentType", b =>
                {
                    b.Property<string>("PaymentTypeId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("PaymentTypeId");

                    b.ToTable("PaymentTypes");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.SystemLog", b =>
                {
                    b.Property<string>("SystemLogId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<DateTime>("LogDate")
                        .HasColumnType("datetime");

                    b.Property<string>("LogDetail")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("SystemLogId");

                    b.HasIndex("UserId");

                    b.ToTable("SystemLogs");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Table", b =>
                {
                    b.Property<string>("TableId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("TableStatus")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("TableTypeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(26)");

                    b.HasKey("TableId");

                    b.HasIndex("TableTypeId");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.TableType", b =>
                {
                    b.Property<string>("TableTypeId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("TableImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<decimal>("TablePrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("TableTypeId");

                    b.ToTable("TableTypes");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Gender")
                        .HasColumnType("varchar(10)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Phone")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<byte[]>("UserImage")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Voucher", b =>
                {
                    b.Property<string>("VoucherId")
                        .HasColumnType("nvarchar(26)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal>("MaxDiscount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("VoucherCondition")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("VoucherName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("VoucherId");

                    b.ToTable("Vouchers");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Bill", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Booking", "Booking")
                        .WithOne("Bill")
                        .HasForeignKey("RestaurantManagement.Domain.Entities.Bill", "BookId");

                    b.HasOne("RestaurantManagement.Domain.Entities.Order", "Order")
                        .WithOne("Bill")
                        .HasForeignKey("RestaurantManagement.Domain.Entities.Bill", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.PaymentType", "PaymentType")
                        .WithMany("Bills")
                        .HasForeignKey("PaymentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.Voucher", "Voucher")
                        .WithMany("Bills")
                        .HasForeignKey("VoucherId");

                    b.Navigation("Booking");

                    b.Navigation("Order");

                    b.Navigation("PaymentType");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Booking", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Customer", "Customer")
                        .WithMany("Bookings")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.BookingChangeLog", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Booking", "Booking")
                        .WithMany("BookingChangeLogs")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.User", "User")
                        .WithMany("BookingChangeLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.BookingDetail", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Booking", "Booking")
                        .WithMany("BookingDetails")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.Table", "Table")
                        .WithMany("BookingDetails")
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Table");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Customer", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.User", "User")
                        .WithOne("Customer")
                        .HasForeignKey("RestaurantManagement.Domain.Entities.Customer", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.CustomerVoucher", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Customer", "Customer")
                        .WithMany("CustomerVouchers")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.Voucher", "Voucher")
                        .WithMany("CustomerVouchers")
                        .HasForeignKey("VoucherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Employee", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.User", "User")
                        .WithOne("Employee")
                        .HasForeignKey("RestaurantManagement.Domain.Entities.Employee", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Meal", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Category", "Category")
                        .WithMany("Meals")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Notification", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Order", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId");

                    b.HasOne("RestaurantManagement.Domain.Entities.Table", "Table")
                        .WithMany("Orders")
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Table");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.OrderChangeLog", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Order", "Order")
                        .WithMany("OrderChangeLogs")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.User", "User")
                        .WithMany("OrderChangeLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.OrderDetail", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.Meal", "Meal")
                        .WithMany("OrderDetails")
                        .HasForeignKey("MealId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantManagement.Domain.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Meal");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.SystemLog", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.User", "User")
                        .WithMany("SystemLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Table", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Entities.TableType", "TableType")
                        .WithMany("Tables")
                        .HasForeignKey("TableTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TableType");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Booking", b =>
                {
                    b.Navigation("Bill");

                    b.Navigation("BookingChangeLogs");

                    b.Navigation("BookingDetails");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Category", b =>
                {
                    b.Navigation("Meals");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Customer", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("CustomerVouchers");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Meal", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Order", b =>
                {
                    b.Navigation("Bill");

                    b.Navigation("OrderChangeLogs");

                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.PaymentType", b =>
                {
                    b.Navigation("Bills");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Table", b =>
                {
                    b.Navigation("BookingDetails");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.TableType", b =>
                {
                    b.Navigation("Tables");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.User", b =>
                {
                    b.Navigation("BookingChangeLogs");

                    b.Navigation("Customer");

                    b.Navigation("Employee");

                    b.Navigation("Notifications");

                    b.Navigation("OrderChangeLogs");

                    b.Navigation("SystemLogs");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Entities.Voucher", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("CustomerVouchers");
                });
#pragma warning restore 612, 618
        }
    }
}
