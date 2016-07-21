using CBSM.Database;
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
            DatabaseManager.AddConnection("82.196.10.160", "cbsm", "root", "M4khMNL&D");

            //Serie[] s = Serie.GetAllFromDatabase();
            //Account.GetAllFromDatabase();

            //Serie s = new Serie("Frank and Dale", "F&D");
            //Season s1 = new Season(1, "Police and criminal", s);
            //Season s2 = new Season(2, "Hiding in the woords", s);

            //s.AddSeason(s1);
            //s.AddSeason(s2);
            //s.WriteToDatabase();
            //s1.WriteToDatabase();

            new Account("admin", "admin", null).WriteToDatabase();

            Console.WriteLine();
            Console.WriteLine("All done . . .");
            Console.ReadKey();
        }
    }
}
