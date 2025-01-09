using BookStore.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja LiteDBContext
builder.Services.AddSingleton<LiteDBContext>(); // Dodajemy usługę LiteDBContext

// Dodanie autoryzacji na podstawie ciasteczek
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Określamy ścieżkę do strony logowania
        options.AccessDeniedPath = "/Account/AccessDenied"; // Określamy, gdzie użytkownik trafi, gdy nie będzie miał uprawnień
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ustawienia środowiska i konfiguracja pipeline'u HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Używanie autoryzacji w aplikacji
app.UseAuthentication(); // Dodajemy tę linię, aby aplikacja obsługiwała autoryzację
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}/{id?}"); // Upewnij się, że wskazujesz na kontroler Book

app.Run();
