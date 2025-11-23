using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Chitietdonhang
{
    public int MaChiTiet { get; set; }

    public string? MaDonHang { get; set; }

    public string? MaSach { get; set; }

    public int SoLuong { get; set; }

    public decimal GiaBan { get; set; }

    public virtual Donhang? MaDonHangNavigation { get; set; }

    public virtual Sach? MaSachNavigation { get; set; }
}
