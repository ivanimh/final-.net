-- =====================================================================
-- 05_data_master_anggota1.sql             PEMILIK: ANGGOTA 1
-- Data dummy armada, rute, jadwal, admin.
-- Tanggal jadwal dibuat RELATIF terhadap hari ini (CURDATE()),
-- jadi data selalu "masa depan" kapan pun script dijalankan.
-- =====================================================================
USE db_shuttle_tiket;

INSERT INTO armada (kode_armada, nama_armada, plat_nomor, kapasitas) VALUES
('SHT-01', 'Toyota HiAce Premio',   'D 1234 ABC', 10),
('SHT-02', 'Toyota HiAce Commuter', 'D 5678 DEF', 14),
('SHT-03', 'Isuzu Elf Long',        'B 9012 GHI', 12),
('SHT-04', 'Mercedes-Benz Sprinter', 'B 3456 JKL', 12),
('SHT-05', 'Toyota HiAce Premio',   'D 2468 MNO', 10),
('SHT-06', 'Toyota HiAce Premio',   'B 1357 PQR', 10),
('SHT-07', 'Isuzu Elf Long',        'D 8642 STU', 12),
('SHT-08', 'Toyota HiAce Commuter', 'B 9753 VWX', 14),
('SHT-09', 'Mercedes-Benz Sprinter', 'F 1122 YZA', 12);

INSERT INTO rute (kota_asal, kota_tujuan, harga, estimasi_menit) VALUES
('Bandung', 'Jakarta',  120000, 180),   -- id 1
('Jakarta', 'Bandung',  120000, 180),   -- id 2
('Bandung', 'Bekasi',   100000, 150),   -- id 3
('Bekasi',  'Bandung',  100000, 150),   -- id 4
('Bandung', 'Bogor',    110000, 200),   -- id 5
('Bogor',   'Bandung',  110000, 200);   -- id 6

INSERT INTO jadwal (rute_id, armada_id, tanggal, jam_berangkat) VALUES
-- Hari ini + 1
(1, 1, CURDATE() + INTERVAL 1 DAY, '06:00:00'),   -- id 1
(1, 2, CURDATE() + INTERVAL 1 DAY, '09:00:00'),   -- id 2
(3, 3, CURDATE() + INTERVAL 1 DAY, '07:00:00'),   -- id 3
(2, 4, CURDATE() + INTERVAL 1 DAY, '13:00:00'),   -- id 4
-- Hari ini + 2
(1, 1, CURDATE() + INTERVAL 2 DAY, '06:00:00'),   -- id 5
(3, 3, CURDATE() + INTERVAL 2 DAY, '07:00:00'),   -- id 6
(4, 2, CURDATE() + INTERVAL 2 DAY, '15:00:00'),   -- id 7
(5, 4, CURDATE() + INTERVAL 2 DAY, '08:00:00');   -- id 8

-- ---------------------------------------------------------------------
-- Jadwal rutin harian, 7 hari (hari ini s/d +6), semua rute DUA ARAH.
-- Dibuat otomatis: daftar "hari" x daftar "slot" (rute + armada + jam).
-- Satu armada berangkat pagi lalu kembali siang/sore ke kota asal.
-- Rute & armada dicari lewat nama kota / kode armada, bukan id angka.
-- ---------------------------------------------------------------------
INSERT INTO jadwal (rute_id, armada_id, tanggal, jam_berangkat)
SELECT r.id, a.id, CURDATE() + INTERVAL h.n DAY, s.jam
FROM (SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3
      UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6) h
CROSS JOIN (
    SELECT 'Bandung' AS asal, 'Jakarta' AS tujuan, 'SHT-05' AS armada, '08:00:00' AS jam UNION ALL
    SELECT 'Jakarta', 'Bandung', 'SHT-05', '14:00:00' UNION ALL
    SELECT 'Jakarta', 'Bandung', 'SHT-06', '07:00:00' UNION ALL
    SELECT 'Bandung', 'Jakarta', 'SHT-06', '13:00:00' UNION ALL
    SELECT 'Jakarta', 'Bandung', 'SHT-08', '18:00:00' UNION ALL
    SELECT 'Bandung', 'Bekasi',  'SHT-07', '10:00:00' UNION ALL
    SELECT 'Bekasi',  'Bandung', 'SHT-07', '16:00:00' UNION ALL
    SELECT 'Bekasi',  'Bandung', 'SHT-08', '07:00:00' UNION ALL
    SELECT 'Bandung', 'Bekasi',  'SHT-08', '12:00:00' UNION ALL
    SELECT 'Bandung', 'Bogor',   'SHT-09', '09:00:00' UNION ALL
    SELECT 'Bogor',   'Bandung', 'SHT-09', '15:00:00'
) s
JOIN rute   r ON r.kota_asal = s.asal AND r.kota_tujuan = s.tujuan
JOIN armada a ON a.kode_armada = s.armada
ORDER BY h.n, s.jam;

-- Akun admin contoh untuk demo -> username: admin | password: admin123
-- (hash dibuat dengan PasswordHasher ASP.NET Core; ganti password ini
--  kalau aplikasi dipakai di luar keperluan tugas)
INSERT INTO admin (username, password_hash, nama_lengkap) VALUES
('admin', 'AQAAAAIAAYagAAAAEM8JAEFEWS1kfMLsncKc3W9x19r4/6fNQftrt2ZJTgRtDuk05QVbEoe2OwgRVdToFw==', 'Admin Operasional');
