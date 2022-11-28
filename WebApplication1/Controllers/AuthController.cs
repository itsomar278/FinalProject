using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Repositories.UsersRepository;
using WebApplication1.Models.UnitOfWork;

namespace WebApplication1.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
	    public AuthController(IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;

        }
		[HttpPost("register")]
		public async Task<ActionResult<Users>> Rigester(UserRegisterRequest request)
		{
			var usedUserName = _unitOfWork.Users.Find(u => u.UserName == request.UserName).ToList();
			var UsedEmail = _unitOfWork.Users.Find(u => u.UserEmail == request.UserEmail).ToList();

            if (usedUserName.Count!=0|| UsedEmail.Count!=0)
			{ 
                return BadRequest("Email or User Name has been used before");
			}

			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
			var user = new Users
			{
				UserEmail= request.UserEmail,
				UserName = request.UserName,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt
			};
			_unitOfWork.Users.Add(user);
            _unitOfWork.complete();
            return Ok(user);
		}
		[HttpPost("login")]
        public async Task<ActionResult<Users>> Login(UserLoginRequest request)
		{
			var result = _unitOfWork.Users.FindByEmail(request.UserEmail);
		
			
		    if (!string.IsNullOrEmpty(result.UserEmail) || !VerifyPasswordHash(request.Password, result.PasswordHash, result.PasswordSalt))
			{
			
					string token = CreateToken(result);
					return Ok(token);
			}
		
			else
			{
                return BadRequest("either email is wrong or password");

            }
        }
		private string CreateToken(Users user)
		{
			List<Claim> claims = new List<Claim>
			{ 
				new Claim(ClaimTypes.Name , user.UserName)
			};
			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("myfinalprojectforts123"));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
			var token = new JwtSecurityToken
				(
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds
				);
			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return jwt; 
		}
		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}
		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
            using (var hmac = new HMACSHA512())
            {
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash == passwordHash;
            }
        }
	}
}