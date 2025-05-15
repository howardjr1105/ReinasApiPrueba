using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReinasApiPrueba.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PromediosController : ControllerBase
{
    private readonly AppDbContext _context;

    public PromediosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Promedios
    [HttpGet]
    public async Task<IActionResult> ObtenerPromediosGenerales()
    {
        var resultados = await EjecutarProcedimientoPromedios("Calcular_Promedios", null);

        return Ok(new
        {
            success = true,
            message = "Promedios generales calculados exitosamente",
            data = resultados
        });
    }

    // GET: api/Promedios/{ronda_id}
    [HttpGet("{ronda_id}")]
    public async Task<IActionResult> ObtenerPromediosPorRonda(int ronda_id)
    {
        var resultados = await EjecutarProcedimientoPromedios("Promedio_ronda", ronda_id);

        return Ok(new
        {
            success = true,
            message = $"Promedios de la ronda {ronda_id} calculados exitosamente",
            data = resultados
        });
    }

    // GET: api/Promedios/ListarTop6
    [HttpGet("ListarTop6")]
    public async Task<IActionResult> ListarTop6()
    {
        var resultados = await EjecutarProcedimientoPromedios("Calcular_Promedios_Top6", null);

        return Ok(new
        {
            success = true,
            message = "Top 6 calculado exitosamente",
            data = resultados
        });
    }

    // GET: api/Promedios/ListarTop3
    [HttpGet("ListarTop3")]
    public async Task<IActionResult> ListarTop3()
    {
        var resultados = await EjecutarProcedimientoPromedios("Calcular_Promedios_Top3", null);

        return Ok(new
        {
            success = true,
            message = "Top 3 calculado exitosamente",
            data = resultados
        });
    }

    // GET: api/Promedios/ListarGanadora
    [HttpGet("ListarGanadora")]
    public async Task<IActionResult> ListarGanadora()
    {
        var resultados = await EjecutarProcedimientoPromedios("Calcular_Promedios_Final", null);

        return Ok(new
        {
            success = true,
            message = "Ganadora calculada exitosamente",
            data = resultados
        });
    }

    // Función auxiliar para ejecutar procedimientos almacenados
    private async Task<List<ParticipantePromedio>> EjecutarProcedimientoPromedios(string procedimiento, int? ronda_id)
    {
        var resultados = new List<ParticipantePromedio>();

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = ronda_id.HasValue
                ? $"EXEC {procedimiento} @ronda_id"
                : $"EXEC {procedimiento}"; // Ejecuta sin parámetro si ronda_id es null
            command.CommandType = System.Data.CommandType.Text;

            if (ronda_id.HasValue)
            {
                var param = command.CreateParameter();
                param.ParameterName = "@ronda_id";
                param.Value = ronda_id.Value;
                param.DbType = System.Data.DbType.Int32;
                command.Parameters.Add(param);
            }

            await _context.Database.OpenConnectionAsync();

            using (var result = await command.ExecuteReaderAsync())
            {
                while (await result.ReadAsync())
                {
                    var participante = new ParticipantePromedio
                    {
                        ParticipanteId = result.GetInt32(result.GetOrdinal("ParticipanteId")),
                        Nombre = result.GetString(result.GetOrdinal("Nombre")),
                        Departamento = result.GetString(result.GetOrdinal("Departamento")),
                        PuntajeFinal = result.GetDecimal(result.GetOrdinal("PuntajeFinal")),
                        Rango = result.GetInt64(result.GetOrdinal("Rango"))
                    };
                    resultados.Add(participante);
                }
            }

            await _context.Database.CloseConnectionAsync();
        }

        return resultados;
    }

}
