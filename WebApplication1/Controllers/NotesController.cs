using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly WebApplication1Context database;

        public NotesController(WebApplication1Context database)
        {
            this.database = database;
        }

        // GET: api/Notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNote([FromQuery] string completed)
        {
            var query = database.Note.AsNoTracking();
            if (completed == "false")
            {
                return await query.Where(n => n.IsDone == false).ToListAsync();
            }
            else if (completed == "true")
            {
                return await query.Where(n => n.IsDone == true).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        

        // GET: api/Notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            var note = await database.Note.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        // PUT: api/Notes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, Note note)
        {
            var dbNote = await database.Note.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }
            dbNote.Text = note.Text;
            dbNote.IsDone = note.IsDone;

            await database.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Notes
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(Note note)
        {
            var dbNote = new Note
            {
                Text = note.Text,
                IsDone = note.IsDone
            };
            database.Add(dbNote);
            await database.SaveChangesAsync();

            return CreatedAtAction("GetNote", new { id = note.ID }, note);
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await database.Note.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            database.Note.Remove(note);
            await database.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("/api/remaining")]
        public ActionResult<int> RemainingNotes()
        {
            return database.Note.Count(n => n.IsDone == false);
        }


        [HttpPost("/api/clear-completed")]
        public async Task<ActionResult<Note>> ClearCompleted()
        {
            foreach (var completed in database.Note.Where(n => n.IsDone == true)) 
            {
                database.Note.Remove(completed);
            }
            await database.SaveChangesAsync();
            return NoContent();
        }

        private bool NoteExists(int id)
        {
            return database.Note.Any(e => e.ID == id);
        }
    }
}
