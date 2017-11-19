using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
namespace WebsiteBanHang.Controllers
{
    [Authorize(Roles = "QLSP")]
    public class KhachHangController : Controller
    {//
     // GET: /KhachHang/
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        public ActionResult Index()
        {
            //Truy vấn dữ liệu thông qua câu lệnh
            //Đối lstKH sẽ lấy toàn bộ dữ liệu từ bản khách hàng
            //Cách 1: Lấy dữ liệu là 1 danh sách khách hàng
            //var lstKH = from KH in db.KhachHangs select KH;
            //Cách 2: Dùng phương thức hổ trợ sẵn
            var lstKH = db.KhachHangs;
            return View(lstKH);

        }
      
     
     
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                return HttpNotFound();
            }

            //Load dropdownlist nhà cung cấp và dropdownlist loại sp, mã nhà sản xuất
          
            return View(kh);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ChinhSua(KhachHang model)
        {

           
            //Nếu dữ liệu đầu vào chắn chắn ok 
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult XoaKhachHang(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }
        [HttpPost]
        public ActionResult XoaKhachHang(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang kh = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            db.KhachHangs.Remove(kh);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}