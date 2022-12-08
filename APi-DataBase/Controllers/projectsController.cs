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
    public class projectsController : ControllerBase
    {
        private readonly APi_DataBaseContext _context;

        public projectsController(APi_DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<projects>>> Getprojects()
        {
            return await _context.projects.ToListAsync();
        }

        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<projects>> Getprojects(int id)
        {
            var projects = await _context.projects.FindAsync(id);

            if (projects == null)
            {
                return NotFound();
            }

            return projects;
        }

        // PUT: api/projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putprojects(int id, projects projects)
        {
            if (id != projects.Id)
            {
                return BadRequest();
            }

            _context.Entry(projects).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!projectsExists(id))
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

        // POST: api/projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<projects>> Postprojects(projects projects)
        {
            _context.projects.Add(projects);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getprojects", new { id = projects.Id }, projects);
        }

        // DELETE: api/projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteprojects(int id)
        {
            var projects = await _context.projects.FindAsync(id);
            if (projects == null)
            {
                return NotFound();
            }

            _context.projects.Remove(projects);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool projectsExists(int id)
        {
            return _context.projects.Any(e => e.Id == id);
        }
    }
}
