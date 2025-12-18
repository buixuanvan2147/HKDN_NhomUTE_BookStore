using HKDN_GroupUTE_BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace HKDN_GroupUTE_BookStore.Controllers
{
    public class QTV_LienHeController : Controller
    {
        private readonly CsharpBookShopContext _context;

        public QTV_LienHeController(CsharpBookShopContext context)
        {
            _context = context;
        }

        // 1. Trang danh sách liên hệ
        public IActionResult DanhSachLienHe()
        {
            var model = _context.LienHe
                .OrderBy(x => x.TrangThai) // 'ChuaXuLy' hiện lên trước 'DaXuLy'
                .ThenBy(x => x.MaNguoiDung) // Nhóm các tin nhắn của cùng 1 user lại gần nhau
                .ThenByDescending(x => x.NgayGui) // Tin nhắn mới nhất của user đó hiện lên đầu
                .ToList();

            return View(model);
        }

        // 2. Trang viết phản hồi
        public IActionResult PhanHoiLienHe(int id)
        {
            var item = _context.LienHe.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // 3. Xử lý lưu phản hồi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LuuPhanHoi(int MaLienHe, string PhanHoiAdmin)
        {
            var item = _context.LienHe.Find(MaLienHe);
            if (item != null)
            {
                item.PhanHoiAdmin = PhanHoiAdmin;
                item.TrangThai = "DaXuLy";
                item.NgayPhanHoi = DateTime.Now;

                _context.SaveChanges();
                TempData["AdminSuccess"] = "Đã gửi phản hồi thành công!";
            }
            // Chuyển hướng về đúng trang danh sách
            return RedirectToAction("DanhSachLienHe");
        }
    }
}