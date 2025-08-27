using Microsoft.EntityFrameworkCore;
using ReinasApiPrueba.Models;

public class AppDbContext : DbContext
{
    public DbSet<Participante> Participantes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Ronda> Ronda { get; set; }
    public DbSet<Votacion> votacions { get; set; }
    // DbSet para el resultado del SP
    public DbSet<PromediosFinalResult> PromediosFinalResultados { get; set; }

    public DbSet<ParticipantePromedio> ParticipantePromedio { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participante>().ToTable("Participantes");
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        modelBuilder.Entity<Ronda>().ToTable("Rondas");
        modelBuilder.Entity<Votacion>().ToTable("Votos");
        modelBuilder.Entity<ParticipantePromedio>().HasNoKey();
        modelBuilder.Entity<PromediosFinalResult>().HasNoKey();

        modelBuilder.Entity<Votacion>()
            .Property(v => v.Voto_ID)
            .ValueGeneratedOnAdd(); // Configura Voto_ID para autoincremento
        modelBuilder.Entity<Votacion>()
    .Property(v => v.TiempoVotacion)
    .HasDefaultValueSql("GETDATE()")
    .ValueGeneratedOnAdd(); // ← EF sabrá que se genera al insertar
       
    }
}

