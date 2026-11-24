# ShuttleKu: Sistem Pemesanan Tiket Shuttle Antarkota

Proyek mata kuliah **Pemrograman Framework .NET**, dikerjakan oleh tim 2 orang.

| Komponen | Pilihan |
|---|---|
| IDE | Visual Studio Community 2026 |
| Framework | ASP.NET Core MVC, **.NET 10** |
| Database | MySQL / MariaDB (XAMPP: MariaDB 10.4) |
| ORM / Driver | `Pomelo.EntityFrameworkCore.MySql` **9.0.0** + `MySqlConnector` **2.6.2** |
| UI | Bootstrap 5 bawaan template + `site.css` sendiri (tanpa ikon / library tambahan) |

---

## 1. Konsep Arsitektur

```
 Browser (Razor View + Bootstrap)
        │  HTTP
        ▼
 Controller (ASP.NET Core MVC) ── ViewModel ──► View (.cshtml)
        │
        ▼
 AppDbContext (EF Core + Pomelo)   ◄── dipecah jadi 3 file partial
        │                                 (bersama / Anggota 1 / Anggota 2)
        ▼
 MySQL / MariaDB
   ├─ Tabel : armada, rute, jadwal, pemesanan, pemesanan_kursi
   └─ VIEW  : vw_jadwal_shuttle, vw_detail_pemesanan, vw_rekap_penjualan_rute
```

Prinsipnya sederhana:
- **Tulis data** (insert/update/delete) lewat **tabel** (`DbSet<Armada>`, `DbSet<Pemesanan>`, ...).
- **Baca data gabungan** (join, hitung sisa kursi, rekap) lewat **SQL VIEW** yang dipetakan sebagai entity *keyless* (`HasNoKey().ToView(...)`).
- Database dibuat dari **script SQL** (database-first), tanpa EF Migrations, karena VIEW memang ditulis di SQL.

### Alur fitur

**Penumpang:** Beranda → Cari jadwal (rute + tanggal) → Pilih jadwal → Isi data + pilih kursi → Kode booking dibuat (status `MENUNGGU`) → Bayar (simulasi: Transfer/QRIS/Tunai) → `LUNAS` → Lihat / cetak e-ticket. Booking bisa dicek lagi lewat menu **Cek Booking**.

**Admin:** Login (`/Akun/Login`) → Kelola Armada, Rute, Jadwal (CRUD) → Daftar Pemesanan (konfirmasi lunas / batalkan) → Laporan penjualan per rute.

### Pemakaian 3 SQL View

| SQL View | Isi | Dipakai di |
|---|---|---|
| `vw_jadwal_shuttle` | jadwal + rute asal-tujuan + harga + armada + **kursi terisi & sisa kursi** | `JadwalController.Index` (admin), `PencarianController.Index` (cari jadwal), `PemesananController.Pesan` (ringkasan & validasi) |
| `vw_detail_pemesanan` | pemesanan + penumpang + kode booking + jadwal + rute + armada + **daftar kursi** (`GROUP_CONCAT`) | `PemesananController`: `Detail`, `Cek`, `ETiket`, `Index` (daftar admin) |
| `vw_rekap_penjualan_rute` | per rute: jumlah jadwal, transaksi lunas, **tiket terjual, total pendapatan** | `LaporanController.Index` |

Cara memetakan (lihat `Data/AppDbContext.Anggota1.cs`):

```csharp
modelBuilder.Entity<VwJadwalShuttle>()
    .HasNoKey()
    .ToView("vw_jadwal_shuttle");
```

Cara memanggil (sama seperti tabel biasa, tapi read-only):

```csharp
var hasil = await _db.VwJadwalShuttle
    .Where(j => j.KotaAsal == "Bandung" && j.Tanggal == tanggal)
    .OrderBy(j => j.JamBerangkat)
    .ToListAsync();
```

---

## 2. Pembagian Tugas

Pembagian dibuat **per modul**: tiap file hanya punya **satu pemilik**, jadi saat `git push` / merge hampir tidak mungkin terjadi conflict.

