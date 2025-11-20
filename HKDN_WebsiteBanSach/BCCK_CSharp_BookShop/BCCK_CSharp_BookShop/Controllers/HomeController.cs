using BCCK_CSharp_BookShop.Models;
using System.Linq;
using System.Web.Mvc;
using BCCK_CSharp_BookShop.ViewModel;

namespace BCCK_CSharp_BookShop.Controllers
{
    public class HomeController : Controller
    {
        CSharp_BookShopEntities db = new CSharp_BookShopEntities();

        // GET: Home
        public ActionResult Index_Home()
        {
            var allBooks = db.Saches.ToList();
            var hotBooks = db.Saches.Where(s => s.SoLuongTon > 50).ToList();
            var trendingBooks = db.Saches.OrderByDescending(s => s.NgayTao).Take(10).ToList();

            var viewModel = new Index_Home_ListSach
            {
                TatCaSach = allBooks,
                SachHot = hotBooks,
                SachXuHuong = trendingBooks
            };

            return View(viewModel);
        }

        // GET: ChiTietSach
        public ActionResult ChiTietSach(string maSach)
        {
            if (string.IsNullOrEmpty(maSach))
            {
                return HttpNotFound();
            }

            var sach = db.Saches.FirstOrDefault(s => s.MaSach == maSach);
            if (sach == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ChiTietSachVM
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                TacGia = sach.TacGia,
                TenTheLoai = sach.TheLoai?.TenTheLoai ?? "Không xác định",
                DonGia = sach.Gia,
                MoTa = sach.MoTa,
                Hinh = sach.URLAnhBia,
                
            };

            return View(viewModel);
        }

        // Các action khác (DangNhap, DangKy, DangXuat, v.v.) giữ nguyên
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.NguoiDungs.FirstOrDefault(u => u.SoDienThoai == model.SoDienThoai);

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

                Session["UserMaNguoiDung"] = user.MaNguoiDung;
                Session["UserName"] = user.HoTen;

                if (user.VaiTro == "Admin")
                {
                    return RedirectToAction("TrangChu_QuanTriVien", "QuanTriVien");
                }

                return RedirectToAction("Index_Home", "Home");
            }

            return View(model);
        }

        public ActionResult DangXuat()
        {
            string maNguoiDung = Session["UserMaNguoiDung"]?.ToString();
            if (!string.IsNullOrEmpty(maNguoiDung))
            {
                Session["GioHang_" + maNguoiDung] = null;
            }
            Session["UserMaNguoiDung"] = null;
            Session["UserName"] = null;
            TempData["Success"] = "Đăng xuất thành công. Chuyển hướng đến Đăng nhập...";
            return RedirectToAction("DangNhap", "Home");
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                if (db.NguoiDungs.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                string newMaNguoiDung = GenerateMaNguoiDung();

                var nguoiDung = new NguoiDung
                {
                    MaNguoiDung = newMaNguoiDung,
                    HoTen = model.HoTen,
                    MatKhau = model.MatKhau,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    VaiTro = "KhachHang",
                    NgayTao = System.DateTime.Now
                };

                db.NguoiDungs.Add(nguoiDung);
                db.SaveChanges();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap", "Home");
            }

            return View(model);
        }

        private string GenerateMaNguoiDung()
        {
            var lastUser = db.NguoiDungs
                .OrderByDescending(u => u.MaNguoiDung)
                .FirstOrDefault();

            int nextId = 1;
            if (lastUser != null && lastUser.MaNguoiDung.StartsWith("ND"))
            {
                if (int.TryParse(lastUser.MaNguoiDung.Substring(2), out int lastId))
                {
                    nextId = lastId + 1;
                }
            }

            return $"ND{nextId:D3}";
        }

        public ActionResult LienHe_Home()
        {
            return View();
        }

        public ActionResult GioHang_Home()
        {
            return View("GioHang_Home");
        }
        // GET: TimKiem
        public ActionResult TimKiem(string tuKhoa)
        {
            var viewModel = new Index_Home_ListSach();

            if (string.IsNullOrEmpty(tuKhoa))
            {
                viewModel.TatCaSach = db.Saches.ToList();
                viewModel.SachHot = db.Saches.Where(s => s.SoLuongTon > 50).ToList();
                viewModel.SachXuHuong = db.Saches.OrderByDescending(s => s.NgayTao).Take(10).ToList();
            }
            else
            {
                // Tìm kiếm sản phẩm dựa trên TenSach (không phân biệt hoa thường)
                var ketQuaTimKiem = db.Saches
                    .Where(s => s.TenSach.Contains(tuKhoa))
                    .ToList();

                viewModel.TatCaSach = ketQuaTimKiem;
                viewModel.SachHot = ketQuaTimKiem.Where(s => s.SoLuongTon > 50).ToList();
                viewModel.SachXuHuong = ketQuaTimKiem.OrderByDescending(s => s.NgayTao).Take(10).ToList();
            }

            ViewBag.TuKhoa = tuKhoa; // Truyền từ khóa để hiển thị trên view
            return View("Index_Home", viewModel);
        }
    }
}