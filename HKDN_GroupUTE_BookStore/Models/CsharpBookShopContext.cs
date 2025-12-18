using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class CsharpBookShopContext : DbContext
{
    public CsharpBookShopContext()
    {
    }

    public CsharpBookShopContext(DbContextOptions<CsharpBookShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LienHe> LienHe { get; set; }

    public virtual DbSet<Apdunggiamgium> Apdunggiamgia { get; set; }

    public virtual DbSet<Chitietdonhang> Chitietdonhangs { get; set; }

    public virtual DbSet<Danhgium> Danhgia { get; set; }

    public virtual DbSet<Danhsachyeuthich> Danhsachyeuthiches { get; set; }

    public virtual DbSet<Donhang> Donhangs { get; set; }

    public virtual DbSet<Magiamgium> Magiamgia { get; set; }

    public virtual DbSet<Nguoidung> Nguoidungs { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    public virtual DbSet<Theloai> Theloais { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<LienHe>(entity =>
        {
            entity.HasKey(e => e.MaLienHe);

            entity.Property(e => e.TrangThai)
                  .HasMaxLength(20)
                  .HasDefaultValue("ChuaXuLy");

            entity.Property(e => e.NgayGui)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.NguoiDung)
                  .WithMany()
                  .HasForeignKey(e => e.MaNguoiDung)
                  .OnDelete(DeleteBehavior.SetNull);
        });


        modelBuilder.Entity<Apdunggiamgium>(entity =>
        {
            entity.HasKey(e => e.MaApDung).HasName("PRIMARY");

            entity.ToTable("apdunggiamgia");

            entity.HasIndex(e => new { e.MaDonHang, e.MaGiamGia }, "MaDonHang").IsUnique();

            entity.HasIndex(e => e.MaGiamGia, "MaGiamGia");

            entity.Property(e => e.MaApDung).HasColumnType("int(11)");
            entity.Property(e => e.MaDonHang).HasMaxLength(10);
            entity.Property(e => e.MaGiamGia).HasMaxLength(10);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.Apdunggiamgia)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("apdunggiamgia_ibfk_1");

            entity.HasOne(d => d.MaGiamGiaNavigation).WithMany(p => p.Apdunggiamgia)
                .HasForeignKey(d => d.MaGiamGia)
                .HasConstraintName("apdunggiamgia_ibfk_2");
        });

        modelBuilder.Entity<Chitietdonhang>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PRIMARY");

            entity.ToTable("chitietdonhang");

            entity.HasIndex(e => e.MaDonHang, "MaDonHang");

            entity.HasIndex(e => e.MaSach, "MaSach");

            entity.Property(e => e.MaChiTiet).HasColumnType("int(11)");
            entity.Property(e => e.GiaBan).HasPrecision(10, 2);
            entity.Property(e => e.MaDonHang).HasMaxLength(10);
            entity.Property(e => e.MaSach).HasMaxLength(10);
            entity.Property(e => e.SoLuong).HasColumnType("int(11)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.Chitietdonhangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chitietdonhang_ibfk_1");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.Chitietdonhangs)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chitietdonhang_ibfk_2");
        });

        modelBuilder.Entity<Danhgium>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PRIMARY");

            entity.ToTable("danhgia");

            entity.HasIndex(e => e.MaNguoiDung, "MaNguoiDung");

            entity.HasIndex(e => new { e.MaSach, e.MaNguoiDung }, "MaSach").IsUnique();

            entity.Property(e => e.MaDanhGia).HasColumnType("int(11)");
            entity.Property(e => e.BinhLuan).HasColumnType("text");
            entity.Property(e => e.DiemDanhGia).HasColumnType("int(11)");
            entity.Property(e => e.MaNguoiDung).HasMaxLength(10);
            entity.Property(e => e.MaSach).HasMaxLength(10);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Danhgia)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("danhgia_ibfk_2");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.Danhgia)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("danhgia_ibfk_1");
        });

        modelBuilder.Entity<Danhsachyeuthich>(entity =>
        {
            entity.HasKey(e => e.MaYeuThich).HasName("PRIMARY");

            entity.ToTable("danhsachyeuthich");

            entity.HasIndex(e => new { e.MaNguoiDung, e.MaSach }, "MaNguoiDung").IsUnique();

            entity.HasIndex(e => e.MaSach, "MaSach");

            entity.Property(e => e.MaYeuThich).HasColumnType("int(11)");
            entity.Property(e => e.MaNguoiDung).HasMaxLength(10);
            entity.Property(e => e.MaSach).HasMaxLength(10);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Danhsachyeuthiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("danhsachyeuthich_ibfk_1");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.Danhsachyeuthiches)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("danhsachyeuthich_ibfk_2");
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PRIMARY");

            entity.ToTable("donhang");

            entity.HasIndex(e => e.MaNguoiDung, "MaNguoiDung");

            entity.Property(e => e.MaDonHang).HasMaxLength(10);
            entity.Property(e => e.DiaChiGiao).HasMaxLength(255);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(10);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasPrecision(10, 2);
            entity.Property(e => e.TrangThaiDonHang)
                .HasMaxLength(20)
                .HasDefaultValueSql("'DangXuLy'");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("donhang_ibfk_1");
        });

        modelBuilder.Entity<Magiamgium>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PRIMARY");

            entity.ToTable("magiamgia");

            entity.HasIndex(e => e.MaVoucher, "MaVoucher").IsUnique();

            entity.Property(e => e.MaGiamGia).HasMaxLength(10);
            entity.Property(e => e.MaVoucher).HasMaxLength(20);
            entity.Property(e => e.NgayHetHan).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.PhanTramGiam).HasPrecision(5, 2);
        });

        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PRIMARY");

            entity.ToTable("nguoidung");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.MaNguoiDung).HasMaxLength(10);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(128);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasDefaultValueSql("'KhachHang'");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PRIMARY");

            entity.ToTable("sach");

            entity.HasIndex(e => e.MaTheLoai, "MaTheLoai");

            entity.Property(e => e.MaSach).HasMaxLength(10);
            entity.Property(e => e.Gia).HasPrecision(10, 2);
            entity.Property(e => e.MaTheLoai).HasMaxLength(10);
            entity.Property(e => e.MoTa).HasColumnType("text");
            entity.Property(e => e.NamXuatBan).HasColumnType("int(11)");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.NhaXuatBan).HasMaxLength(100);
            entity.Property(e => e.SoLuongDaBan)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.SoLuongTon)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.TacGia).HasMaxLength(100);
            entity.Property(e => e.TenSach).HasMaxLength(200);
            entity.Property(e => e.UrlanhBia)
                .HasMaxLength(255)
                .HasColumnName("URLAnhBia");

            entity.HasOne(d => d.MaTheLoaiNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaTheLoai)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sach_ibfk_1");
        });

        modelBuilder.Entity<Theloai>(entity =>
        {
            entity.HasKey(e => e.MaTheLoai).HasName("PRIMARY");

            entity.ToTable("theloai");

            entity.HasIndex(e => e.TenTheLoai, "TenTheLoai").IsUnique();

            entity.Property(e => e.MaTheLoai).HasMaxLength(10);
            entity.Property(e => e.TenTheLoai).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
