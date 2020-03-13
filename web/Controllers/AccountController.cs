using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using web.Models;

namespace web.Controllers
{
    public class AccountController : Controller//Функціїї авторизації та автентифікації користувачів
    {

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {

            if (ModelState.IsValid)
            {

                User user = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);
                }
                if (user != null)
                {

                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Користувача з таким логіном та паролем не існує.");
                }
            }
            return View(model);

        }


        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                User email = null;


                using (UserContext db = new UserContext())
                            {
                                user = db.Users.FirstOrDefault(u => u.Login == model.Login);
                                email = db.Users.FirstOrDefault(u => u.Email == model.Email);
                }
                        if (user == null && email == null)
                        {
                            //новий користувач
                            using (UserContext db = new UserContext())
                            {
                                db.Users.Add(new User { Login = model.Login, Email = model.Email, Password = model.Password });
                                db.SaveChanges();

                                user = db.Users.Where(u => u.Login == model.Login && u.Email == model.Email).FirstOrDefault();

                                    var relativePath = "../DB/data.xml";
                                    var absolutePath = HttpContext.Server.MapPath(relativePath);

                        if (System.IO.File.Exists(absolutePath) == false)
                        {
                            using (XmlWriter xWr = XmlWriter.Create(Server.MapPath("../DB/data.xml")))
                            {
                                xWr.WriteStartDocument();
                                xWr.WriteStartElement("items");
                                xWr.WriteStartElement("item");
                                xWr.WriteStartElement("CDATA");

                                xWr.WriteElementString("login", model.Login);
                                xWr.WriteElementString("name", model.Email);
                                xWr.WriteElementString("password", model.Password);

                                xWr.WriteEndElement();          // CLOSE LIST.
                                xWr.WriteEndElement();          // CLOSE LIBRARY.

                                xWr.WriteEndDocument();         // END DOCUMENT.
                                xWr.Flush();
                                xWr.Close();
                            }
                        }
                        else
                        {
                            string filename = "D:../web/web/DB/data.xml";
                            XElement element = null;
                            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                            {
                                element = XElement.Load(stream);
                            }

                            element.Add(
                                new XElement("item",
                                new XElement("CData",
                                    new XElement("login", model.Login),
                                    new XElement("name", model.Email),
                                    new XElement("password", model.Password))));
                            element.Save(filename);
                        }
                            }//якщо користувач успішно доданий
                            if (user != null)
                            {
                                 
                                FormsAuthentication.SetAuthCookie(model.Login, true);
                                 return RedirectToAction("Index", "Home");
                            }
                   
                        }
                        else
                        {
                         ModelState.AddModelError("", "Користувач з таким аккаунтом вже існує");
                        } 
                        //return Json(user, JsonRequestBehavior.AllowGet);              
                        
            } return View(model);
        }
               
    

        public ActionResult signupAsJSONAsync(string name)
        {
            UserContext db = new UserContext();
            var user = db.Users.Where(a => a.Login.Contains(name) && a.Email.Contains(name)).ToList();
            return PartialView(user);
        }

    }
}