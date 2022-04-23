
using API.data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        //sendin thigs from the webapp body need to be send as a body.
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)                          //our controller doesnt know where the data is comming from, this is handeled by [ApiContoller] /api/contolerrs/BaseApiController.cs
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

            return user;
        
        }

        private async Task<bool> UserExists(string Username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == Username.ToLower());
        }

    }
}