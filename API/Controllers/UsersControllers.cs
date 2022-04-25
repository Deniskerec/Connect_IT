//getting the users 
using System.Threading.Tasks;
using API.data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet] 
        [AllowAnonymous] //you get the users with postman 
        public async Task<ActionResult<List<AppUser>>> GetUsers(){
            //var users = _context.Users.ToList();
            //return users;
            return await _context.Users.ToListAsync();
        }
        
        [Authorize]  //GetUser is protected 
        [HttpGet("{id}")] 
        public async Task<ActionResult<AppUser>> GetUser(int id){
            //var user = _context.Users.Find(id);
            //return user;
            return await _context.Users.FindAsync(id);
        }
    }
}