CREATE DATABASE CSharp_BookShop;
USE CSharp_BookShop;

-- 1. NguoiDung
CREATE TABLE NguoiDung (
    MaNguoiDung VARCHAR(10) PRIMARY KEY,
    HoTen VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    MatKhau VARCHAR(128) NOT NULL,
    SoDienThoai VARCHAR(15),
    DiaChi VARCHAR(255),
    VaiTro VARCHAR(20) DEFAULT 'KhachHang',
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 2. TheLoai
CREATE TABLE TheLoai (
    MaTheLoai VARCHAR(10) PRIMARY KEY,
    TenTheLoai VARCHAR(50) NOT NULL UNIQUE
);

-- 3. Sach
CREATE TABLE Sach (
    MaSach VARCHAR(10) PRIMARY KEY,
    TenSach VARCHAR(200) NOT NULL,
    TacGia VARCHAR(100) NOT NULL,
    NhaXuatBan VARCHAR(100),
    NamXuatBan INT,
    MaTheLoai VARCHAR(10),
    Gia DECIMAL(10,2) NOT NULL CHECK (Gia >= 0),
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0),
    SoLuongDaBan INT DEFAULT 0 CHECK (SoLuongDaBan >= 0),
    MoTa TEXT,
    URLAnhBia VARCHAR(255),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaTheLoai) REFERENCES TheLoai(MaTheLoai) ON DELETE CASCADE
);


-- 4. DonHang
CREATE TABLE DonHang (
    MaDonHang VARCHAR(10) PRIMARY KEY,
    MaNguoiDung VARCHAR(10),
    TongTien DECIMAL(10,2) NOT NULL CHECK (TongTien >= 0),
    TrangThaiDonHang VARCHAR(20) DEFAULT 'DangXuLy',
    DiaChiGiao VARCHAR(255),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung) ON DELETE CASCADE
);

-- 5. ChiTietDonHang
CREATE TABLE ChiTietDonHang (
    MaChiTiet INT PRIMARY KEY AUTO_INCREMENT,
    MaDonHang VARCHAR(10),
    MaSach VARCHAR(10),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    GiaBan DECIMAL(10,2) NOT NULL CHECK (GiaBan >= 0),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach) ON DELETE CASCADE
);

-- 6. DanhGia
CREATE TABLE DanhGia (
    MaDanhGia INT PRIMARY KEY AUTO_INCREMENT,
    MaSach VARCHAR(10),
    MaNguoiDung VARCHAR(10),
    DiemDanhGia INT CHECK (DiemDanhGia BETWEEN 1 AND 5),
    BinhLuan TEXT,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (MaSach, MaNguoiDung),
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach) ON DELETE CASCADE,
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung) ON DELETE CASCADE
);

-- 7. DanhSachYeuThich
CREATE TABLE DanhSachYeuThich (
    MaYeuThich INT PRIMARY KEY AUTO_INCREMENT,
    MaNguoiDung VARCHAR(10),
    MaSach VARCHAR(10),
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (MaNguoiDung, MaSach),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung) ON DELETE CASCADE,
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach) ON DELETE CASCADE
);

