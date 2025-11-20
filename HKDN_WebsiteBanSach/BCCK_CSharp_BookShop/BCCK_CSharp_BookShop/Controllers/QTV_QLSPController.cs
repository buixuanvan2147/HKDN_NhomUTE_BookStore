using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BCCK_CSharp_BookShop.Models;

namespace BCCK_CSharp_BookShop.Controllers
{
    public class QTV_QLSPController : Controller
    {
        private CSharp_BookShopEntities db = new CSharp_BookShopEntities();

        // GET: QTV_QLSP/Details/5
        public ActionResult QTV_QLSP_ChiTietSach(string id)
        {
            // Lấy chi tiết sách theo mã sách
            var chiTietSach = db.Saches.FirstOrDefault(s => s.MaSach == id);

            if (chiTietSach != null)
            {
                // Lấy thể loại của sách
                chiTietSach.TheLoai = db.TheLoais
                    .FirstOrDefault(t => t.MaTheLoai == chiTietSach.MaTheLoai);
            }

            return View(chiTietSach);
        }

        // GET: QTV_QLSP/Create
        public ActionResult QTV_QLSP_ThemSach()
        {
            ViewBag.TheLoaiList = new SelectList(db.TheLoais, "MaTheLoai", "TenTheLoai");
            return View();
        }

        // POST: QTV_QLSP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QTV_QLSP_ThemSach(Sach model, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy mã sách cuối cùng trong cơ sở dữ liệu và tạo mã sách mới
                    string lastMaSach = db.Saches
                                           .OrderByDescending(s => s.MaSach)
                                           .FirstOrDefault()?.MaSach;

                    int newNumber = 1;
                    string newMaSach = "S001"; // Mặc định bắt đầu từ "S001"

                    if (!string.IsNullOrEmpty(lastMaSach))
                    {
                        string numberPart = lastMaSach.Substring(1); // Lấy phần số (bỏ "S")
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newNumber = lastNumber + 1; // Tăng số cuối lên
                        }
                    }

                    // Tạo mã sách mới
                    newMaSach = "S" + newNumber.ToString("D3");

                    // Kiểm tra và đảm bảo mã sách không bị trùng
                    while (db.Saches.Any(s => s.MaSach == newMaSach))
                    {
                        newNumber++;
                        newMaSach = "S" + newNumber.ToString("D3");
                    }

                    // Xử lý ảnh bìa
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName); // Đường dẫn lưu ảnh
                        imageFile.SaveAs(filePath); // Lưu ảnh vào thư mục trên server
                        model.URLAnhBia = fileName; // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                    }

                    // Gán mã sách mới và thêm vào cơ sở dữ liệu
                    model.MaSach = newMaSach;
                    model.NgayTao = DateTime.Now;
                    db.Saches.Add(model);
                    db.SaveChanges();

                    // Chuyển hướng về trang danh sách sách
                    //return RedirectToAction("SachList", "SellerDashboard");
                }
                catch (Exception ex)
                {
                    // Ghi nhận lỗi và hiển thị thông báo
                    Console.WriteLine("Error: " + ex.Message); // Log lỗi ra console hoặc log file
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            // Truyền lại danh sách thể loại để hiển thị dropdown
            ViewBag.TheLoaiList = new SelectList(db.TheLoais, "MaTheLoai", "TenTheLoai", model.MaTheLoai);
            return View(model);
        }

        // GET: QTV_QLSP/Edit/5
        public ActionResult QTV_QLSP_SuaSach(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = db.Saches.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTheLoai = new SelectList(db.TheLoais, "MaTheLoai", "TenTheLoai", sach.MaTheLoai);
            return View(sach);
        }

        // POST: QTV_QLSP/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QTV_QLSP_SuaSach(Sach model, HttpPostedFileBase imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sachedit = db.Saches.FirstOrDefault(s => s.MaSach == model.MaSach);
                    if (sachedit == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật thông tin sách
                    sachedit.TenSach = model.TenSach;
                    sachedit.TacGia = model.TacGia;
                    sachedit.NhaXuatBan = model.NhaXuatBan;
                    sachedit.NamXuatBan = model.NamXuatBan;
                    sachedit.Gia = model.Gia;
                    sachedit.SoLuongTon = model.SoLuongTon;
                    sachedit.MaTheLoai = model.MaTheLoai; // Cập nhật mã thể loại từ dropdown
                    sachedit.MoTa = model.MoTa;

                    // Cập nhật thời gian chỉnh sửa (NgayTao)
                    sachedit.NgayTao = DateTime.Now; // Cập nhật thời gian hiện tại vào NgayTao

                    // Xử lý upload ảnh bìa nếu có
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(sachedit.URLAnhBia))
                        {
                            var oldImagePath = Path.Combine(Server.MapPath("~/Content/images"), sachedit.URLAnhBia);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath); // Xóa ảnh cũ
                            }
                        }

                        // Lưu ảnh mới vào thư mục Images
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        imageFile.SaveAs(path);

                        // Cập nhật tên ảnh vào database
                        sachedit.URLAnhBia = fileName;
                    }

                    // Lưu vào cơ sở dữ liệu
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Chỉnh sửa thông tin sách thành công!";  // Lưu thông báo thành công
                    return RedirectToAction("QuanLySanPham_QuanTriVien","QuanTriVien");
                }
                else
                {
                    // Nếu có lỗi, trả về danh sách thể loại và tiếp tục hiển thị form
                    ViewBag.TheLoaiList = new SelectList(db.TheLoais, "MaTheLoai", "TenTheLoai", model.MaTheLoai);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View("Error");
            }
        }

        // GET: QTV_QLSP/Delete/5
        [HttpGet]
        public ActionResult QTV_QLSP_XoaSach(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");

            var sachDelete = db.Saches.FirstOrDefault(s => s.MaSach == id);
            if (sachDelete == null)
            {
                ViewBag.ErrorMessage = "Sách không tồn tại.";
                return View("Error");
            }

            try
            {
                db.Saches.Remove(sachDelete);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Đã có lỗi xảy ra khi xóa sách.";
                return View("Error");
            }

            return RedirectToAction("QuanLySanPham_QuanTriVien", "QuanTriVien");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
