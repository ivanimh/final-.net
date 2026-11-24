using System.ComponentModel.DataAnnotations.Schema;

namespace ShuttleTiket.Models;

[Table("admin")]
public class Admin
{
    [Column("id")]            public int Id { get; set; }
    [Column("username")]      public string Username { get; set; } = string.Empty;
    [Column("password_hash")] public string PasswordHash { get; set; } = string.Empty;
    [Column("nama_lengkap")]  public string NamaLengkap { get; set; } = string.Empty;
}
