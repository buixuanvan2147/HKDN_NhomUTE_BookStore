using System;
using System.Collections.Generic;

namespace HKDN_GroupUTE_BookStore.Models;

public partial class Magiamgium
{
    public string MaGiamGia { get; set; } = null!;

    public string MaVoucher { get; set; } = null!;

    public decimal? PhanTramGiam { get; set; }

    public DateTime NgayHetHan { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<Apdunggiamgium> Apdunggiamgia { get; set; } = new List<Apdunggiamgium>();
}
