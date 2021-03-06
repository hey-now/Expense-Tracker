﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DziennikWydatkow
{
    public enum Sorting
    {
        Data_malejaco = -1,
        Data_rosnaco = 1,
        Kwota_malejaco = -2,
        Kwota_rosnaco = 2,
        Tytul_malejaco = -3,
        Tytul_rosnaco = 3
    }

    static class ExpenseTrackerReports
    {
        public static Comparison<Expense> ReportOrdering ( Sorting sorting = 0)
        {
            switch(Math.Abs((int)sorting))
            {
               case 3:
                    return ((e1,e2) => String.Compare(e1.Title ,e2.Title ) * (int)sorting); //pomnorzenie przez ujemny kod sortowania odwraca kolejnosc
               case 2:
                    return ((e1, e2) => Decimal.Compare(Math.Abs(e1.Amount),Math.Abs(e2.Amount)) * (int)sorting);                   
               default:
                    return ((e1, e2) => DateTime.Compare(e1.TransactionDate, e2.TransactionDate) * (int)sorting);
                  
            }       
        }


        /*
        Raport ogólny: podanie sumy wydatków, sumy przychodów oraz bilansu i wylistowanie wpisów 
        na podstawie daty początkowej i końcowej 
        (możliwość zmiany sortowania: po dacie, po kwocie, po tytule wydatku)
        */

        public static void GenerateGeneralReport(this ExpenseTracker tracker,  DateTime startDate , DateTime endDate, Sorting sorting = 0)
        {
            Console.WriteLine("\n[RAPORT OGÓLNY]\n");


            var report = from e in tracker.ExpenseList
                      where ( e.TransactionDate >= startDate && e.TransactionDate <=endDate)
                      select e;

            decimal balance = -report.Sum(e => e.Amount);

            Console.WriteLine("Bilans konta: {0}\n", balance.ToString("C"));

            decimal ExpenseSum = report.Sum(x => ((int)x.Category == -1) ? 0 : x.Amount); //suma wydatków i.e. wpisow z kategorii innych niż prychod (-1)
            decimal IncomeSum = report.Sum(x => ((int)x.Category == -1) ? x.Amount : 0);

            Console.WriteLine("Suma wydatków: {0}", ExpenseSum.ToString("C"));
            Console.WriteLine("Suma przychodów: {0}\n", (-IncomeSum).ToString("C"));

            List<Expense> orderedReport = (report.ToList<Expense>());

            orderedReport.Sort(ReportOrdering(sorting));

            Console.WriteLine("| {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "Data", "Kwota", "Kategoria", "Tytuł", "Notatka");
            Console.WriteLine(new string('-', 110));
            foreach (var expense in orderedReport.ToList())
            {
                Console.WriteLine(expense.ToString());
            }
        }

        /*
        Raport kategorii: wylistowanie wpisów z podanej kategorii i zakresu dat,
        podanie ich sumy (możliwość zmiany sortowania: po dacie, po kwocie, po tytule wydatku)
        */

        public static void GenerateCategoryReport(this ExpenseTracker tracker, DateTime startDate, DateTime endDate, Categories category, Sorting sorting = 0)
        {
            Console.WriteLine("\n[RAPORT KATEGORII]\n");


            var report = from e in tracker.ExpenseList
                         where (e.TransactionDate >= startDate && e.TransactionDate <= endDate && e.Category == category)
                         select e;

            decimal balance = report.Sum(e => e.Amount);

            Console.WriteLine(((int)category==-1)?"Suma przychodów: {0}":"Suma wydatków z kategorii "+category+": {0}", balance.ToString("C"));

            List<Expense> orderedReport = (report.ToList<Expense>());

            orderedReport.Sort(ReportOrdering(sorting));

            Console.WriteLine("| {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "Data", "Kwota", "Kategoria", "Tytuł", "Notatka");
            Console.WriteLine(new string('-', 110));
            foreach (var expense in orderedReport.ToList())
            {
                Console.WriteLine(expense.ToString());
            }
        }

        /*
        Raport największych wydatków: lista największych n wydatków w podanym zakresie czasowym 
        (n podawane przez użytkownika)
        */
        public static void GenerateMaxExpensesReport(this ExpenseTracker tracker, DateTime startDate, DateTime endDate, int n)
        {
            Console.WriteLine("\n[RAPORT NAJWIĘKSZYCH WYDATKÓW]\n");


            var report = (from e in tracker.ExpenseList
                          where (e.TransactionDate >= startDate && e.TransactionDate <= endDate)
                          orderby e.Amount descending
                         select e).Take(n);

            //decimal balance = report.Sum(e => e.Amount);

            Console.WriteLine("| {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "Data", "Kwota", "Kategoria", "Tytuł", "Notatka");
            Console.WriteLine(new string('-', 110));
            foreach (var expense in report.ToList())
            {
                Console.WriteLine(expense.ToString());
            }
        }


        /*
        Raport struktury wydatków: prezentuje sumę wydatków oraz procentowy udział każdej kategorii w budżecie w podanym zakresie czasowym
        */
        public static void GenerateStructuralReport(this ExpenseTracker tracker, DateTime startDate , DateTime endDate)
        {
            Console.WriteLine("\n[RAPORT STRUKTURY WYDATKÓW]\n");

            List<Categories> categoriesList = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();


            var report = from e in tracker.ExpenseList
                         where (e.TransactionDate >= startDate && e.TransactionDate <= endDate)
                         group e by e.Category into cat
                         select new
                          {
                            Category = cat.Key,
                            Expenses = cat.Count(),
                            TotalAmount = cat.Sum(e => e.Amount)
                          };

            decimal balance = -report.Sum(e => e.TotalAmount);
            decimal ExpenseSum = report.Sum(x => ((int)x.Category == -1) ? 0 : x.TotalAmount); //suma wydatków i.e. wpisow z kategorii innych niż prychod (-1)
            decimal IncomeSum = report.Sum(x => ((int)x.Category == -1) ? x.TotalAmount : 0);

            Console.WriteLine("Bilans konta: {0}\n", balance.ToString("C"));
            Console.WriteLine("Suma wydatków: {0}", ExpenseSum.ToString("C"));
            Console.WriteLine("Suma przychodów: {0}\n", (-IncomeSum).ToString("C"));


            Console.WriteLine("| {0,-15} | {1,10} | {2,10} | {3,10}  |", "Kategoria", "Ilość", "Suma", "% wydatków");
            Console.WriteLine(new string('-', 55));
            foreach (var row in report.ToList())
            {
                Console.WriteLine("| {0,-15} | {1,10} | {2,10} | {3,10}% |", row.Category.ToString(),row.Expenses,row.TotalAmount, Math.Round(100*row.TotalAmount/ExpenseSum,2));
            }


            foreach (var row in report.ToList())
            {
                if ((int)row.Category != -1)
                {
                    int percent = (int)Math.Abs(20 * row.TotalAmount / ExpenseSum);
                    Console.WriteLine(new string(' ', 15) + new string('_', percent));
                    Console.WriteLine("{0,-15}" + new string('_', percent) + "|  " + Math.Round(100 * row.TotalAmount / ExpenseSum, 2) + "%", row.Category.ToString());
                }
            }


        }

    }
   }
