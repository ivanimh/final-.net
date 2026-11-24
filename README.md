# ShuttleKu - Sistem Pemesanan Tiket Shuttle Antarkota

Tugas akhir mata kuliah Pemrograman Framework .NET.

Aplikasi web sederhana untuk pesan tiket shuttle antarkota. Penumpang bisa cari jadwal, pilih kursi, bayar, dan dapat e-ticket. Admin bisa kelola armada, rute, jadwal, dan lihat laporan penjualan.

## Anggota Kelompok

1. Ivan Miracle Halim - 2481051 
2. Sky Bradley Pantouw - 2481018 

## Tools yang dipakai

- Visual Studio Community 2026
- ASP.NET Core MVC (.NET 10)
- MySQL / MariaDB (XAMPP)
- Pomelo.EntityFrameworkCore.MySql 9.0.0
- Bootstrap 5

## Fitur

Penumpang:
- Cari jadwal berdasarkan kota asal, tujuan, dan tanggal
- Pesan tiket dan pilih kursi
- Bayar (masih simulasi) dan cek status booking
- Lihat dan cetak e-ticket

Admin (harus login):
- CRUD Armada, Rute, dan Jadwal
- Lihat daftar pemesanan, konfirmasi atau batalkan
- Laporan penjualan per rute

## Database

Script SQL ada di folder `database/`. Ada 3 SQL View yang dipakai di aplikasi:

- `vw_jadwal_shuttle` : jadwal + rute + armada + sisa kursi (dipakai di halaman cari jadwal dan jadwal admin)
- `vw_detail_pemesanan` : detail booking + daftar kursi (dipakai di detail booking, cek booking, e-ticket)
- `vw_rekap_penjualan_rute` : total tiket terjual dan pendapatan per rute (dipakai di laporan)

## Cara Menjalankan

1. Nyalakan Apache dan MySQL di XAMPP.
2. Buka phpMyAdmin, lalu import file di folder `database/` sesuai urutan nomornya (01 sampai 06).
3. Buka `ShuttleTiket.slnx` di Visual Studio.
4. Cek connection string di `appsettings.json` (default: user `root`, tanpa password).
5. Tekan F5.

## Akun Admin

- Username: `admin`
- Password: `admin123`

## Pembagian Tugas

| Anggota 1 | Anggota 2 |
|---|---|
| Setup project | Tabel pemesanan |
| Tabel armada, rute, jadwal, admin | View vw_detail_pemesanan |
| View vw_jadwal_shuttle dan vw_rekap_penjualan_rute | Beranda dan cari jadwal |
| Login admin | Pemesanan dan pilih kursi |
| CRUD Armada, Rute, Jadwal | Pembayaran dan e-ticket |
| Laporan penjualan | Daftar pemesanan (admin) dan README |
