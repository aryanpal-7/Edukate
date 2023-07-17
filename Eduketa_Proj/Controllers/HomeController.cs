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

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
        public ActionResult Index()
        {

            var data = ed.Courses.ToList();
            return View(data);
        }
        public ActionResult About()
        {

            return View();
        }
        public ActionResult Contact(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("/Courses");
            }
            if (Session["userid"] == null)
            {
                return RedirectToAction("/Login");
            }
            var data = ed.Sellers.FirstOrDefault(x => x.id == id);
            return View(data);
        }
        [HttpPost]
        public ActionResult Contact(ContactModel cm,int id)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("/Login");
            }
            if (ModelState.IsValid)
            {        
                    int userid = (int)Session["userid"];
                    Contact c = new Contact()
                    {
                        UserName = cm.UserName,
                        Subject = cm.Subject,
                        email = cm.email,
                        Message = cm.Message,
                        userid = userid,
                        Sellerid=id
                    };
                    ed.Contacts.Add(c);
                    ed.SaveChanges();
                
            }
            return View();
        }

        public ActionResult Enquiry()
        {

            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }
            int userid = (int)Session["userid"];

            var data = ed.Contacts.Where(x => x.userid == userid).ToList();

            return View(data);
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
            bought_course data3 = null;
            if (Session["userid"] != null)
            {
                int userid = (int)Session["userid"];
                data3 = ed.bought_course.FirstOrDefault(v => v.course_id == id && v.userid == userid);
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
                var existuser = ed.Users.FirstOrDefault(x => x.email == user.email);
                if (existuser == null)
                {
                    User u = new User() {
                        name = user.name,
                        email = user.email,
                        password = user.password
                    };
                    ed.Users.Add(u);
                    int otp = new Random().Next(100000, 999999);
                    emailOTP e = new emailOTP() {
                        email = user.email,
                        OTP = otp
                    };
                    ed.emailOTPs.Add(e);
                    ed.SaveChanges();
                    sendOTP(user.email, otp);
                    ModelState.Clear();
                    ModelState.AddModelError("Success", "User Registered Successfully");
                    return RedirectToAction("OTP", new { email = user.email });
                }
                else
                {
                    ModelState.AddModelError("Failed", "User Already Exists!");
                }
            }
            return View();
        }

        public ActionResult OTP(string email)
        {
            if (email == null)
            {
                return RedirectToAction("register");
            }
            emailOTP e = new emailOTP()
            {
                email = email
            };
            return View(e);
        }
        [HttpPost]
        public ActionResult OTP(OTPModel op)
        {
            var data = ed.emailOTPs.FirstOrDefault(x => x.email == op.email);
            if (data == null)
            {
                ViewBag.datanull = "Email doesn't Exists";
                return RedirectToAction("register");
            }
            if (data.OTP != op.OTP) {
                ViewBag.OTPerr = "OTP Doesn't Match";
                return RedirectToAction("OTP", new { email = data.email });
            }
            if (data.OTP == op.OTP)
            {
                TempData["success"] = "Email Verified and User successfully registered";
                ed.emailOTPs.Remove(data);
                return RedirectToAction("Login");
            }
            return RedirectToAction("OTP", new { email = data.email });
        }
        public ActionResult Login()
        {
            if (TempData.ContainsKey("success"))
            {
                ViewBag.success = TempData["success"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginUserModel user)
        {
            if (ModelState.IsValid)
            {
                var data = ed.Users.FirstOrDefault(x => x.email == user.email);
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

        public ActionResult Account()
        {
            Session["userid"] = 2;
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }
            int userid = (int)Session["userid"];
            var data = ed.Users.FirstOrDefault(x => x.id == userid);

            return View(data);
        }
        [HttpPost]
        public ActionResult Account(string name)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }
            if (name == null)
            {
                return RedirectToAction("Account");
            }
            else
            {
                int userid = (int)Session["userid"];
                var data = ed.Users.FirstOrDefault(x => x.id == userid);
                data.name = name;
                ed.SaveChanges();
                return RedirectToAction("Account");
            }
        }
        public ActionResult Password()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Password(pwdmodel pwd) {

            if (ModelState.IsValid)
            {

                if (Session["userid"] == null)
                {
                    return RedirectToAction("Login");
                }
                if (pwd.newpwd == null || pwd.password == null)
                {
                    return RedirectToAction("Password");
                }
                int userid = (int)Session["userid"];
                var data = ed.Users.FirstOrDefault(x => x.id == userid);
                if (pwd.password == pwd.newpwd)
                {
                    ViewBag.msg1 = "Old and new Password can't be same";
                    return View();
                }
                if (pwd.password == data.password)
                {
                    data.password = pwd.newpwd;
                    ed.SaveChanges();
                    return RedirectToAction("Account");
                }
                else
                {
                    ViewBag.msg = "Old Password is wrong";
                    return View();
                }
            }
            return View();
        }
        public ActionResult dashboard()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                int userid = Convert.ToInt32(Session["userid"]);
                User u = ed.Users.FirstOrDefault(x => x.id == userid);
                List<bought_course> b = ed.bought_course.Where(y => y.userid == userid).ToList();
                List<Course> c = new List<Course>();
                List<coursepayment> cp = ed.coursepayments.Where(p => p.userid == userid).ToList();
                List<Course> c1 = new List<Course>();

                for (int i = 0; i < b.Count; i++)
                {
                    int xy = (int)b[i].course_id;
                    c = ed.Courses.Where(v => v.id == xy).ToList();
                    c1.Add(c[0]);
                };
                UserDashboard ud = new UserDashboard()
                {
                    user = u,
                    bought = b,
                    course = c1,
                    coursepayments = cp

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
            var data1 = ed.bought_course.FirstOrDefault(x => x.course_id == id && x.userid == usrid);
            if (data1 != null)
            {
                TempData["BoughtMsg"] = "You Have Already purchased " + data.name + " course";
                return RedirectToAction("dashboard");
            }
            else
            {
                string orderId;
                int amount = (int)data.price * 100;
                int usersid = (int)Session["userid"];
                var check = ed.coursepayments.FirstOrDefault(x => x.userid == usersid && x.courseid == id && x.status == "pending");
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
            int u_id = (int)check.userid;
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
                price = course.price
            };
            ed.bought_course.Add(b);
            ed.SaveChanges();
            sendMail(user.email, o_id, course.name, c_price);
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
                    email = email,
                    UpdateOn = DateTime.Now.Date,
                    keycode = keycode
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
        public ActionResult Reset(string id, string password)
        {
            var data = ed.forgets.FirstOrDefault(x => x.keycode == id);
            User u = ed.Users.FirstOrDefault(x => x.email == data.email);
            forget f = ed.forgets.FirstOrDefault(x => x.email == data.email);
            u.password = password;
            ed.forgets.Remove(f);
            ed.SaveChanges();
            return RedirectToAction("Login");
        }
    
        public ActionResult logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("login");
        }

        public ActionResult Seller()
        {
            return View();
        }

        public ActionResult SellerSignup() {

            return View();
        }
        [HttpPost]
        public ActionResult SellerDetails(Seller_Model seller_model)
        {
            if (ModelState.IsValid)
            {
                Seller sell = ed.Sellers.FirstOrDefault(x => x.email_add == seller_model.email_add);
                if (sell != null)
                {
                    ModelState.AddModelError("exists", "User already exists");
                    return View();
                }
                Seller s = new Seller()
                {
                    First_name = seller_model.first_name,
                    Last_name = seller_model.last_name,
                    email_add = seller_model.email_add,
                    mobile_no = seller_model.mobile_no,
                    social_prof = seller_model.social_profile,
                    company_name=seller_model.company_name
                };
                ed.Sellers.Add(s);
                ed.SaveChanges();
                Seller sd = ed.Sellers.FirstOrDefault(y => y.email_add == seller_model.email_add);
                Random rnd = new Random();
                int OTP = rnd.Next(111111, 999999);
                seller_verify s1 = new seller_verify() {
                    OTP = OTP,
                    email_add = seller_model.email_add,
                    Modify_Date = DateTime.Now.Date
                };
                ed.seller_verify.Add(s1);
                ed.SaveChanges();
                SellerOTP(seller_model.email_add, OTP);
                return RedirectToAction("Seller_Verification", new { id = sd.id });
            }
            return RedirectToAction("SellerSignup");
        }

        public ActionResult Seller_Verification(int id) {
            var Sellers_data = ed.Sellers.FirstOrDefault(x => x.id == id);
            return View(Sellers_data);
        }
        [HttpPost]
        public ActionResult Seller_Verifications(int? OTP, string email)
        {
            var data2 = ed.Sellers.FirstOrDefault(y=>y.email_add==email);
            if (OTP == null)
            {
                ModelState.AddModelError("msg", "Enter The OTP");
                return View();
            }
            if (email == null)
            {
                return RedirectToAction("SellerSignup");
            }
            var data = ed.seller_verify.FirstOrDefault(x => x.email_add == email);
            if (OTP == data.OTP)
            {
                ed.seller_verify.Remove(data);
                ed.SaveChanges();
                return RedirectToAction("Seller_Password",new { data2.id});
            }
            return View();
        }

        public ActionResult Seller_Password(int id)
        {
            var data = ed.Sellers.FirstOrDefault(x => x.id == id);
            return View(data);
        }
        [HttpPost]
        public ActionResult SellerPassword(Seller_Pwd sel_pwd,int s_id)
        {
            var data = ed.Sellers.FirstOrDefault(x => x.id == s_id);
            data.password = sel_pwd.password;
            ed.SaveChanges();             
            return RedirectToAction("../Admin/Login");
        }
        /* Methods for OTP/Emails */
        private void sendMail(string email, string orderid, string c_name, string price)
        {
            string body = string.Empty;
            using (StreamReader sr = new StreamReader(Server.MapPath("~/js/Invoice.html")))
            {
                body = sr.ReadToEnd();
            }
            body = body.Replace("{email}", email);
            body = body.Replace("{orderid}", orderid);
            body = body.Replace("{c_name}", c_name);
            body = body.Replace("{c_price}", price);
            body = body.Replace("{amount}", price);
            body = body.Replace("{p_date}", DateTime.Now.ToShortDateString());
            using (MailMessage mail = new MailMessage())
            {
                string msg = body;
                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Purchased Summary";
                mail.Body = msg;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "jyzfmazdjnwnabjh");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        private void Forgotmail(string usermail, string keycode)
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
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "jyzfmazdjnwnabjh");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        private void DemoMail(string usermail, string c_name, string date, string time)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/js/DemoMail.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{email}", usermail);
            body = body.Replace("{c_name}", c_name);
            body = body.Replace("{Date}", date);
            body = body.Replace("{Time}", time);
            body = body.Replace("{p_date}", DateTime.Now.Date.ToString("d"));

            using (MailMessage mail = new MailMessage()) {


                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(usermail);
                mail.Subject = "Demo Class Registered";
                mail.Body = body;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "jyzfmazdjnwnabjh");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }


            }
        }
      
        private void sendOTP(string usermail, int OTP)
        {
            using (MailMessage mail = new MailMessage())
            {

                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(usermail);
                mail.Subject = "Email Verfication :";
                mail.Body = "Your OTP is" + OTP;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "jyzfmazdjnwnabjh");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

        }

        private void SellerOTP(string email, int OTP)
        {
            using (MailMessage mail = new MailMessage()) {
                mail.From = new MailAddress("aryanpal77788888@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Email Verification";
                mail.Body = "Your OTP is" + OTP;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("aryanpal77788888@gmail.com", "jyzfmazdjnwnabjh");
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

    public class OTPModel {

        public string email { get; set; }

        public int OTP { get; set; }
    }

    public class pwdmodel
    {
        [Required(ErrorMessage = "Please Enter Old Password")]
        public string password
        {
            get; set;
        }
        [Required(ErrorMessage = "Please Enter New Password")]
        public string newpwd { get; set; }
    }

    public class Seller_Model {

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string email_add { get; set; }

        public string mobile_no { get; set; }

        public string social_profile { get; set; }

        public string company_name { get; set; }


    }

    public class Seller_Pwd{

        public string password { get; set; }

        public string confirm_pwd { get; set; }
        }

}