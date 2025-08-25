using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using ReinasApiPrueba.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

    [HttpGet("ReportePDF/{ronda_id}")]
    public async Task<IActionResult> GenerarReportePDF(int ronda_id)
    {
        var resultados = await EjecutarProcedimientoPromedios("Promedio_ronda", ronda_id);

        if (resultados == null || !resultados.Any())
        {
            return NotFound(new
            {
                success = false,
                message = "No se encontraron resultados para generar el reporte."
            });
        }

        // Creamos el documento PDF
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Encabezado
                page.Header().Row(row =>
                {
                    row.RelativeColumn().AlignCenter().Text($"Reporte de Promedios - Ronda {ronda_id}")
                       .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);
                });

                // Contenido
                page.Content().Table(table =>
                {
                    // Definir columnas
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50);   // Rango
                        columns.RelativeColumn(2);    // Nombre
                        columns.RelativeColumn(2);    // Departamento
                        columns.RelativeColumn(1);    // PuntajeFinal
                    });

                    // Encabezados
                    table.Header(header =>
                    {
                        header.Cell().Element(c => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten2)).Text("Rango");
                        header.Cell().Element(c => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten2)).Text("Nombre");
                        header.Cell().Element(c => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten2)).Text("Departamento");
                        header.Cell().Element(c => c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten2)).Text("Puntaje");

                    });

                    // Filas dinámicas
                    foreach (var r in resultados.OrderBy(x => x.Rango))
                    {
                        table.Cell().Element(c => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5)).Text(r.Rango.ToString());
                        table.Cell().Element(c => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5)).Text(r.Nombre);
                        table.Cell().Element(c => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5)).Text(r.Departamento);
                        table.Cell().Element(c => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5)).Text(r.PuntajeFinal.ToString("0.00"));

                    }
                });

                // Pie de página
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generado el ").FontSize(10);
                    x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
        });

        // Generar PDF en memoria
        byte[] pdfBytes = doc.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"ReporteRonda_{ronda_id}.pdf");
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
