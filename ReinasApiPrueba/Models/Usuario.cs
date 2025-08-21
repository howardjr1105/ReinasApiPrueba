namespace ReinasApiPrueba.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Usuario
    {
        [Key]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string? Correo { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("password")]
        public string Contraseña { get; set; }

        [Column("rol_id")]
        public int Id_rol { get; set; }
    }

}
