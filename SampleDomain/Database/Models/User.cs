using Microsoft.AspNetCore.Identity;

namespace SampleDomain.Database.Models
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // ذخیره پسورد هش‌شده
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }


    public class ApplicationUser : IdentityUser
    {
        // می‌توانید فیلدهای اضافی برای کاربر اضافه کنید
        public string FullName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    public class ApplicationRole : IdentityRole
    {
        // می‌توانید فیلدهای اضافی برای نقش اضافه کنید
    }
}
