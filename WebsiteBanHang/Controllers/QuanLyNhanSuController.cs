using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    public class QuanLyNhanSuController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: QuanLyNhanSu
        public ActionResult Index()
        {
            var tv = db.ThanhViens;
            var lstQuyen = db.Quyens;
            var lstLoaiTV = db.LoaiThanhViens;
            ViewBag.lstQuyen = lstQuyen;
            ViewBag.lstLoaiTV = lstLoaiTV;
            return View(tv);
        }
        [HttpGet]
        public ActionResult PhanQuyen(int? id)
        {
            //Lấy mã loại tv dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            LoaiThanhVien ltv = db.LoaiThanhViens.SingleOrDefault(n => n.MaLoaiTV == id);
            if (ltv == null)
            {
                return HttpNotFound();
            }
            //Lấy ra danh sách quyền để load ra check box
            ViewBag.MaQuyen = db.Quyens;
            //Lấy ra danh sách quyền của loại thành viên đó
            //Bước 1: Lấy ra những quyền thuộc loại thành viên đó dựa vào bảng LoaiThanhVien_Quyen
            ViewBag.LoaiTVQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == id);
            return View(ltv);
        }
        [HttpPost]
        public ActionResult PhanQuyen(int? MaLTV, IEnumerable<LoaiThanhVien_Quyen> lstPhanQuyen)
        {

            //Trường hợp : Nếu đã đã tiến hành phân quyền rồi nhưng muốn phân quyền lại
            //Bước 1: Xóa những quyền cũa thuộc loại TV đó
            var lstDaPhanQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == MaLTV);
            if (lstDaPhanQuyen.Count() != 0)
            {
                //foreach (var item in lstDaPhanQuyen)
                //{
                //    db.LoaiThanhVien_Quyen.Remove(item);
                //    db.SaveChanges();
                //}
                db.LoaiThanhVien_Quyen.RemoveRange(lstDaPhanQuyen);
                db.SaveChanges();
            }
           
            if (lstPhanQuyen != null)
            {
                //Kiểm tra list danh sách quyền được check
                foreach (var item in lstPhanQuyen)
                {
                    item.MaLoaiTV = int.Parse(MaLTV.ToString());
                    //Nếu được check thì insert dữ liệu vào bảng phân quyền
                    db.LoaiThanhVien_Quyen.Add(item);


                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
       
        [HttpGet]
        public ActionResult ThemQuyen()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ThemQuyen(Quyen quyen)
        {
            if (ModelState.IsValid)
            {
                db.Quyens.Add(quyen);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
   
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (db != null)
                db.Dispose();
            db.Dispose();
        }
        base.Dispose(disposing);
    }

}


}