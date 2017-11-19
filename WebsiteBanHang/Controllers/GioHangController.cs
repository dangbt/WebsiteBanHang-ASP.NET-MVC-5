using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    public class GioHangController : Controller
    {
        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        //Lấy giỏ hàng
        public List<ItemGioHang> LayGioHang()
        {
            //Giỏ hàng đã tồn tại 
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                //Nếu session giỏ hàng chưa tồn tại thì khởi tạo giỏ hàng
                lstGioHang = new List<ItemGioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Thêm giỏ hàng thông thường (Load lại trang)
        public ActionResult ThemGioHang(int MaSP, string strURL)
        {
            //Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //TRang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Trường hợp 1 nếu sản phẩm đã tồn tại trong giỏ hàng 
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                //Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                if (sp.SoLuongTon < spCheck.SoLuong)
                {
                    return View("ThongBao");
                }
                else
                {
                    spCheck.SoLuong++;
                    spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                    return Redirect(strURL);
                }
            }

            ItemGioHang itemGH = new ItemGioHang(MaSP);
            if (sp.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }

            lstGioHang.Add(itemGH);
            return Redirect(strURL);
        }
        //Tính tổng số lượng
        public double TinhTongSoLuong()
        {
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.SoLuong);
        }
        //Tính Tổng tiền 
        public decimal TinhTongTien()
        {
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.ThanhTien);
        }
        public ActionResult GioHangPartial()
        {
            if (TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }
        [HttpGet]
        public ActionResult XemGioHang()
        {
            //Lấy giỏ hàng 
            List<ItemGioHang> lstGioHang = LayGioHang();
          
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult XemGioHang(KhachHang kh, FormCollection f)
        {  // tạo một khách hàng
            KhachHang khachhang = new KhachHang();
            KhachHang makh = new KhachHang();
            // tạo một đơn đặt hàng        
            DonDatHang ddh = new DonDatHang();
            ThanhVien tv = new ThanhVien();
            //Lấy giỏ hàng 
            List<ItemGioHang> lstGioHang = LayGioHang();

            // khách đặt hàng không phải thành viên
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"] == "")
            {
                string HoTen = f["txtTenKhachHang"].ToString();
                string DiaChi = f["txtDiaChi"].ToString();
                string SoDienThoai = f["txtSoDienThoai"].ToString();
                string Email = f["txtEmail"].ToString();
                khachhang.TenKh = HoTen;
                khachhang.DiaChi = DiaChi;
                khachhang.SoDienThoai = SoDienThoai;
                khachhang.Email = Email;
                // thêm 1 khách hàng vào database
                db.KhachHangs.Add(khachhang);
              
               
               
            }
            // khách đặt hàng là thành viên
            else
            {
                tv = (ThanhVien)Session["TaiKhoan"];
                khachhang.TenKh = tv.HoTen;
                khachhang.DiaChi = tv.DiaChi;
                khachhang.SoDienThoai = tv.SoDienThoai;
                khachhang.Email = tv.Email;
                khachhang.MaThanhVien = tv.MaThanhVien;
                db.KhachHangs.Add(khachhang);
               
            }
            // phải save để lấy mã kh
            db.SaveChanges();
            // lấy Mã khách hàng để tạo đơn đặt hàng
            makh = LayMaKhachHang(khachhang);
        
        
            // lấy ngày giờ hiện tại
            DateTime ngaydat = DateTime.Now;
            DateTime tempNgayDat = ngaydat;
            // thêm 3 ngày từ khi đặt hàng đên khi giao hàng
            TimeSpan ngaythem = new System.TimeSpan(3, 0, 0, 0);
            DateTime ngaygiao = ngaydat.Add(ngaythem);
            ddh.MaKH = makh.MaKH;
            ddh.TinhTrangGiao = false;
            ddh.DaThanhToan = false;
            ddh.NgayDat = ngaydat;
            ddh.NgayGiao = ngaygiao;          
            // thêm 1 đơn hàng
            db.DonDatHangs.Add(ddh);
            // phải save để lấy mã DDH
            db.SaveChanges();
            // chi tiết đơn đặt hàng 
            DonDatHang  maDDH = LayMaDonDatHang(ddh);
            for (int i = 0; i < lstGioHang.Count(); i++)
            {
                // tạo một chi tiết đơn hàng
                ChiTietDonDatHang ctddh = new ChiTietDonDatHang();
                ctddh.MaDDH = maDDH.MaDDH;
                ctddh.MaSP = lstGioHang[i].MaSP;
                ctddh.TenSP = lstGioHang[i].TenSP;
                ctddh.SoLuong = lstGioHang[i].SoLuong;
                ctddh.DonGia = Convert.ToInt32(lstGioHang[i].DonGia);
                db.ChiTietDonDatHangs.Add(ctddh);

            }


             db.SaveChanges();
            ViewBag.ThongBao = "Đặt hàng thành công !";
            Session.Remove("GioHang") ;
            return View();
        }
        //Chỉnh sửa giỏ hàng
        [HttpGet]
        public ActionResult SuaGioHang(int MaSP)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa 
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //TRang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Kiểm tra xem sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Lấy list giỏ hàng tạo giao diện
            ViewBag.GioHang = lstGioHang;

            //Nếu tồn tại rồi
            return View(spCheck);
        }
        //Xử lý cập nhật giỏ hàng
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {
            //Kiểm tra số lượng tồn 
            SanPham spCheck = db.SanPhams.Single(n => n.MaSP == itemGH.MaSP);
            if (spCheck.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            //Cập nhật số lượng trong session giỏ hàng 
            //Bước 1: Lấy List<GioHang> từ session["GioHang"]
            List<ItemGioHang> lstGH = LayGioHang();
            //Bước 2: Lấy sản phẩm cần cập nhật từ trong list<GioHang> ra
            ItemGioHang itemGHUpdate = lstGH.Find(n => n.MaSP == itemGH.MaSP);
            //Bước 3: Tiến hành cập nhật lại số lượng cũng thành tiền
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            itemGHUpdate.ThanhTien = itemGHUpdate.SoLuong * itemGHUpdate.DonGia;
            return RedirectToAction("XemGioHang");
        }

        public ActionResult XoaGioHang(int MaSP)
        {
            //Kiểm tra session giỏ hàng tồn tại chưa 
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //TRang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Kiểm tra xem sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xóa item trong giỏ hàng
            lstGioHang.Remove(spCheck);
            return RedirectToAction("XemGioHang");
        }
        public KhachHang LayMaKhachHang(KhachHang kh)
        {
            KhachHang makh = db.KhachHangs.FirstOrDefault(n=>n.TenKh==kh.TenKh && n.SoDienThoai==kh.SoDienThoai && n.Email==kh.Email );
            return makh;
        }
        public DonDatHang LayMaDonDatHang(DonDatHang ddh)
        {
            DonDatHang dondathang= new DonDatHang();
            var maDDH = db.DonDatHangs.Where(n=>n.MaKH==ddh.MaKH);
            foreach(var item in maDDH)
            {
               
               
                if (DateTime.Compare(Convert.ToDateTime(item.NgayDat), Convert.ToDateTime(ddh.NgayDat))==0)
                {
                     dondathang = item;
                }
            }
            return dondathang;
        }
        //Thêm giỏ hàng Ajax
        public ActionResult ThemGioHangAjax(int MaSP, string strURL)
        {
            //Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                //TRang đường dẫn không hợp lệ
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            //Trường hợp 1 nếu sản phẩm đã tồn tại trong giỏ hàng 
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck != null)
            {
                //Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                if (sp.SoLuongTon < spCheck.SoLuong)
                {
                    return Content("<script> alert(\"Sản phẩm đã hết hàng!\")</script>");
                }
                spCheck.SoLuong++;
                spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;
                ViewBag.TongSoLuong = TinhTongSoLuong();
                ViewBag.TongTien = TinhTongTien();
                return PartialView("GioHangPartial");
            }

            ItemGioHang itemGH = new ItemGioHang(MaSP);
            if (sp.SoLuongTon < itemGH.SoLuong)
            {
                return Content("<script> alert(\"Sản phẩm đã hết hàng!\")</script>");
            }

            lstGioHang.Add(itemGH);
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView("GioHangPartial");
        }

    }
}