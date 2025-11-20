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
    public class QTV_QLSP_TheLoaiController : Controller
    {
        private CSharp_BookShopEntities db = new CSharp_BookShopEntities();

        // GET: QTV_QLSP_TheLoai
        public ActionResult Index()
        {
            var theloaiList = db.TheLoais
                .Select(tl => new QTV_QLSP_TheLoai_List
                {
                        MaTheLoai = tl.MaTheLoai,
                        TenTheLoai = tl.TenTheLoai,
                        SoLoaiSach = tl.Saches.Count(),// Số đầu sách khác nhau
                        TongSachTheoTheLoai = tl.Saches.Sum(s => (int?)s.SoLuongTon) ?? 0 // Tổng số sách (tính theo tồn kho)
                })
                .ToList();

            return View(theloaiList);
        }

        // GET: QTV_QLSP_TheLoai/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TheLoai theLoai = db.TheLoais.Find(id);
            if (theLoai == null)
            {
                return HttpNotFound();
            }
            return View(theLoai);
        }

        // GET: QTV_QLSP_TheLoai/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QTV_QLSP_TheLoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaTheLoai,TenTheLoai")] TheLoai theLoai)
        {
            if (ModelState.IsValid)
            {
                db.TheLoais.Add(theLoai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(theLoai);
        }

        // GET: QTV_QLSP_TheLoai/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TheLoai theLoai = db.TheLoais.Find(id);
            if (theLoai == null)
            {
                return HttpNotFound();
            }
            return View(theLoai);
        }

        // POST: QTV_QLSP_TheLoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaTheLoai,TenTheLoai")] TheLoai theLoai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(theLoai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(theLoai);
        }

        // GET: QTV_QLSP_TheLoai/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TheLoai theLoai = db.TheLoais.Find(id);
            if (theLoai == null)
            {
                return HttpNotFound();
            }
            return View(theLoai);
        }

        // POST: QTV_QLSP_TheLoai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TheLoai theLoai = db.TheLoais.Find(id);
            db.TheLoais.Remove(theLoai);
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
