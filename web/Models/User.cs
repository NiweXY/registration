using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Login { get; set; }

        public string Email { get; set; }
      
        public string Password { get; set; }
    }
}