using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Eduketa_Proj.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using Razorpay.Api ;
using System.IO;

namespace Eduketa_Proj.Controllers
{
    public class HomeController : Controller
    {
        eduketaEntities1 ed = new eduketaEntities1();
        public ActionResult Index()
        {
            var data = ed.Courses.ToList();
            return View(data);
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(ContactModel cm)
        {
            if (ModelState.IsValid)
            {
                if (Session["userid"] == null)
                {
                    ContactMail(cm.UserName, cm.email, cm.Subject, cm.Message);
                }
                else
                {
                    Contact c = new Contact()
                    {
                        UserName = cm.UserName,
                        Subject = cm.Subject,
                        email = cm.email,
                        Message = cm.Message
                    };
                    ed.Contacts.Add(c);
                    ed.SaveChanges();
                }
            }
            return View();
        }
        public ActionResult Courses()
        {
            var data = ed.Courses.ToList();
            return View(data);
        }
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Courses");
            }
            var data = ed.Courses.FirstOrDefault(x => x.id == id);
            if (data == null)
            {
                return RedirectToAction("Courses");
            }
            var data2 = ed.Courses.ToList();
            DetailsModel d = new DetailsModel();
            bought_course data3=null;
            if (Session["userid"] != null)
            {
                int userid = (int)Session["userid"];
                data3= ed.bought_course.FirstOrDefault(v => v.course_id == id && v.userid == userid);
                d.course = data;
                d.courses = data2;
                d.bought_course = data3;
                return View(d);
            }
                d.course = data;
                d.courses = data2;
                d.bought_course = null;
            
            return View(d);
        }


