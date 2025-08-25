namespace ReinasApiPrueba.Models
{
    public class ParticipantePromedio
    {
        public int ParticipanteId { get; set; }
        public string? Nombre { get; set; }
        public string? Departamento { get; set; }
        public decimal PuntajeFinal { get; set; }  // Cambiar a decimal
        public long Rango { get; set; }  // BigInt en SQL se mapea a long en C#
    }



}
