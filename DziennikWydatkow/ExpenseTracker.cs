using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace DziennikWydatkow
{
    class ExpenseTracker
    {
        public List<Expense> ExpenseList;

        public ExpenseTracker()
        {
            ExpenseList = new List<Expense>();
        }

        public Expense this[int index]    // Indexer declaration  
        {
            get
            {
                return ExpenseList[index];
            }
            set
            {
                ExpenseList[index] = value;
            }
        }

        public void add(Expense e)
        {
            ExpenseList.Add(e);
        }

        public bool remove(Expense e)
        {
           return ExpenseList.Remove(e);
        }

        public void edit(int index,Expense changed)
        {
            ExpenseList[index] = changed;
        }

        public void editAmountOf(int index, decimal amount)
        {
            Expense changed = ExpenseList[index];
            if ((int)changed.Category * amount < 0)
            {
                throw new AmountCategoryIncosistencyException("Wydatki powinny mieć kwotę dodatnią, a przychody ujemną.");
            }
            changed.Amount = amount;
            
            ExpenseList[index] = changed;
        }

        public void editDateOf(int index, DateTime date)
        {
            Expense changed = ExpenseList[index];
            changed.TransactionDate = date;
            ExpenseList[index] = changed;
        }

        public void editCategoryOf(int index, Categories category )
        {
            Expense changed = ExpenseList[index];
            if ((int)category * changed.Amount < 0)
            {
                throw new AmountCategoryIncosistencyException("Wydatki powinny mieć kwotę dodatnią, a przychody ujemną.");
            }
            changed.Category = category;
            ExpenseList[index] = changed;
        }

        public void editTitleOf(int index, string title)
        {
            Expense changed = ExpenseList[index];
            changed.Title = title;
            ExpenseList[index] = changed;
        }

        public void editNoteOf(int index, string note)
        {
            Expense changed = ExpenseList[index];
            changed.Note = note;
            ExpenseList[index] = changed;
        }

        public void printWithIndexes(DateTime startDate, DateTime endDate)
        {
            var report = from e in ExpenseList
                         where (e.TransactionDate > startDate && e.TransactionDate < endDate)
                         orderby e.TransactionDate
                         select e;

                      
            Console.WriteLine("  Nr  | {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "1. Data", "2. Kwota", "3. Kategoria", "4. Tytuł", "5. Notatka");
            Console.WriteLine(new string('-', 116));
            foreach (Expense expense in report.ToList())
            {
                Console.WriteLine(" {0,4} {1}", ExpenseList.IndexOf(expense), expense.ToString());
            }
         
        }

        public void Serialize(string fileName)
        {
            using (FileStream writer = new FileStream(fileName + ".xml", FileMode.Create, FileAccess.Write))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(List<Expense>));
                ser.WriteObject(writer, ExpenseList);
            }
        }

 
        public void Deserialize(string fileName)
        {
            try {
                using (FileStream reader = new FileStream(fileName + ".xml", FileMode.Open, FileAccess.Read))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(List<Expense>));
                    ExpenseList = (List<Expense>)ser.ReadObject(reader);
                }
            }
            catch (FileNotFoundException e)
            {
                ExpenseList = new List<Expense>();
                Console.WriteLine("Nie ma dzinnika dla podanego uzytkownika. Tworze nowy.");
            }
        }

    }
}
