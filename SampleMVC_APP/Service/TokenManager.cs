namespace Sample_Identity.Service
{
    public class TokenManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SaveTokens(string accessToken, string refreshToken)
        {
            var context = _httpContextAccessor.HttpContext;
            context.Response.Cookies.Append("AccessToken", accessToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, });
            context.Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, });
        }

        public (string AccessToken, string RefreshToken) GetTokens()
        {
            var context = _httpContextAccessor.HttpContext;
            var accessToken = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];
            return (accessToken, refreshToken);
        }

        public void ClearTokens()
        {
            var context = _httpContextAccessor.HttpContext;
            context.Response.Cookies.Delete("AccessToken");
            context.Response.Cookies.Delete("RefreshToken");
        }
    }
}
