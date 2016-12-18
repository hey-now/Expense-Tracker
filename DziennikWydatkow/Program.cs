using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DziennikWydatkow
{
    class Program
    {
        private static Users users;
        private static User user;

        static void login()
        {
            Console.WriteLine("\n[LOGOWANIE]\n");
            Console.WriteLine("Podaj nazwę użytkownika");
            string login = Console.ReadLine();

            if (!users.userExists(login))
            {
                Console.WriteLine("Podany użytkoenik nie istnieje. Przejście do zakładania nowego konta.");
                newAccount();
                return;
            }

            Console.WriteLine("Podaj hasło");
            string pass = Console.ReadLine();

            while (!users.UserList.Find(x => String.Compare(x.Username, login) == 0).checkPassword(pass))
            {
                Console.WriteLine("Podano złe hasło, spróbój ponownie.");
                Console.WriteLine("Podaj hasło:");
                pass = Console.ReadLine();
            }

            user = users.UserList.Find(x => String.Compare(x.Username, login) == 0);
            user.Expenses = new ExpenseTracker();
            user.Expenses.Deserialize(user.Username);

            Console.WriteLine("Zalogowano pomyślnie.");

            mainMenu();

            return;
        }

        static void newAccount()
        {
            Console.WriteLine("\n[NOWE KONTO]\n");
            Console.WriteLine("Podaj nazwę użytkownika:");
            string login = Console.ReadLine();

            while (users.userExists(login))
            {
                Console.WriteLine("Użytkownik o podanej nazwie istnieje. Podaj nową:");
                login = Console.ReadLine();
            }

            Console.WriteLine("Podaj hasło:");
            string pass = Console.ReadLine();

            User newUser = new User(login, pass);
            users.addUser(newUser);
            user = newUser;
            user.Expenses.Deserialize(user.Username);


            Console.WriteLine("Nowe konto utworzone. Użytkownik zalogowany.");

            mainMenu();

            return;
        }

        public static void mainMenu()
        {

            int userInput = 0;
            do
            {
                Console.WriteLine("\nMENU\n");
                Console.WriteLine("1 - Dodaj wpis");
                Console.WriteLine("2 - Edytuj / usuń wpis");
                Console.WriteLine("3 - Generuj raport");
                Console.WriteLine("4 - Zmiana hasła");
                Console.WriteLine("0 - Zakończ");

                string result = Console.ReadLine();
                userInput = Convert.ToInt32(result);

                switch (userInput)
                {
                    case 1:
                        newExpense();
                        break;
                    case 2:
                        editExpense();
                        break;
                    case 3:
                        reports();
                        break;
                    case 4:
                        passwordChange();
                        break;
                    case 0:
                        return;
                    default:
                        break;             
                }
            } while (userInput != 0);

            return;
        }

        private static void passwordChange()
        {
            Console.WriteLine("\n[ZMIANA HASŁA]\n");
            Console.WriteLine("Podaj nowe hasło:");
            string pass = Console.ReadLine();

            user.changePassword(pass);

            Console.WriteLine("Hasło zmienione.");

            return;
        }

        private static void reports()
        {

            int userInput = 0;
            do
            {
                Console.WriteLine("Jaki typ raportu chcesz zobaczyć?");
                Console.WriteLine("1 - Raport ogólny");
                Console.WriteLine("2 - Raport kategorii");
                Console.WriteLine("3 - Raport największych wydatków");
                Console.WriteLine("4 - Raport struktury wydatków");
                Console.WriteLine("0 - Zakończ");

                string result = Console.ReadLine();
                userInput = Convert.ToInt32(result);

                if (userInput == 0) return;

                Console.WriteLine("Podaj date poczatkową (w formacie dd/mm/yyyy):");
                string dateString = Console.ReadLine();
                string[] dateStrings = dateString.Split('/');
                DateTime startDate = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));

                Console.WriteLine("Podaj date końcową (w formacie dd/mm/yyyy):");
                dateString = Console.ReadLine();
                dateStrings = dateString.Split('/');
                DateTime endDate = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));

                Console.WriteLine(startDate.ToString() + endDate);

                switch (userInput)
                {
                    case 1:
                        Sorting sort = chooseSorting();
                        user.Expenses.GenerateGeneralReport(startDate, endDate, sort);
                        break;
                    case 2:
                        Categories category = chooseCategory();
                        Sorting sorttype = chooseSorting();
                        user.Expenses.GenerateCategoryReport(startDate, endDate, category, sorttype);
                        break;
                    case 3:
                        Console.WriteLine("Ile maksymalnych wydatków chcesz wylistować?");
                        string input = Console.ReadLine();
                        int n = Convert.ToInt32(input);
                        user.Expenses.GenerateMaxExpensesReport(startDate, endDate, n);
                        break;
                    case 4:
                        user.Expenses.GenerateStructuralReport(startDate, endDate);
                        break;
                    case 0:
                        return;

                }

                mainMenu();

            } while (userInput != 0);
          
        }

        private static Sorting chooseSorting()
        {
            Console.WriteLine("Lista typów sortowania");
            List<Sorting> sortingList = Enum.GetValues(typeof(Sorting)).Cast<Sorting>().ToList();
            foreach (Sorting s in sortingList)
            {
                Console.WriteLine((int)s + " - " + s.ToString());
            }

            Console.WriteLine("Który typ sortowania wybierasz?");

            string input = Console.ReadLine();
            Sorting  sort = (Sorting)Convert.ToInt32(input);

            return sort;
        }

        private static Categories chooseCategory()
        {
            Console.WriteLine("Lista kategorii");
            List<Categories> categoriesList = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            foreach (Categories c in categoriesList)
            {
                Console.WriteLine((int)c + " - " + c.ToString());
            }

            Console.WriteLine("Którą kategorię wybierasz?");

            string input = Console.ReadLine();
            Categories category = (Categories)Convert.ToInt32(input);

            return category;
        }

        private static void editExpense()
        {
            Console.WriteLine("\n[TRYB EDYCJI]\n");

            Console.WriteLine("W jakim zakresie czasowym był wpis do edycji?");
            Console.WriteLine("Podaj date poczatkową (w formacie dd/mm/yyyy):");
            string dateString = Console.ReadLine();
            string[] dateStrings = dateString.Split('/');
            DateTime startDate = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));

            Console.WriteLine("Podaj date końcową (w formacie dd/mm/yyyy):");
            dateString = Console.ReadLine();
            dateStrings = dateString.Split('/');
            DateTime endDate = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));

            user.Expenses.printWithIndexes(startDate,endDate);

            Console.WriteLine("Podaj nr wpisu do edycji:");
            string input = Console.ReadLine();
            int nr = Convert.ToInt32(input);

            

            int userInput = 0;
            do
            {
                Console.WriteLine("Wybrano\n" + user.Expenses[nr].ToString()+ "\n");

                Console.WriteLine("1 - Usuń");
                Console.WriteLine("2 - Edytuj");
                Console.WriteLine("0 - Zapisz i zakończ");

                string result = Console.ReadLine();
                userInput = Convert.ToInt32(result);

                switch (userInput)
                {
                    case 1:
                        if (user.Expenses.remove(user.Expenses[nr])) Console.WriteLine("Usunięto wpis");
                        break;
                    case 2:
                        Console.WriteLine("Podaj nr kolumny do edycji:");
                        input = Console.ReadLine();
                        nr = Convert.ToInt32(input);

                        Console.WriteLine("Podaj nową wartość:");
                        input = Console.ReadLine();
                       
                        switch (nr)
                        {
                            case 1:
                                dateString = Console.ReadLine();
                                dateStrings = dateString.Split('/');
                                DateTime date = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));
                                user.Expenses.editDateOf(nr, date);
                                break;
                            case 2:
                                decimal amount = Convert.ToDecimal(input);
                                user.Expenses.editAmountOf(nr, amount);
                                break;
                            case 3:
                                Categories category = (Categories)Convert.ToInt32(input);
                                user.Expenses.editCategoryOf(nr, category);
                                break;
                            case 4:
                                user.Expenses.editTitleOf(nr, input);
                                break;
                            case 5:
                                user.Expenses.editNoteOf(nr, input);
                                break;
                        }

                        Console.WriteLine("Zmieniony wpis:\n" + user.Expenses[nr].ToString() + "\n");

                        break;
                    case 0:
                        user.Expenses.Serialize(user.Username);
                        return;

                }

                mainMenu();

            } while (userInput != 0);


        }

        private static void newExpense()
        {
            Console.WriteLine("\n[DODAWANIE WPISU]\n");
            Console.WriteLine("Podaj wydatek w formacie dd/mm/yyyy:kwota:nr kategori:tytul:notatka\n");

            Console.WriteLine("Lista kategorii");
            List<Categories> categoriesList = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            foreach(Categories c in categoriesList)
            {
                Console.WriteLine((int)c + " - " + c.ToString());
            }

            Console.WriteLine("\nPodaj wpis");
            string expenseString = Console.ReadLine();
            string[] expenseData = expenseString.Split(':');

            string[] dateStrings = expenseData[0].Split('/');
            DateTime date = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));
            decimal amount = Convert.ToDecimal(expenseData[1]);
            Categories category = (Categories)Convert.ToInt32(expenseData[2]);
            string title = expenseData[3];
            string note = expenseData[4];

            user.Expenses.add(new Expense(date, amount, category, title, note));

            Console.WriteLine("Wpis dodany pomyślnie");

            user.Expenses.Serialize(user.Username);

            return;

        }

        static void Main(string[] args)
        {
            user = new User("","");
            users = new Users();


            users.Deserialize();

            

            int userInput = 0;
            do
            {
                Console.WriteLine("1 - Zaloguj");
                Console.WriteLine("2 - Nowe konto");
                Console.WriteLine("0 - Zakończ");

                string result = Console.ReadLine();
                userInput = Convert.ToInt32(result);

                switch (userInput)
                {
                    case 1:
                        login();
                        break;
                    case 2:
                        newAccount();
                        break;
                    case 0:
                        break;
                                    
                }

               

            } while (userInput != 0);

                  

            /*

       ////     Console.WriteLine(users.userExists("Maja"));
        //    users.addUser(new User("Maja", "maslo"));
       //     Console.WriteLine(users.userExists("Maja"));

         //  user = new User("Maja", "maslo");
            user.Expenses.Deserialize(user.Username);

            //user.Expenses.add(new Expense(new DateTime(2016, 12, 1), (decimal)7.95, Categories.Jedzenie, "Obiad", "Subway"));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 1), (decimal)24.90, Categories.Dom, "Patelnia", ""));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 2), (decimal)109.00, Categories.Okazjonalne, "Lot", "Bilet do Londynu"));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 3), (decimal)8.00, Categories.Rozrywka, "Piwo", "Pawilony"));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 3), (decimal)22.40, Categories.Jedzenie, "Kolacja", "Manekin"));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 4), (decimal)35.00, Categories.Rozrywka, "Kino", "Star Wars"));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 4), (decimal)10.00, Categories.Inne, "Parking", "Złote tarasy"));
            //user.Expenses.add(new Expense(new DateTime(2016, 12, 5), (decimal)24.90, Categories.Dom, "Patelnia", ""));

            DateTime s = DateTime.Today.AddMonths(-3);
            DateTime e = DateTime.Today;

            user.Expenses.printWithIndexes(s,e);

            Expense changed = new Expense(new DateTime(2016, 12, 2, 7, 0, 0), (decimal)8.00, Categories.Rozrywka, "Piwo", "Spectrum");
            user.Expenses.edit(2, changed);

            user.Expenses.editAmountOf(3, (decimal)200.00);
            //user.Expenses.ToXml();
            user.Expenses.printWithIndexes(s,e);

           

            user.Expenses.GenerateGeneralReport(s,e,(Sorting)(-2));

            user.Expenses.GenerateCategoryReport(s, e, Categories.Okazjonalne);

           // user.ExpenseTrackerReports.endDate = DateTime.Today.AddMonths(-1);
            user.Expenses.GenerateMaxExpensesReport(s, e, 5);

            user.Expenses.GenerateStructuralReport(s, e);
 */
            user.Expenses.Serialize(user.Username);
           
            users.Serialize();
        }
    }
}
