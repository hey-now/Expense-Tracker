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

        private static void login()
        {
            Console.WriteLine("\n[LOGOWANIE]\n");
            Console.Write("Podaj nazwę użytkownika: ");
            string login = Console.ReadLine();

            if (!users.userExists(login))
            {
                Console.WriteLine("Podany użytkownik nie istnieje. Przejście do zakładania nowego konta.");
                newAccount();
                return;
            }

            Console.Write("Podaj hasło: ");
            string pass = Console.ReadLine();
            User u = users.UserList.Find(x => String.Compare(x.Username, login) == 0);
            int i = 1;

            while (!u.checkPassword(pass))
            {
                if (i>2)
                {
                    Console.WriteLine("Podano złe hasło 3 razy. Powrót do menu.");
                    return;
                }
                Console.WriteLine("Podano złe hasło, spróbuj ponownie.");
                Console.Write("Podaj hasło: ");
                pass = Console.ReadLine();
                i++;
            }

            user = u;
            user.Expenses = new ExpenseTracker();
            user.Expenses.Deserialize(user.Username);

            Console.WriteLine("\nZalogowano pomyślnie.");

            mainMenu();

            return;
        }

        private static void newAccount()
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

            users.Serialize();
            Console.WriteLine("Nowe konto utworzone. Użytkownik zalogowany.");

            mainMenu();

            return;
        }

        private static void mainMenu()
        {

            int userInput = 0;
            do
            {
                Console.WriteLine("\n[MENU]\n");
                Console.WriteLine("1 - Dodaj wpis");
                Console.WriteLine("2 - Edytuj / usuń wpis");
                Console.WriteLine("3 - Generuj raport");
                Console.WriteLine("4 - Zmiana hasła");
                Console.WriteLine("0 - Wyloguj \n");

                string result = Console.ReadLine();
                try
                {
                    userInput = Convert.ToInt32(result);
                }
                catch (System.FormatException e)
                {
                    Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry, podano: " + result);
                    mainMenu();
                }

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
                        Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry z zakresu 0-4, podano: " + userInput);
                        break;             
                }
            } while (userInput != 0);

            return;
        }

        private static void passwordChange()
        {
            Console.WriteLine("\n[ZMIANA HASŁA]\n");

            string pass = Console.ReadLine();
            int i = 0;

            while (!user.checkPassword(pass))
            {
                if (i > 2)
                {
                    Console.WriteLine("Podano złe hasło 3 razy. Powrót do menu.");
                    return;
                }
                Console.WriteLine("Podano złe hasło, spróbuj ponownie.");
                Console.Write("Podaj hasło: ");
                pass = Console.ReadLine();
                i++;
            }

            Console.WriteLine("/nPodaj nowe hasło:");
            pass = Console.ReadLine();

            user.changePassword(pass);
            users.Serialize();

            Console.WriteLine("Hasło zmienione.");

            return;
        }


        private static void reports()
        {
            Console.WriteLine("\n[TRYB RAPOTOWANIA]\n");

            int userInput = 0;
            do
            {
                Console.WriteLine("\nJaki typ raportu chcesz zobaczyć?");
                Console.WriteLine("1 - Raport ogólny");
                Console.WriteLine("2 - Raport kategorii");
                Console.WriteLine("3 - Raport największych wydatków");
                Console.WriteLine("4 - Raport struktury wydatków");
                Console.WriteLine("0 - Zakończ");

                string result = Console.ReadLine();

                try
                {
                    userInput = Convert.ToInt32(result);
                }
                catch (System.FormatException e)
                {
                    Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry, podano: " + result);
                    reports();
                }

                if (userInput == 0) return;

                Console.Write("Podaj date poczatkową (w formacie dd/mm/yyyy): ");
                DateTime startDate = chooseDate();

                Console.Write("Podaj date końcową (w formacie dd/mm/yyyy): ");
                DateTime endDate = chooseDate();

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
                        Console.Write("Ile maksymalnych wydatków chcesz wylistować? ");
                        string input = Console.ReadLine();
                        int n = Convert.ToInt32(input);
                        user.Expenses.GenerateMaxExpensesReport(startDate, endDate, n);
                        break;
                    case 4:
                        user.Expenses.GenerateStructuralReport(startDate, endDate);
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry z zakresu 0-4, podano: " + userInput);
                        break;
                }
            } while (userInput != 0);
          
        }

        private static DateTime chooseDate()
        {
            //Obsługa wprowadzania daty prze użytkownika 

            string dateString = Console.ReadLine();
            DateTime chosenDate = new DateTime();

            try {
                string[] dateStrings = dateString.Split('/');
                chosenDate = new DateTime(Convert.ToInt32(dateStrings[2]), Convert.ToInt32(dateStrings[1]), Convert.ToInt32(dateStrings[0]));
            }
            catch (System.IndexOutOfRangeException e)
            {
                Console.WriteLine("Niepełna data, podano: " + dateString + "\n Podaj nową:");
                chosenDate = chooseDate();
            }
            catch (System.FormatException e)
            {
                Console.WriteLine("Nieprawidłowa data, podano: " + dateString + "\n Podaj nową:");
                chosenDate = chooseDate();
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Nieprawidłowa data, podano: "+ dateString + "\n Podaj nową:");
                chosenDate = chooseDate();
            }

            return chosenDate;
        }

        private static Sorting chooseSorting()
        {
            //Wybór sortowania przez użytkownika

            Console.WriteLine("\nLista typów sortowania");
            List<Sorting> sortingList = Enum.GetValues(typeof(Sorting)).Cast<Sorting>().ToList();
            foreach (Sorting s in sortingList)
            {
                Console.WriteLine((int)s + " - " + s.ToString());
            }

            Console.Write("Nr typu sortowania: ");

            string input = Console.ReadLine();
            Sorting  sort = (Sorting)Convert.ToInt32(input);

            return sort;
        }

        private static Categories chooseCategory()
        {
            //Wybór kategorii przez użytkownika

            Console.WriteLine("\nLista kategorii:");
            List<Categories> categoriesList = Enum.GetValues(typeof(Categories)).Cast<Categories>().ToList();
            foreach (Categories c in categoriesList)
            {
                Console.WriteLine((int)c + " - " + c.ToString());
            }

            Console.Write("Podaj nr kategorii: ");

            string input = Console.ReadLine();
            Categories category = (Categories)Convert.ToInt32(input);

            return category;
        }

        private static decimal chooseAmount()
        {
            //Obsługa wprowadzania kwoty przez użytkownika 

            string input = Console.ReadLine();
            decimal amount;
            
            try
            {
               amount = Convert.ToDecimal(input);
            }         
            catch (System.FormatException e)
            {
                Console.WriteLine("Nieprawidłowa wartość, podano: " + input + "\n Podaj nową:");
                amount = chooseAmount();
            }

            return amount;
        }


        private static void editExpense()
        {
            Console.WriteLine("\n[TRYB EDYCJI]\n");

            Console.WriteLine("W jakim zakresie czasowym był wpis do edycji?");
            Console.Write("Podaj date poczatkową (w formacie dd/mm/yyyy): ");
            DateTime startDate = chooseDate();

            Console.Write("Podaj date końcową (w formacie dd/mm/yyyy): ");
            DateTime endDate = chooseDate();


            int userInput = 0;
            do
            {
                user.Expenses.printWithIndexes(startDate, endDate);

                Console.WriteLine("\nPodaj nr wpisu do edycji:");
                string input = Console.ReadLine();
                int nr = Convert.ToInt32(input);

                Console.WriteLine("\nWybrano: ");        
                Console.WriteLine("| {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "1. Data", "2. Kwota", "3. Kategoria", "4. Tytuł", "5. Notatka");
                Console.WriteLine(new string('-', 110));
                Console.WriteLine(user.Expenses[nr].ToString() + "\n");

                Console.WriteLine("\n1 - Usuń wpis");
                Console.WriteLine("2 - Edytuj wpis");
                Console.WriteLine("0 - Zapisz i zakończ");

                string result = Console.ReadLine();
                try
                {
                    userInput = Convert.ToInt32(result);
                }
                catch (System.FormatException e)
                {
                    userInput = -1; //aby wpaść w default case switcha poniżej
                }

                switch (userInput)
                {
                    case 1:
                        if (user.Expenses.remove(user.Expenses[nr])) Console.WriteLine("Usunięto wpis");
                        break;
                    case 2:
                        Console.Write("Podaj nr kolumny do edycji:");
                        input = Console.ReadLine();
                        int col = 0;
                        try
                        {
                            col = Convert.ToInt32(input);
                       
                        switch (col)
                        {
                            case 1:
                                    Console.Write("Podaj nową datę: ");
                                    DateTime date = chooseDate();
                                    user.Expenses.editDateOf(nr, date);
                                    break;
                            case 2:
                                    Console.Write("Podaj nową kwotę: ");
                                    decimal amount = chooseAmount();
                                    user.Expenses.editAmountOf(nr, amount);
                                    break;
                            case 3:
                                    Categories category = chooseCategory();
                                    user.Expenses.editCategoryOf(nr, category);
                                    break;
                            case 4:
                                    Console.Write("Podaj nowy tutuł: ");
                                    input = Console.ReadLine();
                                    user.Expenses.editTitleOf(nr, input);
                                    break;
                            case 5:
                                    Console.Write("Podaj nową notatkę: ");
                                    input = Console.ReadLine();
                                    user.Expenses.editNoteOf(nr, input);
                                    break;
             
                            default:
                                    Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry z zakresu 1-5, podano: " + nr);
                                    break;
                        }

                        
                            Console.WriteLine("\nObecny wpis: ");
                            Console.WriteLine("| {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "1. Data", "2. Kwota", "3. Kategoria", "4. Tytuł", "5. Notatka");
                            Console.WriteLine(new string('-', 110));
                            Console.WriteLine(user.Expenses[nr].ToString() + "\n");

                        }
                        catch (AmountCategoryIncosistencyException e)
                        {
                            Console.WriteLine("Operacja nie została wykonana, spróbuj ponownie.");
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry, input: " + result);
                        }
                        break;
                    case 0:
                        user.Expenses.Serialize(user.Username);
                        Console.WriteLine("Zmiany zostały zapisane\n");
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry z zakresu 0-2, podano: " + result);
                        break;
                }



                Console.WriteLine("\n1 - Edytuj kolejny wpis");
                Console.WriteLine("0 - Zapisz i zakończ");

                result = Console.ReadLine();
                try
                {
                    userInput = Convert.ToInt32(result);
                }
                catch (System.FormatException e)
                {
                    Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry, podano: " + result);
                    userInput = 0;
                }

                if (userInput == 0)
                {     
                    user.Expenses.Serialize(user.Username);
                    Console.WriteLine("Zmiany zostały zapisane\n");
                    return;
                }



            } while (userInput != 0);


        }

        private static void newExpense()
        {
            Console.WriteLine("\n[DODAWANIE WPISU]\n");
            Console.WriteLine("Wprowadź dane"); 
            Console.Write("Data (w formacie dd/mm/yyyy): ");
            DateTime date = chooseDate();

            Console.Write("Kwota: ");
            decimal amount = chooseAmount();

            Categories category = chooseCategory();

            Console.Write("Tytul: ");
            string title = Console.ReadLine();

            Console.Write("Notatka: ");
            string note = Console.ReadLine();

            try {
                Expense newE = new Expense(date, amount, category, title, note);
                user.Expenses.add(newE);

                Console.WriteLine("Wpis dodany pomyślnie");

                Console.WriteLine();
                Console.WriteLine("| {0,-10} | {1,-10} | {2,-15} | {3,-20} | {4,-40}|", "1. Data", "2. Kwota", "3. Kategoria", "4. Tytuł", "5. Notatka");
                Console.WriteLine(new string('-', 110));
                Console.WriteLine(newE.ToString() + "\n");

                user.Expenses.Serialize(user.Username);
            }
            catch (AmountCategoryIncosistencyException e)
            {
                Console.WriteLine("Spróbuj ponownie.");
                newExpense();
                return;
            }

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
                Console.WriteLine("\n[DZIENNIK WYDATKÓW]\n");
                Console.WriteLine("1 - Zaloguj");
                Console.WriteLine("2 - Nowe konto");
                Console.WriteLine("0 - Zakończ");

                string result = Console.ReadLine();

                try
                {
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
                        default:
                            Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry z zakresu 0-2, podano: " + result);
                            break;

                    }
                        
                }
                catch (System.FormatException e)
                {
                    Console.WriteLine("Nieprawidłowe dane. Oczekiwano cyfry, podano: " + result);
                    userInput = -1;
                }

            } while (userInput != 0);

                  
            user.Expenses.Serialize(user.Username);
           
            users.Serialize();
        }
    }
}
