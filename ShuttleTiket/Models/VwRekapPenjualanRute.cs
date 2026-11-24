using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

public class VwRekapPenjualanRute
{
    [Column("rute_id")]          public int RuteId { get; set; }
    [Column("kota_asal")]        public string KotaAsal { get; set; } = string.Empty;
    [Column("kota_tujuan")]      public string KotaTujuan { get; set; } = string.Empty;
    [Column("jumlah_jadwal")]    public long JumlahJadwal { get; set; }
    [Column("jumlah_transaksi")] public long JumlahTransaksi { get; set; }
    [Column("tiket_terjual")]    public long TiketTerjual { get; set; }
    [Column("total_pendapatan")] public decimal TotalPendapatan { get; set; }

    [NotMapped] public string NamaRute => $"{KotaAsal} - {KotaTujuan}";
}
