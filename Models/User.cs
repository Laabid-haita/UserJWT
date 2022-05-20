using System;
using System.Collections.Generic;

#nullable disable

namespace UserWebAPI.Models
{
    public partial class User
    {
        public int IdU { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Pseudo { get; set; }
        public string Role { get; set; }
    }
}
