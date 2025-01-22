using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class StoreVM
    {
        [Required]
        [MinLength(3, ErrorMessage = "الاسم يجب أن يحتوي على 3 أحرف على الأقل.")]
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
