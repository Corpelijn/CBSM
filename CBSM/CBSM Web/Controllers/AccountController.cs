using CBSM.Database;
using CBSM.Domain;
using CBSM_Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBSM_Web.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            DatabaseManager.AddConnection("82.196.10.160", "cbsm", "root", "M4khMNL&D");
            //Account test = new Account("test", "test", null);
            //test.WriteToDatabase();

            return View("Login");
        }

        [HttpPost]
        public ActionResult Validate(string username, string password)
        {
            if (username == "" || username == null || password == "" || password == null)
            {
                ViewData["errormessage"] = "Username and/or password cannot be empty";
                return View("Login");
            }

            Account account = Account.GetFromDatabase("username=?", username).FirstOrDefault();

            if (account == null)
            {
                ViewData["errormessage"] = "Cannot find the specified username";
                return View("Login");
            }
            if (!account.ValidatePassword(password))
            {
                ViewData["errormessage"] = "Password does not match the database";
                return View("Login");
            }

            SessionClass.SetAccountSession(true, username);

            return RedirectToAction("", "Home");
        }

        public ActionResult Logout()
        {
            SessionClass.SetAccountSession(false, "");
            return View("Login");
        }

        [HttpPost]
        public ActionResult Delete(int account_id)
        {
            if (Account.GetRecordCountFromDatabase() > 1)
            {
                Account account = Account.GetFromDatabaseById(account_id);
                account.RemoveFromDatabase();
            }
            else
            {
                TempData["erroraccount"] = "There is only one account left in the system. There must always be at least one account active in the application otherwise nobody can access the applcation anymore.";
            }

            return RedirectToAction("Access", "Settings");
        }

        [HttpPost]
        public ActionResult Edit(int account_id)
        {
            Account account = Account.GetFromDatabaseById(account_id);
            TempData["username"] = account.Username;
            TempData["account_id"] = account.Id;
            TempData["hidepassword"] = true;
            TempData["updateaccount"] = account.Username;

            TempData["savebutton"] = "Save changes";
            TempData["post"] = "UpdateAccount";

            return View("Details");
        }

        public ActionResult Add()
        {
            TempData["savebutton"] = "Create account";
            return View("Details");
        }

        [HttpPost]
        public ActionResult ResetPassword(int account_id)
        {
            Account account = Account.GetFromDatabaseById(account_id);

            TempData["resetpassword"] = true;
            TempData["savebutton"] = "Reset password";
            TempData["post"] = "SetPassword";

            return View("Details");
        }

        [HttpPost]
        public ActionResult CheckUsername(string username, string unique = "")
        {
            Account[] accounts = Account.GetFromDatabase("username=?", username);
            if (unique == "")
            {
                return Json(new { unique = accounts.Length == 0 });
            }
            else
            {
                return Json(new { unique = accounts.Length == 0 || accounts[0].Username == unique });
            }
        }

        [HttpPost]
        public ActionResult CheckPasswordComplexity(string password)
        {
            bool length = password.Length >= 8;
            bool upper = false, lower = false, numeric = false;
            foreach (char c in password)
            {
                if (c >= 48 && c <= 57)
                    numeric = true;
                if (c >= 65 && c <= 90)
                    upper = true;
                if (c >= 97 && c <= 122)
                    lower = true;
            }
            return Json(new { length = length, lower = lower, upper = upper, numeric = numeric });
        }

        [HttpPost]
        public ActionResult CreateAccount(string username, string password, string confirm)
        {
            JsonResult uniqueUsername = CheckUsername(username) as JsonResult;
            JsonResult complexPassword = CheckPasswordComplexity(password) as JsonResult;

            TempData["username"] = username;
            TempData["password"] = password;
            TempData["confirm"] = confirm;

            IDictionary<string, bool> dictionary = uniqueUsername.Data.GetType().GetProperties().ToDictionary(x => x.Name, x => (bool)x.GetValue(uniqueUsername.Data, null));

            if (!dictionary["unique"])
            {
                TempData["usernameerror"] = "The username is not unique";
                return RedirectToAction("Add");
            }

            dictionary = complexPassword.Data.GetType().GetProperties().ToDictionary(x => x.Name, x => (bool)x.GetValue(complexPassword.Data, null));

            if (!dictionary["length"] || !dictionary["lower"] || !dictionary["upper"] || !dictionary["numeric"])
            {
                TempData["passworderror"] = "The password does not meet the complexity rules";
                return RedirectToAction("Add");
            }

            if (password != confirm)
            {
                TempData["confirmerror"] = "The password do not match";
                return RedirectToAction("Add");
            }

            Account newaccount = new Account(username, password, null);
            newaccount.WriteToDatabase();

            TempData["username"] = "";
            TempData["password"] = "";
            TempData["confirm"] = "";

            return RedirectToAction("Access", "Settings");
        }

        public ActionResult UpdateAccount(int account_id, string username)
        {
            JsonResult uniqueUsername = CheckUsername(username) as JsonResult;

            TempData["username"] = username;

            IDictionary<string, bool> dictionary = uniqueUsername.Data.GetType().GetProperties().ToDictionary(x => x.Name, x => (bool)x.GetValue(uniqueUsername.Data, null));

            if (!dictionary["unique"])
            {
                TempData["usernameerror"] = "The username is not unique";
                return RedirectToAction("Edit");
            }

            Account account = Account.GetFromDatabaseById(account_id);
            account.Username = username;
            account.WriteToDatabase();

            TempData["username"] = "";
            TempData["password"] = "";
            TempData["confirm"] = "";

            return RedirectToAction("Access", "Settings");
        }

        [HttpPost]
        public ActionResult SetPassword(string username, string password, string confirm)
        {
            return RedirectToAction("Access", "Settings");
        }
    }
}