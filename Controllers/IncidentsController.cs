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
        [Tags("Search")]
        public async Task<ActionResult<IEnumerable<DTO>>> GetIncident()
        {
            if (_context.Incident == null)
            {
                return Problem("Incident is empty.");
            }
            var Incidents = _context.Incident;

            return await Incidents.AsNoTracking().Include(a => a.Account).ThenInclude(c => c.Contact).OrderBy(n => n.IncidentName).Select(d => new DTO
            {
                AccountName = d.Account.Name,
                Description = d.Description,
                Email = d.Account.Contact.Email,
                FirstName = d.Account.Contact.FirstName,
                LastName = d.Account.Contact.LastName,
            }).ToListAsync();

        }
        [HttpGet("{Name}")]
        [Tags("Search")]
        public async Task<ActionResult<IEnumerable<DTO>>> GetIncident(string Name)
        {
            if (AccountExists(Name))
            {
                var Incidents = _context.Incident;
                return await Incidents.Include(a => a.Account).ThenInclude(c => c.Contact).Where(x => x.Account.Name == Name).Select(d => new DTO
                {
                    AccountName = d.Account.Name,
                    Description = d.Description,
                    Email = d.Account.Contact.Email,
                    FirstName = d.Account.Contact.FirstName,
                    LastName = d.Account.Contact.LastName,
                }).ToListAsync();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Tags("Testing")]
        public async Task<ActionResult<DTO>> PostIncident(DTO dto)
        {
            if (_context.Incident == null)
            {
                return Problem("Incident is empty.");
            }
            if (!AccountExists(dto.AccountName))
            {
                if (!ContactExists(dto.Email))
                {
                    CreateContact(dto);

                    CreateAccount(dto);

                }
                else
                {
                    UpdateContact(dto);

                    CreateAccount(dto);
                }
            }
            else
            {

                if (!ContactExists(dto.Email))
                {
                    CreateContact(dto);

                    UpdateAccount(dto);

                }
                else
                {
                    UpdateContact(dto);

                    UpdateAccount(dto);
                }

            }
            CreateIncident(dto);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Problem("Error Update DataBase");
            }

            return CreatedAtAction("GetIncident", new { id = dto.AccountName }, dto);

        }
        private bool ContactExists(string email)
        {
            return (_context.Contact?.Any(e => e.Email == email)).GetValueOrDefault();
        }
        private bool AccountExists(string name)
        {
            return (_context.Account?.Any(e => e.Name == name)).GetValueOrDefault();
        }

        private void UpdateContact(DTO dto)
        {
            var contact = _context.Contact.FirstOrDefault(e => e.Email == dto.Email);
            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            _context.Update(contact);
            _context.SaveChanges();
        }
        private void UpdateAccount(DTO dto)
        {
            var account = _context.Account.FirstOrDefault(e => e.Name == dto.AccountName);
            var contact = _context.Contact.FirstOrDefault(e => e.Email == dto.Email);

            account.Contact = contact;
            _context.Update(account);
            _context.SaveChanges();
        }
        private void CreateAccount(DTO dto)
        {
            var contact = _context.Contact.FirstOrDefault(e => e.Email == dto.Email);

            Account account = new Account { Name = dto.AccountName, Contact = contact };
            _context.Account.Add(account);
            _context.SaveChanges();
        }
        private void CreateContact(DTO dto)
        {
            Contact contact = new Contact { Email = dto.Email, FirstName = dto.FirstName, LastName = dto.LastName };
            _context.Contact.Add(contact);
            _context.SaveChanges();
        }
        private void CreateIncident(DTO dto)
        {
            var account = _context.Account.FirstOrDefault(e => e.Name == dto.AccountName);
            Incident incident = new Incident { Description = dto.Description, Account = account };
            _context.Incident.Add(incident);
            _context.SaveChanges();
        }



    }
}