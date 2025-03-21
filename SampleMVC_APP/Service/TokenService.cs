using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RestSharpSample.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using SampleDomain.Dtos;

namespace Sample_Identity.Service
{
    public class TokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private JwtSecurityTokenHandler _tokenHandler;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly TokenManager _tokenManager;

        public TokenService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, TokenManager tokenManager)
        {
            _tokenManager = tokenManager;
            _httpClient = httpClient;
            _configuration = configuration;
            this._tokenHandler = new JwtSecurityTokenHandler();
            _httpContextAccessor = httpContextAccessor;
        }


        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            //  claims.Add(new Claim(ClaimTypes.Name, tokenContent?.Subject));
            return claims;
        }

        public async Task<(string AccessToken, string RefreshToken)> SignUpAsync(RegisterDto model)
        {
            RestApiService restApiService = new RestApiService();
            restApiService.baseUrl = _configuration["BaseUrl"];
            var res = restApiService.PostRegister(model);


            if (!res.Success)
                throw new Exception("Login failed.");

            var tokenContent = _tokenHandler.ReadJwtToken(res.AccessToken);
            var claims = ParseClaims(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            return (res.AccessToken, res.RefleshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password)
        {
            RestApiService restApiService = new RestApiService();
            restApiService.baseUrl = _configuration["BaseUrl"];
            var res = restApiService.PostLogin(new LoginDto { Password=password,Email=email});
     
     
            if (!res.Success)
                throw new Exception("Login failed.");

            var tokenContent = _tokenHandler.ReadJwtToken(res.AccessToken);
            var claims = ParseClaims(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);

            return (res.AccessToken, res.RefleshToken);
        }
        public async Task LogoutAsync()
        {
            var (accessToken, refreshToken) = _tokenManager.GetTokens();
            RestApiService restApiService = new RestApiService();
            restApiService.baseUrl = _configuration["BaseUrl"];
            var res = restApiService.PostLogout(accessToken);

            if (res)
            {
         //       _tokenManager.ClearTokens();
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _tokenManager.ClearTokens();
            }


            
        }
        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            RestApiService restApiService = new RestApiService();
            restApiService.baseUrl = _configuration["BaseUrl"];
            var res = restApiService.PostRefleshToken(refreshToken);
            if (!res.Success)
                throw new Exception("Token refresh failed.");

        
            return (res.AccessToken, res.RefleshToken);
        }
        public async Task<bool> ValidAccessTokenAsync(string AccessToken)
        {
            RestApiService restApiService = new RestApiService();
            restApiService.baseUrl = _configuration["BaseUrl"];
            var res = restApiService.PostValidAccessToken(AccessToken);
            return res;
        }
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
