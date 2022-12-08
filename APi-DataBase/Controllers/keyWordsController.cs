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
    public class keyWordsController : ControllerBase
    {
        private readonly APi_DataBaseContext _context;

        public keyWordsController(APi_DataBaseContext context)
        {
            _context = context;
        }

        // GET: api/keyWords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<keyWords>>> GetkeyWords()
        {
            return await _context.keyWords.ToListAsync();
        }

        // GET: api/keyWords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<keyWords>> GetkeyWords(int id)
        {
            var keyWords = await _context.keyWords.FindAsync(id);

            if (keyWords == null)
            {
                return NotFound();
            }

            return keyWords;
        }

        // PUT: api/keyWords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutkeyWords(int id, keyWords keyWords)
        {
            if (id != keyWords.Project_Id)
            {
                return BadRequest();
            }

            _context.Entry(keyWords).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!keyWordsExists(id))
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

        // POST: api/keyWords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<keyWords>> PostkeyWords(keyWords keyWords)
        {
            _context.keyWords.Add(keyWords);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetkeyWords", new { id = keyWords.Project_Id }, keyWords);
        }

        // DELETE: api/keyWords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletekeyWords(int id)
        {
            var keyWords = await _context.keyWords.FindAsync(id);
            if (keyWords == null)
            {
                return NotFound();
            }

            _context.keyWords.Remove(keyWords);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool keyWordsExists(int id)
        {
            return _context.keyWords.Any(e => e.Project_Id == id);
        }
    }
}
