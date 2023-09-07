using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PomaBrothers.Models;

namespace PomaBrothers.Data;

public partial class PomaBrothersDbContext : DbContext
{
    public PomaBrothersDbContext(DbContextOptions<PomaBrothersDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Config> Configs { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DeliveryDetail> DeliveryDetails { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemModel> Item_Model { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Categories");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.LastName).IsFixedLength();
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SecondLastName).IsFixedLength();
        });

        modelBuilder.Entity<DeliveryDetail>(entity =>
        {
            entity.HasOne(d => d.Invoice).WithMany(p => p.DeliveryDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryDetail_Invoice");

            entity.HasOne(d => d.Item).WithMany(p => p.DeliveryDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryDetail_Item");

            entity.HasOne(d => d.Supplier).WithMany(p => p.DeliveryDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryDetail_Supplier");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Ci).HasComment("Carnet de identidad");
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_Categories");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Sales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_Customer");

            entity.HasOne(d => d.Employee).WithMany(p => p.Sales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_Employee");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.SaleDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetails_Item");

            entity.HasOne(d => d.IdSaleNavigation).WithMany(p => p.SaleDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetails_Sale");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasOne(d => d.Item).WithMany(p => p.Sections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Section_Item");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Sections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Section_Warehouse");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Ci).HasComment("Carnet de identidad");
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
