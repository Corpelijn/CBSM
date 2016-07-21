using System;
using System.Collections.Generic;
using System.Text;
using CBSM.Domain.Object;
using CBSM.Database.Attributes;
using CBSM.Database;
using System.Security.Cryptography;

namespace CBSM.Domain
{
    public class Account : BaseClass<Account>
    {
        private string username;
        private string password;
        private string salt;
        private Person owner;
        private List<Right> rights;
        private List<RightGroup> rightGroups;

        public Account()
        {
        }

        public Account(string username, string password, Person owner)
        {
            this.username = username;
            this.owner = owner;
            this.rights = new List<Right>();
            this.rightGroups = new List<RightGroup>();

            GenerateSalt();

            this.password = GenerateHashFromPassword(password);
        }

        private void GenerateSalt()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            // Generate a random binary string
            string binary = "";
            for (int i = 0; i < 256; i++)
            {
                binary += rand.Next(0, 10) % 2;
            }

            // Store the binary string as hex
            string hex = "";
            for (int i = 0; i < binary.Length - 3; i += 4)
            {
                byte value = Convert.ToByte(binary.Substring(i, 4), 2);
                hex += Convert.ToString(value, 16);
            }

            this.salt = hex;
        }

        private string GenerateHashFromPassword(string password)
        {
            byte[] saltbytes = new byte[32];
            int index = 0;
            for (int i = 0; i < salt.Length - 1; i += 2)
            {
                string value = Convert.ToString(Convert.ToInt32(salt[i].ToString(), 16), 2);
                value += Convert.ToString(Convert.ToInt32(salt[i + 1].ToString(), 16), 2);
                saltbytes[index] = Convert.ToByte(value, 2);
                index++;
            }

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, saltbytes, 10000);
            byte[] hash = pbkdf2.GetBytes(32);

            return Convert.ToBase64String(hash);
        }

        public void AddRight(Right right)
        {
            if (!rights.Contains(right))
                rights.Add(right);
        }

        public void AddRightGroup(RightGroup group)
        {
            if (!rightGroups.Contains(group))
                rightGroups.Add(group);
        }

        public bool ValidatePassword(string password)
        {
            string hash = GenerateHashFromPassword(password);
            return hash == this.password;
        }

        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = GenerateHashFromPassword(value); }
        }

        public string Salt
        {
            get { return salt; }
        }
    }
}
