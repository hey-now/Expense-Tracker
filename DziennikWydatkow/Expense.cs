using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.DataContractSerializer;


namespace DziennikWydatkow
{
    
    public enum Categories
    {
        Jedzenie = 1,
        Dom,
        Rozrywka,
        Okazjonalne,
        Transport,
        Inne = 0,
        Przychod = -1   
    }

    public class AmountCategoryIncosistencyException : Exception
    {
        //wyrzucane jeśli przychód ma dodatnią kwotę lub wydatek ujemną
        public AmountCategoryIncosistencyException()
        {         
        }
        public AmountCategoryIncosistencyException(string message)
        {
            Console.WriteLine(message);
        }

    }

    [DataContract(Namespace="DziennikWydatkow")]
    public struct Expense
    {
        [DataMember]
        public DateTime TransactionDate;

        [DataMember]
        public decimal Amount;

        [DataMember]
        public Categories Category;

        [DataMember]
        public string Title;

        [DataMember]
        public string Note;


        public Expense(DateTime _TransactionDate, decimal _amount,  Categories _Category, string _Title, string _Note) 
        {
            if ((int)_Category * _amount < 0) //przychód ma dodatnią kwotę lub wydatek ujemną
                throw new AmountCategoryIncosistencyException("Wydatki powinny mieć kwotę dodatnią, a przychody ujemną.");
            TransactionDate = _TransactionDate;
            Amount = _amount;
            Category = _Category;
            Title = _Title;
            Note = _Note;
        }

        public override string ToString()
        {
            return String.Format("| {0,-10} | {1,10} | {2,-15} | {3,-20} | {4,-40}|", TransactionDate.ToString("dd/MM/yyyy") , Amount.ToString("C") , Category, Title ,Note);
        }
    }
}
