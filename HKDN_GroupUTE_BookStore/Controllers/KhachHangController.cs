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

        // -------------------- CHECK LOGIN --------------------
        [HttpPost]
        public JsonResult KiemTraDangNhap()
        {
            bool loggedIn = HttpContext.Session.GetString("UserName") != null;
            return Json(new { loggedIn });
        }

        // -------------------- THÊM / GIẢM / XOÁ GIỎ HÀNG --------------------
        [HttpPost]
        public JsonResult ThemVaoGioHang(string maSach, bool giamSoLuong = false, bool xoaSanPham = false)
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

                var item = gioHang.FirstOrDefault(x => x.MaSach == maSach);

                if (giamSoLuong && item != null)
                {
                    if (item.SoLuong > 1) item.SoLuong--;
                    else gioHang.Remove(item);

                    HttpContext.Session.SetObject(key, gioHang);
                    return Json(new { success = true, message = "Giảm số lượng!" });
                }

                if (item != null)
                {
                    item.SoLuong++;
                }
                else
                {
                    gioHang.Add(new CartItem
                    {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        Gia = sach.Gia,
                        SoLuong = 1,
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

        // -------------------- ĐẶT HÀNG --------------------
        [HttpPost]
        public JsonResult DatHang(string diaChiGiao)
        {
            try
            {
                var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");
                if (maNguoiDung == null)
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });

                string key = GetCartSessionKey();
                var gioHang = HttpContext.Session.GetObject<List<CartItem>>(key);

                if (gioHang == null || !gioHang.Any())
                    return Json(new { success = false, message = "Giỏ hàng trống!" });

                string maDon = "DH" + (_shopContext.Donhangs.Count() + 1).ToString("D3");

                var donHang = new Donhang
                {
                    MaDonHang = maDon,
                    MaNguoiDung = maNguoiDung,
                    TongTien = gioHang.Sum(x => x.Gia * x.SoLuong),
                    TrangThaiDonHang = "DangXuLy",
                    DiaChiGiao = diaChiGiao,
                    NgayTao = DateTime.Now
                };

                _shopContext.Donhangs.Add(donHang);

                foreach (var item in gioHang)
                {
                    var sach = _shopContext.Saches.First(s => s.MaSach == item.MaSach);

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

                _shopContext.SaveChanges();
                HttpContext.Session.Remove(key);

                return Json(new { success = true, message = "Đặt hàng thành công!" });
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
    }
}
