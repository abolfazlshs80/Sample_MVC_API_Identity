using Sample_Identity.Service;

namespace Sample_Identity
{
    public class TokenValidationMiddleware
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RequestDelegate _next;
       

        public TokenValidationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
      
        }

        public async Task InvokeAsync(HttpContext context)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var _tokenManager = scope.ServiceProvider.GetRequiredService<TokenManager>();
                var _tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

                var (accessToken, refreshToken) = _tokenManager.GetTokens();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // اعتبارسنجی AccessToken
                    if (!await IsAccessTokenValid(accessToken))
                    {
                        if (!string.IsNullOrEmpty(refreshToken))
                        {
                            try
                            {
                                var (newAccessToken, newRefreshToken) = await _tokenService.RefreshTokenAsync(refreshToken);
                                _tokenManager.SaveTokens(newAccessToken, newRefreshToken);
                                context.Request.Headers["Authorization"] = $"Bearer {newAccessToken}";
                            }
                            catch
                            {
                              await  _tokenService.LogoutAsync();
                               // _tokenManager.ClearTokens();
                                context.Response.Redirect("/Home/LoginUser");
                                return;
                            }
                        }
                        else
                        {
                            await _tokenService.LogoutAsync();
                            context.Response.Redirect("/Home/LoginUser");
                        }
                    }
                    else
                    {
                        context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
                    }
                }
            
                // اعتبارسنجی توکن‌ها
            }
        

            await _next(context);
        }

        private async Task<bool> IsAccessTokenValid(string token)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _tokenManager = scope.ServiceProvider.GetRequiredService<TokenManager>();
                var _tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();

              return await  _tokenService.ValidAccessTokenAsync(token);
            }
        }
    }
}
