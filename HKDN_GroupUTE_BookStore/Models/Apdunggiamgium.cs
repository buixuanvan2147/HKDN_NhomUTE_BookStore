using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Apdunggiamgium
{
    public int MaApDung { get; set; }

    public string? MaDonHang { get; set; }

    public string? MaGiamGia { get; set; }

    public virtual Donhang? MaDonHangNavigation { get; set; }

    public virtual Magiamgium? MaGiamGiaNavigation { get; set; }
}
