// See https://aka.ms/new-console-template for more information
// Project from "Learn C# in one day and learn it well by Jaime Chan
// Code written by Henry Cates following the book prompts.
// An application that generates the salary slips of a small company

namespace BookCsProject
{
    public class Staff
    {
        // Declare private fields
        private float _hWorked;
        private float _hourlyRate;

        // Declare public properties/accessors
        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }
        public float HoursWorked
        {
            get { return _hWorked; }
            set
            {
                if (value > 0)
                    _hWorked = value;
                else
                    _hWorked = 0;
            }
        }

        // Constructor
        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            _hourlyRate = rate;
        }

        // Declare a virtual method so each inherited class can overwrite
        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = _hWorked * _hourlyRate;
            TotalPay = BasicPay;
        }

        // Overwrite ToString() method so it prints values of the fields and properties in the Staff class.
        // I need to come back to this, as I don't recall exactly how to override the method.
        //public override string ToString()
        //{
        //    return base.ToString();
        //}

    }

    class Manager : Staff
    {
        private const float managerHourlyRate = 50;

        public float Allowance { get; private set; }

        // Constructor
        public Manager(string name) : base(name, managerHourlyRate) { }

        // Override CalculatePay()
        public override void CalculatePay()
        {
            // Set the values of the BasicPay and TotalPay
            base.CalculatePay();
            Allowance = 0;
            if (HoursWorked > 160)
            {
                Allowance = 1000;
                TotalPay += Allowance;
            }

        }
        public override string ToString()
        {
            return "Name of Staff: " + NameOfStaff + ", hourly rate: " + managerHourlyRate + ", Hours worked: " + HoursWorked;
        }
    }

    class Admin : Staff
    {
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        public float Overtime { get; private set; }

        // Constructor
        public Admin(string name) : base(name, adminHourlyRate) { }

        // Call base class CalculatePay() method then override it
        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay += Overtime;
            }
        }

        // Override ToString() Method from System
        public override string ToString()
        {
            return "Name of Staff: " + NameOfStaff + ", hourly rate: " + adminHourlyRate + ", Hours worked: " + HoursWorked;
        }
    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "zzstaff.txt";
            string[] separator = { ", " };

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.EndOfStream != true)
                    {
                        // Read a line from the file and break into an array containing name and position
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        // If the staff member is a Manager  or Admin based on input then create their respective classes
                        // and add them to the list of staff
                        if (result[1] == "Manager")
                        {
                            Manager manager = new Manager(result[0]);
                            myStaff.Add(manager);
                        }
                        else
                        {
                            Admin admin = new Admin(result[0]);
                            myStaff.Add(admin);
                        }
                    }
                    Console.WriteLine("End of File reached, closing reader.");
                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Error: file does not exist");
            }
            return myStaff;
        }
    }

    class PaySlip
    {
        private int month;
        private int year;
        private string border = "=======================================";

        enum MonthsOfYear
        {
            JAN = 1,
            FEB = 2,
            MAR = 3,
            APR = 4,
            MAY = 5,
            JUN = 6,
            JUL = 7,
            AUG = 8,
            SEP = 9,
            OCT = 10,
            NOV = 11,
            DEC = 12,
        }

        public PaySlip(int payMonth, int payYear)
        {
            this.month = payMonth;
            this.year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;
            foreach (Staff member in myStaff)
            {
                path = member.NameOfStaff + ".txt";

                // Create a stream writer to save the pay slips to the current working directory
                //using (FileStream fs = new FileStream(path, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("Payslip for {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine(border);
                    sw.WriteLine("Name of staff: {0}", member.NameOfStaff);
                    sw.WriteLine("Hours worked: {0}", member.HoursWorked);
                    sw.WriteLine("\nBasic Pay: {0:C}", member.BasicPay);

                    // Allowance (manager) or Overtime (admin) depending the staff member type
                    if (member.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Allowance: {0:C}", ((Manager)member).Allowance);
                    }

                    if (member.GetType() == typeof(Admin))
                    {
                        sw.WriteLine("Overtime: {0:C}", ((Admin)member).Overtime);
                    }

                    sw.WriteLine(border);
                    sw.WriteLine("Total Pay: {0:C}", member.TotalPay);
                    sw.WriteLine(border);

                    sw.Close();
                }
            }

        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            string path = "summary.txt";

            // Create a LINQ requst to see which member of staff did not work more than 10 hours
            var w = from Staff member in myStaff
                    where (member.HoursWorked <= 10)
                    select member;

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 workign hours");
                foreach (var i in w)
                {
                    sw.WriteLine("Name of Staff: {0}", i.NameOfStaff);
                    sw.WriteLine("Hours worked: {0}", i.HoursWorked);
                }

                sw.Close();
            }
        }

        public override string ToString()
        {
            return "I am overriding ToString() method in Summary list";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader getContent = new FileReader();
            int month = 0, year = 0;

            while (year == 0)
            {
                Console.WriteLine("Enter the 4 digit year: ");
                try
                {
                    string? temp = Console.ReadLine();
                    if (temp == null)
                    {
                        Console.WriteLine("You did not enter the 4 digit year: ");
                        continue;
                    }
                    else if (Convert.ToInt32(temp) < 0 || Convert.ToInt32(temp) > DateTime.Now.Year)
                    {
                        Console.WriteLine("You did not enter the 4 digit year correctly must be greater than 0 or less than equal to current year (no future dates): ");
                        continue;
                    }
                    else
                    {
                        year = Convert.ToInt32(temp);
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Format Exeception Error");
                }
            }

            while (month == 0)
            {
                Console.WriteLine("Enter the digit for the month: ");
                try
                {
                    string? temp = Console.ReadLine();
                    if (temp == null)
                    {
                        Console.WriteLine("Enter the numerical month (1 or 2 digits): ");
                    }
                    else if (Convert.ToInt32(temp) > 12 || Convert.ToInt32(temp) < 1)
                    {
                        Console.WriteLine("The input is not valid. Must be an Integer greater than 0 or less than equal to  12");
                    }
                    else
                    {
                        month = Convert.ToInt32(temp);
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Format Exeception Error");
                }
            }

            // Call file reader here
            myStaff = getContent.ReadFile();

            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine("Please enter the number of hours worked for {0}", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = (float)Convert.ToDouble(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    //Console.WriteLine("Staffer info:\n Name: {0}\nHours worked {1}\nTotalPay: {2}", myStaff[i].NameOfStaff, myStaff[i].HoursWorked, myStaff[i].TotalPay);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.ToString());
                    // Don't skip over the staffer that had an error
                    i--;
                }
            }



            // Build pay slips
            PaySlip paySlip = new(month, year);
            paySlip.GeneratePaySlip(myStaff);
            paySlip.GenerateSummary(myStaff);

            Console.WriteLine("Press any key to close");
            Console.ReadLine();
        }
    }
}