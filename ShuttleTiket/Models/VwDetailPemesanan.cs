using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

// Model READ-ONLY untuk SQL VIEW `vw_detail_pemesanan`
// Dipakai untuk detail booking, cek booking, e-ticket, daftar pemesanan admin.
public class VwDetailPemesanan
{
    [Column("pemesanan_id")]      public int PemesananId { get; set; }
    [Column("kode_booking")]      public string KodeBooking { get; set; } = string.Empty;
    [Column("nama_penumpang")]    public string NamaPenumpang { get; set; } = string.Empty;
    [Column("no_hp")]             public string NoHp { get; set; } = string.Empty;
    [Column("email")]             public string? Email { get; set; }
    [Column("jumlah_tiket")]      public int JumlahTiket { get; set; }
    [Column("total_harga")]       public decimal TotalHarga { get; set; }
    [Column("status_pembayaran")] public string StatusPembayaran { get; set; } = string.Empty;
    [Column("metode_pembayaran")] public string? MetodePembayaran { get; set; }
    [Column("tanggal_pesan")]     public DateTime TanggalPesan { get; set; }
    [Column("tanggal_bayar")]     public DateTime? TanggalBayar { get; set; }
    [Column("jadwal_id")]         public int JadwalId { get; set; }
    [Column("tanggal")]           public DateTime Tanggal { get; set; }
    [Column("jam_berangkat")]     public TimeSpan JamBerangkat { get; set; }
    [Column("kota_asal")]         public string KotaAsal { get; set; } = string.Empty;
    [Column("kota_tujuan")]       public string KotaTujuan { get; set; } = string.Empty;
    [Column("harga")]             public decimal Harga { get; set; }
    [Column("nama_armada")]       public string NamaArmada { get; set; } = string.Empty;
    [Column("plat_nomor")]        public string PlatNomor { get; set; } = string.Empty;
    [Column("daftar_kursi")]      public string? DaftarKursi { get; set; }   // contoh: "1, 2, 5"

    [NotMapped] public string NamaRute => $"{KotaAsal} - {KotaTujuan}";

    [NotMapped]
    public string BadgeStatus => StatusPembayaran switch
    {
        Pemesanan.StatusLunas => "text-bg-success",
        Pemesanan.StatusBatal => "text-bg-danger",
        _ => "text-bg-warning"
    };
}
