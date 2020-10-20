
using System.Web.Mvc;
using System.Linq;
using docu3c.Models;

namespace docu3c.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
      //  private TableManager TableManagerObj = new TableManager("UserRegistration");
        string role = string.Empty;
        string userName = string.Empty;
        // GET: Login

    //    [SessionFilter(Enable = false)]
        public ActionResult Login()
        {
            return View();

        }

 //       [SessionFilter(Enable = false)]
                [HttpPost]
        public ActionResult Login(FormCollection formInfo)
        {
            if (ModelState.IsValid)
            {
                string userEmailID = formInfo["Emailid"] == "" ? null : formInfo["Emailid"];
                string password = formInfo["password"] == "" ? null : formInfo["password"];


                if (!string.IsNullOrEmpty(userEmailID) && !string.IsNullOrEmpty(password))
                {

                    using (docu3cEntities db = new docu3cEntities())
                    {
                        var User = db.UserDetails.Where(a => a.LoginID.Equals(userEmailID) && a.LoginPassword.Equals(password)).FirstOrDefault();
                       
                        if (User != null)
                        {


                            role = User.UserRole;
                            userName = User.FirstName + " " + User.LastName;
                            role = role.Replace(" ", "");
                            Session["UserEmailID"] = userEmailID;
                            Session["UserName"] = userName;
                            Session["Role"] = role;
                            return RedirectToAction("Home", "Home");
                        }
                        else {
                            ViewBag.ErrorMsg = "Please enter the correct username and password";
                            //ModelState.AddModelError("CustomError", "Please enter the username and password"); 

                        }
                    }
                   
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Please enter the username and password");
                }
            }
            return View();
        }


       
        public ActionResult Dashboard()
        {
            if (Session["UserName"] != null && Session["Role"] != null && Session["UserEmailID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}