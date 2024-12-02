namespace BookStorage
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Storage storage = new();
            storage.OpenMenu();
        }
    }

    public static class UserUtilits
    {
        public static bool TryRequestString(string massage, out string returnedString)
        {
            Console.WriteLine(massage);
            returnedString = Console.ReadLine();

            if (returnedString == string.Empty)
            {
                Console.WriteLine("Вы ничего не ввели.");
                return false;
            }

            return true;
        }

        public static bool TryRequestNumber(string massage, out int returnedNumber)
        {
            if (TryRequestString(massage, out string renurnedStringNumber))
            {
                if (int.TryParse(renurnedStringNumber, out returnedNumber))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"\"{renurnedStringNumber}\" не является числом");
                }
            }

            returnedNumber = 0;
            return false;
        }
    }

    public class Storage
    {
        private List<Book> _books;

        public Storage()
        {
            _books = new();
        }

        public void OpenMenu()
        {
            const string CommandAddBook = "1";
            const string CommandRemoveBook = "2";
            const string CommandShowAllBooks = "3";
            const string CommandShowFilteredBooks = "4";
            const string CommandExit = "5";

            bool isActive = true;

            while (isActive)
            {
                Console.Clear();
                Console.WriteLine($"{CommandAddBook}) Добавить книгу\n" +
                                  $"{CommandRemoveBook}) Удалить книгу\n" +
                                  $"{CommandShowAllBooks}) Показать все книги\n" +
                                  $"{CommandShowFilteredBooks}) Показать книги по параметру\n" +
                                  $"{CommandExit}) Закрыть программу");
                string userCommand = Console.ReadLine();
                Console.Clear();

                switch (userCommand)
                {
                    case CommandAddBook:
                        AddBook();
                        break;

                    case CommandRemoveBook:
                        RemoveBook();
                        break;

                    case CommandShowAllBooks:
                        ShowBooks(_books);
                        break;

                    case CommandShowFilteredBooks:
                        ShowBooksByParametr(SelectParametr());
                        break;

                    case CommandExit:
                        isActive = false;
                        break;
                }
            }
        }

        private void AddBook()
        {
            if (TryRequsetAllParametrs(out string name, out string author, out int year))
            {
                Book addedBook = new(name, author, year);

                if (TryFindBook(_books, name, author, year, out Book _))
                {
                    Console.WriteLine("Такая книга уже существует");
                }
                else
                {
                    _books.Add(addedBook);
                    Console.WriteLine("Добавлена следующая книга:");
                    ShowBook(addedBook);
                }
            }

            Console.ReadKey();
        }

        private void RemoveBook()
        {
            ShowBooks(_books);

            if (TryRequsetAllParametrs(out string name, out string author, out int year))
            {
                if (TryFindBook(_books, name, author, year, out Book book))
                {
                    _books.Remove(book);
                    Console.WriteLine("Удалена следующая книга:");
                    ShowBook(book);
                }
                else
                {
                    Console.WriteLine("книга не найдена");
                }
            }

            Console.ReadKey();
        }

        private void ShowBooks(List<Book> books)
        {
            foreach (var book in books)
            {
                ShowBook(book);
            }

            Console.ReadKey();
        }

        private void ShowBook(Book book)
        {
            Console.WriteLine($"название: {book.Name} | автор: {book.Author} | год выпуска: {book.Year}");
        }

        private bool TryFindBook(List<Book> books, string name, string author, int year, out Book book)
        {
            List<Book> filteredBooks = FilterBooks(books, BookParameter.Name, name);
            filteredBooks = FilterBooks(filteredBooks, BookParameter.Author, author);
            filteredBooks = FilterBooks(filteredBooks, BookParameter.Year, year);

            if (filteredBooks.Count > 0)
            {
                book = filteredBooks[0];
                return true;
            }
            else
            {
                book = null;
                return false;
            }
        }

        private List<Book> FilterBooks(List<Book> books, BookParameter bookParameter, object parameterValue)
        {
            List<Book> filteredBooks = new();
            bool isCorrectParametr = false;
            bool isCorrectType = false;

            switch (bookParameter)
            {
                case BookParameter.Name:
                    isCorrectType = parameterValue is string;
                    break;

                case BookParameter.Author:
                    isCorrectType = parameterValue is string;
                    break;

                case BookParameter.Year:
                    isCorrectType = parameterValue is int;
                    break;
            }

            if (isCorrectType == false)
            {
                throw new ArgumentException("incorrect parameter type", nameof(parameterValue));
            }

            foreach (var book in books)
            {
                switch (bookParameter)
                {
                    case BookParameter.Name:
                        isCorrectParametr = book.Name == (string)parameterValue;
                        break;

                    case BookParameter.Author:
                        isCorrectParametr = book.Author == (string)parameterValue;
                        break;

                    case BookParameter.Year:
                        isCorrectParametr = book.Year == (int)parameterValue;
                        break;
                }

                if (isCorrectParametr)
                {
                    filteredBooks.Add(book);
                }
            }

            return filteredBooks;
        }

        private bool TryRequestParametr(BookParameter bookParametr, out object parameterValue)
        {
            string massage = "Введите ";

            switch (bookParametr)
            {
                case BookParameter.Name:
                    massage += "название ";
                    break;

                case BookParameter.Author:
                    massage += "автора ";
                    break;

                case BookParameter.Year:
                    massage += "год выпуска ";
                    break;
            }

            massage += "книги";

            if (bookParametr == BookParameter.Year)
            {
                if (UserUtilits.TryRequestNumber(massage, out int number))
                {
                    if (number < 0)
                    {
                        Console.WriteLine("Год не может быть отрицательным.");
                        parameterValue = null;
                        return false;
                    }

                    parameterValue = number;
                    return true;
                }
            }
            else
            {
                if (UserUtilits.TryRequestString(massage, out string str))
                {
                    parameterValue = str;
                    return true;
                }
            }

            parameterValue = null;
            return false;
        }

        private bool TryRequsetAllParametrs(out string name, out string author, out int year)
        {
            if (TryRequestParametr(BookParameter.Name, out object returnedName))
            {
                if (TryRequestParametr(BookParameter.Author, out object returnedAuthor))
                {
                    if (TryRequestParametr(BookParameter.Year,out object returnedYear))
                    {
                        name = (string)returnedName;
                        author = (string)returnedAuthor;
                        year = (int)returnedYear;
                        return true;
                    }
                }
            }

            name = string.Empty;
            author = string.Empty;
            year = -1;
            return false;
        }

        private void ShowBooksByParametr(BookParameter bookParameter)
        {
            if (TryRequestParametr(bookParameter, out object parameterValue))
            {
                List<Book> filteredBooks = FilterBooks(_books, bookParameter, parameterValue);

                if (filteredBooks.Count > 0)
                {
                    ShowBooks(filteredBooks);
                }
                else
                {
                    Console.WriteLine("Книги не найдены");
                }

                Console.ReadKey();
            }
        }

        private BookParameter SelectParametr()
        {
            const string NameParameter = "1";
            const string AuthorParameter = "2";
            const string YearParameter = "3";

            bool isSelecting = true;
            BookParameter selectedParametr = default;

            while (isSelecting)
            {
                Console.WriteLine("Выберите параметр книги");
                Console.WriteLine($"{NameParameter}) Название");
                Console.WriteLine($"{AuthorParameter}) Автор");
                Console.WriteLine($"{YearParameter}) Год выпуска");

                switch (Console.ReadLine())
                {
                    case NameParameter:
                        selectedParametr = BookParameter.Name;
                        isSelecting = false;
                        break;

                    case AuthorParameter:
                        selectedParametr = BookParameter.Author;
                        isSelecting = false;
                        break;

                    case YearParameter:
                        selectedParametr = BookParameter.Year;
                        isSelecting = false;
                        break;
                }
            }

            return selectedParametr;
        }
    }

    public class Book
    {
        public Book(string name, string author, int year)
        {
            Name = name;
            Author = author;
            Year = year;
        }

        public string Name { get; }

        public string Author { get; }

        public int Year { get; }
    }

    public enum BookParameter
    {
        Name,
        Author,
        Year
    }
}
