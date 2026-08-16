using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Verd.Api.Data;

namespace Verd.Api.Services;

/// <summary>
/// Blocks a core feature until the account's email address has been verified.
///
/// The check reads the database rather than a JWT claim on purpose: a token
/// minted before verification would otherwise keep reporting "unverified" for its
/// full seven-day life, forcing the user to log out and back in after clicking
/// the link. Pair with <c>[Authorize]</c> — this filter only handles verification.
/// </summary>
public class RequireVerifiedEmailAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        // Unauthenticated requests are [Authorize]'s problem, not ours.
        if (user.Identity?.IsAuthenticated != true)
        {
            await next();
            return;
        }

        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        if (!int.TryParse(raw, out var userId))
        {
            await next();
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var verified = await db.Users
            .Where(u => u.Id == userId)
            .Select(u => (bool?)u.IsEmailVerified)
            .FirstOrDefaultAsync();

        // A deleted account is handled by the endpoint itself; only an explicit
        // "false" means the user still has to verify.
        if (verified == false)
        {
            context.Result = new ObjectResult(new
            {
                message = "Verifikasi alamat email Anda untuk menggunakan fitur ini.",
                code = "email_not_verified",
            })
            { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }

        await next();
    }
}
