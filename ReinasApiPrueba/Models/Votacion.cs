using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReinasApiPrueba.Models
{
    public class Votacion
    {
        [Key]
        [Column("voto_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indica que la base de datos genera el valor
        public int Voto_ID { get; set; }

        [Column("usuario_id")]
        public int Usuario_ID { get; set; }

        [Column("participante_id")]
        public int Participante_ID { get; set; }

        [Column("ronda_id")]
        public int Ronda_ID { get; set; }

        [Column("puntuacion")]
        public int Puntuacion { get; set; }

        [Column("tiempo_votacion")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // ← IMPORTANTE
        public DateTime? TiempoVotacion { get; set; }
    }

}