| | **Anggota 1: Operasional / Admin** | **Anggota 2: Pemesanan / Penumpang** |
|---|---|---|
| Fokus | Data master & laporan | Transaksi & tampilan penumpang |
| Tabel SQL | `armada`, `rute`, `jadwal`, `admin` | `pemesanan`, `pemesanan_kursi` |
| SQL View | `vw_jadwal_shuttle`, `vw_rekap_penjualan_rute` | `vw_detail_pemesanan` |
| Fitur | Login admin, CRUD Armada, CRUD Rute, CRUD Jadwal, Laporan Penjualan | Beranda, Cari Jadwal, Pesan + Pilih Kursi, Pembayaran, Cek Booking, E-Ticket, Daftar Pemesanan (admin) |
| Bobot | 5 controller (4 CRUD sederhana + login) + 2 view SQL + setup awal project | 3 controller dengan logika lebih kompleks (transaksi, kunci kursi) + 1 view SQL + README |

### Kepemilikan file

| File | Pemilik |
|---|---|
| `Program.cs`, `appsettings.json`, `Data/AppDbContext.cs`, `Helpers/Format.cs`, `Views/Shared/_Layout.cshtml`, `Views/_ViewImports.cshtml`, `wwwroot/css/site.css`, `.gitignore` | **Bersama**. Dibuat Anggota 1 saat *setup awal*, setelah itu **dibekukan** |
| `database/01_tabel_master_anggota1.sql`, `03_views_anggota1.sql`, `05_data_master_anggota1.sql` | Anggota 1 |
| `Models/Armada.cs`, `Rute.cs`, `Jadwal.cs`, `VwJadwalShuttle.cs`, `VwRekapPenjualanRute.cs` | Anggota 1 |
| `Data/AppDbContext.Anggota1.cs` | Anggota 1 |
| `Controllers/ArmadaController.cs`, `RuteController.cs`, `JadwalController.cs`, `LaporanController.cs` | Anggota 1 |
| `Views/Armada/*`, `Views/Rute/*`, `Views/Jadwal/*`, `Views/Laporan/*` | Anggota 1 |
| `Models/Admin.cs`, `Models/LoginViewModel.cs`, `Controllers/AkunController.cs`, `Views/Akun/*` | Anggota 1 |
| `database/02_tabel_transaksi_anggota2.sql`, `04_views_anggota2.sql`, `06_data_transaksi_anggota2.sql` | Anggota 2 |
| `Models/Pemesanan.cs`, `PemesananKursi.cs`, `VwDetailPemesanan.cs`, `Models/ViewModels/*` | Anggota 2 |
| `Data/AppDbContext.Anggota2.cs` | Anggota 2 |
| `Controllers/HomeController.cs`, `PencarianController.cs`, `PemesananController.cs` | Anggota 2 |
| `Views/Home/Index.cshtml`, `Views/Pencarian/*`, `Views/Pemesanan/*` | Anggota 2 |
| `README.md` | Anggota 2 |

> **Trik anti-conflict:** `AppDbContext` dibuat sebagai `partial class` yang dipecah jadi 3 file. File bersama hanya memanggil `KonfigurasiAnggota1()` dan `KonfigurasiAnggota2()` (*partial method*), dan masing-masing anggota mengisi miliknya sendiri. Menu navbar di `_Layout.cshtml` juga sudah disiapkan lengkap sejak awal, jadi tidak ada yang perlu mengedit layout lagi.

---

## 3. Strategi Git & GitHub

### 3.1 Persiapan (sekali saja, oleh masing-masing anggota)

**Penting agar kontribusi tercatat:** email di `git config` harus **sama** dengan email akun GitHub masing-masing. Kalau beda, commit tidak muncul di grafik *Contributors*.

```powershell
git config --global user.name "Nama Lengkap"
```

```powershell
git config --global user.email "email-akun-github@contoh.com"
```

### 3.2 Setup repository (Anggota 1)

1. Buat repo baru di GitHub (misal `shuttle-tiket-dotnet`), **tanpa** README.
2. *Settings → Collaborators* → undang Anggota 2.
3. *Settings → Branches → Add rule* untuk `main`: centang **Require a pull request before merging** (opsional: *Require approvals = 1*). Jadi tidak ada yang bisa push langsung ke `main`.
4. Commit file **bersama** saja (Tahap 0), lalu push:

```powershell
git init -b main
```

```powershell
git add .gitignore ShuttleTiket.slnx ShuttleTiket/Program.cs ShuttleTiket/appsettings*.json ShuttleTiket/ShuttleTiket.csproj ShuttleTiket/Properties ShuttleTiket/wwwroot ShuttleTiket/Data/AppDbContext.cs ShuttleTiket/Helpers ShuttleTiket/Views/Shared ShuttleTiket/Views/_ViewImports.cshtml ShuttleTiket/Views/_ViewStart.cshtml ShuttleTiket/Models/ErrorViewModel.cs
```

