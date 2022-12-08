using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APi_DataBase.Data;
using APi_DataBase.modals;

namespace APi_DataBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class likesController : ControllerBase
    {
        private readonly APi_DataBaseContext _context;

        public likesController(APi_DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<likes>>> Getlikes()
        {
            return await _context.likes.ToListAsync();
        }

        // GET: api/likes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<likes>> Getlikes(string id)
        {
            var likes = await _context.likes.FindAsync(id);

            if (likes == null)
            {
                return NotFound();
            }

            return likes;
        }

        // PUT: api/likes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putlikes(string id, likes likes)
        {
            if (id != likes.User_Name)
            {
                return BadRequest();
            }

            _context.Entry(likes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!likesExists(id))
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

        // POST: api/likes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<likes>> Postlikes(likes likes)
        {
            _context.likes.Add(likes);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (likesExists(likes.User_Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Getlikes", new { id = likes.User_Name }, likes);
        }

        // DELETE: api/likes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletelikes(string id)
        {
            var likes = await _context.likes.FindAsync(id);
            if (likes == null)
            {
                return NotFound();
            }

            _context.likes.Remove(likes);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool likesExists(string id)
        {
            return _context.likes.Any(e => e.User_Name == id);
        }
    }
}
