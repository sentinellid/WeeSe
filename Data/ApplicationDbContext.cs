using Microsoft.EntityFrameworkCore;
using WeeSe.Models;

namespace WeeSe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Tabella di autenticazione esistente
        public DbSet<AnagraficaAccesso> AnagraficaAccesso { get; set; }
        public DbSet<Preventivo> Preventivi { get; set; } = default!;
        public DbSet<DimensioneFinita> DimensioniFinite { get; set; } = default!;
        public DbSet<Commessa> Commesse { get; set; }
        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<ListinoPrezzo> ListiniPrezzi { get; set; }
        public DbSet<AttivitaCommessa> AttivitaCommesse { get; set; }
        public DbSet<Listino> Listini { get; set; }
        public DbSet<ListinoProdotto> ListinoProdotti { get; set; }
        public DbSet<Prodotto> Prodotti { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ AGGIUNGI CONFIGURAZIONE PREVENTIVI
            builder.Entity<Preventivo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cliente).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NumeroPreventivo).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NumeroPreventivo).IsUnique();
            });

            builder.Entity<DimensioneFinita>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Preventivo)
                      .WithMany(p => p.Dimensioni)
                      .HasForeignKey(d => d.PreventivoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // Configurazioni specifiche se necessarie
            builder.Entity<Listino>()
                .Property(e => e.PerTrasporto)
                .HasPrecision(5, 2);

            builder.Entity<Listino>()
                .Property(e => e.CostoCapra)
                .HasPrecision(10, 2);

            builder.Entity<Listino>()
                .Property(e => e.PerImballo)
                .HasPrecision(5, 2);
        }


    }
}
