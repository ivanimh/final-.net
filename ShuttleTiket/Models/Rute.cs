using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

[Table("rute")]
public class Rute
{
    [Column("id")]
    public int Id { get; set; }

    [Column("kota_asal")]
    [Required(ErrorMessage = "Kota asal wajib diisi")]
    [StringLength(50)]
    [Display(Name = "Kota Asal")]
    public string KotaAsal { get; set; } = string.Empty;

    [Column("kota_tujuan")]
    [Required(ErrorMessage = "Kota tujuan wajib diisi")]
    [StringLength(50)]
    [Display(Name = "Kota Tujuan")]
    public string KotaTujuan { get; set; } = string.Empty;

    [Column("harga")]
    [Range(1000, 10_000_000, ErrorMessage = "Harga tidak valid")]
    [Display(Name = "Harga per Kursi (Rp)")]
    public decimal Harga { get; set; }

    [Column("estimasi_menit")]
    [Range(10, 1440, ErrorMessage = "Estimasi 10 - 1440 menit")]
    [Display(Name = "Estimasi Perjalanan (menit)")]
    public int EstimasiMenit { get; set; } = 180;

    [NotMapped]
    public string NamaRute => $"{KotaAsal} - {KotaTujuan}";
}
