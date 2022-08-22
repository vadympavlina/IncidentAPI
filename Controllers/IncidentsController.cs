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
            var accountExists = await AccountExistsAsync(Name);
            if (accountExists)
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
            var accountExists = await AccountExistsAsync(dto.AccountName);
            var contactExists = await ContactExistsAsync(dto.Email);

            await (contactExists ? UpdateContactAsync(dto) : CreateContactAsync(dto));

            await (accountExists ? UpdateAccountAsync(dto) : CreateAccountAsync(dto));

            await CreateIncidentAsync(dto);

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
        private async Task<bool> ContactExistsAsync(string email) => await _context.Contact.AnyAsync(e => e.Email == email);
        private async Task<bool> AccountExistsAsync(string name) => await _context.Account.AnyAsync(e => e.Name == name);
        private async Task UpdateContactAsync(DTO dto)
        {
            var contact = await _context.Contact.FirstOrDefaultAsync(e => e.Email == dto.Email);
            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;

            await _context.SaveChangesAsync();
        }
        private async Task UpdateAccountAsync(DTO dto)
        {
            var account = await _context.Account.FirstOrDefaultAsync(e => e.Name == dto.AccountName);
            account.Contact = await _context.Contact.FirstOrDefaultAsync(e => e.Email == dto.Email);

            await _context.SaveChangesAsync();
        }
        private async Task CreateAccountAsync(DTO dto)
        {
            var contact = await _context.Contact.FirstOrDefaultAsync(e => e.Email == dto.Email);
            _context.Account.Add(new Account { Name = dto.AccountName, Contact = contact });

            await _context.SaveChangesAsync();
        }
        private async Task CreateContactAsync(DTO dto)
        {
            var contact = new Contact { Email = dto.Email, FirstName = dto.FirstName, LastName = dto.LastName };
            _context.Contact.Add(contact);

            await _context.SaveChangesAsync();
        }
        private async Task CreateIncidentAsync(DTO dto)
        {
            var account = await _context.Account.FirstOrDefaultAsync(e => e.Name == dto.AccountName);
            var incident = new Incident { Description = dto.Description, Account = account };
            _context.Incident.Add(incident);

            await _context.SaveChangesAsync();
        }
    }
}