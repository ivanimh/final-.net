-- =====================================================================
-- 01_tabel_master_anggota1.sql            PEMILIK: ANGGOTA 1
-- Database + tabel master: armada, rute, jadwal, admin
-- Jalankan PERTAMA (urutan: 01 -> 02 -> 03 -> 04 -> 05 -> 06)
-- =====================================================================

-- PERHATIAN: baris DROP di bawah menghapus database db_shuttle_tiket beserta
-- seluruh isinya (berguna untuk reset ulang data dummy).
DROP DATABASE IF EXISTS db_shuttle_tiket;
CREATE DATABASE db_shuttle_tiket CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE db_shuttle_tiket;

-- Armada / kendaraan shuttle
CREATE TABLE armada (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    kode_armada  VARCHAR(10)  NOT NULL UNIQUE,
    nama_armada  VARCHAR(100) NOT NULL,          -- contoh: Toyota HiAce Premio
    plat_nomor   VARCHAR(15)  NOT NULL UNIQUE,
    kapasitas    INT          NOT NULL DEFAULT 10,
    CONSTRAINT chk_kapasitas CHECK (kapasitas BETWEEN 1 AND 50)
) ENGINE=InnoDB;

-- Rute antarkota (harga tiket per kursi ditaruh di rute)
CREATE TABLE rute (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    kota_asal       VARCHAR(50)   NOT NULL,
    kota_tujuan     VARCHAR(50)   NOT NULL,
    harga           DECIMAL(12,2) NOT NULL,
    estimasi_menit  INT           NOT NULL DEFAULT 180,
    CONSTRAINT uq_rute UNIQUE (kota_asal, kota_tujuan)
) ENGINE=InnoDB;

-- Jadwal keberangkatan: 1 jadwal = 1 rute + 1 armada + tanggal + jam
CREATE TABLE jadwal (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    rute_id        INT  NOT NULL,
    armada_id      INT  NOT NULL,
    tanggal        DATE NOT NULL,
    jam_berangkat  TIME NOT NULL,
    status         ENUM('AKTIF','BATAL') NOT NULL DEFAULT 'AKTIF',
    CONSTRAINT fk_jadwal_rute   FOREIGN KEY (rute_id)   REFERENCES rute(id),
    CONSTRAINT fk_jadwal_armada FOREIGN KEY (armada_id) REFERENCES armada(id),
    CONSTRAINT uq_jadwal_armada UNIQUE (armada_id, tanggal, jam_berangkat)
) ENGINE=InnoDB;

-- Akun admin untuk login halaman operasional.
-- Password TIDAK disimpan polos: kolom password_hash berisi hash dari
-- PasswordHasher bawaan ASP.NET Core (PBKDF2 + salt).
CREATE TABLE admin (
    id             INT AUTO_INCREMENT PRIMARY KEY,
    username       VARCHAR(50)  NOT NULL UNIQUE,
    password_hash  VARCHAR(255) NOT NULL,
    nama_lengkap   VARCHAR(100) NOT NULL
) ENGINE=InnoDB;
