﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace nike_website_backend.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bag> Bags { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<DiscountVoucher> DiscountVouchers { get; set; }

    public virtual DbSet<FlashSale> FlashSales { get; set; }

    public virtual DbSet<GoodsReceipt> GoodsReceipts { get; set; }

    public virtual DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }

    public virtual DbSet<HistorySearch> HistorySearches { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductIcon> ProductIcons { get; set; }

    public virtual DbSet<ProductImg> ProductImgs { get; set; }

    public virtual DbSet<ProductObject> ProductObjects { get; set; }

    public virtual DbSet<ProductParent> ProductParents { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserDiscountVoucher> UserDiscountVouchers { get; set; }

    public virtual DbSet<UserFavoriteProduct> UserFavoriteProducts { get; set; }

    public virtual DbSet<UserOrder> UserOrders { get; set; }

    public virtual DbSet<UserOrderProduct> UserOrderProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bag>(entity =>
        {
            entity.HasKey(e => e.BagId).HasName("PK__bag__35AAA7693D1CDA3C");

            entity.ToTable("bag");

            entity.HasIndex(e => new { e.UserId, e.ProductSizeId }, "UQ__bag__29DC9EA84924102B").IsUnique();

            entity.Property(e => e.BagId).HasColumnName("bag_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.Bags)
                .HasForeignKey(d => d.ProductSizeId)
                .HasConstraintName("FK__bag__product_siz__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.Bags)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__bag__user_id__6383C8BA");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoriesId);

            entity.ToTable("categories");

            entity.Property(e => e.CategoriesId).HasColumnName("categories_id");
            entity.Property(e => e.CategoriesName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("categories_name");
            entity.Property(e => e.ProductObjectId).HasColumnName("product_object_id");

            entity.HasOne(d => d.ProductObject).WithMany(p => p.Categories)
                .HasForeignKey(d => d.ProductObjectId)
                .HasConstraintName("fk_categories_object");
        });

        modelBuilder.Entity<DiscountVoucher>(entity =>
        {
            entity.HasKey(e => e.DiscountVoucherId).HasName("PK__discount__63A429D13D44B5BA");

            entity.ToTable("discount_voucher");

            entity.Property(e => e.DiscountVoucherId).HasColumnName("discount_voucher_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.VoucherName)
                .HasMaxLength(255)
                .HasColumnName("voucher_name");
        });

        modelBuilder.Entity<FlashSale>(entity =>
        {
            entity.HasKey(e => e.FlashSaleId).HasName("PK__flash_sa__55B42396A2BA18EC");

            entity.ToTable("flash_sale");

            entity.Property(e => e.FlashSaleId)
                .ValueGeneratedNever()
                .HasColumnName("flash_sale_id");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");

            entity.HasOne(d => d.Product).WithMany(p => p.FlashSales)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_flash_sale_product");
        });

        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.HasKey(e => e.GoodsReceiptId).HasName("PK__goods_re__B3795317E4F85609");

            entity.ToTable("goods_receipt");

            entity.Property(e => e.GoodsReceiptId).HasColumnName("goods_receipt_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsHandle).HasColumnName("is_handle");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Supplier).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_goods_receipt_supplier");

            entity.HasOne(d => d.User).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_goods_receipt_user_account");
        });

        modelBuilder.Entity<GoodsReceiptDetail>(entity =>
        {
            entity.HasKey(e => e.GoodsReceiptDetailsId);

            entity.ToTable("goods_receipt_details");

            entity.Property(e => e.GoodsReceiptDetailsId).HasColumnName("goods_receipt_details_id");
            entity.Property(e => e.GoodReceiptId).HasColumnName("good_receipt_id");
            entity.Property(e => e.ImportPrice)
                .HasColumnType("money")
                .HasColumnName("import_price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("money")
                .HasColumnName("total_price");

            entity.HasOne(d => d.GoodReceipt).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.GoodReceiptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_good_receipt_id");

            entity.HasOne(d => d.Product).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_id");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.GoodsReceiptDetails)
                .HasForeignKey(d => d.ProductSizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_size_id");
        });

        modelBuilder.Entity<HistorySearch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__history___3213E83F5EF404E5");

            entity.ToTable("history_search");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TextSearch)
                .HasMaxLength(255)
                .HasColumnName("text_search");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.HistorySearches)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__history_s__user___6B24EA82");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__product__47027DF5B4C8ED74");

            entity.ToTable("product");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductColorShown)
                .HasMaxLength(50)
                .HasColumnName("product_color_shown");
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(255)
                .HasColumnName("product_description");
            entity.Property(e => e.ProductDescription2)
                .HasMaxLength(255)
                .HasColumnName("product_description2");
            entity.Property(e => e.ProductImg)
                .HasMaxLength(255)
                .HasColumnName("product_img");
            entity.Property(e => e.ProductMoreInfo)
                .HasMaxLength(255)
                .HasColumnName("product_more_info");
            entity.Property(e => e.ProductParentId).HasColumnName("product_parent_id");
            entity.Property(e => e.ProductSizeAndFit)
                .HasMaxLength(255)
                .HasColumnName("product_size_and_fit");
            entity.Property(e => e.ProductStyleCode)
                .HasMaxLength(50)
                .HasColumnName("product_style_code");
            entity.Property(e => e.SalePrices)
                .HasDefaultValue(0m)
                .HasColumnType("money")
                .HasColumnName("sale_prices");
            entity.Property(e => e.Sold).HasColumnName("sold");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TotalStock).HasColumnName("total_stock");

            entity.HasOne(d => d.ProductParent).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductParentId)
                .HasConstraintName("fk_p_pp");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_product_supplier");
        });

        modelBuilder.Entity<ProductIcon>(entity =>
        {
            entity.HasKey(e => e.ProductIconsId).HasName("PK__product___0630B7414516EE84");

            entity.ToTable("product_icons");

            entity.Property(e => e.ProductIconsId).HasColumnName("product_icons_id");
            entity.Property(e => e.IconName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("icon_name");
            entity.Property(e => e.Thumbnail)
                .HasColumnType("text")
                .HasColumnName("thumbnail");
        });

        modelBuilder.Entity<ProductImg>(entity =>
        {
            entity.HasKey(e => e.ProductImgId).HasName("PK__product___C6E033978EF7A8FE");

            entity.ToTable("product_img");

            entity.Property(e => e.ProductImgId).HasColumnName("product_img_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductImgFileName)
                .HasMaxLength(255)
                .HasColumnName("product_img_file_name");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImgs)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_pi_p");
        });

        modelBuilder.Entity<ProductObject>(entity =>
        {
            entity.HasKey(e => e.ProductObjectId).HasName("PK__product___B8691505FCA9504D");

            entity.ToTable("product_object");

            entity.Property(e => e.ProductObjectId).HasColumnName("product_object_id");
            entity.Property(e => e.ProductObjectName)
                .HasMaxLength(100)
                .HasColumnName("product_object_name");
        });

        modelBuilder.Entity<ProductParent>(entity =>
        {
            entity.HasKey(e => e.ProductParentId).HasName("PK__product___0FF19A995365A227");

            entity.ToTable("product_parent");

            entity.Property(e => e.ProductParentId).HasColumnName("product_parent_id");
            entity.Property(e => e.IsNewRelease).HasColumnName("is_new_release");
            entity.Property(e => e.ProductIconsId).HasColumnName("product_icons_id");
            entity.Property(e => e.ProductParentName)
                .HasMaxLength(100)
                .HasColumnName("product_parent_name");
            entity.Property(e => e.ProductPrice)
                .HasColumnType("money")
                .HasColumnName("product_price");
            entity.Property(e => e.SubCategoriesId).HasColumnName("sub_categories_id");
            entity.Property(e => e.Thumbnail)
                .HasColumnType("text")
                .HasColumnName("thumbnail");

            entity.HasOne(d => d.ProductIcons).WithMany(p => p.ProductParents)
                .HasForeignKey(d => d.ProductIconsId)
                .HasConstraintName("FK_PP_PI");

            entity.HasOne(d => d.SubCategories).WithMany(p => p.ProductParents)
                .HasForeignKey(d => d.SubCategoriesId)
                .HasConstraintName("fk_pr_sc");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId).HasName("PK__product___8440EB03653B3977");

            entity.ToTable("product_review");

            entity.Property(e => e.ProductReviewId).HasColumnName("product_review_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductReviewContent)
                .HasMaxLength(255)
                .HasColumnName("product_review_content");
            entity.Property(e => e.ProductReviewRate).HasColumnName("product_review_rate");
            entity.Property(e => e.ProductReviewTime)
                .HasColumnType("datetime")
                .HasColumnName("product_review_time");
            entity.Property(e => e.ProductReviewTitle)
                .HasMaxLength(255)
                .HasColumnName("product_review_Title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_pr_p");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_pr_u");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.ToTable("product_size");

            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SizeId).HasColumnName("size_id");
            entity.Property(e => e.Soluong).HasColumnName("soluong");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ps_p");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ps_s");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__role__760965CC49A1EA35");

            entity.ToTable("role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__size__0DCACE3137949468");

            entity.ToTable("size");

            entity.Property(e => e.SizeId).HasColumnName("size_id");
            entity.Property(e => e.SizeName)
                .HasMaxLength(50)
                .HasColumnName("size_name");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoriesId).HasName("PK__sub_cate__F899CDB55C7C9BE9");

            entity.ToTable("sub_categories");

            entity.Property(e => e.SubCategoriesId).HasColumnName("sub_categories_id");
            entity.Property(e => e.CategoriesId).HasColumnName("categories_id");
            entity.Property(e => e.SubCategoriesName)
                .HasMaxLength(255)
                .HasColumnName("sub_categories_name");

            entity.HasOne(d => d.Categories).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoriesId)
                .HasConstraintName("fk_sc_c");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__6EE594E8871B2C9F");

            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(255)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user_acc__B9BE370F1B733B7D");

            entity.ToTable("user_account");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserAddress).HasColumnName("user_address");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_email");
            entity.Property(e => e.UserFirstName)
                .HasMaxLength(100)
                .HasColumnName("user_first_name");
            entity.Property(e => e.UserGender)
                .HasMaxLength(5)
                .HasColumnName("user_gender");
            entity.Property(e => e.UserLastName)
                .HasMaxLength(100)
                .HasColumnName("user_last_name");
            entity.Property(e => e.UserMemberTier).HasColumnName("user_member_tier");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_password");
            entity.Property(e => e.UserPhoneNumber)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("user_phone_number");
            entity.Property(e => e.UserPoint).HasColumnName("user_point");
            entity.Property(e => e.UserUrl)
                .HasColumnType("text")
                .HasColumnName("user_url");
            entity.Property(e => e.UserUsername)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_username");

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_ROLE");
        });

        modelBuilder.Entity<UserDiscountVoucher>(entity =>
        {
            entity.HasKey(e => new { e.DiscountVoucherId, e.UserId });

            entity.ToTable("user_discount_voucher");

            entity.Property(e => e.DiscountVoucherId).HasColumnName("discount_voucher_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.DiscountVoucher).WithMany(p => p.UserDiscountVouchers)
                .HasForeignKey(d => d.DiscountVoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_discount_voucher_discount_voucher");

            entity.HasOne(d => d.User).WithMany(p => p.UserDiscountVouchers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_discount_voucher_user_account");
        });

        modelBuilder.Entity<UserFavoriteProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_user_favorite_products_1");

            entity.ToTable("user_favorite_products");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "u_userID_ProductID").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.UserFavoriteProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PD");

            entity.HasOne(d => d.User).WithMany(p => p.UserFavoriteProducts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_US");
        });

        modelBuilder.Entity<UserOrder>(entity =>
        {
            entity.HasKey(e => e.UserOrderId).HasName("PK__user_ord__3E79E7F1DA606466");

            entity.ToTable("user_order");

            entity.Property(e => e.UserOrderId).HasColumnName("user_order_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(60)
                .HasColumnName("first_name");
            entity.Property(e => e.IsProcessed).HasColumnName("is_processed");
            entity.Property(e => e.LastName)
                .HasMaxLength(60)
                .HasColumnName("last_name");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .HasColumnName("payment_method");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phone_number");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("money")
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<UserOrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.UserOrderId, e.ProductSizeId }).HasName("PK_UOP");

            entity.ToTable("user_order_products");

            entity.Property(e => e.UserOrderId).HasColumnName("user_order_id");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.UserOrderProducts)
                .HasForeignKey(d => d.ProductSizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_uop_ps");

            entity.HasOne(d => d.UserOrder).WithMany(p => p.UserOrderProducts)
                .HasForeignKey(d => d.UserOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_uop_uo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
