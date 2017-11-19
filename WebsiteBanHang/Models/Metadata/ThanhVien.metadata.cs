using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace WebsiteBanHang.Models
{
	[MetadataTypeAttribute(typeof(ThanhVienMetadata))]

    public partial class ThanhVien
    {
        internal sealed class ThanhVienMetadata
        {
            public int MaThanhVien { get; set; }
            [DisplayName("Tài khoản ")]
            [Required(ErrorMessage="{0} không được bỏ trống!")]
            public string TaiKhoan { get; set; }
            [DisplayName("Mật khẩu")]
            [Required(ErrorMessage = "{0} không được bỏ trống!")]
            public string MatKhau { get; set; }
            [DisplayName("Họ tên")]
            [Required(ErrorMessage = "{0} không được bỏ trống!")]
            public string HoTen { get; set; }
            [DisplayName("Địa chỉ")]
            [Required(ErrorMessage = "{0} không được bỏ trống!")]
            public string DiaChi { get; set; }
            [DisplayName("Email")]
            [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Email khônghợp lệ!")]
            public string Email { get; set; }
            [DisplayName("Số điện thoại")]
            [StringLength(11,ErrorMessage="{0} không quá 11 số")]
            public string SoDienThoai { get; set; }
            [DisplayName("Câu hỏi bí mật")]
            public string CauHoi { get; set; }
            [DisplayName("Câu trả lời")]
            [Required(ErrorMessage = "{0} không được bỏ trống!")]
            public string CauTraLoi { get; set; }
            public Nullable<int> MaLoaiTV { get; set; }

           
        }


    }
}