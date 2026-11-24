using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

public class VwJadwalShuttle
{
    [Column("jadwal_id")]      public int JadwalId { get; set; }
    [Column("tanggal")]        public DateTime Tanggal { get; set; }
    [Column("jam_berangkat")]  public TimeSpan JamBerangkat { get; set; }
    [Column("status_jadwal")]  public string StatusJadwal { get; set; } = string.Empty;
    [Column("rute_id")]        public int RuteId { get; set; }
    [Column("kota_asal")]      public string KotaAsal { get; set; } = string.Empty;
    [Column("kota_tujuan")]    public string KotaTujuan { get; set; } = string.Empty;
    [Column("harga")]          public decimal Harga { get; set; }
    [Column("estimasi_menit")] public int EstimasiMenit { get; set; }
    [Column("armada_id")]      public int ArmadaId { get; set; }
    [Column("nama_armada")]    public string NamaArmada { get; set; } = string.Empty;
    [Column("plat_nomor")]     public string PlatNomor { get; set; } = string.Empty;
    [Column("kapasitas")]      public int Kapasitas { get; set; }
    [Column("kursi_terisi")]   public long KursiTerisi { get; set; }   // COUNT(*) -> BIGINT
    [Column("sisa_kursi")]     public long SisaKursi { get; set; }

    [NotMapped] public string NamaRute => $"{KotaAsal} - {KotaTujuan}";
    [NotMapped] public DateTime WaktuBerangkat => Tanggal.Date + JamBerangkat;
}
