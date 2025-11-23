using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Donhang
{
    public string MaDonHang { get; set; } = null!;

    public string? MaNguoiDung { get; set; }

    public decimal TongTien { get; set; }

    public string? TrangThaiDonHang { get; set; }

    public string? DiaChiGiao { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Apdunggiamgium> Apdunggiamgia { get; set; } = new List<Apdunggiamgium>();

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    public virtual Nguoidung? MaNguoiDungNavigation { get; set; }
}
