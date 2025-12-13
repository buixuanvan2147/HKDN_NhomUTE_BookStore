using System.Diagnostics;
using HKDN_GroupUTE_BookStore.ViewModel;
using HKDN_GroupUTE_BookStore.Models;
using Microsoft.AspNetCore.Mvc;

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

            var sach = _shopContext.Saches.FirstOrDefault(s => s.MaSach == maSach);
            if (sach == null)
                return NotFound();

            var viewModel = new ChiTietSachVM
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                TacGia = sach.TacGia,
                TenTheLoai = sach.MaTheLoaiNavigation?.TenTheLoai ?? "Không xác định",
                DonGia = sach.Gia,
                MoTa = sach.MoTa,
                Hinh = sach.UrlanhBia
            };

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
            if (!ModelState.IsValid)
                return View(model);

            var user = _shopContext.Nguoidungs
                .FirstOrDefault(u => u.SoDienThoai == model.SoDienThoai);

            if (user == null)
            {
                ModelState.AddModelError("", "Số điện thoại không tồn tại.");
                return View(model);
            }

            if (user.MatKhau != model.MatKhau)
            {
                ModelState.AddModelError("", "Mật khẩu không đúng.");
                return View(model);
            }

            HttpContext.Session.SetString("UserMaNguoiDung", user.MaNguoiDung);
            HttpContext.Session.SetString("UserName", user.HoTen);

            if (user.VaiTro == "Admin")
                return RedirectToAction("TrangChu_QuanTriVien", "QuanTriVien");

            return RedirectToAction("Index_Home");
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
            if (!ModelState.IsValid)
                return View(model);

            if (_shopContext.Nguoidungs.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            string maND = GenerateMaNguoiDung();

            var user = new Nguoidung
            {
                MaNguoiDung = maND,
                HoTen = model.HoTen,
                MatKhau = model.MatKhau,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                VaiTro = "KhachHang",
                NgayTao = DateTime.Now
            };

            _shopContext.Nguoidungs.Add(user);
            _shopContext.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
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
        public ActionResult LienHe_Home()
        {
            return View();
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
    }
}
