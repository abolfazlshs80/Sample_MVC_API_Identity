using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SampleDomain.Database.Models;


public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }



    // ایجاد Refresh Token
    public string GenerateRefreshToken()
    {

        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);

        // return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }


    public string GenerateAccessToken(ApplicationUser user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("UserId", user.Id.ToString()) ,new Claim("Name",user.FullName)}),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:AccessTokenExpirationMinutes"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool IsAccessTokenValid(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
        var issuer = jwtSettings["Issuer"]; // اختیاری
        var audience = jwtSettings["Audience"]; // اختیاری

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // اعتبارسنجی توکن
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // بررسی نوع توکن
            if (!(validatedToken is JwtSecurityToken jwtSecurityToken))
                return false;

            // بررسی الگوریتم امضای توکن
            if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true; // توکن معتبر است
        }
        catch
        {
            // اگر هر خطایی رخ دهد، توکن نامعتبر است
            return false;
        }
    }
    public void RemoveExpiredTokens()
    {
        //using (var _dbContext = new AppDbContext())
        //{
        //    var expiredTokens = _dbContext.RefreshTokens
        //        .Where(r => r.ExpiryDate < DateTime.UtcNow).ToList();
        //    _dbContext.RefreshTokens.RemoveRange(expiredTokens);
        //    await _dbContext.SaveChangesAsync();
        //}
        Console.WriteLine($"Log: {DateTime.Now} - Job executed");
    }

}