using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Models;

namespace ShuttleTiket.Data;

// DbSet & mapping untuk tabel transaksi + 1 SQL View.
public partial class AppDbContext
{
    // Tabel
    public DbSet<Pemesanan> Pemesanan => Set<Pemesanan>();
    public DbSet<PemesananKursi> PemesananKursi => Set<PemesananKursi>();

    // SQL View (read-only)
    public DbSet<VwDetailPemesanan> VwDetailPemesanan => Set<VwDetailPemesanan>();

    partial void KonfigurasiAnggota2(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PemesananKursi>()
            .HasOne(k => k.Pemesanan)
            .WithMany(p => p.Kursi)
            .HasForeignKey(k => k.PemesananId);

        // SQL VIEW -> class C#
        modelBuilder.Entity<VwDetailPemesanan>()
            .HasNoKey()
            .ToView("vw_detail_pemesanan");
    }
}
