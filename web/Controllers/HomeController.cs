using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Models;

namespace web.Controllers
{
    public class HomeController : Controller
    {
        
       [Authorize]
        public ActionResult Index(User user)
        {
             
            if (User.Identity.IsAuthenticated)
            {
               ViewBag.user = user;
            } 
            return View();

        }
        
           
    }
    
}