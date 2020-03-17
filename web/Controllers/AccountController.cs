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
using System.Xml.Serialization;
using web.Models;


namespace web.Controllers
{
    public class AccountController : Controller//Функціїї авторизації та автентифікації користувачів
    {

        [AllowAnonymous]
        public ActionResult NotAuthorized()
        {
            return View();
        }

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
            if (User.IsInRole(model.Login))
            {
                FormsAuthentication.SetAuthCookie(model.Login, true);
                return RedirectToAction("Index", "Home");
            }
            else return View(model);
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

                                    var relativePath = "../data.xml";
                                    var absolutePath = HttpContext.Server.MapPath(relativePath);
                        
                        if (System.IO.File.Exists(absolutePath) == false)
                        {
                          string filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data.xml");
                            XmlDocument xdoc = new XmlDocument();
                            XmlDeclaration xmlDeclaration = xdoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                            XmlElement root = xdoc.DocumentElement;
                            xdoc.InsertBefore(xmlDeclaration, root);
                                XmlElement el1 = xdoc.CreateElement(string.Empty, "items", string.Empty);
                                xdoc.AppendChild(el1);
                                XmlElement el2 = xdoc.CreateElement(string.Empty, "item", string.Empty);
                                el1.AppendChild(el2);

                                XmlNode xlog = xdoc.SelectSingleNode("items/item");
                                xlog.InnerXml = "<![CDATA["+"{ 'login' :" + "'"+ model.Login+"'" +","
                                                        + "'name':" + "'"+ model.Email + "'" +","
                                                        + "'password':"+ "'" + model.Password+ "'" +"}]]> ";
                            xdoc.Save(filename);    
                        }
                        else
                        {
                            string filename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data.xml");

                            XmlDocument doc = new XmlDocument();
                            doc.Load(filename);
                            XmlNode elem = doc.CreateNode(XmlNodeType.Element, "", "item", "");
                            elem.InnerXml = "<![CDATA[" + "{ 'login' :" + "'" + model.Login + "'" + ","
                                                        + "'name':" + "'" + model.Email + "'" + ","
                                                        + "'password':" + "'" + model.Password + "'" + "}]]> ";
                            
                            doc.LastChild.AppendChild(elem);
                            doc.Save(filename);
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


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }


        public ActionResult signupAsJSONAsync(string name)
        {
            UserContext db = new UserContext();
            var user = db.Users.Where(a => a.Login.Contains(name) && a.Email.Contains(name)).ToList();
            return PartialView(user);
        }

    }
}

