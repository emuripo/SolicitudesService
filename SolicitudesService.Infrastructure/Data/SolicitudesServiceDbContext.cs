using Microsoft.EntityFrameworkCore;
using SolicitudesService.Core.Entities;

namespace SolicitudesService.Infrastructure.Data
{
    public class SolicitudesServiceDbContext : DbContext
    {
        public SolicitudesServiceDbContext(DbContextOptions<SolicitudesServiceDbContext> options) : base(options)
        {
        }

        public DbSet<SolicitudPersonal> SolicitudesPersonales { get; set; }
        public DbSet<SolicitudDocumentos> SolicitudDocumentos { get; set; }
        public DbSet<SolicitudHorasExtra> SolicitudesHorasExtra { get; set; }
        public DbSet<SolicitudVacaciones> SolicitudesVacaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para SolicitudPersonal
            modelBuilder.Entity<SolicitudPersonal>().ToTable("SolicitudesPersonales");
            modelBuilder.Entity<SolicitudPersonal>()
                .Property(sp => sp.Motivo)
                .IsRequired()
                .HasMaxLength(250);
            modelBuilder.Entity<SolicitudPersonal>()
                .Property(sp => sp.Estado)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<SolicitudPersonal>()
                .Property(sp => sp.MotivoRechazo)
                .HasMaxLength(250);

            // Configuración para SolicitudDocumentos
            modelBuilder.Entity<SolicitudDocumentos>().ToTable("SolicitudesDocumentos");
            modelBuilder.Entity<SolicitudDocumentos>()
                .Property(sd => sd.TipoDocumento)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<SolicitudDocumentos>()
                .Property(sd => sd.Descripcion)
                .HasMaxLength(250);
            modelBuilder.Entity<SolicitudDocumentos>()
                .Property(sd => sd.Estado)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<SolicitudDocumentos>()
                .Property(sd => sd.MotivoRechazo)
                .HasMaxLength(250);

            // Configuración para SolicitudHorasExtra
            modelBuilder.Entity<SolicitudHorasExtra>().ToTable("SolicitudesHorasExtra");
            modelBuilder.Entity<SolicitudHorasExtra>()
                .Property(she => she.CantidadHoras)
                .IsRequired();
            modelBuilder.Entity<SolicitudHorasExtra>()
                .Property(she => she.FechaSolicitud)
                .IsRequired();
            modelBuilder.Entity<SolicitudHorasExtra>()
                .Property(she => she.Estado)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<SolicitudHorasExtra>()
                .Property(she => she.MotivoRechazo)
                .HasMaxLength(250);

            // Configuración para SolicitudVacaciones
            modelBuilder.Entity<SolicitudVacaciones>().ToTable("SolicitudesVacaciones");
            modelBuilder.Entity<SolicitudVacaciones>()
                .Property(sv => sv.FechaInicio)
                .IsRequired();
            modelBuilder.Entity<SolicitudVacaciones>()
                .Property(sv => sv.FechaFin)
                .IsRequired();
            modelBuilder.Entity<SolicitudVacaciones>()
                .Property(sv => sv.CantidadDias)
                .IsRequired();
            modelBuilder.Entity<SolicitudVacaciones>()
                .Property(sv => sv.Estado)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<SolicitudVacaciones>()
                .Property(sv => sv.MotivoRechazo)
                .HasMaxLength(250);
        }
    }
}
