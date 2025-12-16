using HKDN_GroupUTE_BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class QTV_QLSPController : Controller
    {
        private readonly CsharpBookShopContext _shopContext;
        private readonly IWebHostEnvironment _webHostEnvironment; // Để lấy wwwroot path

        public QTV_QLSPController(CsharpBookShopContext shopContext, IWebHostEnvironment webHostEnvironment)
        {
            _shopContext = shopContext;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: QTV_QLSP/QTV_QLSP_ChiTietSach/S001
        public async Task<IActionResult> QTV_QLSP_ChiTietSach(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var sach = await _shopContext.Saches
                .Include(s => s.MaTheLoaiNavigation)
                .FirstOrDefaultAsync(s => s.MaSach == id);

            if (sach == null)
                return NotFound();

            return View(sach);
        }

        // GET: QTV_QLSP/QTV_QLSP_ThemSach
        public IActionResult QTV_QLSP_ThemSach()
        {
            ViewBag.TheLoaiList = new SelectList(_shopContext.Theloais, "MaTheLoai", "TenTheLoai");
            return View();
        }

        // POST: QTV_QLSP/QTV_QLSP_ThemSach
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QTV_QLSP_ThemSach(Sach model, IFormFile imageFile)
        {
            ModelState.Remove("MaSach");
            if (!ModelState.IsValid)
            {
                ViewBag.TheLoaiList = new SelectList(_shopContext.Theloais, "MaTheLoai", "TenTheLoai", model.MaTheLoai);
                return View(model);
            }

            try
            {
                // Tạo mã sách mới S001, S002,...
                string lastMaSach = await _shopContext.Saches
                    .OrderByDescending(s => s.MaSach)
                    .Select(s => s.MaSach)
                    .FirstOrDefaultAsync();

                int newNumber = 1;
                if (!string.IsNullOrEmpty(lastMaSach) && lastMaSach.StartsWith("S"))
                {
                    if (int.TryParse(lastMaSach.Substring(1), out int lastNumber))
                        newNumber = lastNumber + 1;
                }

                string newMaSach = "S" + newNumber.ToString("D3");
                while (await _shopContext.Saches.AnyAsync(s => s.MaSach == newMaSach))
                {
                    newNumber++;
                    newMaSach = "S" + newNumber.ToString("D3");
                }

                model.MaSach = newMaSach;
                model.NgayTao = DateTime.Now;

                // XỬ LÝ ẢNH - SỬA CHÍNH Ở ĐÂY
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Tên file duy nhất
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Đường dẫn đúng: wwwroot/images
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa có

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    model.UrlanhBia = fileName; // Lưu tên file vào DB
                }
                else
                {
                    model.UrlanhBia = "sach_default.jpg";
                }

                _shopContext.Saches.Add(model);
                await _shopContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm sách mới thành công!";
                return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm sách: " + ex.Message);
                if (ex.InnerException != null)
                    ModelState.AddModelError("", "Chi tiết: " + ex.InnerException.Message);

                ViewBag.TheLoaiList = new SelectList(_shopContext.Theloais, "MaTheLoai", "TenTheLoai", model.MaTheLoai);
                return View(model);
            }
        }

        // GET: QTV_QLSP/QTV_QLSP_SuaSach/S001
        public async Task<IActionResult> QTV_QLSP_SuaSach(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var sach = await _shopContext.Saches.FindAsync(id);
            if (sach == null)
                return NotFound();

            ViewBag.MaTheLoai = new SelectList(_shopContext.Theloais, "MaTheLoai", "TenTheLoai", sach.MaTheLoai);
            return View(sach);
        }

        // POST: QTV_QLSP/QTV_QLSP_SuaSach
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QTV_QLSP_SuaSach(Sach model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaTheLoai = new SelectList(_shopContext.Theloais, "MaTheLoai", "TenTheLoai", model.MaTheLoai);
                return View(model);
            }

            try
            {
                var sachEdit = await _shopContext.Saches.FirstOrDefaultAsync(s => s.MaSach == model.MaSach);
                if (sachEdit == null) return NotFound();

                // Cập nhật thông tin
                sachEdit.TenSach = model.TenSach;
                sachEdit.TacGia = model.TacGia;
                sachEdit.NhaXuatBan = model.NhaXuatBan;
                sachEdit.NamXuatBan = model.NamXuatBan;
                sachEdit.Gia = model.Gia;
                sachEdit.SoLuongTon = model.SoLuongTon;
                sachEdit.MaTheLoai = model.MaTheLoai;
                sachEdit.MoTa = model.MoTa;
                sachEdit.NgayTao = DateTime.Now;

                // Xử lý ảnh mới
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(sachEdit.UrlanhBia) && sachEdit.UrlanhBia != "sach_default.jpg")
                    {
                        string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", sachEdit.UrlanhBia);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Lưu ảnh mới
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string newPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(newPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    sachEdit.UrlanhBia = fileName;
                }

                await _shopContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật sách thành công!";
                return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi sửa sách: " + ex.Message);
                ViewBag.MaTheLoai = new SelectList(_shopContext.Theloais, "MaTheLoai", "TenTheLoai", model.MaTheLoai);
                return View(model);
            }
        }

        // GET: QTV_QLSP/QTV_QLSP_XoaSach/S001
        public async Task<IActionResult> QTV_QLSP_XoaSach(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");

            var sach = await _shopContext.Saches.FirstOrDefaultAsync(s => s.MaSach == id);
            if (sach == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sách.";
                return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");
            }

            try
            {
                // Xóa ảnh vật lý
                if (!string.IsNullOrEmpty(sach.UrlanhBia) && sach.UrlanhBia != "sach_default.jpg")
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", sach.UrlanhBia);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                _shopContext.Saches.Remove(sach);
                await _shopContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa sách thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa sách: " + ex.Message;
            }

            return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");
        }
    }
}