using LiteDB;

namespace BookStore.Data
{
    public class LiteDBContext
    {
        private readonly string _databasePath = "BookStore.db";
        private LiteDatabase _database;

        public LiteDatabase Database => _database ??= new LiteDatabase(_databasePath);

        
        public ILiteCollection<Book> Books => Database.GetCollection<Book>("Books");
        public ILiteCollection<User> Users => Database.GetCollection<User>("Users");
        public ILiteCollection<CartItem> CartItems => Database.GetCollection<CartItem>("CartItems");

        
        public LiteDBContext()
        {
            
            var adminExists = Users.Exists(u => u.Username == "admin1");

            
            if (!adminExists)
            {
                var admin = new User
                {
                    Username = "admin1",
                    Password = "admin1", 
                    IsAdmin = true
                };

                
                Users.Insert(admin);
            }

            
            CartItems.EnsureIndex(x => x.UserId);
            CartItems.EnsureIndex(x => x.BookId);
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
        public string Password { get; set; } /
        public bool IsAdmin { get; set; }
    }

    // Model pozycji w koszyku
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }   
        public int BookId { get; set; }   
    }
}
