using BruteForceLab.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// --- Databas: SQLite via EF Core -------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=bruteforcelab.db"));

// --- ASP.NET Core Identity --------------------------------------------------
// Startläget medvetet svagt: lösenordskraven är avstängda så att testanvändaren
// kan ha ett riktigt uselt lösenord, och kontolåsningen är INTE konfigurerad.
// Det är det du lägger på i steg 6.
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;

        // TODO steg 6: aktivera kontolåsning här.
        // options.Lockout.MaxFailedAccessAttempts = 5;
        // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        // options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// TODO steg 4: aktivera rate limiting här (AddRateLimiter med en fixed window).
// builder.Services.AddRateLimiter(options => { ... });
// Usings för rate limiting ligger redan högst upp i filen.

var app = builder.Build();

// Skapa databasen och så en svag testanvändare vid uppstart.
await SeedAsync(app);

// TODO steg 4: koppla på rate limiting-middleware.
// app.UseRateLimiter();

// Inloggningsendpointen som din attack ska köra mot.
// Tar emot { "anvandarnamn": "...", "losenord": "..." } och svarar
// 200 OK vid rätt lösenord, annars 401 Unauthorized.
app.MapPost("/login", async (
    LoginRequest req,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.FindByNameAsync(req.Anvandarnamn);
    if (user is null)
        return Results.Unauthorized();

    // lockoutOnFailure styr om ett misslyckat försök räknas mot kontolåsningen.
    // TODO steg 6: ändra false till true när du aktiverat kontolåsning.
    var result = await signInManager.CheckPasswordSignInAsync(
        user, req.Losenord, lockoutOnFailure: false);

    if (result.Succeeded)
        return Results.Ok(new { status = "inloggad" });

    if (result.IsLockedOut)
        return Results.StatusCode(StatusCodes.Status423Locked);

    return Results.Unauthorized();
});
// TODO steg 4: skydda endpointen med .RequireRateLimiting("login");

app.MapGet("/", () => "BruteForceLab kör. POST till /login med { anvandarnamn, losenord }.");

app.Run();

// Sår en testanvändare med ett medvetet svagt lösenord som finns i wordlist.txt.
static async Task SeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string anvandarnamn = "offer";
    const string svagtLosenord = "sommar24";

    if (await userManager.FindByNameAsync(anvandarnamn) is null)
    {
        var user = new ApplicationUser { UserName = anvandarnamn, Email = "offer@minapp.se" };
        await userManager.CreateAsync(user, svagtLosenord);
    }
}

// Formen på JSON-kroppen till /login.
public record LoginRequest(string Anvandarnamn, string Losenord);
