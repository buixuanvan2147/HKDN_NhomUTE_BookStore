CREATE DATABASE CSharp_BookShop;
GO

USE CSharp_BookShop;
GO

--1 Bảng NguoiDung
CREATE TABLE NguoiDung (
    MaNguoiDung VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) UNIQUE NOT NULL, -- Tăng chiều dài để phù hợp với email dài
    MatKhau NVARCHAR(128) NOT NULL, -- Đủ cho mật khẩu mã hóa
    SoDienThoai NVARCHAR(15),
    DiaChi NVARCHAR(255),
    VaiTro NVARCHAR(20) DEFAULT 'KhachHang', -- 'Admin', 'KhachHang'
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

--2 Bảng TheLoai
CREATE TABLE TheLoai (
    MaTheLoai VARCHAR(10) PRIMARY KEY,
    TenTheLoai NVARCHAR(50) NOT NULL UNIQUE
);
GO

--3 Bảng Sach
CREATE TABLE Sach (
    MaSach VARCHAR(10) PRIMARY KEY,
    TenSach NVARCHAR(200) NOT NULL,
    TacGia NVARCHAR(100) NOT NULL,
    NhaXuatBan NVARCHAR(100),
    NamXuatBan INT,
    MaTheLoai VARCHAR(10) FOREIGN KEY REFERENCES TheLoai(MaTheLoai) ON DELETE CASCADE,
    Gia DECIMAL(10, 2) NOT NULL CHECK (Gia >= 0), -- Giá không âm
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0), -- Số lượng không âm
    MoTa NVARCHAR(MAX),
    URLAnhBia NVARCHAR(255), -- Đổi tên để rõ ràng hơn
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

--4 Bảng DonHang
CREATE TABLE DonHang (
    MaDonHang VARCHAR(10) PRIMARY KEY,
    MaNguoiDung VARCHAR(10) FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung) ON DELETE CASCADE,
    TongTien DECIMAL(10, 2) NOT NULL CHECK (TongTien >= 0), -- Tổng tiền không âm
    TrangThaiDonHang NVARCHAR(20) DEFAULT 'DangXuLy', -- DangXuLy, DaGiao, DaHuy
    DiaChiGiao NVARCHAR(255),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

--5 Bảng ChiTietDonHang
CREATE TABLE ChiTietDonHang (
    MaChiTiet INT PRIMARY KEY IDENTITY(1,1),
    MaDonHang VARCHAR(10) FOREIGN KEY REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    MaSach VARCHAR(10) FOREIGN KEY REFERENCES Sach(MaSach) ON DELETE CASCADE,
    SoLuong INT NOT NULL CHECK (SoLuong > 0), -- Số lượng phải dương
    GiaBan DECIMAL(10, 2) NOT NULL CHECK (GiaBan >= 0) -- Giá không âm
);
GO

--6 Bảng DanhGia
CREATE TABLE DanhGia (
    MaDanhGia INT PRIMARY KEY IDENTITY(1,1),
    MaSach VARCHAR(10) FOREIGN KEY REFERENCES Sach(MaSach) ON DELETE CASCADE,
    MaNguoiDung VARCHAR(10) FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung) ON DELETE CASCADE,
    DiemDanhGia INT CHECK (DiemDanhGia BETWEEN 1 AND 5), -- Đánh giá từ 1-5 sao
    BinhLuan NVARCHAR(MAX),
    NgayTao DATETIME DEFAULT GETDATE(),
    UNIQUE (MaSach, MaNguoiDung) -- Một người chỉ được đánh giá một sách một lần
);
GO

