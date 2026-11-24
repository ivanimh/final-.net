-- =====================================================================
-- 03_views_anggota1.sql                   PEMILIK: ANGGOTA 1
-- VIEW 1: vw_jadwal_shuttle        -> dipakai JadwalController (admin)
--                                     & PencarianController (penumpang)
-- VIEW 3: vw_rekap_penjualan_rute  -> dipakai LaporanController
-- =====================================================================
USE db_shuttle_tiket;

-- ---------------------------------------------------------------------
-- VIEW 1: jadwal + rute + harga + armada + kursi terisi + sisa kursi
-- Kursi dihitung dari pemesanan yang statusnya BUKAN 'BATAL'.
-- (Pakai subquery di SELECT agar kompatibel MySQL 5.7+/MariaDB 10.x)
-- ---------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_jadwal_shuttle AS
SELECT
    j.id             AS jadwal_id,
    j.tanggal,
    j.jam_berangkat,
    j.status         AS status_jadwal,
    r.id             AS rute_id,
    r.kota_asal,
    r.kota_tujuan,
    r.harga,
    r.estimasi_menit,
    a.id             AS armada_id,
    a.nama_armada,
    a.plat_nomor,
    a.kapasitas,
    (SELECT COUNT(*)
       FROM pemesanan_kursi pk
       JOIN pemesanan p ON p.id = pk.pemesanan_id
      WHERE p.jadwal_id = j.id
        AND p.status_pembayaran <> 'BATAL')                AS kursi_terisi,
    a.kapasitas - (SELECT COUNT(*)
       FROM pemesanan_kursi pk
       JOIN pemesanan p ON p.id = pk.pemesanan_id
      WHERE p.jadwal_id = j.id
        AND p.status_pembayaran <> 'BATAL')                AS sisa_kursi
FROM jadwal j
JOIN rute   r ON r.id = j.rute_id
JOIN armada a ON a.id = j.armada_id;

-- ---------------------------------------------------------------------
-- VIEW 3: rekap tiket terjual & pendapatan per rute (hanya LUNAS)
-- ---------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_rekap_penjualan_rute AS
SELECT
    r.id                                   AS rute_id,
    r.kota_asal,
    r.kota_tujuan,
    COUNT(DISTINCT j.id)                   AS jumlah_jadwal,
    COUNT(p.id)                            AS jumlah_transaksi,
    -- SUM(INT) di MySQL menghasilkan DECIMAL, di-CAST agar cocok ke 'long' di C#
    CAST(COALESCE(SUM(p.jumlah_tiket), 0) AS SIGNED) AS tiket_terjual,
    COALESCE(SUM(p.total_harga), 0)        AS total_pendapatan
FROM rute r
LEFT JOIN jadwal j    ON j.rute_id = r.id
LEFT JOIN pemesanan p ON p.jadwal_id = j.id
                     AND p.status_pembayaran = 'LUNAS'
GROUP BY r.id, r.kota_asal, r.kota_tujuan;
