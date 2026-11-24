using System.ComponentModel.DataAnnotations;

namespace ShuttleTiket.Models.ViewModels;

// form + hasil pencarian jadwal
public class CariJadwalViewModel
{
    [Display(Name = "Kota Asal")]
    public string? Asal { get; set; }

    [Display(Name = "Kota Tujuan")]
    public string? Tujuan { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Tanggal Berangkat")]
    public DateTime Tanggal { get; set; } = DateTime.Today;

    public List<string> DaftarKotaAsal { get; set; } = new();
    public List<string> DaftarKotaTujuan { get; set; } = new();

    public bool SudahCari { get; set; }
    public List<VwJadwalShuttle> Hasil { get; set; } = new();

    // - belum mencari -> jadwal beberapa hari ke depan
    // - hasil kosong  -> jadwal terdekat untuk rute yang dicari
    public List<VwJadwalShuttle> JadwalLain { get; set; } = new();

    // Ringkasan per rute untuk beranda
    public List<RingkasanRute> RuteTersedia { get; set; } = new();
}

// Satu baris rute di beranda
public class RingkasanRute
{
    public string KotaAsal { get; set; } = string.Empty;
    public string KotaTujuan { get; set; } = string.Empty;
    public decimal Harga { get; set; }
    public DateTime BerangkatBerikutnya { get; set; }
    public int JumlahJadwal { get; set; }
}

// Model untuk _TabelJadwal.cshtml
public record TabelJadwalModel(List<VwJadwalShuttle> Data, bool TampilkanTanggal);
