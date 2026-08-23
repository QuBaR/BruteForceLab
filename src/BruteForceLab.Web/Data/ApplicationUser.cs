using Microsoft.AspNetCore.Identity;

namespace BruteForceLab.Web.Data;

// Egen användarklass ovanpå ASP.NET Core Identity. Här kan du lägga till
// egna fält senare om du vill, men för övningen räcker basen som den är.
public class ApplicationUser : IdentityUser
{
}
