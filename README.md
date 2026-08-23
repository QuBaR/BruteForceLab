# BruteForceLab

Startrepo för **övning 5.2: Brute force i C#** i kursen IT-säkerhet för utvecklare.

Här finns en färdig inloggning och ett attackverktyg. Din uppgift är att köra attacken, mäta, och sedan bygga försvaret och mäta skillnaden. Hela uppgiftstexten med stegen ligger i Learnpoint.

## Vad som finns i repot

- **src/BruteForceLab.Web** är en ASP.NET Core-app på .NET 10 med ASP.NET Core Identity och en SQLite-databas. Den har en inloggningsendpoint på `POST /login` och sår en testanvändare med ett medvetet svagt lösenord vid uppstart.
- **src/BruteForceLab.Attacker** är ett konsolprogram som anropar `/login`. Attackloopen är förberedd med ett skelett och ett TODO, den skriver du själv i steg 2.
- **wordlist.txt** är en kort ordlista att prova mot. Testanvändarens lösenord finns med i listan.

Testanvändaren är `offer` och startläget saknar med flit både rate limiting och kontolåsning. Det är just de skydden du lägger på under övningen.

## Så kör du

Öppna två terminaler.

**Terminal 1, starta webben:**

```powershell
dotnet run --project src/BruteForceLab.Web
```

När webben är igång skriver den ut `Now listening on: http://localhost:5080`. Låt den ligga igång.

**Terminal 2, kör attacken:**

```powershell
dotnet run --project src/BruteForceLab.Attacker
```

> **Får du `address already in use` på port 5080?** Då kör webben redan i en annan terminal. Stäng den gamla körningen med Ctrl+C först, du behöver bara en instans igång.

## Var du gör vad

| Steg i övningen | Var i koden |
|-----------------|-------------|
| Steg 2, bygg attackloopen | `src/BruteForceLab.Attacker/Program.cs`, se TODO steg 2 |
| Steg 4, rate limiting | `src/BruteForceLab.Web/Program.cs`, se TODO steg 4 (tre ställen) |
| Steg 6, kontolåsning | `src/BruteForceLab.Web/Program.cs`, se TODO steg 6, och byt `lockoutOnFailure` till `true` |

Attackverktyget känner redan igen svaren 429 (rate limiting slog till) och 423 (kontot låst) och skriver ut det, så att du ser exakt vid vilket försök ett skydd griper in.

## Databasen

SQLite-filen `bruteforcelab.db` skapas automatiskt vid första körningen. Vill du börja om från en ren databas, till exempel för att låsa upp ett låst konto, stäng webben och radera `.db`-filen.
