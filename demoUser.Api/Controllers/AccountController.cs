using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using demoUser.Infrastructure.DTO;
using demoUser.Core.Entities;
using demoUser.Infrastructure.Repository;
using demoUser.Services.Interfaces;
using demoUser.Services.Services;
using TMS.API.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace demoUser.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IUserOtpService _userOtpService;

        public AccountController(IUserService userService, IConfiguration config, IUserOtpService userOtpService)
        {
            _userService = userService;
            _config = config;
            _userOtpService = userOtpService;
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<LoginResponseDTO> SignIn(LoginRequestDTO loginRequest)
        {
            LoginResponseDTO response = new LoginResponseDTO();

            var user = await _userService.GetByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                response.Message = "user not Found";
                return response;
            }
            else if (user.PasswordHash != UtilityHelper.PasswordHashMaker(loginRequest.Password))
            {
                response.Message = "Enter Valid Password";
                return response;
            }
            else
            {
                var token = GenerateJSONWebToken(user);

                response.FullName = user.FullName;
                response.UserId = user.Id;
                response.Email = user.Email;
                response.Token = token;
                return response;
            }
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_config["Jwt:SessionTimeout"])),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("Registration")]
        public async Task<User> Registration(RegistrationRequestDTO user)
        {

            User userToBeAdd = new User()
            {
                FullName = user.FirstName + " " + user.LastName,
                Address1 = user.Address1,
                City = user.City,
                Country = user.Country,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Gender = user.Gender,
                IsActive = true,
                MobileNo = user.MobileNo,
                PasswordHash = UtilityHelper.PasswordHashMaker(user.PasswordHash),
                PostalCode = user.PostalCode,
                State = user.State
            };

            await _userService.AddAsync(userToBeAdd);

            return userToBeAdd;
        }

        [HttpPost("ChangePassword")]
        public async Task<string> ChangePassword(ChangePasswordDTO ChangePassword)
        {
            var user = await _userService.GetByEmailAsync(ChangePassword.Email);
            if (user == null)
            {
                string Message = "user not Found";
                return Message;
            }
            else if (user.PasswordHash != UtilityHelper.PasswordHashMaker(ChangePassword.CurrentPassword))
            {
                string Message = "Enter Valid Password";
                return Message;
            }
            else
            {
                await _userService.ChangePassword(ChangePassword.Email, ChangePassword.NewPassword);
                string Message = "Password changed successfully";
                return Message;
            }
        }

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<string> ForgetPassword(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                string message = "user not Found";
                return message;
            }
            else
            {
                userOTP oTP = new userOTP()
                {
                    CreationTime = DateTime.Now.ToString(),
                    ExpirationTime = DateTime.Now.AddHours(1).ToString(),
                    IsUsed = false,
                    OTP = UtilityHelper.GenerateOTP().ToString(),
                    UserId = user.Id
                };
                _userOtpService.AddAsync(oTP);

                string message = oTP.OTP;
                return message;
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<string> ResetPassword(ResetPasswordDTO resetPass)
        {
            var user = await _userService.GetByEmailAsync(resetPass.Email);
            
            if (user == null)
            {
                string message = "user not Found";
                return message;
            }

            userOTP otpRow = await _userOtpService.GetByUserId(user.Id);
            var temp = DateTime.Now.ToString();
            if (otpRow == null)
            {
                string message = "request for OTP... ?";
                return message;
            }
            var otpCheck = await _userOtpService.IsValidOtp(user);

            if (otpCheck == false)
            {
                string message = "Otp has been expired or Used once.";
                return message;
            }
            else
            {
                string passwordHash = UtilityHelper.PasswordHashMaker(resetPass.NewPassword);
                await _userService.ResetPassword(resetPass.Email, passwordHash);

                string message = "password changed Successfully";
                otpRow.IsUsed = true;
                await _userOtpService.UpdateAsync(otpRow);
                return message;
            }
        }
    }
}
