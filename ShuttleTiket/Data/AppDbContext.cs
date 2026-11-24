using Microsoft.EntityFrameworkCore;

namespace ShuttleTiket.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        KonfigurasiAnggota1(modelBuilder);
        KonfigurasiAnggota2(modelBuilder);
    }

    partial void KonfigurasiAnggota1(ModelBuilder modelBuilder);
    partial void KonfigurasiAnggota2(ModelBuilder modelBuilder);
}
