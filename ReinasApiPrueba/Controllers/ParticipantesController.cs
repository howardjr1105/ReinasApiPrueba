using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReinasApiPrueba.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ReinasApiPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParticipantesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Participantes
        [HttpGet]
        public async Task<ActionResult<object>> GetParticipantes()
        {
            //if (_context.Participantes == null)
            //{
            //    return NotFound();
            //}
            //  return await _context.Participantes.ToListAsync();

            // Asegúrate de que el nombre del stored procedure sea el correcto y coincide con el nombre en tu base de datos


            var participantesList = await _context.Participantes
                .FromSqlRaw("EXEC List_Participantes")
                .ToListAsync();

            // Si necesitas devolver una respuesta genérica personalizada
            return new GenricResponse
            {
                success = participantesList.Any(),
                Message = "Listado de participantes",
                Data = participantesList
            };

            //  var particioanteslist = await _context.Participantes.ToListAsync();

            //return new GenricResponse() { success = particioanteslist.Any(), Message = "Listado de participantes", Data = particioanteslist };
        }

        // GET: api/Participantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Participante>> GetParticipante(int id)
        {
            if (_context.Participantes == null)
            {
                return NotFound();
            }
            var participante = await _context.Participantes.FindAsync(id);

            if (participante == null)
            {
                return NotFound();
            }

            return participante;
        }

        // PUT: api/Participantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipante(int id, Participante participante)
        {
            if (id != participante.ParticipanteId)
            {
                return BadRequest();
            }

            _context.Entry(participante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipanteExists(id))
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

        // POST: api/Participantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Participante>> PostParticipante(Participante participante)
        {
            if (_context.Participantes == null)
            {
                return Problem("Entity set 'AppDbContext.Participantes'  is null.");
            }
            _context.Participantes.Add(participante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParticipante", new { id = participante.ParticipanteId }, participante);
        }

        // DELETE: api/Participantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipante(int id)
        {
            if (_context.Participantes == null)
            {
                return NotFound();
            }
            var participante = await _context.Participantes.FindAsync(id);
            if (participante == null)
            {
                return NotFound();
            }

            _context.Participantes.Remove(participante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParticipanteExists(int id)
        {
            return (_context.Participantes?.Any(e => e.ParticipanteId == id)).GetValueOrDefault();
        }


        [HttpGet("ActualizarTop10")]
        public async Task<IActionResult> ActualizarTop10()
        {
            // Ejecutar el procedimiento almacenado para actualizar el estado de los participantes al Top 3

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[Actualizar_Estado_Participantes]";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                }

                await _context.Database.CloseConnectionAsync();

                return Ok("Estado de los participantes actualizado para el Top 10.");
            }


        }


        [HttpGet("ActualizarTop6")]
        public async Task<IActionResult> ActualizarTop6()
        {
            // Ejecutar el procedimiento almacenado para actualizar el estado de los participantes al Top 3

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[Actualizar_Estado_Participantes_Top6]";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                }

                await _context.Database.CloseConnectionAsync();

                return Ok("Estado de los participantes actualizado para el Top 6.");
            }




        }


        [HttpGet("ActualizarTop3")]
        public async Task<IActionResult> ActualizarTop3()
        {
            // Ejecutar el procedimiento almacenado para actualizar el estado de los participantes al Top 3

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[Actualizar_Estado_Participantes_Top3]";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                }

                await _context.Database.CloseConnectionAsync();

                return Ok("Estado de los participantes actualizado para el Top 3.");
            }


        }

        [HttpGet("ActualizarGanadora")]
        public async Task<IActionResult> ActualizarGanadora()
        {
            // Ejecutar el procedimiento almacenado para actualizar el estado de los participantes al Top 3

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "[dbo].[Actualizar_Estado_Participantes_Ganadora]";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                await _context.Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                }

                await _context.Database.CloseConnectionAsync();

                return Ok("Estado de los participantes actualizado para la Ganadora");
            }


        }

    }


}
