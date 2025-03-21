using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RestSharpSample.Service;

using Sample_Identity.Service;
using System.IdentityModel.Tokens.Jwt;
using SampleDomain.Dtos;
using SampleMVC_APP.Models;

namespace Sample_Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private JwtSecurityTokenHandler _tokenHandler;

        private readonly ILogger<HomeController> _logger;
        private readonly TokenManager _tokenManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, TokenManager tokenManager, TokenService tokenService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _tokenManager = tokenManager;
            _tokenService = tokenService;
            this._tokenHandler = new JwtSecurityTokenHandler();
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult GetCategory()
        {
            RestApiService restApiService = new RestApiService();
            restApiService.baseUrl = _configuration["BaseUrl"];
            restApiService.AccessToken = _tokenManager.GetTokens().AccessToken;
            var res = restApiService.GetCategories();

            return Content(res);
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> CreateUser(RegisterDto model)
        {
            var token = await _tokenService.SignUpAsync(model);

            _tokenManager.SaveTokens(token.AccessToken, token.RefreshToken);
            return RedirectToAction("index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            
            await _tokenService.LogoutAsync();
            return RedirectToAction("LoginUser");
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var token = await _tokenService.LoginAsync(model.Email, model.Password);

            _tokenManager.SaveTokens(token.AccessToken, token.RefreshToken);

            return RedirectToAction("index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
