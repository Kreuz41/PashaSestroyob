using Microsoft.EntityFrameworkCore;
using PashaSestroyob.Data.Models.Db.Models;

namespace PashaSestroyob.Data.Models.Db
{
    public class PashaGandonEbaniyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }

        public PashaGandonEbaniyContext(DbContextOptions<PashaGandonEbaniyContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=PashaPidorasGorbatiyTzEbanutoeProstoTupoiDaun;Username=postgres;Password=admin");
        }
    }
}
