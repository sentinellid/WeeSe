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
        public DbSet<Preventivo> Preventivi { get; set; }
        // Qui aggiungerai le tue altre tabelle esistenti del progetto
        // public DbSet<Cliente> Clienti { get; set; }
        // public DbSet<Prodotto> Prodotti { get; set; }
        // public DbSet<Listino> Listini { get; set; }
        // public DbSet<Preventivo> Preventivi { get; set; }
        // public DbSet<Ordine> Ordini { get; set; }
        // public DbSet<Commessa> Commesse { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurazione per tabella di autenticazione esistente
            modelBuilder.Entity<AnagraficaAccesso>(entity =>
            {
                entity.HasKey(e => e.IDAnagraficaAccesso);
                entity.Property(e => e.sUtente).IsRequired().HasMaxLength(100);
                entity.Property(e => e.sPassword).IsRequired().HasMaxLength(100);
                entity.Property(e => e.bAttivo).HasDefaultValue(true);
            });
        }
    }
}
