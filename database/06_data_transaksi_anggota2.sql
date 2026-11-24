USE db_shuttle_tiket;

INSERT INTO pemesanan (kode_booking, jadwal_id, nama_penumpang, no_hp, email,
                       jumlah_tiket, total_harga, status_pembayaran,
                       metode_pembayaran, tanggal_pesan, tanggal_bayar) VALUES
('SHTA1B2C3D4', 1, 'Budi Santoso',   '081234567890', 'budi@mail.com',  2, 240000, 'LUNAS',    'TRANSFER', NOW() - INTERVAL 2 DAY, NOW() - INTERVAL 2 DAY),
('SHTE5F6G7H8', 1, 'Siti Aminah',    '081298765432', NULL,             1, 120000, 'LUNAS',    'QRIS',     NOW() - INTERVAL 1 DAY, NOW() - INTERVAL 1 DAY),
('SHTJ9K1L2M3', 3, 'Andi Wijaya',    '085711112222', 'andi@mail.com',  3, 300000, 'LUNAS',    'TUNAI',    NOW() - INTERVAL 1 DAY, NOW() - INTERVAL 1 DAY),
('SHTN4P5Q6R7', 4, 'Dewi Lestari',   '082233334444', NULL,             2, 240000, 'MENUNGGU', NULL,       NOW(),                  NULL),
('SHTS8T9U1V2', 2, 'Rizky Pratama',  '087755556666', NULL,             1, 120000, 'BATAL',    NULL,       NOW() - INTERVAL 1 DAY, NULL);

INSERT INTO pemesanan_kursi (pemesanan_id, nomor_kursi) VALUES
(1, 1), (1, 2),
(2, 5),
(3, 1), (3, 2), (3, 3),
(4, 4), (4, 5),
(5, 7);
