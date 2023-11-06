using AutoMapper;
using FinalProject.DTO;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProject.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccountController : Controller
    {
        public PRN231DBContext _context;
        public IConfiguration _configuration;
/*        private readonly IEmailService _emailService;*/
        public AccountController(PRN231DBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
/*            _emailService = emailService;*/
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post(Account account)
        {
            if (account != null && account.Email != null && account.Password != null)
            {
                var acc = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email && x.Password == account.Password);
                if (acc != null)
                {
                    // Create Claim details
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("AccountId", acc.AccountId.ToString()),
                        new Claim("Password", acc.Password),
                        new Claim("Email", acc.Email),
                        new Claim("Password", acc.Password),
                        new Claim("CustomerId", acc.CustomerId == null ? "": acc.CustomerId),
                        new Claim("EmployeeId", acc.EmployeeId == null ? "" : acc.EmployeeId + ""),
                        new Claim("Role", acc.Role.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(10),
                            signingCredentials: signIn
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("[action]/{id}")]
        public IActionResult GetById(int id)
        {
            var account = _context.Accounts.Where(e => e.AccountId == id).FirstOrDefault();
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        private async Task<Account> GetAccount(string email, string password)
        {
            var result = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            return result;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            Account account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Email.Equals(registerDTO.Email));

            if (account != null)
            {
                return BadRequest("Email existed!");
            }
            else
            {
                if (registerDTO.Password.Equals(registerDTO.RePassword))
                {
                    string cusId = await CreateCustomerId();
                    Customer customer = new Customer()
                    {
                        CustomerId = cusId,
                        ContactName = registerDTO.ContactName,
                        CompanyName = registerDTO.CompanyName,
                        Address = registerDTO.Address,
                        ContactTitle = registerDTO.ContactTitle
                    };
                    await _context.AddAsync<Customer>(customer);
                    _context.SaveChanges();
                    Account acc = new Account()
                    {
                        CustomerId = cusId,
                        Email = registerDTO.Email,
                        Password = registerDTO.Password,
                        Role = 2
                    };
                    await _context.AddAsync<Account>(acc);
                    _context.SaveChanges();
                    return Ok("Regist Success!");
                }
                else
                {
                    return BadRequest("Password doesn't match");
                }
            }
        }

        private async Task<string> CreateCustomerId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            bool existed = true;
            string cusId = "";
            while (existed)
            {

                cusId = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId.Equals(cusId));
                if (customer == null)
                {
                    existed = false;
                }
            }
            return cusId;

        }
        [HttpGet("[action]")]
        public IActionResult getClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var CustomerId = jwt.Claims.First(claim => claim.Type == "CustomerId").Value;
            var EmployeeId = jwt.Claims.First(claim => claim.Type == "EmployeeId").Value;
            var AccountId = jwt.Claims.First(claim => claim.Type == "AccountId").Value;
            var Role = jwt.Claims.First(claim => claim.Type == "Role").Value;
            var Email = jwt.Claims.First(claim => claim.Type == "Email").Value;
            return Ok(new { CustomerId, EmployeeId, AccountId, Role, Email });
        }

/*        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            string newPass = "abcde@123";
            Account account = await _context.Accounts.FirstOrDefaultAsync(acc => acc.Email.Equals(email));

            if (account == null)
            {
                return BadRequest("Email doesn't existed!");
            }
            account.Password = newPass;
            _context.Accounts.Update(account);
            _context.SaveChanges();
            var emailDTO = new EmailDTO
            {
                To = email,
                Body = "<h1>This is your new password</h1> <p>" + newPass + "</p>",
                Subject = "Reset Password"
            };
            _emailService.SendEmail(emailDTO);
            return Ok();
        }

        [HttpPost]
        public IActionResult SendMail(EmailDTO emailDTO)
        {
            _emailService.SendEmail(emailDTO);
            return Ok();
        }*/

    }
}
