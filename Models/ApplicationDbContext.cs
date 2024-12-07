using System;
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

    public virtual DbSet<FlashSaleTimeFrame> FlashSaleTimeFrames { get; set; }

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

    public virtual DbSet<RegisterFlashSaleProduct> RegisterFlashSaleProducts { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<ReturnRequest> ReturnRequests { get; set; }

    public virtual DbSet<ReturnRequestImg> ReturnRequestImgs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<TempImportsProduct> TempImportsProducts { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<UserDiscountVoucher> UserDiscountVouchers { get; set; }

    public virtual DbSet<UserFavoriteProduct> UserFavoriteProducts { get; set; }

    public virtual DbSet<UserOrder> UserOrders { get; set; }

    public virtual DbSet<UserOrderProduct> UserOrderProducts { get; set; }

    public virtual DbSet<UserOrderStatus> UserOrderStatuses { get; set; }

    public virtual DbSet<UserWallet> UserWallets { get; set; }

    public virtual DbSet<UserWalletTransaction> UserWalletTransactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bag>(entity =>
        {
            entity.HasKey(e => e.BagId).HasName("PK__bag__35AAA769A77D00F8");

            entity.ToTable("bag");

            entity.HasIndex(e => new { e.UserId, e.ProductSizeId }, "UQ__bag__29DC9EA856D336D8").IsUnique();

            entity.Property(e => e.BagId).HasColumnName("bag_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.IsSelected).HasColumnName("is_selected");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.Bags)
                .HasForeignKey(d => d.ProductSizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bag__product_siz__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.Bags)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bag__user_id__5629CD9C");
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
            entity.HasKey(e => e.DiscountVoucherId).HasName("PK__discount__63A429D125214A88");

            entity.ToTable("discount_voucher");

            entity.Property(e => e.DiscountVoucherId).HasColumnName("discount_voucher_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DiscountMaxValue).HasColumnName("discount_max_value");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue).HasColumnName("discount_value");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.MinOrderValue).HasColumnName("min_order_value");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.Usage).HasColumnName("usage");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("voucher_code");
            entity.Property(e => e.VoucherName)
                .HasMaxLength(255)
                .HasColumnName("voucher_name");
        });

        modelBuilder.Entity<FlashSale>(entity =>
        {
            entity.HasKey(e => e.FlashSaleId).HasName("PK__flash_sa__55B42396731CCB98");

            entity.ToTable("flash_sale");

            entity.Property(e => e.FlashSaleId).HasColumnName("flash_sale_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.FlashSaleName)
                .HasMaxLength(255)
                .HasDefaultValue("Event No Name")
                .HasColumnName("flash_sale_name");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("waiting")
                .HasColumnName("status");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("thumbnail");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<FlashSaleTimeFrame>(entity =>
        {
            entity.ToTable("flash_sale_time_frame");

            entity.Property(e => e.FlashSaleTimeFrameId).HasColumnName("flash_sale_time_frame_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.FlashSaleId).HasColumnName("flash_sale_id");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("waiting")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.FlashSale).WithMany(p => p.FlashSaleTimeFrames)
                .HasForeignKey(d => d.FlashSaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_flash_sale_time_frame_flash_sale");
        });

        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.HasKey(e => e.GoodsReceiptId).HasName("PK__goods_re__B37953175D3E39B5");

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
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

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
            entity.HasKey(e => e.Id).HasName("PK__history___3213E83F5022006F");

            entity.ToTable("history_search");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.TextSearch)
                .HasMaxLength(255)
                .HasColumnName("text_search");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.HistorySearches)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__history_s__user___5EBF139D");
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
            entity.Property(e => e.TotalStock).HasColumnName("total_stock");

            entity.HasOne(d => d.ProductParent).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductParentId)
                .HasConstraintName("fk_p_pp");
        });

        modelBuilder.Entity<ProductIcon>(entity =>
        {
            entity.HasKey(e => e.ProductIconsId).HasName("PK__product___0630B74151941119");

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
            entity.HasKey(e => e.ProductImgId).HasName("PK__product___C6E03397766BCC11");

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
            entity.HasKey(e => e.ProductObjectId).HasName("PK__product___B86915056067220F");

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
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IsNewRelease).HasColumnName("is_new_release");
            entity.Property(e => e.Length).HasColumnName("length");
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
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.ProductIcons).WithMany(p => p.ProductParents)
                .HasForeignKey(d => d.ProductIconsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PP_PI");

            entity.HasOne(d => d.SubCategories).WithMany(p => p.ProductParents)
                .HasForeignKey(d => d.SubCategoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pr_sc");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId).HasName("PK__product___8440EB03689CF31F");

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
            entity.Property(e => e.ProductSizeName)
                .HasMaxLength(255)
                .HasColumnName("product_size_name");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_pr_p");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_pr_u");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.ToTable("product_size", tb => tb.HasTrigger("trg_updated_total_stock"));

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

        modelBuilder.Entity<RegisterFlashSaleProduct>(entity =>
        {
            entity.HasKey(e => e.RegisterFlashSaleProduct1);

            entity.ToTable("register_flash_sale_product");

            entity.Property(e => e.RegisterFlashSaleProduct1).HasColumnName("register_flash_sale_product");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.FlashSalePrice).HasColumnName("flash_sale_price");
            entity.Property(e => e.FlashSaleTimeFrameId).HasColumnName("flash_sale_time_frame_id");
            entity.Property(e => e.OriginalPrice).HasColumnName("original_price");
            entity.Property(e => e.ProductParentId).HasColumnName("product_parent_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Sold).HasColumnName("sold");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.FlashSaleTimeFrame).WithMany(p => p.RegisterFlashSaleProducts)
                .HasForeignKey(d => d.FlashSaleTimeFrameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_register_flash_sale_product_flash_sale_time_frame");

            entity.HasOne(d => d.ProductParent).WithMany(p => p.RegisterFlashSaleProducts)
                .HasForeignKey(d => d.ProductParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flash_sale_product_parent");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId).HasName("PK__request___C24DD950F43292E4");

            entity.ToTable("request_types");

            entity.Property(e => e.RequestTypeId).HasColumnName("request_type_id");
            entity.Property(e => e.RequestTypeName)
                .HasMaxLength(255)
                .HasColumnName("request_type_name");
        });

        modelBuilder.Entity<ReturnRequest>(entity =>
        {
            entity.HasKey(e => e.ReturnRequestId).HasName("PK__return_r__C456CAE1806E7B14");

            entity.ToTable("return_request");

            entity.Property(e => e.ReturnRequestId).HasColumnName("return_request_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.RequestTypeId).HasColumnName("request_type_id");
            entity.Property(e => e.ResolverId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("resolver_id");
            entity.Property(e => e.ReturnRequestReason)
                .HasMaxLength(255)
                .HasColumnName("return_request_reason");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserOrderId).HasColumnName("user_order_id");

            entity.HasOne(d => d.RequestType).WithMany(p => p.ReturnRequests)
                .HasForeignKey(d => d.RequestTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rr_rt");

            entity.HasOne(d => d.UserOrder).WithMany(p => p.ReturnRequests)
                .HasForeignKey(d => d.UserOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rr_uo");
        });

        modelBuilder.Entity<ReturnRequestImg>(entity =>
        {
            entity.HasKey(e => e.ReturnRequestImgsId).HasName("PK__return_r__169F90B758E5BE51");

            entity.ToTable("return_request_imgs");

            entity.Property(e => e.ReturnRequestImgsId).HasColumnName("return_request_imgs_id");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(255)
                .HasColumnName("img_url");
            entity.Property(e => e.ReturnRequestId).HasColumnName("return_request_id");

            entity.HasOne(d => d.ReturnRequest).WithMany(p => p.ReturnRequestImgs)
                .HasForeignKey(d => d.ReturnRequestId)
                .HasConstraintName("fk_rr_rri");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CCD277F87B");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__size__0DCACE31D9890687");

            entity.ToTable("size");

            entity.Property(e => e.SizeId).HasColumnName("size_id");
            entity.Property(e => e.SizeName)
                .HasMaxLength(50)
                .HasColumnName("size_name");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoriesId).HasName("PK__sub_cate__F899CDB5A65606EB");

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
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__6EE594E8FAC15B7F");

            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(255)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<TempImportsProduct>(entity =>
        {
            entity.HasKey(e => e.TempId).HasName("PK__temp_imp__FEEC6BDBE3B5AB4D");

            entity.ToTable("temp_imports_product");

            entity.Property(e => e.TempId).HasColumnName("temp_id");
            entity.Property(e => e.ImportPrice)
                .HasColumnType("money")
                .HasColumnName("import_price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ReceiptId).HasColumnName("receipt_id");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([import_price]*[quantity])", false)
                .HasColumnType("money")
                .HasColumnName("total_price");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user_acc__B9BE370F690F0C05");

            entity.ToTable("user_account");

            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
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
            entity.Property(e => e.UserPhoneNumber)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("user_phone_number");
            entity.Property(e => e.UserUrl)
                .HasColumnType("text")
                .HasColumnName("user_url");
            entity.Property(e => e.UserUsername)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("user_username");

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_uc_role");
        });

        modelBuilder.Entity<UserDiscountVoucher>(entity =>
        {
            entity.HasKey(e => new { e.DiscountVoucherId, e.UserId });

            entity.ToTable("user_discount_voucher");

            entity.Property(e => e.DiscountVoucherId).HasColumnName("discount_voucher_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.TotalUsed).HasColumnName("total_used");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

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
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

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
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.DiscountPrice)
                .HasColumnType("money")
                .HasColumnName("discount_price");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FinalPrice)
                .HasColumnType("money")
                .HasColumnName("final_price");
            entity.Property(e => e.FirstName)
                .HasMaxLength(60)
                .HasColumnName("first_name");
            entity.Property(e => e.GhnService)
                .HasMaxLength(120)
                .HasColumnName("ghn_service");
            entity.Property(e => e.IsCanceledBy).HasColumnName("is_canceled_by");
            entity.Property(e => e.IsProcessed).HasColumnName("is_processed");
            entity.Property(e => e.IsReviewed).HasColumnName("is_reviewed");
            entity.Property(e => e.LastName)
                .HasMaxLength(60)
                .HasColumnName("last_name");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("order_code");
            entity.Property(e => e.OrderCodeReturn)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("order_code_return");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .HasColumnName("payment_method");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .HasColumnName("phone_number");
            entity.Property(e => e.ReturnExpirationDate)
                .HasColumnType("datetime")
                .HasColumnName("return_expiration_date");
            entity.Property(e => e.ShippingFee)
                .HasColumnType("money")
                .HasColumnName("shipping_fee");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("money")
                .HasColumnName("total_price");
            entity.Property(e => e.TotalQuantity).HasColumnName("total_quantity");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("transaction_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");
            entity.Property(e => e.UserOrderStatusId).HasColumnName("user_order_status_id");
            entity.Property(e => e.VouchersApplied)
                .HasMaxLength(120)
                .HasColumnName("vouchers_applied");

            entity.HasOne(d => d.User).WithMany(p => p.UserOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_uo_uc");

            entity.HasOne(d => d.UserOrderStatus).WithMany(p => p.UserOrders)
                .HasForeignKey(d => d.UserOrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_uo_uos");
        });

        modelBuilder.Entity<UserOrderProduct>(entity =>
        {
            entity.HasKey(e => new { e.UserOrderId, e.ProductSizeId }).HasName("PK_UOP");

            entity.ToTable("user_order_products");

            entity.Property(e => e.UserOrderId).HasColumnName("user_order_id");
            entity.Property(e => e.ProductSizeId).HasColumnName("product_size_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.OnRegisterFlashSalesId).HasColumnName("on_register_flash_sales_id");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.SizeName)
                .HasMaxLength(50)
                .HasColumnName("size_name");
            entity.Property(e => e.Thumbnail)
                .HasColumnType("text")
                .HasColumnName("thumbnail");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.UserOrderProducts)
                .HasForeignKey(d => d.ProductSizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_uop_ps");

            entity.HasOne(d => d.UserOrder).WithMany(p => p.UserOrderProducts)
                .HasForeignKey(d => d.UserOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_uop_uo");
        });

        modelBuilder.Entity<UserOrderStatus>(entity =>
        {
            entity.HasKey(e => e.UserOrderStatusId).HasName("PK__user_ord__3E79E7F18C016521");

            entity.ToTable("user_order_status");

            entity.Property(e => e.UserOrderStatusId).HasColumnName("user_order_status_id");
            entity.Property(e => e.UserOrderStatusName)
                .HasMaxLength(255)
                .HasColumnName("user_order_status_name");
        });

        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.UserWalletId).HasName("PK__user_wal__0612ED8D6B40F438");

            entity.ToTable("user_wallet");

            entity.Property(e => e.UserWalletId).HasColumnName("user_wallet_id");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0L)
                .HasColumnName("balance");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserWallets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_uw_uc");
        });

        modelBuilder.Entity<UserWalletTransaction>(entity =>
        {
            entity.HasKey(e => e.WalletTransactionId).HasName("PK__user_wal__1C2AF68166B9C2C8");

            entity.ToTable("user_wallet_transactions");

            entity.Property(e => e.WalletTransactionId).HasColumnName("wallet_transaction_id");
            entity.Property(e => e.Amount)
                .HasDefaultValue(0L)
                .HasColumnName("amount");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(255)
                .HasColumnName("transaction_type");
            entity.Property(e => e.UserWalletId).HasColumnName("user_wallet_id");

            entity.HasOne(d => d.UserWallet).WithMany(p => p.UserWalletTransactions)
                .HasForeignKey(d => d.UserWalletId)
                .HasConstraintName("fk_uwt_uw");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
