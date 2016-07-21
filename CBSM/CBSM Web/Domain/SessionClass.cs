using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBSM_Web.Domain
{
    public class SessionClass
    {
        public static void SetAccountSession(bool status, string username)
        {
            HttpContext.Current.Session["account"] = status;
            HttpContext.Current.Session["accountusername"] = username;
        }

        public static bool GetAccountSession()
        {
            foreach (string key in HttpContext.Current.Session.Keys)
            {
                if (key == "account")
                {
                        return (bool)HttpContext.Current.Session["account"];
                }
            }
            return false;
        }
    }
}