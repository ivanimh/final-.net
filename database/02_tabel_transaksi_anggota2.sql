USE db_shuttle_tiket;

CREATE TABLE pemesanan (
    id                 INT AUTO_INCREMENT PRIMARY KEY,
    kode_booking       VARCHAR(12)   NOT NULL UNIQUE,      
    jadwal_id          INT           NOT NULL,
    nama_penumpang     VARCHAR(100)  NOT NULL,
    no_hp              VARCHAR(20)   NOT NULL,
    email              VARCHAR(100)  NULL,
    jumlah_tiket       INT           NOT NULL,
    total_harga        DECIMAL(12,2) NOT NULL,
    status_pembayaran  ENUM('MENUNGGU','LUNAS','BATAL') NOT NULL DEFAULT 'MENUNGGU',
    metode_pembayaran  VARCHAR(20)   NULL,                 
    tanggal_pesan      DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    tanggal_bayar      DATETIME      NULL,
    CONSTRAINT fk_pemesanan_jadwal FOREIGN KEY (jadwal_id) REFERENCES jadwal(id)
) ENGINE=InnoDB;

CREATE TABLE pemesanan_kursi (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    pemesanan_id  INT NOT NULL,
    nomor_kursi   INT NOT NULL,
    CONSTRAINT fk_kursi_pemesanan FOREIGN KEY (pemesanan_id)
        REFERENCES pemesanan(id) ON DELETE CASCADE,
    CONSTRAINT uq_kursi_per_pemesanan UNIQUE (pemesanan_id, nomor_kursi)
) ENGINE=InnoDB;