-- 8. MaGiamGia
CREATE TABLE MaGiamGia (
    MaGiamGia VARCHAR(10) PRIMARY KEY,
    MaVoucher VARCHAR(20) UNIQUE NOT NULL,
    PhanTramGiam DECIMAL(5,2) CHECK (PhanTramGiam BETWEEN 1 AND 100),
    NgayHetHan DATETIME NOT NULL,
    NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 9. ApDungGiamGia
CREATE TABLE ApDungGiamGia (
    MaApDung INT PRIMARY KEY AUTO_INCREMENT,
    MaDonHang VARCHAR(10),
    MaGiamGia VARCHAR(10),
    UNIQUE (MaDonHang, MaGiamGia),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    FOREIGN KEY (MaGiamGia) REFERENCES MaGiamGia(MaGiamGia)
);

-- 10. LienHe
CREATE TABLE LienHe (
    MaLienHe INT PRIMARY KEY AUTO_INCREMENT,
    MaNguoiDung VARCHAR(10) NULL,
    HoTen VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    TieuDe VARCHAR(200) NOT NULL,
    NoiDung TEXT NOT NULL,
    TrangThai VARCHAR(20) DEFAULT 'ChuaXuLy',
    PhanHoiAdmin TEXT NULL,
    NgayGui DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayPhanHoi DATETIME NULL,

    FOREIGN KEY (MaNguoiDung)
        REFERENCES NguoiDung(MaNguoiDung)
        ON DELETE SET NULL
);

SELECT * FROM LienHe;

INSERT INTO NguoiDung (MaNguoiDung, HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTro, NgayTao)
VALUES 
('ND001', 'Nguyen Van A', 'a@gmail.com', 'password1', '0912345678', 'Ha Noi', 'KhachHang', NOW()),
('ND002', 'Tran Thi B', 'b@gmail.com', 'password2', '0912345679', 'Da Nang', 'KhachHang', NOW()),
('ND003', 'Pham Van C', 'c@gmail.com', 'password3', '0912345680', 'Tp. Ho Chi Minh', 'KhachHang', NOW()),
('ND004', 'Le Thi D', 'd@gmail.com', 'password4', '0912345681', 'Hai Phong', 'Admin', NOW()),
('ND005', 'Hoang Van E', 'e@gmail.com', 'password5', '0912345682', 'Can Tho', 'KhachHang', NOW());

INSERT INTO TheLoai (MaTheLoai, TenTheLoai)
VALUES 
('TL001', 'Tiểu Thuyết'),
('TL002', 'Khoa Học'),
('TL003', 'Tâm Lý - Kỹ Năng Sống'),
('TL004', 'Thiếu Nhi'),
('TL005', 'Lịch Sử'),
('TL006', 'Kinh Tế'),
('TL007', 'Triết Học'),
('TL008', 'Văn Học Việt Nam'),
('TL009', 'Tiếng Anh'),
('TL010', 'Chính Trị'),
('TL011', 'Tâm Linh'),
('TL012', 'Âm Nhạc'),
('TL013', 'Phim - Điện Ảnh'),
('TL014', 'Tuổi Trẻ'),
('TL015', 'Thể Thao'),
('TL016', 'Sách Hướng Dẫn'),
('TL017', 'Ma Quái - Kinh Dị'),
('TL018', 'Kỹ Thuật Công Nghệ'),
('TL019', 'Văn Học Nước Ngoài'),
('TL020', 'Sách Học Ngoại Ngữ'),
('TL021', 'Ẩm Thực'),
('TL022', 'Sức Khỏe'),
('TL023', 'Phát Triển Bản Thân'),
('TL024', 'Môi Trường - Thiên Nhiên'),
('TL025', 'Sách Pháp Luật'),
('TL026', 'Ngữ Pháp - Luyện Thi'),
('TL027', 'Văn Hóa - Xã Hội'),
('TL028', 'Tiểu Sử - Hồi Ký'),
('TL029', 'Hoa Kỳ - Châu Âu'),
('TL030', 'Khoa Học Xã Hội');

INSERT INTO Sach (MaSach, TenSach, TacGia, NhaXuatBan, NamXuatBan, MaTheLoai, Gia, SoLuongTon, SoLuongDaBan, MoTa, URLAnhBia)
VALUES 
('S001', 'Nhà Giả Kim', 'Paulo Coelho', 'Nhã Nam', 2021, 'TL001', 85000, 50, 0, 'Câu chuyện về hành trình khám phá bản thân.', 'nhaGiaKim.jpg'),
('S002', 'Lược Sử Thời Gian', 'Stephen Hawking', 'Tre', 2020, 'TL002', 120000, 30, 0, 'Khám phá bí ẩn của vũ trụ.', 'luocSuThoiGian.jpg'),
('S003', 'Đắc Nhân Tâm', 'Dale Carnegie', 'Tre', 2019, 'TL003', 100000, 40, 0, 'Cẩm nang giao tiếp và quản lý mối quan hệ.', 'dacNhanTam.jpg'),
('S004', 'Dế Mèn Phiêu Lưu Ký', 'Tô Hoài', 'Kim Đồng', 2022, 'TL004', 50000, 60, 0, 'Câu chuyện thiếu nhi nổi tiếng của Việt Nam.', 'deMenPhieuLuuKy.jpg'),
('S005', 'Lịch Sử Việt Nam', 'Nguyen Van Y', 'Giáo Dục', 2023, 'TL005', 150000, 20, 0, 'Tài liệu về lịch sử dân tộc Việt Nam.', 'lichSuVietNam.jpg'),
('S006', 'Thiên Đường Mùa Hè', 'John Green', 'Nhà Xuất Bản Văn Học', 2021, 'TL001', 95000, 50, 0, 'Một câu chuyện tình yêu đầy cảm xúc.', 'thienDuongMuaHe.jpg'),
('S007', 'Đi Tìm Lẽ Sống', 'Viktor Frankl', 'Đà Nẵng', 2020, 'TL003', 130000, 40, 0, 'Cuốn sách giúp tìm lại ý nghĩa cuộc sống.', 'diTimLeSong.jpg'),
('S008', 'Khoa Học Về Tương Lai', 'Carl Sagan', 'Nhà Xuất Bản Trẻ', 2018, 'TL002', 110000, 60, 0, 'Khám phá tương lai của khoa học và công nghệ.', 'khoaHocVeTuongLai.jpg'),
('S009', 'Chìa Khóa Thành Công', 'Napoleon Hill', 'Nhà Xuất Bản Lao Động', 2017, 'TL003', 80000, 35, 0, 'Cẩm nang phát triển bản thân và thành công.', 'chiaKhoaThanhCong.jpg'),
('S010', 'Vương Quốc Thực Vật', 'Richard Dawkins', 'Tri Thức', 2019, 'TL002', 140000, 25, 0, 'Một nghiên cứu về sự phát triển của thực vật trong tự nhiên.', 'vuongQuocThucVat.jpg'),
('S011', 'Bí Ẩn Vũ Trụ', 'Stephen Hawking', 'Trẻ', 2022, 'TL002', 100000, 30, 0, 'Sự hiểu biết về vũ trụ từ quan điểm khoa học.', 'biAnVuTru.jpg'),
('S012', 'Bản Đồ Tâm Hồn', 'Thich Nhat Hanh', 'Nhà Xuất Bản Hồng Đức', 2020, 'TL003', 95000, 45, 0, 'Một cuốn sách hướng dẫn cách sống tâm linh.', 'banDoTamHon.jpg'),
('S013', 'Cuộc Sống Đúng Nghĩa', 'Mark Manson', 'NXB Thế Giới', 2021, 'TL003', 85000, 50, 0, 'Một cái nhìn thẳng thắn và thực tế về cuộc sống.', 'cuocSongDungNghia.jpg'),
('S014', 'Mật Mã Da Vinci', 'Dan Brown', 'Kim Đồng', 2018, 'TL001', 120000, 40, 0, 'Một cuốn tiểu thuyết về các bí mật lịch sử và tín ngưỡng.', 'matMaDaVinci.jpg'),
('S015', 'Những Điều Kỳ Diệu Của Cuộc Sống', 'Haruki Murakami', 'NXB Văn Học', 2022, 'TL001', 130000, 55, 0, 'Một câu chuyện kể về những kỳ diệu trong cuộc sống hàng ngày.', 'nhungDieuKyDieuCuaCuocSong.jpg');

INSERT INTO DonHang (MaDonHang, MaNguoiDung, TongTien, TrangThaiDonHang, DiaChiGiao, NgayTao)
VALUES 
('DH001', 'ND001', 185000, 'DangXuLy', 'Ha Noi', NOW()),
('DH002', 'ND002', 120000, 'DaGiao', 'Da Nang', NOW()),
('DH003', 'ND003', 150000, 'DaHuy', 'Tp. Ho Chi Minh', NOW()),
('DH004', 'ND001', 50000, 'DangXuLy', 'Hai Phong', NOW()),
('DH005', 'ND005', 100000, 'DangXuLy', 'Can Tho', NOW());

INSERT INTO ChiTietDonHang (MaDonHang, MaSach, SoLuong, GiaBan)
VALUES 
('DH001', 'S001', 2, 150000),
('DH001', 'S002', 3, 120000),
('DH001', 'S003', 1, 200000),
('DH001', 'S004', 5, 85000),
('DH002', 'S005', 4, 170000),
('DH002', 'S006', 2, 140000),
('DH002', 'S007', 3, 95000),
('DH002', 'S008', 6, 100000),
('DH003', 'S009', 1, 220000),
('DH003', 'S010', 3, 130000),
('DH003', 'S001', 2, 180000),
('DH003', 'S002', 4, 110000),
('DH004', 'S003', 5, 160000),
('DH004', 'S004', 2, 125000),
('DH004', 'S005', 6, 90000),
('DH004', 'S006', 3, 110000),
('DH005', 'S007', 2, 250000),
('DH005', 'S008', 4, 95000),
('DH005', 'S009', 1, 300000),
('DH005', 'S006', 5, 105000);

INSERT INTO DanhGia (MaSach, MaNguoiDung, DiemDanhGia, BinhLuan, NgayTao)
VALUES 
('S001', 'ND001', 5, 'Rất hay và ý nghĩa.', NOW()),
('S002', 'ND002', 4, 'Cuốn sách rất thú vị.', NOW()),
('S003', 'ND003', 3, 'Nội dung ổn nhưng trình bày chưa hấp dẫn.', NOW()),
('S004', 'ND004', 5, 'Sách tuyệt vời cho thiếu nhi.', NOW()),
('S005', 'ND005', 4, 'Tài liệu rất hữu ích.', NOW());

INSERT INTO DanhSachYeuThich (MaNguoiDung, MaSach, NgayTao)
VALUES 
('ND001', 'S001', NOW()),
('ND001', 'S002', NOW()),
('ND002', 'S003', NOW()),
('ND003', 'S004', NOW()),
('ND005', 'S005', NOW());

INSERT INTO MaGiamGia (MaGiamGia, MaVoucher, PhanTramGiam, NgayHetHan, NgayTao)
VALUES 
('MGG001', 'SALE10', 10, '2026-12-31', NOW()),
('MGG002', 'SALE20', 20, '2026-12-31', NOW()),
('MGG003', 'SALE30', 30, '2026-12-31', NOW()),
('MGG004', 'SALE40', 40, '2026-12-31', NOW()),
('MGG005', 'SALE50', 50, '2026-12-31', NOW());

INSERT INTO ApDungGiamGia (MaDonHang, MaGiamGia)
VALUES 
('DH001', 'MGG001'),
('DH002', 'MGG002'),
('DH003', 'MGG003'),
('DH004', 'MGG004'),
('DH005', 'MGG005');
