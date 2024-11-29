using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SportsWear.Models
{
    public partial class SportsWearContext : DbContext
    {
        public SportsWearContext()
        {
        }

        public SportsWearContext(DbContextOptions<SportsWearContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admin");

                entity.Property(e => e.AdminId).HasColumnName("adminId");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("fullName");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("role");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");

                entity.Property(e => e.CartId).HasColumnName("cartId");

                entity.Property(e => e.FkCustomerId).HasColumnName("fk_customerId");

                entity.Property(e => e.FkProductId).HasColumnName("fk_productId");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.FkCustomerId)
                    .HasConstraintName("FK_CustomerId_Cart");

                entity.HasOne(d => d.FkProduct)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.FkProductId)
                    .HasConstraintName("FK_ProductId_Cart");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(70)
                    .HasColumnName("categoryName");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.CustomerAddress)
                    .IsRequired()
                    .HasColumnName("customerAddress");

                entity.Property(e => e.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(60)
                    .HasColumnName("customerEmail");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("customerName");

                entity.Property(e => e.CustomerPassword)
                    .IsRequired()
                    .HasMaxLength(60)
                    .HasColumnName("customerPassword");

                entity.Property(e => e.CustomerPhone)
                    .IsRequired()
                    .HasMaxLength(24)
                    .HasColumnName("customerPhone");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.AddressDetail)
                    .IsRequired()
                    .HasColumnName("addressDetail");

                entity.Property(e => e.FkCustomerId).HasColumnName("fk_customerId");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("fullName");

                entity.Property(e => e.OrderDate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("orderDate");

                entity.Property(e => e.OrderStatus).HasColumnName("orderStatus");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(24)
                    .HasColumnName("phoneNumber");

                entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.FkCustomerId)
                    .HasConstraintName("FK_CustomerId_Orders");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_details");

                entity.Property(e => e.OrderDetailId).HasColumnName("orderDetail_Id");

                entity.Property(e => e.FkOrderId).HasColumnName("fk_orderId");

                entity.Property(e => e.FkProductId).HasColumnName("fk_productId");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.FkOrderId)
                    .HasConstraintName("FK_OrderId_Order_Details");

                entity.HasOne(d => d.FkProduct)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.FkProductId)
                    .HasConstraintName("FK_ProdutId_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.FkCategoryId).HasColumnName("fk_categoryId");

                entity.Property(e => e.ProductDesc)
                    .IsRequired()
                    .HasColumnName("productDesc");

                entity.Property(e => e.ProductGender)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("productGender");

                entity.Property(e => e.ProductImage)
                    .IsRequired()
                    .HasColumnName("productImage");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("productName");

                entity.Property(e => e.ProductPrice).HasColumnName("productPrice");

                entity.Property(e => e.ProductQty).HasColumnName("productQty");

                entity.Property(e => e.ProductSize)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("productSize");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.FkCategoryId)
                    .HasConstraintName("FK_CategoryId_Products");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
