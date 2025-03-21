namespace SampleDomain.Database.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Revoked { get; set; }
        public string? ReplacedByToken { get; set; }

        // ارتباط با کاربر
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
