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
                    image=mypath+"/"+Filename,
                    description = course.description
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
            var data = ed.Courses.ToList();

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
                return RedirectToAction("/Index");
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

            return RedirectToAction("/ShowData");
        }

        public ActionResult Delete(int? id,CourseModel course)
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
            string oldpath = Path.Combine(Server.MapPath(data.image));
            System.IO.File.Delete(oldpath);
            ed.Courses.Remove(data);
            ed.SaveChanges();
           return RedirectToAction("/ShowData");
            
        }

        public ActionResult signup()
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult signup(AdminSignupModel ad)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            if (ModelState.IsValid)
            {
                var existuser = ed.adminsignups.FirstOrDefault(x => x.email == ad.email);
                if (existuser == null)
                {
                    adminsignup u = new adminsignup()
                    {
                        name = ad.name,
                        email = ad.email,
                        password = ad.password
                    };
                    ed.adminsignups.Add(u);
                    ed.SaveChanges();
                    ModelState.Clear();
                    ModelState.AddModelError("Success", "User Registered Successfully");
                    return RedirectToAction("/Admin/Login");
                }
                else
                {
                    ModelState.AddModelError("Failed", "User Already Exists!");
                }
            }
            return View();            
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminLogin ad)
        {
            if (ModelState.IsValid)
            {
                var data = ed.adminsignups.FirstOrDefault(x => x.email == ad.email);
                if (data == null)
                {
                    ModelState.AddModelError("msg", "Email Doesn't Exists!");
                }
                else
                {
                    if (ad.password == data.password)
                    {
                        Session["Adminid"] = data.id;
                        Session["Adminname"] = data.name;
                        return RedirectToAction("/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("msg3", "Password is Wrong!");
                    }
                }
            }
            return View();
        }
        public ActionResult Enquiry()
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }

            var data = ed.Contacts.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Enquiry(string Response,int? id)
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            var data = ed.Contacts.FirstOrDefault(x=>x.Resp==null&&x.userid==id);
            if (data.Resp == null)
            {
                data.Resp = Response;
                ed.SaveChanges();
            }
            return RedirectToAction("/Enquiry");
        }

        public ActionResult UserData()
        {
            if (Session["Adminid"] == null)
            {
                return RedirectToAction("/Index");
            }
            var data = ed.Users.ToList();
            return View(data);


        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("/Index");
        }

    }
}