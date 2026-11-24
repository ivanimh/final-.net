using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

[Table("jadwal")]
public class Jadwal
{
    public const string StatusAktif = "AKTIF";
    public const string StatusBatal = "BATAL";

    [Column("id")]
    public int Id { get; set; }

    [Column("rute_id")]
    [Display(Name = "Rute")]
    [Range(1, int.MaxValue, ErrorMessage = "Pilih rute")]
    public int RuteId { get; set; }

    [Column("armada_id")]
    [Display(Name = "Armada")]
    [Range(1, int.MaxValue, ErrorMessage = "Pilih armada")]
    public int ArmadaId { get; set; }

    [Column("tanggal")]
    [DataType(DataType.Date)]
    [Display(Name = "Tanggal Berangkat")]
    public DateTime Tanggal { get; set; } = DateTime.Today.AddDays(1);

    [Column("jam_berangkat")]
    [DataType(DataType.Time)]
    [Display(Name = "Jam Berangkat")]
    public TimeSpan JamBerangkat { get; set; } = new(7, 0, 0);

    [Column("status")]
    [Display(Name = "Status")]
    public string Status { get; set; } = StatusAktif;

    public Rute? Rute { get; set; }
    public Armada? Armada { get; set; }
}
