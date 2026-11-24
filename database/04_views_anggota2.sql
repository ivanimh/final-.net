USE db_shuttle_tiket;

CREATE OR REPLACE VIEW vw_detail_pemesanan AS
SELECT
    p.id               AS pemesanan_id,
    p.kode_booking,
    p.nama_penumpang,
    p.no_hp,
    p.email,
    p.jumlah_tiket,
    p.total_harga,
    p.status_pembayaran,
    p.metode_pembayaran,
    p.tanggal_pesan,
    p.tanggal_bayar,
    j.id               AS jadwal_id,
    j.tanggal,
    j.jam_berangkat,
    r.kota_asal,
    r.kota_tujuan,
    r.harga,
    a.nama_armada,
    a.plat_nomor,
    (SELECT GROUP_CONCAT(pk.nomor_kursi ORDER BY pk.nomor_kursi SEPARATOR ', ')
       FROM pemesanan_kursi pk
      WHERE pk.pemesanan_id = p.id)  AS daftar_kursi
FROM pemesanan p
JOIN jadwal j ON j.id = p.jadwal_id
JOIN rute   r ON r.id = j.rute_id
JOIN armada a ON a.id = j.armada_id;
