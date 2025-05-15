using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReinasApiPrueba.Models
{
    public class Ronda
    {
        [Key]
        [Column("ronda_id")]
        public int Ronda_ID { get; set; }
        [Column("nombre")]
        public string nombre { get; set; }
    }
}
