using HKDN_GroupUTE_BookStore.Models;
using HKDN_GroupUTE_BookStore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HKDN_GroupUTE_BookStore.Extensions;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly CsharpBookShopContext _shopContext;

        public KhachHangController(CsharpBookShopContext context)
        {
            _shopContext = context;
        }

        // -------------------- MODEL CART --------------------
        public class CartItem
        {
            public string MaSach { get; set; }
            public string TenSach { get; set; }
            public decimal Gia { get; set; }
            public int SoLuong { get; set; }
            public string URLAnhBia { get; set; }
        }

        public class CouponSession
        {
            public string MaGiamGia { get; set; }   
            public string MaVoucher { get; set; }   
            public decimal PhanTramGiam { get; set; } 
        }

        // -------------------- MODEL USER VIEW --------------------
        public class UserViewModel
        {
            public string MaNguoiDung { get; set; }
            public string HoTen { get; set; }
            public string Email { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public List<DonHangInfo> DonHangs { get; set; }
        }

        public class DonHangInfo
        {
            public string MaDonHang { get; set; }
            public DateTime NgayTao { get; set; }
            public string TrangThaiDonHang { get; set; }
            public decimal TongTien { get; set; }
            public List<ChiTietDonHangInfo> ChiTiets { get; set; }
        }

        public class ChiTietDonHangInfo
        {
            public string MaSach { get; set; }
            public string TenSach { get; set; }
            public string URLAnhBia { get; set; }
            public decimal GiaBan { get; set; }
            public int SoLuong { get; set; }
        }

        // -------------------- SESSION KEY --------------------
        private string GetCartSessionKey()
        {
            var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
            return string.IsNullOrEmpty(maNguoiDung)
                ? "GioHang_Temp"
                : $"GioHang_{maNguoiDung}";
        }

        private string GetCouponSessionKey()
        {
            var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
            return string.IsNullOrEmpty(maNguoiDung)
                ? "Voucher_Temp"
                : $"Voucher_{maNguoiDung}";
        }


        // -------------------- CHECK LOGIN --------------------
        [HttpPost]
        public JsonResult KiemTraDangNhap()
        {
            bool loggedIn = HttpContext.Session.GetString("UserName") != null;
            return Json(new { loggedIn });
        }

        // -------------------- THÊM / GIẢM / XOÁ GIỎ HÀNG --------------------
        [HttpPost]
        public JsonResult ThemVaoGioHang(string maSach, int soLuong = 1, bool giamSoLuong = false, bool xoaSanPham = false)
        {
            try
            {
                string key = GetCartSessionKey();
                var gioHang = HttpContext.Session.GetObject<List<CartItem>>(key) ?? new List<CartItem>();

                if (xoaSanPham)
                {
                    gioHang.RemoveAll(x => x.MaSach == maSach);
                    HttpContext.Session.SetObject(key, gioHang);
                    return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
                }

                var sach = _shopContext.Saches.FirstOrDefault(s => s.MaSach == maSach);
                if (sach == null)
                    return Json(new { success = false, message = "Sách không tồn tại!" });

                // Kiểm tra tồn kho trước khi thêm
                if (sach.SoLuongTon < soLuong)
                    return Json(new { success = false, message = "Số lượng tồn kho không đủ!" });

                var item = gioHang.FirstOrDefault(x => x.MaSach == maSach);

                // Logic giảm số lượng (Thường dùng ở trang giỏ hàng)
                if (giamSoLuong && item != null)
                {
                    if (item.SoLuong > 1) item.SoLuong--;
                    else gioHang.Remove(item);

                    HttpContext.Session.SetObject(key, gioHang);
                    return Json(new { success = true, message = "Giảm số lượng!" });
                }

                // Logic thêm vào giỏ
                if (item != null)
                {
                    item.SoLuong += soLuong;
                }
                else
                {
                    gioHang.Add(new CartItem
                    {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        Gia = sach.Gia,
                        SoLuong = soLuong,
                        URLAnhBia = sach.UrlanhBia
                    });
                }

                HttpContext.Session.SetObject(key, gioHang);
                return Json(new { success = true, message = "Thêm giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ApDungMaGiamGia(string maVoucher)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maVoucher))
                    return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });

                maVoucher = maVoucher.Trim();

                var voucher = _shopContext.Magiamgia
                    .FirstOrDefault(v => v.MaVoucher == maVoucher);

                if (voucher == null)
                    return Json(new { success = false, message = "Mã giảm giá không tồn tại." });

                if (voucher.NgayHetHan < DateTime.Now)
                    return Json(new { success = false, message = "Mã giảm giá đã hết hạn." });

                if (voucher.PhanTramGiam == null || voucher.PhanTramGiam <= 0 || voucher.PhanTramGiam > 100)
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ." });

                // lấy giỏ để tính thử số tiền giảm
                string cartKey = GetCartSessionKey();
                var gioHang = HttpContext.Session.GetObject<List<CartItem>>(cartKey) ?? new List<CartItem>();
                if (!gioHang.Any())
                    return Json(new { success = false, message = "Giỏ hàng trống, không thể áp mã." });

                decimal subTotal = gioHang.Sum(x => x.Gia * x.SoLuong);
                decimal discount = Math.Round(subTotal * (voucher.PhanTramGiam.Value / 100m), 0);

                // lưu vào session
                string couponKey = GetCouponSessionKey();
                HttpContext.Session.SetObject(couponKey, new CouponSession
                {
                    MaGiamGia = voucher.MaGiamGia,
                    MaVoucher = voucher.MaVoucher,
                    PhanTramGiam = voucher.PhanTramGiam.Value
                });

                return Json(new
                {
                    success = true,
                    message = $"Áp dụng mã {voucher.MaVoucher} thành công (-{voucher.PhanTramGiam}% ).",
                    subTotal,
                    discount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // -------------------- ĐẶT HÀNG --------------------
        [HttpPost]
        public JsonResult DatHang(string diaChiGiao)
        {
            try
            {
                var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
                if (string.IsNullOrEmpty(maNguoiDung))
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });

                if (string.IsNullOrWhiteSpace(diaChiGiao))
                    return Json(new { success = false, message = "Vui lòng nhập địa chỉ giao hàng!" });

                string cartKey = GetCartSessionKey();
                var gioHang = HttpContext.Session.GetObject<List<CartItem>>(cartKey);

                if (gioHang == null || !gioHang.Any())
                    return Json(new { success = false, message = "Giỏ hàng trống!" });

                // 1) Tính tổng tiền trước giảm
                decimal subTotal = gioHang.Sum(x => x.Gia * x.SoLuong);

                // 2) Lấy voucher từ session (nếu có) và kiểm tra lại DB
                string couponKey = GetCouponSessionKey();
                var coupon = HttpContext.Session.GetObject<CouponSession>(couponKey);

                decimal discount = 0;
                string maGiamGiaHopLe = null;

                if (coupon != null)
                {
                    var v = _shopContext.Magiamgia
                        .FirstOrDefault(x => x.MaGiamGia == coupon.MaGiamGia);

                    if (v != null && v.NgayHetHan >= DateTime.Now && v.PhanTramGiam.HasValue)
                    {
                        discount = Math.Round(subTotal * (v.PhanTramGiam.Value / 100m), 0);
                        if (discount < 0) discount = 0;
                        if (discount > subTotal) discount = subTotal;

                        maGiamGiaHopLe = v.MaGiamGia;
                    }
                    else
                    {
                        // voucher hết hạn/không hợp lệ -> xoá khỏi session để tránh lỗi
                        HttpContext.Session.Remove(couponKey);
                    }
                }

                decimal tongSauGiam = subTotal - discount;
                if (tongSauGiam < 0) tongSauGiam = 0;

                // 3) Tạo mã đơn hàng
                string maDon = "DH" + (_shopContext.Donhangs.Count() + 1).ToString("D3");

                var donHang = new Donhang
                {
                    MaDonHang = maDon,
                    MaNguoiDung = maNguoiDung,
                    TongTien = tongSauGiam,              // ✅ đã trừ giảm giá
                    TrangThaiDonHang = "DangXuLy",
                    DiaChiGiao = diaChiGiao.Trim(),
                    NgayTao = DateTime.Now
                };

                _shopContext.Donhangs.Add(donHang);

                // 4) Tạo chi tiết đơn + trừ tồn kho
                foreach (var item in gioHang)
                {
                    var sach = _shopContext.Saches.FirstOrDefault(s => s.MaSach == item.MaSach);
                    if (sach == null)
                        return Json(new { success = false, message = $"Sách {item.MaSach} không tồn tại!" });

                    if (sach.SoLuongTon < item.SoLuong)
                        return Json(new { success = false, message = $"Không đủ hàng cho {item.TenSach}" });

                    sach.SoLuongTon -= item.SoLuong;
                    sach.SoLuongDaBan += item.SoLuong;

                    _shopContext.Chitietdonhangs.Add(new Chitietdonhang
                    {
                        MaDonHang = maDon,
                        MaSach = sach.MaSach,
                        SoLuong = item.SoLuong,
                        GiaBan = item.Gia
                    });
                }

                // 5) Nếu voucher hợp lệ -> lưu bảng ApDungGiamGia
                if (!string.IsNullOrEmpty(maGiamGiaHopLe))
                {
                    // chống trùng (phòng hờ)
                    bool existed = _shopContext.Apdunggiamgia
                        .Any(x => x.MaDonHang == maDon && x.MaGiamGia == maGiamGiaHopLe);

                    if (!existed)
                    {
                        _shopContext.Apdunggiamgia.Add(new Apdunggiamgium
                        {
                            MaDonHang = maDon,
                            MaGiamGia = maGiamGiaHopLe
                        });
                    }
                }

                _shopContext.SaveChanges();

                // 6) Clear session
                HttpContext.Session.Remove(cartKey);
                HttpContext.Session.Remove(couponKey);

                return Json(new
                {
                    success = true,
                    message = "Đặt hàng thành công!",
                    maDonHang = maDon,
                    subTotal,
                    discount,
                    tongTien = tongSauGiam
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // -------------------- TRANG THÔNG TIN USER --------------------
        public IActionResult User()
        {
            var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
            if (maNguoiDung == null) return RedirectToAction("DangNhap", "Home");

            var user = _shopContext.Nguoidungs.FirstOrDefault(n => n.MaNguoiDung == maNguoiDung);
            if (user == null) return NotFound();

            var donHangs = _shopContext.Donhangs
                .Where(d => d.MaNguoiDung == maNguoiDung)
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            var vm = new UserViewModel
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                Email = user.Email,
                SoDienThoai = user.SoDienThoai,
                DiaChi = user.DiaChi,
                DonHangs = donHangs.Select(d => new DonHangInfo
                {
                    MaDonHang = d.MaDonHang,
                    NgayTao = d.NgayTao.Value,
                    TrangThaiDonHang = d.TrangThaiDonHang,
                    TongTien = d.TongTien,
                    ChiTiets = _shopContext.Chitietdonhangs
                        .Where(ct => ct.MaDonHang == d.MaDonHang)
                        .Select(ct => new ChiTietDonHangInfo
                        {
                            MaSach = ct.MaSach,
                            TenSach = ct.MaSachNavigation.TenSach,
                            URLAnhBia = ct.MaSachNavigation.UrlanhBia,
                            GiaBan = ct.GiaBan,
                            SoLuong = ct.SoLuong
                        }).ToList()
                }).ToList()
            };

            return View("~/Views/Home/User.cshtml", vm);
        }

        // -------------------- CẬP NHẬT USER --------------------
        [HttpPost]
        public JsonResult CapNhatThongTin(string hoTen, string email, string soDienThoai, string diaChi)
        {
            var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
            if (maNguoiDung == null)
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            var user = _shopContext.Nguoidungs.First(n => n.MaNguoiDung == maNguoiDung);

            if (_shopContext.Nguoidungs.Any(n => n.Email == email && n.MaNguoiDung != maNguoiDung))
                return Json(new { success = false, message = "Email đã được dùng." });

            user.HoTen = hoTen;
            user.Email = email;
            user.SoDienThoai = soDienThoai;
            user.DiaChi = diaChi;

            _shopContext.SaveChanges();
            HttpContext.Session.SetString("UserName", hoTen);

            return Json(new { success = true, message = "Cập nhật thành công!" });
        }

        // -------------------- HỦY ĐƠN HÀNG --------------------
        [HttpPost]
        public JsonResult HuyDonHang(string maDonHang)
        {
            var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
            if (maNguoiDung == null)
                return Json(new { success = false, message = "Bạn chưa đăng nhập!" });

            var donHang = _shopContext.Donhangs
                .FirstOrDefault(d => d.MaDonHang == maDonHang && d.MaNguoiDung == maNguoiDung);

            if (donHang == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });

            if (donHang.TrangThaiDonHang != "DangXuLy")
                return Json(new { success = false, message = "Không thể hủy đơn đã xử lý!" });

            donHang.TrangThaiDonHang = "DaHuy";

            var details = _shopContext.Chitietdonhangs.Where(ct => ct.MaDonHang == maDonHang).ToList();
            foreach (var ct in details)
            {
                var sach = _shopContext.Saches.First(s => s.MaSach == ct.MaSach);
                sach.SoLuongTon += ct.SoLuong;
                sach.SoLuongDaBan -= ct.SoLuong;
            }

            _shopContext.SaveChanges();

            return Json(new { success = true, message = "Hủy đơn thành công!" });
        }

        public IActionResult ThongBao()
        {
            string uId = HttpContext.Session.GetString("UserMaNguoiDung");

            // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            if (string.IsNullOrEmpty(uId))
            {
                return RedirectToAction("DangNhap", "Home");
            }

            // Lấy toàn bộ danh sách, sắp xếp cái mới nhất lên trên
            var danhSachThongBao = _shopContext.LienHe
                .Where(lh => lh.MaNguoiDung == uId && lh.TrangThai == "DaXuLy")
                .OrderByDescending(lh => lh.NgayPhanHoi)
                .ToList();

            return View(danhSachThongBao);
        }
    }
}
