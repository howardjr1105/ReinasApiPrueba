namespace ReinasApiPrueba.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Participante
    {
        [Key]
        [Column("participante_id")]
        public int ParticipanteId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [Column("edad")]
        public int Edad { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("departamento")]
        public string Departamento { get; set; }

        [Required]
        [Column("peso")]
        [Range(0, 999.99)]
        public decimal Peso { get; set; }

        [Required]
        [Column("estatura")]
        [Range(0, 999.99)]
        public decimal Estatura { get; set; }

        [Required]
        [Column("biografia")]
        public string Biografia { get; set; }

        [Column("Estado_Participante")]
        public bool Estado { get; set; }

        [Column("Imagen")]
        public String Img { get; set; }
    }


}
