using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

// tabel `pemesanan_kursi` (1 baris = 1 kursi/tiket)
[Table("pemesanan_kursi")]
public class PemesananKursi
{
    [Column("id")]           public int Id { get; set; }
    [Column("pemesanan_id")] public int PemesananId { get; set; }
    [Column("nomor_kursi")]  public int NomorKursi { get; set; }

    public Pemesanan? Pemesanan { get; set; }
}