        public ActionResult register()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var existuser = ed.Users.FirstOrDefault(x=>x.email==user.email);
                if (existuser == null)
                {
                    User u = new User() { 
                    name=user.name,
                    email=user.email,
                    password=user.password
                    };
                    ed.Users.Add(u);
                    ed.SaveChanges();
                    ModelState.Clear();
                    ModelState.AddModelError("Success", "User Registered Successfully");
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
        public ActionResult Login(LoginUserModel user)
        {
            if (ModelState.IsValid)
            {
                var data = ed.Users.FirstOrDefault(x=>x.email==user.email);
                if (data == null)
                {
                    ModelState.AddModelError("msg", "Email Doesn't Exists!");                  
                }
                else
                {
                    if (data.password == user.password)
                    {
                        Session["userid"] = data.id;
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

        public ActionResult dashboard()
        {
            Session["userid"] = 2;
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                int userid = Convert.ToInt32(Session["userid"]);
                User u = ed.Users.FirstOrDefault(x=>x.id==userid);                
                List<bought_course> b = ed.bought_course.Where(y=>y.userid==userid).ToList();                
                List<Course> c = new List<Course>();                
                List<coursepayment> cp = new List<coursepayment>();                
                List<String> image = new List<string>();
                List<String> course = new List<string>();
                List<String> desc = new List<string>();
                List<int> price = new List<int>();
                List<string> order = new List<string>();
                for (int i = 0; i < b.Count; i++)
                {
                    int xy = (int)b[i].course_id;
                    c = ed.Courses.Where(v => v.id == xy).ToList();
                    cp = ed.coursepayments.Where(p => p.userid == userid).ToList();
                    string image1 = c[0].image;
                    string course1 = c[0].name;
                    string descr = c[0].description;
                    int pric = (int)c[0].price;
                    string order1 = cp[0].orderid;
                    image.Add(image1);
                    course.Add(course1);
                    desc.Add(descr);
                    price.Add(pric);
                    order.Add(order1);
                };
                UserDashboard ud = new UserDashboard()
                {
                    user=u,
                    bought=b,
                   image=image,
                   coursename=course,
                   Description=desc,
                   Price=price,
                   orderid=order
                   
                };
                return View(ud);
            }           
        }
        public ActionResult Enroll(int? id)
        {           
            if (id == null)
            {
                return RedirectToAction("Login");
            }
            var data = ed.Courses.FirstOrDefault(x => x.id == id);
            if (data == null)
            {
                return RedirectToAction("Login");
            }
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }
            int usrid = (int)Session["userid"];
            var data1 = ed.bought_course.FirstOrDefault(x=>x.course_id==id&&x.userid==usrid);
            if(data1 != null)
            {
                TempData["BoughtMsg"] = "You Have Already purchased "+data.name+" course";
                return RedirectToAction("dashboard");
            }
            else
            {
                string orderId;
                int amount = (int)data.price * 100;
                int usersid = (int)Session["userid"];
                var check = ed.coursepayments.FirstOrDefault(x=>x.userid==usersid&&x.courseid==id&&x.status=="pending");
                if (check != null)
                {
                    orderId = check.orderid;
                }                
                else
                {               
                    RazorpayClient client = new RazorpayClient("rzp_test_BwjcHWb56BIttF", "9MKRi70sfDXbwU5a79e2XpAF");

                    Dictionary<string, object> options = new Dictionary<string, object>();
                    options.Add("amount", amount); // amount in the smallest currency unit
                    options.Add("currency", "INR");
                    Order order = client.Order.Create(options);
                    orderId = order["id"].ToString();
                    coursepayment cp = new coursepayment()
                    {
                        userid = usersid,
                        courseid = id,
                        orderid = orderId,
                        status = "pending"
                    };
                    ed.coursepayments.Add(cp);
                    ed.SaveChanges();
                }
                ViewBag.amount = amount;
                ViewBag.orderId = orderId;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Enroll(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            var check = ed.coursepayments.FirstOrDefault(x => x.orderid == razorpay_order_id);
           int u_id=(int)check.userid;
            int c_id = (int)check.courseid;
            string o_id = check.orderid;
            var course = ed.Courses.FirstOrDefault(x => x.id == c_id);
            var user = ed.Users.FirstOrDefault(x => x.id == u_id);
            string c_price = course.price.ToString();
            check.paymentid = razorpay_payment_id;
            check.signature = razorpay_signature;
            check.status = "success";          
            ed.SaveChanges();
            bought_course b = new bought_course()
            {
                userid = u_id,
                course_id = c_id,
                Purchased_Date = DateTime.Now.Date,
                price=course.price
            };
            ed.bought_course.Add(b);
            ed.SaveChanges();
            sendMail(user.email,o_id,course.name,c_price);
            return RedirectToAction("dashboard");
        }
        public ActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forget(string email)
        {
            var data = ed.Users.FirstOrDefault(x => x.email == email);
            if (data == null)
            {
                ViewBag.msg = "Email Not Found!";
                return View();                
            }

            var exist = ed.forgets.FirstOrDefault(x => x.email == email);
            string keycode;
            if (exist != null)
            {
                keycode = exist.keycode;
            }
            else
            {
                var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[30];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                keycode = new String(stringChars);

                forget f = new forget()
                {
                    email=email,
                    UpdateOn=DateTime.Now.Date,
                    keycode=keycode
                };
                ed.forgets.Add(f);
                ed.SaveChanges();
                Forgotmail(email, keycode);
            }
            return View();
        }

        public ActionResult Reset(string id)
        {
            var data = ed.forgets.FirstOrDefault(x => x.keycode == id);
            if (data == null)
            {
                TempData["CheckMsg"] = "Reset Link Was Wrong Please Send it Again!";
                return RedirectToAction("Forget");
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult Reset(string id,string password)
        {
            var data = ed.forgets.FirstOrDefault(x => x.keycode == id);
            User u = ed.Users.FirstOrDefault(x => x.email == data.email);
            forget f = ed.forgets.FirstOrDefault(x => x.email == data.email);
            u.password = password;
            ed.forgets.Remove(f);            
            ed.SaveChanges();
           return RedirectToAction("Login");
        }

        public ActionResult Demo()
        {
           
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }          
            return View();
        }
        [HttpPost]
        public ActionResult Demo(DemoModel demo)
        {
            var data = ed.DemoCourses.FirstOrDefault(x=>x.email==demo.email&&x.course==demo.course);
            if (data != null)
            {
                ViewBag.Demomsg = "You Have already booked "+demo.course+" Demo ";
                return View();
            }
            DemoCourse dc = new DemoCourse()
            {
                name=demo.name,
                email=demo.email,
                course=demo.course,
                mobile=demo.mobile,
                demodate=demo.demodate,
                demotime=demo.demotime
            };
            ed.DemoCourses.Add(dc);           
            ed.SaveChanges();
            DateTime ds = (DateTime)demo.demodate;
            DateTime ts = DateTime.Today.Add((TimeSpan)demo.demotime);
            string d = ds.Date.ToString("d");           
            string t = ts.ToString("hh:mm tt");
            DemoMail(demo.email, demo.course, d, t);
            ViewBag.Demomsg = "Demo Booked Successfully!";
            return View();
        }

        public ActionResult logout()
        {
            Session.Clear();
            Session.Abandon();          
            return RedirectToAction("login");
        }
        public void sendMail(string email,string orderid,string c_name,string price)
        {
            string body = string.Empty;
            using(StreamReader sr=new StreamReader(Server.MapPath("~/js/Invoice.html")))
            {
                body = sr.ReadToEnd();
            }            
            body = body.Replace("{email}", email);
            body = body.Replace("{orderid}",orderid);
            body = body.Replace("{c_name}",c_name);
            body = body.Replace("{c_price}", price);
            body = body.Replace("{amount}", price);
            body = body.Replace("{p_date}", DateTime.Now.ToShortDateString());
            using (MailMessage mail=new MailMessage())
            {
                string msg = body;
                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Purchased Summary";
                mail.Body = msg;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "bisrprszlzquwhkz");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        public void Forgotmail(string usermail,string keycode)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/js/forgetMail.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{requestdate}", DateTime.Now.ToString());
            body = body.Replace("{useremail}", usermail);
            body = body.Replace("{keycode}", keycode);
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(usermail);
                mail.Subject = "Forget Password";
                mail.Body = body;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "bisrprszlzquwhkz");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        public void DemoMail(string usermail,string c_name,string date,string time)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/js/DemoMail.html")))
            {
                body = reader.ReadToEnd();
            }
           body= body.Replace("{email}", usermail);
           body= body.Replace("{c_name}", c_name);
           body= body.Replace("{Date}", date);
          body=  body.Replace("{Time}", time);
           body= body.Replace("{p_date}", DateTime.Now.Date.ToString("d"));
           
            using (MailMessage mail = new MailMessage()) {


                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(usermail);
                mail.Subject = "Demo Class Registered";
                mail.Body = body;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "bisrprszlzquwhkz");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }


            }
        }
            
        public void ContactMail(string username,string usermail,string Subject,string enquiry)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/js/Enquiry.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Name}", username);
            body = body.Replace("{Your Enquiry}", enquiry);
            using (MailMessage mail=new MailMessage()) {

                mail.From =new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(usermail);
                mail.Subject = "Your Enquiry Related :"+Subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "bisrprszlzquwhkz");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }                        
            }
        }
        
    }
    public class LoginUserModel
    {
        [Required]
        public string email { get; set; }
        [Required]

        public string password { get; set; }
    }


    
}