```powershell
git commit -m "chore: setup awal project ASP.NET Core MVC + koneksi MySQL"
```

```powershell
git remote add origin https://github.com/USERNAME/shuttle-tiket-dotnet.git
```

```powershell
git push -u origin main
```

> Catatan: `HomeController.cs` dan folder `Views/Home/` **tidak** ikut di Tahap 0, karena versi finalnya memakai `PencarianController` dan `CariJadwalViewModel` milik Anggota 2. Keduanya di-commit Anggota 2 bersama fitur pencarian (`feature/a2-pencarian-jadwal`). Sampai saat itu, halaman `/` memang masih 404, dan itu normal.

5. Anggota 2: `git clone https://github.com/USERNAME/shuttle-tiket-dotnet.git`

### 3.3 Penamaan branch

Format: `<tipe>/<anggota>-<modul>`

| Anggota 1 | Anggota 2 |
|---|---|
| `feature/a1-database-master` | `feature/a2-database-transaksi` |
| `feature/a1-login-admin` | `feature/a2-pencarian-jadwal` |
| `feature/a1-crud-armada` | `feature/a2-pemesanan` |
| `feature/a1-crud-rute` | `feature/a2-eticket` |
| `feature/a1-crud-jadwal` | `feature/a2-admin-pemesanan` |
| `feature/a1-laporan` | `docs/a2-readme` |

Perbaikan bug: `fix/a1-validasi-jadwal`, `fix/a2-kursi-bentrok`, dst.

### 3.4 Urutan pengerjaan

| Tahap | Anggota 1 | Anggota 2 |
|---|---|---|
| 0 | Setup project + file bersama → push `main` | Clone repo, install XAMPP |
| 1 (paralel) | SQL 01, 03, 05 + Model Armada/Rute/Jadwal/Vw* + `AppDbContext.Anggota1.cs` → **PR & merge duluan** | SQL 02, 04, 06 + Model Pemesanan/Vw* + `AppDbContext.Anggota2.cs` → PR |
| 2 (paralel) | Login admin → CRUD Armada → Rute → Jadwal → Laporan (1 PR per modul) | `git pull` dulu (butuh `VwJadwalShuttle` dari A1), lalu Pencarian → Pemesanan → Pembayaran & E-ticket → Admin Pemesanan |
| 3 | Review PR teman, testing, perbaikan bug | README, screenshot, testing |

### 3.5 Alur kerja harian (diulang untuk setiap fitur)

```powershell
git checkout main
```

```powershell
git pull origin main
```

```powershell
git checkout -b feature/a1-crud-armada
```

Kerjakan fiturnya, lalu commit **kecil-kecil tapi bermakna** (misal: model → controller → view):

```powershell
git add ShuttleTiket/Models/Armada.cs
```

```powershell
git commit -m "feat(armada): tambah model Armada"
```

```powershell
git push -u origin feature/a1-crud-armada
```

Lalu di GitHub: **Compare & pull request** → base `main` → isi deskripsi → minta teman jadi *Reviewer* → setelah di-approve, **Merge pull request** (pilih **Create a merge commit**, jangan *Squash*, supaya setiap commit tetap terhitung) → **Delete branch**.

### 3.6 Format pesan commit

`<tipe>(<modul>): <deskripsi singkat>`

| Tipe | Contoh |
|---|---|
| `feat` | `feat(jadwal): tampilkan sisa kursi dari vw_jadwal_shuttle` |
| `fix` | `fix(pemesanan): cegah kursi dipesan dua kali` |
| `db` | `db(view): tambah vw_rekap_penjualan_rute` |
| `style` | `style(eticket): rapikan tampilan cetak` |
| `docs` | `docs(readme): tambah panduan setup` |
| `chore` | `chore: update package MySqlConnector` |

### 3.7 Tips agar kontribusi terlihat seimbang

- Masing-masing commit **file miliknya sendiri** dari akun sendiri. Jangan ada satu orang yang commit semuanya.
- Targetkan jumlah commit dan PR yang mirip (misal ±15–25 commit per orang).
- Saling **review PR** (komentar review juga tercatat di GitHub).
- Pakai tab **Issues / Projects** untuk daftar tugas; tautkan PR ke issue (`Closes #3`).
- Cek hasilnya di **Insights → Contributors** dan **Insights → Network**.

---

