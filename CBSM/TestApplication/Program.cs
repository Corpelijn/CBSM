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
            DatabaseManager.AddConnection("localhost", "bmt", "root", "");

            Season[] s = Season.GetAllFromDatabase();

            //Serie s = new Serie("Frank and Dale", "F&D");
            //Season s1 = new Season(1, "Police and criminal", s);
            //s1.WriteToDatabase();

            Console.WriteLine();
            Console.WriteLine("All done . . .");
            Console.ReadKey();
        }
    }
}
