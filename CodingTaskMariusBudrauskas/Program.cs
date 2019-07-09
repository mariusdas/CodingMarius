using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodingTaskMariusBudrauskas
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Proccess();
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured in proccess!");
            }

            Console.ReadLine();
        }

        private static void Proccess()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Input.txt");
            if (path.Count() == 0)
                return;

            string[] lines = System.IO.File.ReadAllLines(path);

            var results = new List<Result>();

            foreach (string line in lines)
            {
                var result = new Result();
                var splitLine = line.Split(' ');

                if (splitLine[0] is string)
                    result.Municipality = splitLine[0];

                result.Date = DateTime.ParseExact(splitLine[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                results.Add(result);
            }

            var taxes = new List<Tax>();

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Taxes.xml"));

            XmlNodeList items = doc.SelectNodes("/Taxes/Tax");
            foreach (XmlNode item in items)
            {
                Tax newTax = new Tax();
                try
                {
                    if (!string.IsNullOrWhiteSpace(item["type"].InnerText))
                        newTax.Type = (TaxType)Enum.Parse(typeof(TaxType), item["type"].InnerText);
                    if (!string.IsNullOrWhiteSpace(item["atDay"].InnerText))
                        newTax.AtDay = DateTime.ParseExact(item["atDay"].InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrWhiteSpace(item["from"].InnerText))
                        newTax.FromPeriod = DateTime.ParseExact(item["from"].InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrWhiteSpace(item["to"].InnerText))
                        newTax.ToPeriod = DateTime.ParseExact(item["to"].InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (!string.IsNullOrWhiteSpace(item["municipality"].InnerText))
                        newTax.Municipality = item["municipality"].InnerText;
                    if (!string.IsNullOrWhiteSpace(item["taxPrice"].InnerText))
                        newTax.TaxPrice = double.Parse(item["taxPrice"].InnerText);
                }
                catch (Exception)
                {
                    continue;
                }

                taxes.Add(newTax);

            }

            Console.WriteLine("- Press 'a' to calculate taxes from input file!");
            Console.WriteLine("- Press 'b' to add taxes record!");
            Console.WriteLine("- Press 'c' to get specific tax!");
            Console.WriteLine("- Press 'd' to see schedule taxes!");

            var readKey = Console.ReadLine();

            if (readKey.ToLower().Equals("a"))
            {
                foreach (var result in results)
                {
                    var findDay = taxes.FirstOrDefault(x => x.AtDay == result.Date && x.Type == TaxType.Daily && x.Municipality == result.Municipality);
                    if (findDay != null)
                    {
                        result.TaxResult = findDay.TaxPrice;
                        continue;
                    }

                    var findWeekly = taxes.FirstOrDefault(x => x.FromPeriod >= result.Date && x.ToPeriod <= result.Date && x.Type == TaxType.Weekly && x.Municipality == result.Municipality);
                    if (findWeekly != null)
                    {
                        result.TaxResult = findWeekly.TaxPrice;
                        continue;
                    }

                    var findMonthly = taxes.FirstOrDefault(x => x.FromPeriod <= result.Date && x.ToPeriod >= result.Date && x.Type == TaxType.Monthly && x.Municipality == result.Municipality);
                    if (findMonthly != null)
                    {
                        result.TaxResult = findMonthly.TaxPrice;
                        continue;
                    }

                    var findYearly = taxes.FirstOrDefault(x => x.FromPeriod <= result.Date && x.ToPeriod >= result.Date && x.Type == TaxType.Yearly && x.Municipality == result.Municipality);
                    if (findYearly != null)
                    {
                        result.TaxResult = findYearly.TaxPrice;
                        continue;
                    }

                }

                Console.WriteLine("Municipality | Date | Result");

                foreach (var result in results)
                {
                    Console.WriteLine(result.Municipality + " " + result.Date + " " + result.TaxResult);
                }
            }
            else if (readKey.Equals("b"))
            {
                var tax = new Tax();

                Console.WriteLine("Type of tax? Possible answer: 'Daily', 'Weekly', 'Monthly', 'Yearly'");
                var type = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(type))
                {
                    tax.Type = (TaxType)Enum.Parse(typeof(TaxType), type);
                }

                Console.WriteLine("if tax is 'daily' type date: 'YYYY-MM-DD' else skip it by pressing enter");
                var dailyDate = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(dailyDate))
                {
                    tax.AtDay = DateTime.ParseExact(dailyDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                Console.WriteLine("if tax is 'not daily' type 'From Date': 'YYYY-MM-DD' else skip it by pressing enter");
                var fromDate = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(fromDate))
                {
                    tax.FromPeriod = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                Console.WriteLine("if tax is 'not daily' type 'To Date': 'YYYY-MM-DD' else skip it by pressing enter");
                var toDate = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(toDate))
                {
                    tax.ToPeriod = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                Console.WriteLine("Municipality name:");
                var munName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(munName))
                {
                    tax.Municipality = munName;
                }

                Console.WriteLine("Tax Price:");
                var taxPrice = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(taxPrice))
                {
                    tax.TaxPrice = double.Parse(taxPrice);
                }

            }
            else if (readKey.Equals("c"))
            {

                Console.WriteLine("Enter Municipality:");
                var mun = Console.ReadLine();

                Console.WriteLine("Enter Date 'yyyy-MM-dd':");
                var date = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var findDay = taxes.FirstOrDefault(x => x.AtDay == date && x.Type == TaxType.Daily && x.Municipality == mun);
                if (findDay != null)
                {
                    Console.WriteLine("Type:" + findDay.Type.ToString() + "|Price:" + findDay.TaxPrice);
                }

                var findWeekly = taxes.FirstOrDefault(x => x.FromPeriod >= date && x.ToPeriod <= date && x.Type == TaxType.Weekly && x.Municipality == mun);
                if (findWeekly != null)
                {
                    Console.WriteLine("Type:" + findWeekly.Type.ToString() + "|Price:" + findWeekly.TaxPrice);
                }

                var findMonthly = taxes.FirstOrDefault(x => x.FromPeriod <= date && x.ToPeriod >= date && x.Type == TaxType.Monthly && x.Municipality == mun);
                if (findMonthly != null)
                {
                    Console.WriteLine("Type:" + findMonthly.Type.ToString() + "|Price:" + findMonthly.TaxPrice);
                }

                var findYearly = taxes.FirstOrDefault(x => x.FromPeriod <= date && x.ToPeriod >= date && x.Type == TaxType.Yearly && x.Municipality == mun);
                if (findYearly != null)
                {
                    Console.WriteLine("Type:" + findYearly.Type.ToString() + "|Price:" + findYearly.TaxPrice);
                }

            }
            else if (readKey.Equals("d"))
            {
                Console.WriteLine("Write Municipality");

                var mun = Console.ReadLine();

                var allTaxesByMunicipality = taxes.Where(x => x.Municipality == mun).ToList();

                Console.WriteLine("Yearly:");

                foreach (var tax in allTaxesByMunicipality.Where(x => x.Type == TaxType.Yearly).ToList())
                {
                    Console.WriteLine(tax.FromPeriod + "-" + tax.ToPeriod + " Price:" + tax.TaxPrice);
                }

                Console.WriteLine("Monthly:");

                foreach (var tax in allTaxesByMunicipality.Where(x => x.Type == TaxType.Monthly).ToList())
                {
                    Console.WriteLine(tax.FromPeriod + "-" + tax.ToPeriod + " Price:" + tax.TaxPrice);
                }

                Console.WriteLine("Weekly:");

                foreach (var tax in allTaxesByMunicipality.Where(x => x.Type == TaxType.Weekly).ToList())
                {
                    Console.WriteLine(tax.FromPeriod + "-" + tax.ToPeriod + " Price:" + tax.TaxPrice);
                }
                Console.WriteLine("Daily:");

                foreach (var tax in allTaxesByMunicipality.Where(x => x.Type == TaxType.Daily).ToList())
                {
                    Console.WriteLine(tax.AtDay + " Price:" + tax.TaxPrice);
                }
            }
        }
    }
}