## 4. Setup Database (XAMPP / MySQL)

1. Nyalakan **MySQL** di XAMPP Control Panel.
2. Buka **phpMyAdmin** (`http://localhost/phpmyadmin`) → tab **Import**, lalu impor file di folder `database/` **berurutan**:

| Urutan | File | Pemilik |
|---|---|---|
| 1 | `01_tabel_master_anggota1.sql` | A1 |
| 2 | `02_tabel_transaksi_anggota2.sql` | A2 |
| 3 | `03_views_anggota1.sql` | A1 |
| 4 | `04_views_anggota2.sql` | A2 |
| 5 | `05_data_master_anggota1.sql` | A1 |
| 6 | `06_data_transaksi_anggota2.sql` | A2 |

Atau lewat terminal (PowerShell, dari folder `database`):

```powershell
Get-Content 01_*.sql,02_*.sql,03_*.sql,04_*.sql,05_*.sql,06_*.sql | C:\xampp\mysql\bin\mysql.exe -uroot
```

> ⚠️ File `01` berisi `DROP DATABASE IF EXISTS db_shuttle_tiket;`, yang akan **menghapus** database `db_shuttle_tiket` lalu membuatnya ulang. Ini berguna untuk reset data dummy, tapi jangan dijalankan kalau ada data yang ingin disimpan.

Data dummy jadwal memakai `CURDATE() + INTERVAL n DAY`, jadi tanggalnya selalu dihitung dari hari script dijalankan: ada **jadwal rutin harian selama 7 hari** (hari ini s/d +6) untuk semua rute **dua arah** (Bandung ⇄ Jakarta, Bandung ⇄ Bekasi, Bandung ⇄ Bogor), total 85 jadwal. Jalankan ulang script kalau jadwalnya sudah lewat.

---

## 5. Setup Project di Visual Studio Community 2026

### 5.1 Membuat project dari nol (kalau ingin mengulang sendiri)

1. **Create a new project** → **ASP.NET Core Web App (Model-View-Controller)** → Next.
2. Project name: `ShuttleTiket` → Framework: **.NET 10.0 (Long Term Support)** → Authentication: **None** → centang *Configure for HTTPS* → Create.
3. Buat folder: `Data`, `Helpers`, `Models/ViewModels`.

### 5.2 NuGet packages

*Tools → NuGet Package Manager → Package Manager Console*:

```powershell
Install-Package Pomelo.EntityFrameworkCore.MySql -Version 9.0.0
```

```powershell
Install-Package MySqlConnector -Version 2.6.2
```

Atau dengan .NET CLI:

```powershell
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
```

```powershell
dotnet add package MySqlConnector --version 2.6.2
```

Kenapa versi ini?
- **Pomelo 9.0.0** adalah versi stabil terbaru dan memakai **EF Core 9**, yang tetap berjalan normal di .NET 10.
- `MySqlConnector 2.6.2` dipasang eksplisit karena versi bawaan Pomelo (2.4.0) punya peringatan keamanan (NU1902).
- **Jangan** memasang paket `Microsoft.EntityFrameworkCore.*` versi **10.x** (misal Tools/Design 10), karena akan bentrok dengan Pomelo 9. Proyek ini juga tidak membutuhkan Migrations.

### 5.3 `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=db_shuttle_tiket;User=root;Password=;"
  },
  "Database": {
    "ServerVersion": "10.4.32-mariadb"
  }
}
```

Untuk MySQL 8, ganti `ServerVersion` menjadi `"8.0.36-mysql"` (sesuaikan dengan versi Anda; cek lewat `SELECT VERSION();`).

### 5.4 `Program.cs` (bagian penting)

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var serverVersion = ServerVersion.Parse(builder.Configuration["Database:ServerVersion"] ?? "10.4.32-mariadb");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));
```

Versi server sengaja ditulis manual (bukan `ServerVersion.AutoDetect`) supaya aplikasi tetap bisa start walaupun MySQL belum menyala.

### 5.5 Struktur folder

