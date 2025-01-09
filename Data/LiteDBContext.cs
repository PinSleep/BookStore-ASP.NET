using LiteDB;

namespace BookStore.Data
{
    public class LiteDBContext
    {
        private readonly string _databasePath = "BookStore.db";
        private LiteDatabase _database;

        public LiteDatabase Database => _database ??= new LiteDatabase(_databasePath);

        // Kolekcje
        public ILiteCollection<Book> Books => Database.GetCollection<Book>("Books");
        public ILiteCollection<User> Users => Database.GetCollection<User>("Users");

        // Dodanie domyślnego administratora, jeśli baza jest pusta
        public LiteDBContext()
        {
            // Sprawdzamy, czy użytkownik admin1 już istnieje w bazie
            var adminExists = Users.Exists(u => u.Username == "admin1");

            // Jeśli użytkownik nie istnieje, dodajemy go
            if (!adminExists)
            {
                var admin = new User
                {
                    Username = "admin1",
                    Password = "admin1", // W przyszłości zaszyfrujemy hasło
                    IsAdmin = true
                };

                // Dodanie użytkownika do kolekcji "Users"
                Users.Insert(admin);
            }
        }
    }

    // Model Książki
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }

    // Model Użytkownika
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Zaszyfrujemy później
        public bool IsAdmin { get; set; }
    }
}
