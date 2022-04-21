using System.Threading.Tasks;
using API.data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet] 
        public async Task<ActionResult<List<AppUser>>> GetUsers(){
            //var users = _context.Users.ToList();
            //return users;
            return await _context.Users.ToListAsync();
        }
        //api/users/3
        [HttpGet("{id}")] 
        public async Task<ActionResult<AppUser>> GetUser(int id){
            //var user = _context.Users.Find(id);
            //return user;
            return await _context.Users.FindAsync(id);
        }
    }
}