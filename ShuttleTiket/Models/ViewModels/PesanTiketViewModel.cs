using System.ComponentModel.DataAnnotations;

namespace ShuttleTiket.Models.ViewModels;

// form pemesanan tiket + pilih kursi
public class PesanTiketViewModel
{
    public int JadwalId { get; set; }

    [Required(ErrorMessage = "Nama penumpang wajib diisi")]
    [StringLength(100)]
    [Display(Name = "Nama Penumpang")]
    public string NamaPenumpang { get; set; } = string.Empty;

    [Required(ErrorMessage = "No. HP wajib diisi")]
    [RegularExpression(@"^(\+62|0)8[0-9]{7,12}$", ErrorMessage = "Format No. HP tidak valid (contoh: 081234567890)")]
    [Display(Name = "No. HP / WhatsApp")]
    public string NoHp { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    [Display(Name = "Email (opsional)")]
    public string? Email { get; set; }

    // Nomor kursi yang dicentang
    public List<int> KursiDipilih { get; set; } = new();

    // --- data tampilan (tidak dikirim dari form) ---
    public VwJadwalShuttle? Jadwal { get; set; }
    public List<int> KursiTerisi { get; set; } = new();
}
