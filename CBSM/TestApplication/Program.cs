﻿using CBSM.Database;
using CBSM.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager.AddConnection("localhost", "bmt", "root", "");

            Account[] accounts = Account.GetAllFromDatabase();

            Console.ReadKey();
        }
    }
}