// Controllers/AuthController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAPI_APP.Database.Context;
using SampleDomain.Database.Models;
using SampleDomain.Dtos;

namespace SampleAPI_APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
             RoleManager<ApplicationRole> roleManager,
            JwtService jwtService,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponseDto { Success = false, Message = "Invalid input data" });

            // Create a new role using the custom type

            try
            {
                //var adminRole = new IdentityRole
                //{
                //    Name = "Admin"
                //};

            //      await _roleManager.CreateAsync(new ApplicationRole { Name="Admin",NormalizedName="ADMIN"});

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    var AccessToken = _jwtService.GenerateAccessToken(user);
                    var RefleshToken = _jwtService.GenerateRefreshToken();
                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "Registration successful",
                        RefleshToken = RefleshToken,
                        AccessToken = AccessToken,
                        User = new UserDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            FullName = user.FullName,
                            PhoneNumber = user.PhoneNumber
                        }
                    });
                }

                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new AuthResponseDto { Success = false, Message = "Invalid email or password" });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                var AccessToken = _jwtService.GenerateAccessToken(user);
                var RefleshToken = _jwtService.GenerateRefreshToken();
                var newRefreshToken = new RefreshToken
                {
                    Token = RefleshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:RefreshTokenExpiryDays"])),
                    CreatedDate = DateTime.UtcNow,
                    UserId = user.Id
                };
                using (var _dbContext = new AppDbContext())
                {
                    _dbContext.RefreshTokens.Add(newRefreshToken);
                    await _dbContext.SaveChangesAsync();
                }


                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    RefleshToken = RefleshToken,
                    AccessToken = AccessToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber
                    }
                });
            }

            return Unauthorized(new AuthResponseDto { Success = false, Message = "Invalid email or password" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword(ChangePasswordDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new AuthResponseDto { Success = false, Message = "User not found" });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto { Success = true, Message = "Password changed successfully" });
            }

            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new AuthResponseDto { Success = false, Message = "User not found" });

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<AuthResponseDto>> UpdateProfile(UserDto model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new AuthResponseDto { Success = false, Message = "User not found" });

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber
                    }
                });
            }

            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            using (var _dbContext = new AppDbContext())
            {
                var refreshToken = await _dbContext.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

                if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow || refreshToken.Revoked)
                    return Unauthorized("Invalid refresh token.");

                var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                refreshToken.Revoked = true;
                refreshToken.ReplacedByToken = newRefreshToken;

                var newTokenEntry = new RefreshToken
                {
                    Token = newRefreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:RefreshTokenExpiryDays"])),
                    CreatedDate = DateTime.UtcNow,
                    UserId = refreshToken.UserId
                };

                _dbContext.RefreshTokens.Add(newTokenEntry);
                await _dbContext.SaveChangesAsync();

                return Ok(new { AccessToken = newAccessToken,Success=newAccessToken!=null, RefleshToken = newRefreshToken });
            }

        }
        [HttpPost("valid-AccessToken")]
        public async Task<IActionResult> ValidationAccessToken([FromBody] AccessRequest model)
        {

            var res = _jwtService.IsAccessTokenValid(model.AccessToken);
            using (var _dbContext = new AppDbContext())
            {
                var isBlacklisted = _dbContext.TokenBlacklist
                    .Any(t => t.Token == model.AccessToken);
                if (isBlacklisted)
                {
                    return Ok(false);
                }
            }

            return Ok(res);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestForTokenBlacklist request)
        {
            var tokenEntry = new TokenBlacklist
            {
                Token = request.AccessToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(15) // زمان انقضای توکن
            };


            using (var _dbContext = new AppDbContext())
            {
                _dbContext.TokenBlacklist.Add(tokenEntry);
                await _dbContext.SaveChangesAsync();
            }


            return Ok(true);
        }

    }
}
