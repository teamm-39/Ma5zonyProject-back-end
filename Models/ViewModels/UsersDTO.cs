﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UsersDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImgUrl { get; set; }
        public string? Address { get; set; }
    }
}
