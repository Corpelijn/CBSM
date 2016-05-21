using System;
using System.Collections.Generic;
using System.Text;
using CBSM.Domain.Object;
using CBSM.Database.Attributes;
using CBSM.Database;

namespace CBSM.Domain
{
    public class Account : BaseClass<Account>
    {
        private string username;
        private string password;
        [DBMSDefaultValue(31193)]
        private int recht;

        public Account()
        {
        }

        public Account(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public int Recht
        {
            get { return this.recht; }
            set { this.recht = value; }
        }
    }
}
