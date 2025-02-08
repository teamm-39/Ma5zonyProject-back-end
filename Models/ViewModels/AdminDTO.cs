using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AdminDTO
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress,ErrorMessage ="البريد الالكترونى غير صحيح")]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImgUrl { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Password,ErrorMessage ="كلمة المرور يجب ان تتكون من حرف وصغير وحرف كبير وطول كلمة المرور لا يقل عن 6 احرف")]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="كلمة المرور و تاكيد المرور غير متطابقين")]
        public string? ConfirmPassword { get; set; }
    }
}
