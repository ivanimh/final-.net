using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

// tabel `pemesanan`
[Table("pemesanan")]
public class Pemesanan
{
    // Nilai kolom status_pembayaran
    public const string StatusMenunggu = "MENUNGGU";
    public const string StatusLunas = "LUNAS";
    public const string StatusBatal = "BATAL";

    [Column("id")]                public int Id { get; set; }
    [Column("kode_booking")]      public string KodeBooking { get; set; } = string.Empty;
    [Column("jadwal_id")]         public int JadwalId { get; set; }
    [Column("nama_penumpang")]    public string NamaPenumpang { get; set; } = string.Empty;
    [Column("no_hp")]             public string NoHp { get; set; } = string.Empty;
    [Column("email")]             public string? Email { get; set; }
    [Column("jumlah_tiket")]      public int JumlahTiket { get; set; }
    [Column("total_harga")]       public decimal TotalHarga { get; set; }
    [Column("status_pembayaran")] public string StatusPembayaran { get; set; } = StatusMenunggu;
    [Column("metode_pembayaran")] public string? MetodePembayaran { get; set; }
    [Column("tanggal_pesan")]     public DateTime TanggalPesan { get; set; } = DateTime.Now;
    [Column("tanggal_bayar")]     public DateTime? TanggalBayar { get; set; }

    public List<PemesananKursi> Kursi { get; set; } = new();
}
