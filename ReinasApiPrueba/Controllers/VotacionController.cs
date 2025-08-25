using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReinasApiPrueba.Models;

namespace ReinasApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VotacionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Votacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Votacion>>> GetVotacion()
        {
            if (_context.votacions == null)
            {
                return NotFound();
            }
            return await _context.votacions.ToListAsync();
        }

        // PUT: api/Votacion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVotacion(int id, Votacion votacion)
        {
            if (id != votacion.Voto_ID)
            {
                return BadRequest();
            }

            var votacionExistente = await _context.votacions.FindAsync(id);
            if (votacionExistente == null)
            {
                return NotFound();
            }

            votacionExistente.Puntuacion = votacion.Puntuacion;
            votacionExistente.Usuario_ID = votacion.Usuario_ID;
            votacionExistente.Ronda_ID = votacion.Ronda_ID;
            votacionExistente.Participante_ID = votacion.Participante_ID;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotacionExists(id))
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

        // POST: api/Votacion
        [HttpPost]
        public async Task<ActionResult<GenricResponse>> PostVotacion(VotacionCreateDto dto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Verificar si ya existe un voto en esta ronda para el usuario
                    var votoExistente = await _context.votacions
                        .FirstOrDefaultAsync(v => v.Usuario_ID == dto.Usuario_ID
                                               && v.Participante_ID == dto.Participante_ID
                                               && v.Ronda_ID == dto.Ronda_ID);

                    if (votoExistente != null)
                    {
                        return Conflict(new GenricResponse
                        {
                            success = false,
                            Message = "El usuario ya ha votado en esta ronda.",
                            Data = null
                        });
                    }

                    var votacion = new Votacion
                    {
                        Usuario_ID = dto.Usuario_ID,
                        Participante_ID = dto.Participante_ID,
                        Ronda_ID = dto.Ronda_ID,
                        Puntuacion = dto.Puntuacion
                    };

                    _context.votacions.Add(votacion);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok(new GenricResponse
                    {
                        success = true,
                        Message = "Voto registrado exitosamente.",
                        Data = votacion
                    });
                }
                catch (DbUpdateException ex) // <-- más adecuado
                {
                    transaction.Rollback();

                    // Detecta si es por restricción UNIQUE (índice Voto_Unico)
                    if (ex.InnerException?.Message.Contains("Voto_Unico") == true)
                    {
                        return Conflict(new GenricResponse
                        {
                            success = false,
                            Message = "Ya existe un voto con la misma combinación de usuario, participante y ronda.",
                            Data = null
                        });
                    }

                    // Manejo de error genérico si es que no se detecta en codigo pero si en la BD

                    return StatusCode(StatusCodes.Status500InternalServerError, new GenricResponse
                    {
                        success = false,
                        Message = "Error al guardar el voto: " + ex.InnerException?.Message ?? ex.Message,
                        Data = null
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, new GenricResponse
                    {
                        success = false,
                        Message = "Error inesperado: " + ex.Message,
                        Data = null
                    });
                }
            }
        }

        private bool VotacionExists(int id)
        {
            return _context.votacions.Any(e => e.Voto_ID == id);
        }
    }

    // DTO para evitar que el cliente envíe voto_id
    public class VotacionCreateDto
    {
        public int Usuario_ID { get; set; }
        public int Participante_ID { get; set; }
        public int Ronda_ID { get; set; }
        public int Puntuacion { get; set; }
    }
}
