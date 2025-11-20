using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BCCK_CSharp_BookShop.Models;
using BCCK_CSharp_BookShop.ViewModel;

namespace BCCK_CSharp_BookShop.Controllers
{
    public class QTV_QLDHController : Controller
    {
        private CSharp_BookShopEntities db = new CSharp_BookShopEntities();

        // GET: QTV_QLDH
        public ActionResult Index()
        {
            var donHangs = db.DonHangs.Include(d => d.NguoiDung);

            var viewModel = new QTV_DoanhThuViewModel
            {
                SoLuongDonHangDangXuLy = donHangs.Count(d => d.TrangThaiDonHang == "DangXuLy"),
                TongTienDangXuLy = donHangs.Where(d => d.TrangThaiDonHang == "DangXuLy").Sum(d => (decimal?)d.TongTien) ?? 0,

                SoLuongDonHangDaGiao = donHangs.Count(d => d.TrangThaiDonHang == "DaGiao"),
                TongTienDaGiao = donHangs.Where(d => d.TrangThaiDonHang == "DaGiao").Sum(d => (decimal?)d.TongTien) ?? 0,

                SoLuongDonHangDaHuy = donHangs.Count(d => d.TrangThaiDonHang == "DaHuy"),
                TongTienDaHuy = donHangs.Where(d => d.TrangThaiDonHang == "DaHuy").Sum(d => (decimal?)d.TongTien) ?? 0
            };

            ViewBag.DoanhThuThongKe = viewModel;

            return View(donHangs.ToList());
        }


        // GET: QTV_QLDH/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonHang donHang = db.DonHangs
                                .Include(d => d.NguoiDung) // Tải thông tin người dùng của đơn hàng
                                .Include(d => d.ChiTietDonHangs) // Tải danh sách chi tiết đơn hàng
                                .Include(d => d.ChiTietDonHangs.Select(ct => ct.Sach)) // Tải thông tin Sách cho mỗi chi tiết đơn hàng (để hiển thị tên sách)
                                .SingleOrDefault(d => d.MaDonHang == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            return View(donHang);
        }

        // GET: QTV_QLDH/Create
        public ActionResult Create()
        {
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "HoTen");
            return View();
        }

        // POST: QTV_QLDH/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDonHang,MaNguoiDung,TongTien,TrangThaiDonHang,DiaChiGiao,NgayTao")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.DonHangs.Add(donHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "HoTen", donHang.MaNguoiDung);
            return View(donHang);
        }

        // GET: QTV_QLDH/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "HoTen", donHang.MaNguoiDung);
            return View(donHang);
        }

        // POST: QTV_QLDH/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDonHang,MaNguoiDung,TongTien,TrangThaiDonHang,DiaChiGiao,NgayTao")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoiDung = new SelectList(db.NguoiDungs, "MaNguoiDung", "HoTen", donHang.MaNguoiDung);
            return View(donHang);
        }

        // GET: QTV_QLDH/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: QTV_QLDH/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DonHang donHang = db.DonHangs.Find(id);
            db.DonHangs.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("Index");
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
