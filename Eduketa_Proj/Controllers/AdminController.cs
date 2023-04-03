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
        [HttpPost]
        public ActionResult AddCourse(CourseModel course)
        {
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
            return RedirectToAction("ShowData");
        }
        public ActionResult ShowData()
        {
            
            var data = ed.Courses.ToList();

            return View(data);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
              return RedirectToAction("AddCourse");
            }
            var data = ed.Courses.FirstOrDefault(x=>x.id==id);
            if (data == null)
            {
              return RedirectToAction("AddCourse");
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(int? id,CourseModel course)
        {
            Course c = ed.Courses.FirstOrDefault(x => x.id == id);
            HttpPostedFileBase img = course.image;
            if (img != null)
            {
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
            
            return View(c);
        }

        public ActionResult Delete(int? id,CourseModel course)
        {
            if (id == null)
            {
                return RedirectToAction("AddCourse");
            }
            var data = ed.Courses.FirstOrDefault(x=>x.id==id);
            if (data == null)
            {
                return RedirectToAction("AddCourse");
            }
            ed.Courses.Remove(data);
            ed.SaveChanges();
           return RedirectToAction("AddCourse");
            
        }

        public ActionResult signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult signup(AdminSignupModel ad)
        {
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
                    return RedirectToAction("Login");
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
                    if (data.password == ad.password)
                    {
                        Session["Adminid"] = data.id;
                        return RedirectToAction("dashboard");
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
            var data = ed.Contacts.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Enquiry(string Response,int? id)
        {
            var data = ed.Contacts.FirstOrDefault(x=>x.Resp==null&&x.userid==id);
            if (data.Resp == null)
            {
                data.Resp = Response;
                ed.SaveChanges();
            }
            return RedirectToAction("Enquiry");
            
        }

    }
}