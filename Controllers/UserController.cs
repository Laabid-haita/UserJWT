using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserWebAPI.Models;

namespace UserWebAPI.Controllers
{
    [Authorize]
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DBFormula1Context _formulaContext;
        public UserController(DBFormula1Context formulaContext)
        {
            _formulaContext = formulaContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _formulaContext.Users.ToListAsync();

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUsers(int id)
        {
            var us = await _formulaContext.Users.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            return us;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, User user)
        {
            if (id != user.IdU)
            {
                return BadRequest();
            }
            _formulaContext.Entry(user).State = EntityState.Modified;
            try
            {
                await _formulaContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]

        public async Task<ActionResult<User>> PostUsers(User user)
        {
            _formulaContext.Users.Add(user);
            await _formulaContext.SaveChangesAsync();

            return CreatedAtAction(nameof(PostUsers), new { id = user.IdU }, user);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUsers(int id)
        {
            var us = await _formulaContext.Users.FindAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            _formulaContext.Users.Remove(us);
            await _formulaContext.SaveChangesAsync();

            return us;
        }







        private bool UserExists(int id)
        {
            return _formulaContext.Users.Any(e => e.IdU == id);
        }
    }
}
