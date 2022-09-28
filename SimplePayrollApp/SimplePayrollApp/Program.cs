using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimplePayrollApp
{
    internal class Program
    {
        // main entry point for the application
        static void Main(string[] args)
        {
            {
                List<Staff> myStaff = new List<Staff>();
                ReadData rd = new ReadData();
                int month = 0, year = 0;

                while (year == 0)
                {
                    Console.Write("\nPlease enter the year: ");

                    try
                    {
                        year = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " Please try again.");
                    }
                }

                while (month == 0)
                {
                    Console.Write("\nPlease enter the month(i.e., between 0 to 12: ");

                    try
                    {
                        month = Convert.ToInt32(Console.ReadLine());

                        if (month < 1 || month > 12)
                        {
                            Console.WriteLine("Month must be from 1 to 12. Please try again.");
                            month = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " Please try again.");
                    }
                }

                myStaff = rd.ReadFile();

                for (int i = 0; i < myStaff.Count; i++)
                {
                    try
                    {
                        Console.Write("\nEnter hours worked for {0}: ", myStaff[i].employeeName);
                        myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                        myStaff[i].calcThePaySum();
                        Console.WriteLine(myStaff[i].ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        i--;
                    }
                }

                PaySlip ps = new PaySlip(month, year);
                ps.GeneratePaySlip(myStaff);
                ps.GenerateSummary(myStaff);

                Console.Read();
            }


        }
    }

    class Staff
    {
        // Fields
        private float ratePerHour;
        private int numOfHoursWorked;

        // declaring 3 public auto-implemented properties for the class
        public float TotalPay { get; protected set; } // protected setter - public getter
        public float BasicPay { get; private set; } // private setter - public getter
        public string employeeName { get; private set; } // private setter - public getter

        // return the value of numOfHoursWorked
        public int HoursWorked
        {
            get
            {
                return numOfHoursWorked;
            }
            set
            {
                if (value > 0)
                    numOfHoursWorked = value;
                else
                    numOfHoursWorked = 0;
            }
        }

        // Constructor
        public Staff(string name, float rate)
        {
            employeeName = name;
            ratePerHour = rate;
        }
        // virtual method, has no parameters and does not return a value
        public virtual void calcThePaySum()
        {
            Console.WriteLine("Working out the pay..."); // prints the message on the screen

            BasicPay = numOfHoursWorked * ratePerHour; // assign the total for the operation to BasicPay
            TotalPay = BasicPay; // assigne the same value to TotalPay value
        }
        // method to display the values of the fields and properties of the Staff class
        public override string ToString()
        {
            return "\nEmployeeName = " + employeeName
                + "\nratePerHour = " + ratePerHour + "\nnumOfHoursWorked = " + numOfHoursWorked
                + "\nBasicPay = " + BasicPay + "\n\nTotalPay = " + TotalPay;
        }
    }

    // child class of the Staff class as it inherits from the Staff class and overrides the calcThePaySum() method
    // 
    class Manager : Staff
    {
        private const float managerHPayRate = 50;

        // declaring an auto-implmeneted property
        public int Allowance { get; private set; } // private setter - public getter

        // Constructor - to call the base constructor and pass the parameter name along with the field managerHPayRate
        // to the base constructor.
        public Manager(string name) : base(name, managerHPayRate) { }

        // method to override the calcThePaySum() existent method
        public override void calcThePaySum()
        {
            base.calcThePaySum(); // calling the virtual method in the parent class

            Allowance = 0;

            if (HoursWorked > 160)
            {
                Allowance = 1000; // allowance of £1k if worked more than 160h
                TotalPay = BasicPay + Allowance;
            }

        }
        // method to display the values of the fields and properties of the Manager class
        public override string ToString()
        {
            return "\nemployeeName = " + employeeName + "\nmanagerHPayRate = "
                + managerHPayRate + "\nHoursWorked = " + HoursWorked + "\nBasicPay = "
                + BasicPay + "\nAllowance = " + Allowance + "\n\nTotalPay = " + TotalPay;
        }
    }
    // inherits the Staff class and overrides the CalculatePay() method
    class Admin : Staff
    {
        private const float overtimePayRate = 15.5f;
        private const float adminHPayRate = 30f;

        public float Overtime { get; private set; } // private setter - public getter

        // Constructor - to call the base constructor and pass the parameter name along with the field adminHPayRate
        // to the base constructor.
        public Admin(string name) : base(name, adminHPayRate) { }

        // method to override the calcThePaySum() existent method
        public override void calcThePaySum()
        {
            base.calcThePaySum(); // calling the virtual method in the parent class

            if (HoursWorked > 160)
            {
                Overtime = overtimePayRate * (HoursWorked - 160); // paid overtime on top of the basic pay if worked more than 160h
                TotalPay = BasicPay + Overtime;
            }

        }
        // method to display the values of the fields and properties of the Admin class
        public override string ToString()
        {
            return "\nemployeeName = " + employeeName
            + "\nadminHPayRate = " + adminHPayRate + "\nHoursWorked = " + HoursWorked
            + "\nBasicPay = " + BasicPay + "\nOvertime = " + Overtime
            + "\n\nTotalPay = " + TotalPay;
        }
    }
    // Class to read from the .txt file and create a list of staff objects based on the .txt file contents
    class ReadData
    {
        public List<Staff> ReadFile() // read from the .txt file
        {
            List<Staff> myEmployee = new List<Staff>();
            string[] outcome = new string[2];
            string filePath = "employee.txt";
            string[] divider = { ", " };

            if (File.Exists(filePath)) // check if .txt file exists
            {
                using (StreamReader sr = new StreamReader(filePath)) // StreamReader object to read the text file line by line
                {
                    while (!sr.EndOfStream)
                    {
                        // using split () method to split the outcome in 2 strings i.e, name and position
                        outcome = sr.ReadLine().Split(divider, StringSplitOptions.RemoveEmptyEntries);

                        if (outcome[1] == "Manager")
                            myEmployee.Add(new Manager(outcome[0]));
                        else if (outcome[1] == "Admin")
                            myEmployee.Add(new Admin(outcome[0]));
                    }
                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Error: Requested file cannot be found");
            }

            return myEmployee;
        }
    }

    // generate a payslip for each employee within the company + a summary of the details of staff who
    // worked less than 10h in a month
    class PaySlip
    {
        // Fields
        private int month;
        private int year;

        // Enum -  private by default
        enum MonthsOfYear { JAN = 1, FEB = 2, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC }
        // Constructor
        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }
        // Method - takes a list of Staff objects and does not return anything
        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string filePath;

            foreach (Staff i in myStaff)
            {
                filePath = i.employeeName + ".txt";

                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("====================");
                    sw.WriteLine("Employee Name: {0}", i.employeeName);
                    sw.WriteLine("Hours Worked: {0}", i.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:C}", i.BasicPay);

                    if (i.GetType() == typeof(Manager))
                        sw.WriteLine("Allowance: {0:C}", ((Manager)i).Allowance);
                    else if (i.GetType() == typeof(Admin))
                        sw.WriteLine("Overtime: {0:C}", ((Admin)i).Overtime);

                    sw.WriteLine("");
                    sw.WriteLine("====================");
                    sw.WriteLine("Total Pay: {0:C}", i.TotalPay);
                    sw.WriteLine("====================");

                    sw.Close();
                }
            }

        }
        public void GenerateSummary(List<Staff> myStaff)
        {
            var result
                = from i in myStaff
                  where i.HoursWorked < 10
                  orderby i.employeeName ascending
                  select new { i.employeeName, i.HoursWorked };

            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");

                foreach (var i in result)
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", i.employeeName, i.HoursWorked);

                sw.Close();
            }
        }

        public override string ToString()
        {
            return "month = " + month + "year = " + year;
        }

    }
}
