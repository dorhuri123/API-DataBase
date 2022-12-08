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
    public class repositoriesController : ControllerBase
    {
        private readonly APi_DataBaseContext _context;

        public repositoriesController(APi_DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/repositories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<repositories>>> Getrepositories()
        {
            return await _context.repositories.ToListAsync();
        }

        // GET: api/repositories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<repositories>> Getrepositories(int id)
        {
            var repositories = await _context.repositories.FindAsync(id);

            if (repositories == null)
            {
                return NotFound();
            }

            return repositories;
        }

        // PUT: api/repositories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putrepositories(int id, repositories repositories)
        {
            if (id != repositories.Id)
            {
                return BadRequest();
            }

            _context.Entry(repositories).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!repositoriesExists(id))
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

        // POST: api/repositories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<repositories>> Postrepositories(repositories repositories)
        {
            _context.repositories.Add(repositories);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getrepositories", new { id = repositories.Id }, repositories);
        }

        // DELETE: api/repositories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleterepositories(int id)
        {
            var repositories = await _context.repositories.FindAsync(id);
            if (repositories == null)
            {
                return NotFound();
            }

            _context.repositories.Remove(repositories);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool repositoriesExists(int id)
        {
            return _context.repositories.Any(e => e.Id == id);
        }
    }
}
