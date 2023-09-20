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
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMailService _mailService;

        public AccountController(IUserService userService, IConfiguration config, IUserOtpService userOtpService, IWebHostEnvironment webHostEnvironment, IMailService mailService)
        {
            _userService = userService;
            _config = config;
            _userOtpService = userOtpService;
            _webHostEnvironment = webHostEnvironment;
            _mailService = mailService;
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ResponseDTO<LoginResponseDTO>> SignIn(LoginRequestDTO loginRequest)
        {
            ResponseDTO<LoginResponseDTO> response = new ResponseDTO<LoginResponseDTO>();
            int StatusCode = 0;
            bool isSuccess = false;
            LoginResponseDTO? Response = null;
            string Message = string.Empty;
            string ExceptionMessage = string.Empty;
            try
            {
                var user = await _userService.GetByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    isSuccess = false;
                    StatusCode = 404;
                    Message = "Enter Valid Email";
                }
                else if (user.PasswordHash != UtilityHelper.PasswordHashMaker(loginRequest.Password))
                {
                    isSuccess = false;
                    StatusCode = 400;
                    Message = "Enter Valid Password";
                }
                else
                {
                    var token = GenerateJSONWebToken(user);
                    LoginResponseDTO userResponse = new LoginResponseDTO
                    {
                        Token = token,
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email
                    };
                    Response = userResponse;
                    StatusCode = 200;
                    isSuccess = true;
                    Message = "SignIn successful.";
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                StatusCode = 500;
                Message = "Failed while fetching data.";
                ExceptionMessage = ex.Message.ToString();
            }
            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Response = Response;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;
            return response;
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name,user.FullName),
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
        public async Task<ResponseDTO<User>> Registration(RegistrationRequestDTO user)
        {
            ResponseDTO<User> response = new ResponseDTO<User>();
            int StatusCode;
            bool isSuccess;
            User? Response = null;
            string Message;
            string ExceptionMessage = string.Empty;

            try
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
                    PasswordHash = UtilityHelper.PasswordHashMaker(user.Password),
                    PostalCode = user.PostalCode,
                    State = user.State
                };

                await _userService.AddAsync(userToBeAdd);
                StatusCode = 200;
                isSuccess = true;
                Response = userToBeAdd;
                Message = "Account Created.";

            }
            catch (Exception ex)
            {
                isSuccess = false;
                StatusCode = 500;
                Message = "Failed while Creating data.";
                ExceptionMessage = ex.Message.ToString();
            }
            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Message = Message;
            response.Response = Response;
            response.ExceptionMessage = ExceptionMessage;
            return response;
        }

        [HttpPost("ChangePassword")]
        public async Task<ResponseDTO<string>> ChangePassword(ChangePasswordDTO ChangePassword)
        {
            ResponseDTO<string> response = new ResponseDTO<string>();
            int StatusCode = 0;
            bool isSuccess = false;
            string Message = string.Empty;
            string ExceptionMessage = string.Empty;
            try
            {
                var user = await _userService.GetByEmailAsync(ChangePassword.Email);
                if (user == null)
                {
                    isSuccess = false;
                    StatusCode = 400;
                    Message = "Enter Valid Email";
                }
                else if (user.PasswordHash != UtilityHelper.PasswordHashMaker(ChangePassword.CurrentPassword))
                {
                    isSuccess = false;
                    StatusCode = 400;
                    Message = "Enter valid password";
                }
                else
                {
                    await _userService.ChangePassword(ChangePassword.Email, UtilityHelper.PasswordHashMaker(ChangePassword.NewPassword));
                    isSuccess = true;
                    StatusCode = 200;
                    Message = "New Password Created";
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                StatusCode = 500;
                Message = "Unable to create new Password";
                ExceptionMessage = ex.Message.ToString();
            }
            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;
            return response;
        }

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<ResponseDTO<string>> ForgetPassword(string email)
        {
            ResponseDTO<string> response = new ResponseDTO<string>();
            int StatusCode = 0;
            bool isSuccess = false;
            string Message = string.Empty;
            string ExceptionMessage = string.Empty;

            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    isSuccess = false;
                    StatusCode = 400;
                    Message = "User Not found";
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
                    await _userOtpService.AddAsync(oTP);

                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    string templateFilePath = Path.Combine(wwwrootPath, "EmailTemplates/ForgetPassword_Email_template.html");
                    string htmlTemplate = await System.IO.File.ReadAllTextAsync(templateFilePath);
                    htmlTemplate = htmlTemplate.Replace("#FullName#", user.FullName).Replace("#OTP#", oTP.OTP).Replace("#ProductName#", "Demo-User");


                    MailSettingsDTO mailSettings = new MailSettingsDTO()
                    {
                        Host = _config["EmailSender:Host"],
                        Password = _config["EmailSender:Password"],
                        Port = int.Parse(_config["EmailSender:Port"]),
                        SenderEmail = _config["EmailSender:SenderEmail"],
                        UserName = _config["EmailSender:UserName"]
                    };

                    MailData mailData = new MailData()
                    {
                        Body = htmlTemplate,
                        Subject = "Requested for Forget Password OTP",
                        To = email
                    };
                    string message = string.Empty;
                    var mailSend = _mailService.sendMail(mailSettings, mailData);
                    if (mailSend)
                    {
                        isSuccess = true;
                        StatusCode = 200;
                        Message = "OTP send to email";
                    }
                    else
                    {
                        isSuccess = false;
                        StatusCode = 400;
                        Message = "Error while sending email";
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                StatusCode = 500;
                Message = "Unable to Reset Password";
                ExceptionMessage = ex.Message.ToString();
            }
            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;
            return response;
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<ResponseDTO<string>> ResetPassword(ResetPasswordDTO resetPass)
        {
            ResponseDTO<string> response = new ResponseDTO<string>();
            int StatusCode = 0;
            bool isSuccess = false;
            string Message = string.Empty;
            string ExceptionMessage = string.Empty;

            try
            {
                var user = await _userService.GetByEmailAsync(resetPass.Email);
                if (user == null)
                {
                    response.StatusCode = 400;
                    response.IsSuccess = false;
                    response.Message = "User not found";
                    return response;
                }
                userOTP otpRow = await _userOtpService.GetByUserId(user.Id);
                var temp = DateTime.Now.ToString();
                if (otpRow == null)
                {
                    response.StatusCode = 400;
                    response.IsSuccess = false;
                    response.Message = "Request for OTP...";
                    return response;
                }
                var otpCheck = await _userOtpService.IsValidOtp(user);

                if (otpCheck == false)
                {
                    response.StatusCode = 400;
                    response.IsSuccess = false;
                    response.Message = "Otp has been expired or Used once.";
                    return response;
                }
                else
                {
                    string passwordHash = UtilityHelper.PasswordHashMaker(resetPass.NewPassword);
                    await _userService.ResetPassword(resetPass.Email, passwordHash);

                    otpRow.IsUsed = true;
                    await _userOtpService.UpdateAsync(otpRow);

                    isSuccess = true;
                    StatusCode = 200;
                    Message = "Password changed successfully";
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                StatusCode = 500;
                Message = "Unable to Reset Password";
                ExceptionMessage = ex.Message.ToString();
            }
            response.StatusCode = StatusCode;
            response.IsSuccess = isSuccess;
            response.Message = Message;
            response.ExceptionMessage = ExceptionMessage;
            return response;
        }

    }
}