--7 Bảng DanhSachYeuThich
CREATE TABLE DanhSachYeuThich (
    MaYeuThich INT PRIMARY KEY IDENTITY(1,1),
    MaNguoiDung VARCHAR(10) FOREIGN KEY REFERENCES NguoiDung(MaNguoiDung) ON DELETE CASCADE,
    MaSach VARCHAR(10) FOREIGN KEY REFERENCES Sach(MaSach) ON DELETE CASCADE,
    NgayTao DATETIME DEFAULT GETDATE(),
    UNIQUE (MaNguoiDung, MaSach) -- Không thêm trùng sách vào danh sách yêu thích
);
GO

--8 Bảng MaGiamGia
CREATE TABLE MaGiamGia (
    MaGiamGia VARCHAR(10) PRIMARY KEY,
    MaVoucher NVARCHAR(20) UNIQUE NOT NULL,
    PhanTramGiam DECIMAL(5, 2) CHECK (PhanTramGiam BETWEEN 1 AND 100), -- Hỗ trợ giảm giá dạng thập phân
    NgayHetHan DATETIME NOT NULL CHECK (NgayHetHan > GETDATE()), -- Không cho phép mã hết hạn ngay khi tạo
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

--9 Bảng ApDungGiamGia
CREATE TABLE ApDungGiamGia (
    MaApDung INT PRIMARY KEY IDENTITY(1,1),
    MaDonHang VARCHAR(10) FOREIGN KEY REFERENCES DonHang(MaDonHang) ON DELETE CASCADE,
    MaGiamGia VARCHAR(10) FOREIGN KEY REFERENCES MaGiamGia(MaGiamGia),
    UNIQUE (MaDonHang, MaGiamGia) -- Một đơn hàng chỉ áp dụng một mã giảm giá
);
GO

-- Bảng NguoiDung
INSERT INTO NguoiDung (MaNguoiDung, HoTen, Email, MatKhau, SoDienThoai, DiaChi, VaiTro)
VALUES 
('ND001', N'Nguyen Van A', 'a@gmail.com', 'password1', '0912345678', N'Ha Noi', 'KhachHang'),
('ND002', N'Tran Thi B', 'b@gmail.com', 'password2', '0912345679', N'Da Nang', 'KhachHang'),
('ND003', N'Pham Van C', 'c@gmail.com', 'password3', '0912345680', N'Tp. Ho Chi Minh', 'KhachHang'),
('ND004', N'Le Thi D', 'd@gmail.com', 'password4', '0912345681', N'Hai Phong', 'Admin'),
('ND005', N'Hoang Van E', 'e@gmail.com', 'password5', '0912345682', N'Can Tho', 'KhachHang');

-- Bảng TheLoai
INSERT INTO TheLoai (MaTheLoai, TenTheLoai)
VALUES 
('TL001', N'Tiểu Thuyết'),
('TL002', N'Khoa Học'),
('TL003', N'Tâm Lý - Kỹ Năng Sống'),
('TL004', N'Thiếu Nhi'),
('TL005', N'Lịch Sử'),
('TL006', N'Kinh Tế'),
('TL007', N'Triết Học'),
('TL008', N'Văn Học Việt Nam'),
('TL009', N'Tiếng Anh'),
('TL010', N'Chính Trị'),
('TL011', N'Tâm Linh'),
('TL012', N'Âm Nhạc'),
('TL013', N'Phim - Điện Ảnh'),
('TL014', N'Tuổi Trẻ'),
('TL015', N'Thể Thao'),
('TL016', N'Sách Hướng Dẫn'),
('TL017', N'Ma Quái - Kinh Dị'),
('TL018', N'Kỹ Thuật Công Nghệ'),
('TL019', N'Văn Học Nước Ngoài'),
('TL020', N'Sách Học Ngoại Ngữ'),
('TL021', N'Ẩm Thực'),
('TL022', N'Sức Khỏe'),
('TL023', N'Phát Triển Bản Thân'),
('TL024', N'Môi Trường - Thiên Nhiên'),
('TL025', N'Sách Pháp Luật'),
('TL026', N'Ngữ Pháp - Luyện Thi'),
('TL027', N'Văn Hóa - Xã Hội'),
('TL028', N'Tiểu Sử - Hồi Ký'),
('TL029', N'Hoa Kỳ - Châu Âu'),
('TL030', N'Khoa Học Xã Hội');


-- Bảng Sach
INSERT INTO Sach (MaSach, TenSach, TacGia, NhaXuatBan, NamXuatBan, MaTheLoai, Gia, SoLuongTon, MoTa, URLAnhBia)
VALUES 
('S001', N'Nhà Giả Kim', N'Paulo Coelho', N'Nhã Nam', 2021, 'TL001', 85000, 50, N'Câu chuyện về hành trình khám phá bản thân.', 'nhaGiaKim.jpg'),
('S002', N'Lược Sử Thời Gian', N'Stephen Hawking', N'Tre', 2020, 'TL002', 120000, 30, N'Khám phá bí ẩn của vũ trụ.', 'luocSuThoiGian.jpg'),
('S003', N'Đắc Nhân Tâm', N'Dale Carnegie', N'Tre', 2019, 'TL003', 100000, 40, N'Cẩm nang giao tiếp và quản lý mối quan hệ.', 'dacNhanTam.jpg'),
('S004', N'Dế Mèn Phiêu Lưu Ký', N'Tô Hoài', N'Kim Đồng', 2022, 'TL004', 50000, 60, N'Câu chuyện thiếu nhi nổi tiếng của Việt Nam.', 'deMenPhieuLuuKy.jpg'),
('S005', N'Lịch Sử Việt Nam', N'Nguyen Van Y', N'Giáo Dục', 2023, 'TL005', 150000, 20, N'Tài liệu về lịch sử dân tộc Việt Nam.', 'lichSuVietNam.jpg'),
('S006', N'Thiên Đường Mùa Hè', N'John Green', N'Nhà Xuất Bản Văn Học', 2021, 'TL001', 95000, 50, N'Một câu chuyện tình yêu đầy cảm xúc.', 'thienDuongMuaHe.jpg'),
('S007', N'Đi Tìm Lẽ Sống', N'Viktor Frankl', N'Đà Nẵng', 2020, 'TL003', 130000, 40, N'Cuốn sách giúp tìm lại ý nghĩa cuộc sống.', 'diTimLeSong.jpg'),
('S008', N'Khoa Học Về Tương Lai', N'Carl Sagan', N'Nhà Xuất Bản Trẻ', 2018, 'TL002', 110000, 60, N'Khám phá tương lai của khoa học và công nghệ.', 'khoaHocVeTuongLai.jpg'),
('S009', N'Chìa Khóa Thành Công', N'Napoleon Hill', N'Nhà Xuất Bản Lao Động', 2017, 'TL003', 80000, 35, N'Cẩm nang phát triển bản thân và thành công.', 'chiaKhoaThanhCong.jpg'),
('S010', N'Vương Quốc Thực Vật', N'Richard Dawkins', N'Tri Thức', 2019, 'TL002', 140000, 25, N'Một nghiên cứu về sự phát triển của thực vật trong tự nhiên.', 'vuongQuocThucVat.jpg'),
('S011', N'Bí Ẩn Vũ Trụ', N'Stephen Hawking', N'Trẻ', 2022, 'TL002', 100000, 30, N'Sự hiểu biết về vũ trụ từ quan điểm khoa học.', 'biAnVuTru.jpg'),
('S012', N'Bản Đồ Tâm Hồn', N'Thich Nhat Hanh', N'Nhà Xuất Bản Hồng Đức', 2020, 'TL003', 95000, 45, N'Một cuốn sách hướng dẫn cách sống tâm linh.', 'banDoTamHon.jpg'),
('S013', N'Cuộc Sống Đúng Nghĩa', N'Mark Manson', N'NXB Thế Giới', 2021, 'TL003', 85000, 50, N'Một cái nhìn thẳng thắn và thực tế về cuộc sống.', 'cuocSongDungNghia.jpg'),
('S014', N'Mật Mã Da Vinci', N'Dan Brown', N'Kim Đồng', 2018, 'TL001', 120000, 40, N'Một cuốn tiểu thuyết về các bí mật lịch sử và tín ngưỡng.', 'matMaDaVinci.jpg'),
('S015', N'Những Điều Kỳ Diệu Của Cuộc Sống', N'Haruki Murakami', N'NXB Văn Học', 2022, 'TL001', 130000, 55, N'Một câu chuyện kể về những kỳ diệu trong cuộc sống hàng ngày.', 'nhungDieuKyDieuCuaCuocSong.jpg');


-- Bảng DonHang
INSERT INTO DonHang (MaDonHang, MaNguoiDung, TongTien, TrangThaiDonHang, DiaChiGiao)
VALUES 
('DH001', 'ND001', 185000, N'DangXuLy', N'Ha Noi'),
('DH002', 'ND002', 120000, N'DaGiao', N'Da Nang'),
('DH003', 'ND003', 150000, N'DaHuy', N'Tp. Ho Chi Minh'),
('DH004', 'ND001', 50000, N'DangXuLy', N'Hai Phong'),
('DH005', 'ND005', 100000, N'DangXuLy', N'Can Tho');

-- Bảng ChiTietDonHang
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


-- Bảng DanhGia
INSERT INTO DanhGia (MaSach, MaNguoiDung, DiemDanhGia, BinhLuan)
VALUES 
('S001', 'ND001', 5, N'Rất hay và ý nghĩa.'),
('S002', 'ND002', 4, N'Cuốn sách rất thú vị.'),
('S003', 'ND003', 3, N'Nội dung ổn nhưng trình bày chưa hấp dẫn.'),
('S004', 'ND004', 5, N'Sách tuyệt vời cho thiếu nhi.'),
('S005', 'ND005', 4, N'Tài liệu rất hữu ích.');

-- Bảng DanhSachYeuThich
INSERT INTO DanhSachYeuThich (MaNguoiDung, MaSach)
VALUES 
('ND001', 'S001'),
('ND001', 'S002'),
('ND002', 'S003'),
('ND003', 'S004'),
('ND005', 'S005');

-- Bảng MaGiamGia
INSERT INTO MaGiamGia (MaGiamGia, MaVoucher, PhanTramGiam, NgayHetHan)
VALUES 
('MGG001', N'SALE10', 10, '2024-12-31'),
('MGG002', N'SALE20', 20, '2024-12-31'),
('MGG003', N'SALE30', 30, '2024-12-31'),
('MGG004', N'SALE40', 40, '2024-12-31'),
('MGG005', N'SALE50', 50, '2024-12-31');

-- Bảng ApDungGiamGia
INSERT INTO ApDungGiamGia (MaDonHang, MaGiamGia)
VALUES 
('DH001', 'MGG001'),
('DH002', 'MGG002'),
('DH003', 'MGG003'),
('DH004', 'MGG004'),
('DH005', 'MGG005');
GO

SELECT * FROM NguoiDung;
SELECT * FROM TheLoai;
				
SELECT * FROM DonHang;
SELECT * FROM ChiTietDonHang;
SELECT * FROM DanhGia;
SELECT * FROM DanhSachYeuThich;
SELECT * FROM MaGiamGia;
SELECT * FROM ApDungGiamGia;
SELECT * FROM Sach;

ALTER TABLE Sach
ADD SoLuongDaBan INT;
ALTER TABLE Sach
ADD CONSTRAINT DF_Sach_SoLuongDaBan DEFAULT 0 FOR SoLuongDaBan,
    CONSTRAINT CHK_SoLuongDaBan CHECK (SoLuongDaBan >= 0);

UPDATE Sach
SET SoLuongTon = 100
WHERE SoLuongDaBan IS NULL;
