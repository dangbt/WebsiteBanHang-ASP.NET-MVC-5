using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WebsiteBanHang
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Khởi tạo application để lưu lượt truy cập vào website
            //PageView: Đếm số lượt truy cập của website
            //Online: Đếm số lượt người hiện đang online
            Application["SoNguoiTruyCap"] = 0;
            Application["SoNguoiDangOnline"] = 0;
        }
        // Một pageview được tính khi người dùng bắt đầu vào website của chúng ta
        //Khi đó phương thức Session_Start đã được chạy
        protected void Session_Start()
        {
            Application.Lock(); // Dùng để đồng bộ hóa
            Application["SoNguoiTruyCap"] = (int)Application["SoNguoiTruyCap"] + 1;
            Application["SoNguoiDangOnline"] = (int)Application["SoNguoiDangOnline"] + 1;
            Application.UnLock();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var TaiKhoanCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (TaiKhoanCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(TaiKhoanCookie.Value);
                var Quyen = authTicket.UserData.Split(new Char[] { ',' });
                var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), Quyen);
                Context.User = userPrincipal;
            }
        }
    }
}
