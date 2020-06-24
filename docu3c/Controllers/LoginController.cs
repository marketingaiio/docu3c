using docu3c.TableHandling;
using System.Web.Mvc;
using System.Linq;

namespace docu3c.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private TableManager TableManagerObj = new TableManager("UserRegistration");
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
                    if (isValid(userEmailID, password))
                    {
                        role = role.Replace(" ", "");
                        Session["UserEmailID"] = userEmailID;
                        Session["UserName"] = userName;
                        Session["Role"] = role;
                        // return RedirectToAction("Dashboard", "Document");
                        //if (role.Contains("ChiefExecutiveOfficer"))
                        //{
                        return RedirectToAction("Index", "Home");
                        //}
                        //else if (role.Contains("FinancialAdvisor"))
                        //{
                        //    return RedirectToAction("FinancialAdvisor", "Home");
                        //}
                        //else if (role.Contains("Admin"))
                        //{
                        //    return RedirectToAction("Get", "Home");


                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Please enter the username and password");
                    }


                }
                else
                {
                    ModelState.AddModelError("CustomError", "Please enter the username and password");
                }
            }
            return View();
        }


        private bool isValid(string userEmailID, string password)
        {
            var Users = TableManagerObj.RetrieveEntity<User>("EmailId eq '" + userEmailID + "'");
            var User = Users.Where(v => v.CPassword == password).FirstOrDefault();
            if (User != null)
            {


                role = User.RoleType;
                userName = User.FirstName + " " + User.LastName;

            }
            return User != null ? true : false;
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