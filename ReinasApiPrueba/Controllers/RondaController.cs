using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReinasApiPrueba.Models;

namespace ReinasApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RondaController : ControllerBase
    {
        private readonly AppDbContext _context;        // GET: api/Participantes

        public RondaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ronda>>> GetRondas()
        {
            if (_context.Ronda == null)
            {
                return NotFound();
            }
            return await _context.Ronda.ToListAsync();
        }

    }
}
