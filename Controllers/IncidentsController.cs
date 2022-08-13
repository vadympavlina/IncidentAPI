using IncidentAPI.Data;
using IncidentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : ControllerBase
    {
        private readonly IncidentAPIContext _context;

        public IncidentsController(IncidentAPIContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncident()
        {
            if (_context.Incident == null)
            {
                return NotFound();
            }

            return await _context.Incident.Include(i => i.Account).ThenInclude(a => a.Contact).ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Incident>> GetIncident(string id)
        {
            if (_context.Incident == null)
            {
                return NotFound();
            }
            var incident = await _context.Incident.FindAsync(id);

            if (incident == null)
            {
                return NotFound();
            }

            return incident;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncident(string id, Incident incident)
        {
            if (id != incident.IncidentName)
            {
                return BadRequest();
            }

            _context.Entry(incident).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
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
        public async Task<ActionResult<DTO>> PostIncident(DTO dto)
        {
            if (_context.Incident == null)
            {
                return Problem("Entity set 'IncidentAPIContext.Incident'is null.");
            }

            if (!_context.Contact.Any(u => u.Email == dto.Email))
            {
                Contact contact = new Contact { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email };
                
                _context.Contact.Add(contact);

            }
            else
            {
            #region Contact if exist email
                var Contacts = _context.Contact;
                Contact cont = _context.Contact.First(u => u.Email == dto.Email);
                var contact = new Contact
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Id = cont.Id
                };
                _context.ChangeTracker.Clear();
                var c = Contacts.Attach(contact);

                c.State = EntityState.Modified;
            #endregion
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ContactExists(dto.Email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetIncident", new { id = dto.AccountName }, dto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(string id)
        {
            if (_context.Incident == null)
            {
                return NotFound();
            }
            var incident = await _context.Incident.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            _context.Incident.Remove(incident);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactExists(string email)
        {
            return (_context.Contact?.Any(e => e.Email == email)).GetValueOrDefault();
        }
    }
}
