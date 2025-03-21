using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SampleAPI_APP.Database.Context;

// مسیر DbContext خود را جایگزین کنید

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        using (var dbContext = new AppDbContext())
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring(7); // حذف "Bearer " از ابتدای توکن

                var isBlacklisted = await dbContext.TokenBlacklist.AnyAsync(t => t.Token == token);
                if (isBlacklisted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token is blacklisted.");
                    return;
                }
            }

            await _next(context);
        }
         
    }
}