using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    public class SubmitListModelController : Controller
    {
        //
        // GET: /SubmitListModel/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(PhieuNhap Model, IEnumerable<ChiTietPhieuNhap> ModelList)
        {
            return View();
        }
    }
}
