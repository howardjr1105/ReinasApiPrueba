using Microsoft.EntityFrameworkCore;

namespace ReinasApiPrueba.Models
{
    [Keyless]
    public class PromediosFinalResult
    {
        public int ParticipanteId { get; set; }
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public decimal PuntajeFinal { get; set; }
        public long Rango { get; set; }  // 1=Reina, 2=Princesa1, 3=Princesa2
    }
}
