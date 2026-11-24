using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

[Table("armada")]
public class Armada
{
    [Column("id")]
    public int Id { get; set; }

    [Column("kode_armada")]
    [Required(ErrorMessage = "Kode armada wajib diisi")]
    [StringLength(10)]
    [Display(Name = "Kode Armada")]
    public string KodeArmada { get; set; } = string.Empty;

    [Column("nama_armada")]
    [Required(ErrorMessage = "Nama armada wajib diisi")]
    [StringLength(100)]
    [Display(Name = "Nama / Tipe Kendaraan")]
    public string NamaArmada { get; set; } = string.Empty;

    [Column("plat_nomor")]
    [Required(ErrorMessage = "Plat nomor wajib diisi")]
    [StringLength(15)]
    [Display(Name = "Plat Nomor")]
    public string PlatNomor { get; set; } = string.Empty;

    [Column("kapasitas")]
    [Range(1, 50, ErrorMessage = "Kapasitas harus 1 - 50 kursi")]
    [Display(Name = "Kapasitas Kursi")]
    public int Kapasitas { get; set; } = 10;
}
