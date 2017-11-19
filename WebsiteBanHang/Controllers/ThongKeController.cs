using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;
namespace WebsiteBanHang.Controllers
{
    public class ThongKeController : Controller
    {
        //
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        // GET: /ThongKe/
        public ActionResult ThongKePartial()
        {
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString(); //Lấy số lượng người truy cập từ application đã được tạo
            ViewBag.SoLuongNguoiOnLine = HttpContext.Application["SoNguoiDangOnline"].ToString(); //Lấy số lượng đang truy cập
            ViewBag.TongDoanhThu = ThongKeTongDoanhThu(); //Thống kê tổng doanh thu
            ViewBag.TongDDH = ThongKeDonHang();//Thống kê đơn hàng
            ViewBag.TongThanhVien = ThongKeThanhVien(); //Thống kê thành viên
            return PartialView();
        }
        public decimal ThongKeTongDoanhThu()
        {
            //Thống kê theo tất cả doanh thu từ khi website thành lập
            decimal TongDoanhThu = db.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value;
            return TongDoanhThu;
        }
        public double ThongKeDonHang()
        {
            //Đếm đơn đặt hàng  
            double slDDH = db.DonDatHangs.Count();
            return slDDH;

        }
        public double ThongKeThanhVien()
        {
            //Đếm đơn đặt hàng  
            double slTV = db.ThanhViens.Count();
            return slTV;

        }
        public decimal ThongKeTongDoanhThuThang(int Thang, int Nam)
        {
            //Thống kê theo tất cả doanh thu từ khi website thành lập
            //List ra những đơn hàng nào có tháng, năm tương ứng
            var lstDDH = db.DonDatHangs.Where(n => n.NgayDat.Value.Month == Thang && n.NgayDat.Value.Year == Nam);
            decimal TongTien = 0;
            //Duyệt chi tiết của đơn đặt hàng đó và lấy tổng tiền của tất cả các sản phẩm thuộc đơn hàng đó
            foreach (var item in lstDDH)
            {
                TongTien += decimal.Parse(item.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value.ToString());
            }
            return TongTien;
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