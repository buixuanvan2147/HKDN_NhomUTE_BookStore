using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Sach
{
    public string MaSach { get; set; } = null!;

    public string TenSach { get; set; } = null!;

    public string TacGia { get; set; } = null!;

    public string? NhaXuatBan { get; set; }

    public int? NamXuatBan { get; set; }

    public string? MaTheLoai { get; set; }

    public decimal Gia { get; set; }

    public int? SoLuongTon { get; set; }

    public int? SoLuongDaBan { get; set; }

    public string? MoTa { get; set; }

    public string? UrlanhBia { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    public virtual ICollection<Danhsachyeuthich> Danhsachyeuthiches { get; set; } = new List<Danhsachyeuthich>();

    public virtual Theloai? MaTheLoaiNavigation { get; set; }
}
