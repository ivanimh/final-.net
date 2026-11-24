using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Models;

namespace ShuttleTiket.Data;

// PEMILIK: ANGGOTA 1
// DbSet & mapping untuk tabel master + 2 SQL View milik Anggota 1.
public partial class AppDbContext
{
    // ----- Tabel -----
    public DbSet<Armada> Armada => Set<Armada>();
    public DbSet<Rute> Rute => Set<Rute>();
    public DbSet<Jadwal> Jadwal => Set<Jadwal>();
    public DbSet<Admin> Admin => Set<Admin>();

    // ----- SQL View (read-only, tanpa primary key) -----
    public DbSet<VwJadwalShuttle> VwJadwalShuttle => Set<VwJadwalShuttle>();
    public DbSet<VwRekapPenjualanRute> VwRekapPenjualanRute => Set<VwRekapPenjualanRute>();

    partial void KonfigurasiAnggota1(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Jadwal>(e =>
        {
            e.HasOne(j => j.Rute).WithMany().HasForeignKey(j => j.RuteId);
            e.HasOne(j => j.Armada).WithMany().HasForeignKey(j => j.ArmadaId);
        });

        // Pemetaan SQL VIEW -> class C#
        modelBuilder.Entity<VwJadwalShuttle>()
            .HasNoKey()
            .ToView("vw_jadwal_shuttle");

        modelBuilder.Entity<VwRekapPenjualanRute>()
            .HasNoKey()
            .ToView("vw_rekap_penjualan_rute");
    }
}
