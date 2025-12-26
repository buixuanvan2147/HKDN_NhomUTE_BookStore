using HKDN_GroupUTE_BookStore.Models;
using HKDN_GroupUTE_BookStore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CsharpBookShopContext _shopContext;

        public HomeController(ILogger<HomeController> logger, CsharpBookShopContext shopContext)
        {
            _logger = logger;
            _shopContext = shopContext;
        }

        // TRANG CHỦ
        public IActionResult Index_Home()
        {
            var viewModel = new Index_Home_ListSach
            {
                TatCaSach = _shopContext.Saches.ToList(),
                SachHot = _shopContext.Saches.Where(s => s.SoLuongTon > 50).ToList(),
                SachXuHuong = _shopContext.Saches.OrderByDescending(s => s.NgayTao).Take(10).ToList()
            };

            return View(viewModel);
        }

        // CHI TIẾT SÁCH
        public IActionResult ChiTietSach(string maSach)
        {
            if (string.IsNullOrEmpty(maSach))
                return NotFound();

            // 1. SỬA TRUY VẤN: Thêm .Include để lấy dữ liệu bảng TheLoai
            var sach = _shopContext.Saches
                .Include(s => s.MaTheLoaiNavigation) // Join sang bảng thể loại
                .FirstOrDefault(s => s.MaSach == maSach);

            if (sach == null)
                return NotFound();

            // 2. MAP DỮ LIỆU: Bổ sung SoLuongTon và sửa lỗi Decimal
            var viewModel = new ChiTietSachVM
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                TacGia = sach.TacGia,
                // Lấy tên thể loại an toàn
                TenTheLoai = sach.MaTheLoaiNavigation?.TenTheLoai ?? "Khác",

                // --- SỬA LỖI Ở ĐÂY ---
                // Thêm 'm' vào sau số 0 để ép kiểu thành decimal (0m)
                DonGia = sach.Gia,

                MoTa = sach.MoTa,
                Hinh = sach.UrlanhBia,

                // QUAN TRỌNG: Dòng này sửa lỗi luôn báo hết hàng
                SoLuongTon = sach.SoLuongTon ?? 0
            };

            // 3. LẤY SÁCH TƯƠNG TỰ (Cùng thể loại, trừ cuốn hiện tại)
            var sachTuongTu = _shopContext.Saches
                .Where(s => s.MaTheLoai == sach.MaTheLoai && s.MaSach != maSach)
                .OrderByDescending(s => s.SoLuongTon) // Ưu tiên sách còn hàng
                .Take(4) // Chỉ lấy 4 cuốn
                .Select(s => new ChiTietSachVM
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    Hinh = s.UrlanhBia,

                    // --- SỬA LỖI TƯƠNG TỰ Ở ĐÂY ---
                    DonGia = s.Gia
                }).ToList();

            // Truyền dữ liệu sách tương tự qua ViewBag
            ViewBag.SachTuongTu = sachTuongTu;

            // 4. BỔ SUNG: LOGIC TÍNH ĐIỂM ĐÁNH GIÁ (Phần bạn đang thiếu)
            var danhGias = _shopContext.Danhgia
                .Include(dg => dg.MaNguoiDungNavigation) // Include để hiển thị tên người đánh giá
                .Where(dg => dg.MaSach == maSach)
                .OrderByDescending(dg => dg.NgayTao)
                .ToList();

            ViewBag.DanhGia = danhGias;

            if (danhGias.Any())
            {
                ViewBag.DiemTrungBinh = danhGias.Average(dg => (double)dg.DiemDanhGia); // Tính trung bình cộng
                ViewBag.TongSoDanhGia = danhGias.Count; // Đếm tổng số lượng
            }
            else
            {
                ViewBag.DiemTrungBinh = 0.0;
                ViewBag.TongSoDanhGia = 0;
            }

            return View(viewModel);
        }

        // ĐĂNG NHẬP
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangNhap(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _shopContext.Nguoidungs
                .FirstOrDefault(u => u.SoDienThoai == model.SoDienThoai);

            if (user == null)
            {
                ModelState.AddModelError("", "Số điện thoại không tồn tại.");
                return View(model);
            }

            bool isValidPassword = false;

            // Kiểm tra nếu mật khẩu trong DB bắt đầu bằng $2 (định dạng BCrypt)
            if (user.MatKhau != null && user.MatKhau.StartsWith("$2"))
            {
                try
                {
                    isValidPassword = BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau);
                }
                catch
                {
                    isValidPassword = (user.MatKhau == model.MatKhau);
                }
            }
            else
            {
                // Mật khẩu cũ chưa băm
                isValidPassword = (user.MatKhau == model.MatKhau);

                // Tự động nâng cấp lên mã băm nếu đăng nhập đúng
                if (isValidPassword)
                {
                    user.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                    _shopContext.SaveChanges();
                }
            }

            if (!isValidPassword)
            {
                ModelState.AddModelError("", "Mật khẩu không đúng.");
                return View(model);
            }

            HttpContext.Session.SetString("UserMaNguoiDung", user.MaNguoiDung);
            HttpContext.Session.SetString("UserName", user.HoTen);

            return (user.VaiTro == "Admin")
                ? RedirectToAction("TrangChu_QuanTriVien", "QuanTriVien")
                : RedirectToAction("Index_Home");
        }

        // ĐĂNG XUẤT
        public IActionResult DangXuat()
        {
            var maND = HttpContext.Session.GetString("UserMaNguoiDung");
            if (!string.IsNullOrEmpty(maND))
                HttpContext.Session.Remove("GioHang_" + maND);

            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserMaNguoiDung");

            return RedirectToAction("DangNhap");
        }

        // ĐĂNG KÝ
        [HttpGet]
        public IActionResult DangKy() => View();

        [HttpPost]
        public IActionResult DangKy(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_shopContext.Nguoidungs.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            // Luôn băm mật khẩu khi đăng ký mới
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);

            var user = new Nguoidung
            {
                MaNguoiDung = GenerateMaNguoiDung(),
                HoTen = model.HoTen,
                MatKhau = passwordHash, // Lưu bản đã băm
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                VaiTro = "KhachHang",
                NgayTao = DateTime.Now
            };

            _shopContext.Nguoidungs.Add(user);
            _shopContext.SaveChanges();

            TempData["Success"] = "Đăng ký thành công!";
            return RedirectToAction("DangNhap");
        }

        private string GenerateMaNguoiDung()
        {
            var lastUser = _shopContext.Nguoidungs
                .OrderByDescending(u => u.MaNguoiDung)
                .FirstOrDefault();

            int nextId = 1;

            if (lastUser != null && lastUser.MaNguoiDung.StartsWith("ND"))
                int.TryParse(lastUser.MaNguoiDung.Substring(2), out nextId);

            return $"ND{nextId + 1:D3}";
        }
        // GET: Liên Hệ
        public IActionResult LienHe_Home()
        {
            return View();
        }

        // POST: Liên Hệ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LienHe_Home(LienHe model)
        {
            var maNguoiDung = HttpContext.Session.GetString("UserMaNguoiDung");

            // KIỂM TRA ĐĂNG NHẬP ĐỂ TỰ ĐIỀN THÔNG TIN
            if (!string.IsNullOrEmpty(maNguoiDung))
            {
                var user = _shopContext.Nguoidungs.FirstOrDefault(x => x.MaNguoiDung == maNguoiDung);
                if (user != null)
                {
                    model.MaNguoiDung = user.MaNguoiDung;
                    model.HoTen = user.HoTen;
                    model.Email = user.Email;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.HoTen) || string.IsNullOrWhiteSpace(model.Email))
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ Họ tên và Email.";
                    return View(model);
                }
            }

            try
            {
                model.TrangThai = "ChuaXuLy";
                model.NgayGui = DateTime.Now;

                _shopContext.LienHe.Add(model);
                _shopContext.SaveChanges();

                // Gán thông báo thành công
                TempData["Success"] = "Gửi liên hệ thành công! Chúng tôi sẽ phản hồi sớm.";

                return RedirectToAction("LienHe_Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
                return View(model);
            }
        }

        public ActionResult GioHang_Home()
        {
            return View("GioHang_Home");
        }

        public IActionResult TimKiem(string tuKhoa)
        {
            var vm = new Index_Home_ListSach();

            var list = string.IsNullOrEmpty(tuKhoa)
                ? _shopContext.Saches.ToList()
                : _shopContext.Saches.Where(s => s.TenSach.Contains(tuKhoa)).ToList();

            vm.TatCaSach = list;
            vm.SachHot = list.Where(s => s.SoLuongTon > 50).ToList();
            vm.SachXuHuong = list.OrderByDescending(s => s.NgayTao).Take(10).ToList();

            ViewBag.TuKhoa = tuKhoa;

            return View("Index_Home", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuiDanhGia(string maSach, int diem, string binhLuan)
        {
            var maND = HttpContext.Session.GetString("UserMaNguoiDung");
            if (string.IsNullOrEmpty(maND))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá." });
            }

            // 1. ĐIỀU KIỆN: Chỉ người đã mua và nhận hàng thành công mới được đánh giá
            var daMua = _shopContext.Donhangs
                .Include(dh => dh.Chitietdonhangs)
                .Any(dh => dh.MaNguoiDung == maND &&
                           dh.TrangThaiDonHang == "DaGiao" &&
                           dh.Chitietdonhangs.Any(ct => ct.MaSach == maSach));

            if (!daMua)
            {
                return Json(new { success = false, message = "Quyền đánh giá chỉ dành cho khách hàng đã mua sản phẩm này." });
            }

            // 2. ĐIỀU KIỆN: Giới hạn 24h giữa 2 lần đánh giá
            var danhGiaCu = _shopContext.Danhgia
                .Where(dg => dg.MaSach == maSach && dg.MaNguoiDung == maND)
                .OrderByDescending(dg => dg.NgayTao)
                .FirstOrDefault();

            if (danhGiaCu != null && danhGiaCu.NgayTao >= DateTime.Now.AddDays(-1))
            {
                return Json(new { success = false, message = "Bạn đã gửi đánh giá gần đây. Vui lòng quay lại sau 24 giờ." });
            }

            try
            {
                // 3. LƯU LỊCH SỬ: Luôn thêm mới để lưu vết
                var dg = new Danhgium
                {
                    MaSach = maSach,
                    MaNguoiDung = maND,
                    DiemDanhGia = diem,
                    BinhLuan = binhLuan,
                    NgayTao = DateTime.Now
                };
                _shopContext.Danhgia.Add(dg);
                _shopContext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Lỗi hệ thống khi lưu dữ liệu." });
            }
        }
    }
}
