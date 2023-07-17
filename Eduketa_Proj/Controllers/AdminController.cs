using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Eduketa_Proj.Models;

namespace Eduketa_Proj.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        eduketaEntities1 ed = new eduketaEntities1();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddCourse()
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddCourse(CourseModel course)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                string mypath = "~/img/Course";
                string Filename = Path.GetFileName(course.image.FileName);
                Filename = rd.Next() + Filename;
                string path = Path.Combine(Server.MapPath(mypath), Filename);
                
                course.image.SaveAs(path);


                Course c = new Course()
                {
                    name = course.name,
                    price = course.price,
                    image = mypath + "/" + Filename,
                    description = course.description,
                    Seller = Session["Admin_Company_Name"].ToString(), 
                    SellerId= (int)Session["Adminid"]
                };
                ed.Courses.Add(c);
                ed.SaveChanges();
            }
            return RedirectToAction("/ShowData");
        }
        public ActionResult ShowData()
        {           
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            string _Company = Session["Admin_Company_Name"].ToString();
            int adminid = (int)Session["Adminid"];
            var data = ed.Courses.Where(x => x.Seller == _Company&&x.SellerId==adminid).ToList();

            return View(data);
        }

        public ActionResult Edit(int? id)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            if (id == null)
            {
              return RedirectToAction("/ShowData");
            }
            var data = ed.Courses.FirstOrDefault(x=>x.id==id);
            if (data == null)
            {
              return RedirectToAction("/ShowData");
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int? id,CourseModel course)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("../Admin/Index");
            }
            Course c = ed.Courses.FirstOrDefault(x => x.id == id);
            HttpPostedFileBase img = course.image;
            if (img != null)
            {
                string oldpath = Path.Combine(Server.MapPath(c.image));
                System.IO.File.Delete(oldpath);
                Random rnd = new Random();
                string mypath = "~/img/Course";
                string _FileName = Path.GetFileName(course.image.FileName);

                _FileName = rnd.Next() + _FileName;
                string _path = Path.Combine(Server.MapPath(mypath), _FileName);
                course.image.SaveAs(_path);
                c.image = mypath + "/" + _FileName;
            }
            c.name = course.name;
            c.price = course.price;
            c.description = course.description;
            ed.SaveChanges();

            return RedirectToAction("../Admin/ShowData");
        }

        public ActionResult Delete(int? id,CourseModel course)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("../Admin/Index");
            }
            if (id == null)
            {
                return RedirectToAction("../Admin/ShowData");
            }
            var data = ed.Courses.FirstOrDefault(x=>x.id==id);
            if (data == null)
            {
                return RedirectToAction("../Admin/ShowData");
            }
            string oldpath = Path.Combine(Server.MapPath(data.image));
            System.IO.File.Delete(oldpath);
            ed.Courses.Remove(data);
            ed.SaveChanges();
           return RedirectToAction("../Admin/ShowData");
            
        }      
        public ActionResult Login()
        {
            if (Session["Adminid"] != null)
            {
                return RedirectToAction("../Admin/Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminLogin ad)
        {
            if (Session["Adminid"] != null)
            {
                return RedirectToAction("../Admin/Index");
            }
            if (ModelState.IsValid)
            {
                var data = ed.Sellers.FirstOrDefault(x => x.email_add == ad.email);
                if (data == null)
                {
                    ModelState.AddModelError("msg", "Email Doesn't Exists!");
                }
                else
                {
                    if (ad.password == data.password)
                    {
                        Session["Adminid"] = data.id;
                        Session["Adminname"] = data.First_name;
                        Session["Admin_Company_Name"] = data.company_name;
                        return RedirectToAction("../Admin/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("msg3", "Password is Wrong!");
                    }
                }
            }
            return View();
        }
        public ActionResult Admin_Dashboard()
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("../Admin/Login");
            }
            int id = (int)Session["Adminid"];
            var data = ed.Sellers.FirstOrDefault(x => x.id == id);
            return View(data);
        }
        public ActionResult Enquiry()
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("../Admin/Index");
            }
            int seller_id = (int)Session["Adminid"];
            var data = ed.Contacts.Where(x=>x.Sellerid==seller_id).ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Enquiry(string Response,int? id)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("../Admin/Index");
            }
            int seller_id = (int)Session["Adminid"];
            var data = ed.Contacts.FirstOrDefault(x=>x.Resp==null&&x.userid==id&&x.Sellerid==seller_id);
            if (data.Resp == null)
            {
                data.Resp = Response;
                ed.SaveChanges();
            }
            return RedirectToAction("../Admin/Enquiry");
        }

        public ActionResult Forgot_Password()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot_Password(Admin_pwd admin)
        {
            if (ModelState.IsValid)
            {
                var data = ed.Sellers.FirstOrDefault(x => x.email_add == admin.email);
                if (data == null)
                {
                    ModelState.AddModelError("!Exist", "This Email doesn't exists.");
                    return View();
                }
                return RedirectToAction("../Admin/Reset_Password",new { id=data.id});
            }
            return View();
        }
        public ActionResult Reset_Password(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("tryAgain", "Something went wrong. Please Try Again!");
                return RedirectToAction("../Admin/Forgot_Password");
            }
            var data = ed.Sellers.FirstOrDefault(x => x.id == id);
            return View(data);
        }
       [HttpPost]
       public ActionResult Reset_Password(Admin_password admin,int? id)
        {
            if (ModelState.IsValid)
            {
                var data = ed.Sellers.FirstOrDefault(x => x.id == id);
                if (data == null)
                {
                    ModelState.AddModelError("tryAgain", "Something went wrong. Please Try Again!");
                    return RedirectToAction("../Admin/Forgot_Password");
                }
                data.password = admin.pwd;
                ed.SaveChanges();              
                return RedirectToAction("../Admin/Login");
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("../Admin/Index");
        }

    }
}