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

    public class Storage
    {
        const string CommandAddBook = "1";
        const string CommandRemoveBook = "2";
        const string CommandShowAllBooks = "3";
        const string CommandShowFilteredBooks = "4";

        private List<Book> _books;
        private bool _isActive;

        public Storage()
        {
            _books = new();
        }

        public void OpenMenu()
        {
            _isActive = true;

            while (_isActive)
            {
                Console.Clear();
                Console.WriteLine($"{CommandAddBook}) Добавить книгу\n" +
                                  $"{CommandRemoveBook}) Удалить книгу\n" +
                                  $"{CommandShowAllBooks}) Показать все книги\n" +
                                  $"{CommandShowFilteredBooks}) Показать книги по параметру");
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
                }
            }
        }

        private void AddBook()
        {
            if (TryRequsetAllParametrs(out string name, out string autor, out int year))
            {
                Book addedBook = new(name, autor, year);

                if (BookIsExist(addedBook.Name, addedBook.Autor, addedBook.Year))
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

            if (TryRequsetAllParametrs(out string name, out string autor, out int year))
            {
                if (TryFindBook(_books, name, autor, year, out Book book))
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
            Console.WriteLine($"название: {book.Name} | автор: {book.Autor} | год выпуска: {book.Year}");
        }

        private bool TryFindBook(List<Book> books, string name, string autor, int year, out Book book)
        {
            List<Book> filteredBooks = FilterBooks(_books, BookParametr.Name, name);
            filteredBooks = FilterBooks(filteredBooks, BookParametr.Autor, autor);
            filteredBooks = FilterBooks(filteredBooks, BookParametr.Year, year.ToString());

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

        private List<Book> FilterBooks(List<Book> books, BookParametr bookParametr, string parametrValue)
        {
            List<Book> filteredBooks = new();
            bool isCorrectParametr = false;

            foreach (var book in books)
            {
                switch (bookParametr)
                {
                    case BookParametr.Name:
                        isCorrectParametr = book.Name == parametrValue;
                        break;

                    case BookParametr.Autor:
                        isCorrectParametr = book.Autor == parametrValue;
                        break;

                    case BookParametr.Year:
                        if (int.TryParse(parametrValue, out int year))
                        {
                            isCorrectParametr = book.Year == year;
                        }
                        else
                        {
                            isCorrectParametr = false;
                        }
                        break;
                }

                if (isCorrectParametr)
                {
                    filteredBooks.Add(book);
                }
            }

            return filteredBooks;
        }

        private bool TryRequestParametr(BookParametr bookParametr, out string parametrValue)
        {
            Console.Write("Введите ");

            switch (bookParametr)
            {
                case BookParametr.Name:
                    Console.Write("название ");
                    break;

                case BookParametr.Autor:
                    Console.Write("автора ");
                    break;

                case BookParametr.Year:
                    Console.Write("год выпуска ");
                    break;
            }

            Console.Write("книги: ");
            parametrValue = Console.ReadLine();

            if (parametrValue != string.Empty)
            {
                if (bookParametr == BookParametr.Year)
                {
                    if (int.TryParse(parametrValue, out int year))
                    {
                        if (year < 0)
                        {
                            Console.WriteLine("Год не может быть отрицательным.");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Год должен быть числом.");
                        return false;
                    }
                }

                return true;
            }
            else
            {
                Console.WriteLine("Вы ничего не ввели.");
                return false;
            }
        }

        private bool TryRequsetAllParametrs(out string name, out string autor, out int year)
        {
            if (TryRequestParametr(BookParametr.Name, out string returnedName))
            {
                if (TryRequestParametr(BookParametr.Autor, out string returnedAutor))
                {
                    if (TryRequestParametr(BookParametr.Year, out string returnedYear))
                    {
                        name = returnedName;
                        autor = returnedAutor;
                        year = int.Parse(returnedYear);
                        return true;
                    }
                }
            }

            name = string.Empty;
            autor = string.Empty;
            year = -1;
            return false;
        }

        private void ShowBooksByParametr(BookParametr bookParametr)
        {
            if (TryRequestParametr(bookParametr, out string returnedParametr))
            {
                List<Book> filteredBooks = FilterBooks(_books, bookParametr, returnedParametr);

                if (filteredBooks.Count > 0)
                {
                    ShowBooks(filteredBooks);
                }
                else
                {
                    Console.WriteLine("Книги не найдены");
                }
            }
        }

        private BookParametr SelectParametr()
        {
            const string NameParametr = "1";
            const string AutorParametr = "2";
            const string YearParametr = "3";

            bool isSelecting = true;
            BookParametr selectedParametr = default;

            while (isSelecting)
            {
                Console.WriteLine("Выберите параметр книги");
                Console.WriteLine($"{NameParametr}) Название");
                Console.WriteLine($"{AutorParametr}) Автор");
                Console.WriteLine($"{YearParametr}) Год выпуска");

                switch (Console.ReadLine())
                {
                    case NameParametr:
                        selectedParametr = BookParametr.Name;
                        isSelecting = false;
                        break;

                    case AutorParametr:
                        selectedParametr = BookParametr.Autor;
                        isSelecting = false;
                        break;

                    case YearParametr:
                        selectedParametr = BookParametr.Year;
                        isSelecting = false;
                        break;
                }
            }

            return selectedParametr;
        }

        private bool BookIsExist(string name, string autor, int year)
        {
            return TryFindBook(_books, name, autor, year, out Book _);
        }
    }

    public class Book
    {
        public Book(string name, string autor, int year)
        {
            Name = name;
            Autor = autor;
            Year = year;
        }

        public string Name { get; }

        public string Autor { get; }

        public int Year { get; }
    }

    public enum BookParametr
    {
        Name,
        Autor,
        Year
    }
}
