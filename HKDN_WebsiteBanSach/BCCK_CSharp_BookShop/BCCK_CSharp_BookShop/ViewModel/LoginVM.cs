using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BCCK_CSharp_BookShop.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số.")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
    }
}