```
shuttle-tiket-dotnet/
├─ README.md
├─ ShuttleTiket.slnx
├─ database/                      ← script SQL (01–06)
└─ ShuttleTiket/
   ├─ Program.cs
   ├─ appsettings.json
   ├─ Controllers/
   │  ├─ AkunController.cs        (A1: login/logout admin)
   │  ├─ ArmadaController.cs      (A1)
   │  ├─ RuteController.cs        (A1)
   │  ├─ JadwalController.cs      (A1)
   │  ├─ LaporanController.cs     (A1)
   │  ├─ HomeController.cs        (A2)
   │  ├─ PencarianController.cs   (A2)
   │  └─ PemesananController.cs   (A2)
   ├─ Data/
   │  ├─ AppDbContext.cs          (bersama)
   │  ├─ AppDbContext.Anggota1.cs (A1)
   │  └─ AppDbContext.Anggota2.cs (A2)
   ├─ Helpers/Format.cs           (bersama: format Rupiah & tanggal)
   ├─ Models/
   │  ├─ Armada.cs, Rute.cs, Jadwal.cs                    (A1)
   │  ├─ Admin.cs, LoginViewModel.cs                      (A1)
   │  ├─ VwJadwalShuttle.cs, VwRekapPenjualanRute.cs      (A1)
   │  ├─ Pemesanan.cs, PemesananKursi.cs, VwDetailPemesanan.cs (A2)
   │  └─ ViewModels/CariJadwalViewModel.cs, PesanTiketViewModel.cs (A2)
   └─ Views/
      ├─ Akun/, Armada/, Rute/, Jadwal/, Laporan/          (A1)
      ├─ Home/, Pencarian/, Pemesanan/                     (A2)
      └─ Shared/_Layout.cshtml                             (bersama)
```

### 5.6 Menjalankan

Buka `ShuttleTiket.slnx` → tekan **F5**. Halaman yang tersedia:

| URL | Halaman |
|---|---|
| `/` | Beranda + form cari |
| `/Pencarian` | Hasil cari jadwal |
| `/Pemesanan/Pesan/{jadwalId}` | Form pesan + pilih kursi |
| `/Pemesanan/Detail?kode=...` | Status & pembayaran |
| `/Pemesanan/Cek` | Cek booking |
| `/Pemesanan/ETiket?kode=...` | E-ticket (cetak / simpan PDF) |
| `/Akun/Login` | Login admin |
| `/Armada`, `/Rute`, `/Jadwal` | Admin: data master (wajib login) |
| `/Pemesanan` | Admin: daftar pemesanan (wajib login) |
| `/Laporan` | Admin: laporan penjualan (wajib login) |

Contoh kode booking dari data dummy: `SHTA1B2C3D4` (LUNAS) dan `SHTN4P5Q6R7` (MENUNGGU).

### 5.7 Login admin

Akun demo dari `05_data_master_anggota1.sql`: **username `admin`, password `admin123`**.

- Memakai **Cookie Authentication** bawaan ASP.NET Core (didaftarkan di `Program.cs`), tanpa paket tambahan.
- Password disimpan sebagai **hash** (PBKDF2 + salt dari `PasswordHasher` ASP.NET Core) di kolom `admin.password_hash`, bukan teks asli.
- Halaman admin diberi `[Authorize]`: `ArmadaController`, `RuteController`, `JadwalController`, `LaporanController` (seluruh controller), dan aksi `Index`, `Konfirmasi`, `Batalkan` di `PemesananController`. Yang belum login otomatis diarahkan ke `/Akun/Login`, lalu kembali ke halaman semula setelah login.
- Halaman penumpang (cari, pesan, bayar, cek booking, e-ticket) tetap bisa dibuka **tanpa login**: penumpang cukup memakai kode booking.
- Menu admin di navbar hanya muncul setelah login.

---

## 6. Catatan & Pengembangan Lanjutan (opsional)

- **Akun penumpang** sengaja tidak dibuat; penumpang memakai kode booking. Kalau dosen mensyaratkan multi-role, bisa ditambah tabel `penumpang` + halaman "pesanan saya" dengan pola login yang sama.
- **Ganti password admin**: buat hash baru dengan `new PasswordHasher<Admin>().HashPassword(null!, "password-baru")`, lalu `UPDATE admin SET password_hash = '...' WHERE username = 'admin';`.
- **Pembayaran** masih simulasi (tombol Bayar langsung `LUNAS`). Bisa dikembangkan dengan upload bukti transfer yang diverifikasi admin.
- **Kedaluwarsa booking**: pesanan `MENUNGGU` belum otomatis batal; admin bisa membatalkan manual di Daftar Pemesanan.
- **Kursi bentrok** sudah dicegah: pemesanan dijalankan dalam transaksi dengan `SELECT ... FOR UPDATE` pada baris jadwal, lalu kursi dicek ulang sebelum disimpan.
