using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BruteForceLab.Web.Data;

// EF Core-kontext för Identity. Lagrar användare, roller och,
// när du aktiverat kontolåsning, räknaren över misslyckade försök.
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
}
