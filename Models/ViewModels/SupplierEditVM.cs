using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class SupplierEditVM
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsReliable { get; set; }
    }
}
