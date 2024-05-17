﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FastFood.Models
{
    public partial class FastFoodContext : DbContext
    {
        public FastFoodContext()
        {
        }

        public FastFoodContext(DbContextOptions<FastFoodContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ingredient_Product");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.StockID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ingredient_Stock");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderDeliveredDateTime).HasColumnType("datetime");

                entity.Property(e => e.OrderReceivedDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stock");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleID").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRole_Role"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserID").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRole_User"),
                        j =>
                        {
                            j.HasKey("UserID", "RoleID");

                            j.ToTable("UserRole");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}