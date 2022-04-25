//register and login users 
using API.data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context,ITokenService tokenService )
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        //sendin thigs from the webapp body need to be send as a body.
        public async Task<ActionResult<UserDTO>> Register(RegisterDto registerDto)                          //our controller doesnt know where the data is comming from, this is handeled by [ApiContoller] /api/contolerrs/BaseApiController.cs
        {
            if(await UserExists(registerDto.Username)) 
            {
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();//this class is disposed after it's used - idisposed 

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);//This is saying to Entity, that we want to add this to our users collection/table. - This is only tracking not adding 
            await _context.SaveChangesAsync();//Here we actually call the database and we save our user to our users database table

            return new UserDTO{
                Username = user.UserName,
                Token =_tokenService.CreateToken(user)
            };
        
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username); // we get the user from the database

            if (user == null) return Unauthorized("Invalid username");
            
            using var hmac = new HMACSHA512(user.PasswordSalt); //we give the PasswordSalt( hmac.key, so it can compare the passowrd in the lines below.)

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

         return new UserDTO{
                Username = user.UserName,
                Token =_tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExists(string Username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == Username.ToLower());            
        }

    }
}