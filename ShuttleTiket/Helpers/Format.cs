using System.Globalization;

namespace ShuttleTiket.Helpers;

public static class Format
{
    private static readonly CultureInfo Id = new("id-ID");

    public static string Rupiah(decimal nilai) => "Rp " + nilai.ToString("N0", Id);

    public static string Tanggal(DateTime tgl) => tgl.ToString("dddd, dd MMMM yyyy", Id);

    public static string TanggalPendek(DateTime tgl) => tgl.ToString("dd MMM yyyy", Id);

    public static string Jam(TimeSpan jam) => jam.ToString(@"hh\:mm");
}
