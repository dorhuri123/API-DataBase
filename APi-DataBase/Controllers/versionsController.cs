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
    public class versionsController : ControllerBase
    {
        private readonly APi_DataBaseContext _context;

        public versionsController(APi_DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/versions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<versions>>> Getversions()
        {
            return await _context.versions.ToListAsync();
        }

        // GET: api/versions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<versions>> Getversions(int id)
        {
            var versions = await _context.versions.FindAsync(id);

            if (versions == null)
            {
                return NotFound();
            }

            return versions;
        }

        // PUT: api/versions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putversions(int id, versions versions)
        {
            if (id != versions.Id)
            {
                return BadRequest();
            }

            _context.Entry(versions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!versionsExists(id))
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

        // POST: api/versions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<versions>> Postversions(versions versions)
        {
            _context.versions.Add(versions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getversions", new { id = versions.Id }, versions);
        }

        // DELETE: api/versions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteversions(int id)
        {
            var versions = await _context.versions.FindAsync(id);
            if (versions == null)
            {
                return NotFound();
            }

            _context.versions.Remove(versions);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool versionsExists(int id)
        {
            return _context.versions.Any(e => e.Id == id);
        }
    }
}
