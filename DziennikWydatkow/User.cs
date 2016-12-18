using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace DziennikWydatkow
{
    [DataContract(Namespace = "DziennikWydatkow")]
    class User
    {
        [DataMember]
        public string Username;

        [DataMember]
        private string Password;


        public ExpenseTracker Expenses;

        public User(string _Username, string _Password)
        {
            Username = _Username;
            Password = _Password;
            Expenses = new ExpenseTracker();
        }

        public Boolean checkPassword(string _password)
        {
            return _password.CompareTo(Password) == 0;
        }

        public void changePassword(string _password)
        {
            Password = _password;
        }
    }
}
