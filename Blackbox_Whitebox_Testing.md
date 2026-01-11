# Dokumentasi Pengujian Blackbox dan Whitebox

## Proyek Event Organizer

---

## Daftar Isi

1. [Pendahuluan](#pendahuluan)
2. [Gambaran Sistem](#gambaran-sistem)
3. [Pengujian Blackbox](#pengujian-blackbox)
4. [Pengujian Whitebox](#pengujian-whitebox)
5. [Kasus Uji](#kasus-uji)
6. [Strategi Pengujian](#strategi-pengujian)

---

## Pendahuluan

Dokumen ini menyediakan dokumentasi pengujian blackbox dan whitebox secara komprehensif untuk proyek Event Organizer. Sistem Event Organizer adalah aplikasi web multi-tier yang dibangun menggunakan ASP.NET Core MVC dengan ORM Dapper yang mengelola acara, pesanan, vendor, dan pelanggan melalui tiga portal berbeda: Portal Pelanggan/Vendor, Panel Staf, dan Panel Admin.

Pendekatan pengujian mencakup validasi fungsionalitas eksternal (blackbox) dan analisis struktur kode internal (whitebox) untuk memastikan keandalan, keamanan, dan kinerja sistem.

---

## Gambaran Sistem

### Komponen Arsitektur

- **Frontend**: Bootstrap 5, Razor Views
- **Backend**: ASP.NET Core 8.0 MVC
- **ORM**: Dapper (Micro ORM)
- **Database**: SQL Server
- **Autentikasi**: Berbasis sesi dengan kontrol akses berbasis peran
- **Layanan Email**: MailKit & MimeKit
- **Keamanan**: Hashing password BCrypt

### Peran Sistem

- **Pelanggan**: Menjelajahi paket, membuat pesanan, melihat riwayat pesanan
- **Vendor**: Mengelola profil, menerima/menolak pesanan, menetapkan harga
- **Staf**: Mengelola pesanan, mengirim permintaan ke vendor, mengelola paket
- **Admin**: Manajemen sistem penuh, manajemen pengguna, analitik

### Fungsionalitas Inti

- Otentikasi dan otorisasi pengguna
- Manajemen paket acara
- Alur kerja pemrosesan pesanan
- Sistem konfirmasi vendor
- Notifikasi email
- Manajemen kategori
- Unggahan file (foto paket)

---

## Pengujian Blackbox

### Pengujian Persyaratan Fungsional

#### 1. Modul Otentikasi

- **Fungsionalitas Login**

  - Penerimaan kredensial valid
  - Penolakan kredensial tidak valid
  - Validasi status akun
  - Manajemen sesi
  - Pengalihan berbasis peran

- **Fungsionalitas Registrasi**

  - Registrasi pengguna baru
  - Validasi keunikan email
  - Validasi kekuatan kata sandi
  - Validasi bidang wajib
  - Penanganan email duplikat

- **Fungsionalitas Logout**
  - Pembersihan sesi
  - Pengalihan yang tepat
  - Pembatalan token keamanan

#### 2. Modul Manajemen Pesanan

- **Pembuatan Pesanan**

  - Pemilihan tanggal acara
  - Pemilihan paket
  - Penanganan permintaan tambahan
  - Inisialisasi status pesanan
  - Pemaksaan aturan validasi

- **Pemrosesan Pesanan**

  - Validasi transisi status
  - Komunikasi staf-vendor
  - Penanganan konfirmasi pelanggan
  - Alur kerja seleksi vendor

- **Penampilan Pesanan**
  - Tampilan riwayat pesanan
  - Pelacakan status
  - Informasi pesanan terperinci
  - Fungsionalitas pencarian dan filter

#### 3. Modul Manajemen Paket

- **Pembuatan Paket**

  - Validasi bidang wajib
  - Penanganan unggahan foto
  - Penugasan kategori
  - Validasi harga

- **Pengeditan Paket**

  - Pelestarian modifikasi data
  - Manajemen foto
  - Pembaruan kategori
  - Perubahan status

- **Tampilan Paket**
  - Visibilitas paket aktif
  - Pemfilteran kategori
  - Fungsionalitas pencarian
  - Tampilan galeri foto

#### 4. Modul Manajemen Vendor

- **Manajemen Profil Vendor**

  - Pembaruan informasi perusahaan
  - Seleksi layanan kategori
  - Perubahan status ketersediaan
  - Pembaruan informasi kontak

- **Penanganan Pesanan Vendor**
  - Penerimaan/penolakan pesanan
  - Penawaran harga
  - Penambahan catatan
  - Pembaruan status

#### 5. Modul Notifikasi Email

- **Pemicu Notifikasi**

  - Notifikasi pembuatan pesanan
  - Email permintaan vendor
  - Peringatan perubahan status
  - Konfirmasi pengiriman

- **Validasi Konten Email**
  - Identifikasi penerima yang benar
  - Penyertaan informasi yang akurat
  - Validasi format HTML
  - Kesesuaian baris subjek

### Pengujian Persyaratan Non-Fungsional

#### 1. Pengujian Kinerja

- Pengukuran waktu tanggap untuk operasi penting
- Simulasi beban pengguna bersamaan
- Validasi optimalisasi kueri basis data
- Penilaian kinerja unggahan file

#### 2. Pengujian Keamanan

- Penilaian kerentanan injeksi SQL
- Pencegahan pembajakan sesi
- Perlindungan skrip lintas situs (XSS)
- Upaya penghindaran otentikasi
- Pemeriksaan otorisasi untuk akses berbasis peran

#### 3. Pengujian Kegunaan

- Penilaian navigasi antarmuka pengguna
- Umpan balik validasi formulir
- Keklaran pesan kesalahan
- Validasi responsif seluler

#### 4. Pengujian Kompatibilitas

- Kompatibilitas lintas-browser
- Validasi desain responsif
- Adaptasi ukuran layar berbeda
- Kompatibilitas sistem operasi

#### 5. Pengujian Integrasi

- Validasi konektivitas basis data
- Integrasi layanan email
- Integrasi penyimpanan file
- Interaksi pustaka pihak ketiga

---

## Pengujian Whitebox

### Analisis Struktur Kode

#### 1. Lapisan Otentikasi

- **Kontroler**: Implementasi UserController di semua proyek
- **Repositori**: UserRepository dengan hashing kata sandi BCrypt
- **Middleware**: AuthorizationMiddleware dengan pemeriksaan berbasis peran
- **Interface**: IUser dengan injeksi dependensi

```csharp
// Example from UserRepository.Login method:
public async Task<UserModel?> Login(string email, string password)
{
    var sql = "SELECT * FROM Users WHERE Email = @Email AND IsActive = 1";
    using var connection = _context.CreateConnection();
    var user = await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { Email = email });

    if (user == null) return null;

    // Verifikasi password dengan BCrypt
    return VerifyPassword(password, user.PasswordHash) ? user : null;
}
```

**Test Coverage Points:**

- Null user handling
- Password verification logic
- Active status validation
- SQL injection prevention

#### 2. Lapisan Pemrosesan Pesanan

- **Kontroler**: OrderController dengan logika pembaruan status
- **Repositori**: OrderRepository dengan manajemen transaksi
- **Interface**: IOrder dengan beberapa implementasi
- **Logika Bisnis**: Validasi transisi status

```csharp
// Example from OrderRepository.UpdateStatus:
public async Task UpdateStatus(Guid orderId, string status)
{
    var sql = @"UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId";
    using var conn = context.CreateConnection();
    await conn.ExecuteAsync(sql, new { OrderId = orderId, Status = status });
}
```

**Test Coverage Points:**

- SQL parameter binding
- Connection management
- Transaction isolation
- Status validation logic

#### 3. Sistem Konfirmasi Vendor

- **Kontroler**: VendorController dengan logika Terima/Tolak
- **Repositori**: VendorConfirmationRepository dengan operasi atomik
- **Interface**: IVendorConfirmation dengan aturan bisnis
- **Kontrol Konkurensi**: Fungsionalitas CloseOtherVendors

```csharp
// Example from VendorConfirmationRepository.CloseOtherVendors:
public async Task CloseOtherVendors(Guid orderId, Guid vendorId)
{
    var sql = @"
        UPDATE VendorConfirmation
        SET VendorStatus = 'closed'
        WHERE OrderId = @OrderId
          AND VendorId != @VendorId";

    using var conn = _context.CreateConnection();
    await conn.ExecuteAsync(sql, new
    {
        OrderId = orderId,
        VendorId = vendorId
    });
}
```

**Test Coverage Points:**

- Atomic operation execution
- Concurrency race condition handling
- Parameter validation
- SQL injection prevention

#### 4. Lapisan Layanan Email

- **Layanan**: EmailService dengan konfigurasi SMTP
- **Interface**: IEmailService dengan operasi asinkron
- **Konfigurasi**: EmailSettings dari appsettings.json
- **Penanganan Kesalahan**: Manajemen pengecualian SMTP

```csharp
// Example from EmailService.SendVendorRequestEmailAsync:
public async Task SendVendorRequestEmailAsync(string vendorEmail, string vendorName,
    string clientName, string packageName, DateTime eventDate)
{
    var subject = $"New Order Request - {packageName}";
    var body = $@"
        <html>
        <body>
            <h3>New Order Request</h3>
            <p>Hello {vendorName},</p>
            <p>You have received a new order request from {clientName}.</p>
            <p>Package: {packageName}</p>
            <p>Event Date: {eventDate:yyyy-MM-dd}</p>
            <p>Please log in to your vendor panel to review and respond.</p>
        </body>
        </html>";

    await SendEmailAsync(vendorEmail, subject, body);
}
```

**Test Coverage Points:**

- HTML template validation
- Asynchronous execution
- SMTP error handling
- Configuration loading

### Analisis Cakupan Jalur

#### 1. Jalur Otentikasi

- Login sukses → Pengalihan berbasis peran
- Login gagal → Tampilan pesan kesalahan
- Akun tidak aktif → Penolakan akses
- Timeout sesi → Kebutuhan re-autentikasi

#### 2. Jalur Transisi Status Pesanan

- `waiting validation` → `consultation` → `vendor confirmation` → `booking confirmed`
- `waiting validation` → `cancelled`
- `vendor confirmation` → `rejected` → `booking confirmed`

#### 3. Jalur Respon Vendor

- Terima pesanan → Tetapkan harga → Perbarui status
- Tolak pesanan → Perbarui status → Beri tahu vendor lain
- Seleksi vendor ganda → Otomatis tutup lainnya
- Negosiasi harga → Perbarui konfirmasi

#### 4. Jalur Unggahan File

- Unggah file valid → Simpan ke penyimpanan → Perbarui basis data
- Jenis file tidak valid → Pesan kesalahan → Tidak disimpan
- File besar → Validasi ukuran → Penolakan
- File hilang → Kesalahan validasi → Tidak diproses

### Penilaian Kualitas Kode

#### 1. Penanganan Kesalahan

- Blok try-catch untuk operasi basis data
- Validasi input untuk semua input pengguna
- Pencegahan injeksi SQL melalui parameterisasi
- Logging dan pelaporan pengecualian yang tepat

#### 2. Tindakan Keamanan

- Hashing kata sandi BCrypt
- Otentikasi berbasis sesi
- Kontrol akses berbasis peran
- Binding parameter SQL
- Implementasi token CSRF

#### 3. Optimalisasi Kinerja

- Kueri basis data efisien dengan pengindeksan
- Connection pooling melalui Dapper
- Loading malas untuk entitas terkait
- Paginasi untuk dataset besar

#### 4. Daya Pemeliharaan

- Pemisahan kepedulian yang jelas (pola MVC)
- Injeksi dependensi untuk kopling longgar
- Pola repositori untuk akses data
- Segregasi interface untuk fleksibilitas

---

## Kasus Uji

### Kasus Uji Otentikasi

#### TC_AUTH_001: Login Pengguna Valid

- **Tujuan**: Verifikasi login sukses dengan kredensial valid
- **Prasyarat**: Akun pengguna ada dan aktif
- **Input**: Email valid dan kata sandi benar
- **Hasil yang Diharapkan**: Pengguna dialihkan ke dasbor yang sesuai
- **Pasca Kondisi**: Sesi dibuat dengan peran pengguna

#### TC_AUTH_002: Login dengan Kredensial Tidak Valid

- **Tujuan**: Verifikasi penolakan kredensial login tidak valid
- **Prasyarat**: Akun pengguna ada
- **Input**: Email valid dan kata sandi salah
- **Hasil yang Diharapkan**: Pesan kesalahan ditampilkan, tidak ada sesi dibuat
- **Pasca Kondisi**: Pengguna tetap di halaman login

#### TC_AUTH_003: Login Akun Tidak Aktif

- **Tujuan**: Verifikasi pemblokiran akses untuk akun tidak aktif
- **Prasyarat**: Akun pengguna ada tetapi tidak aktif
- **Input**: Email valid dan kata sandi benar
- **Hasil yang Diharapkan**: Akses ditolak dengan pesan yang sesuai
- **Pasca Kondisi**: Pengguna tetap di halaman login

### Kasus Uji Manajemen Pesanan

#### TC_ORD_001: Buat Pesanan Baru

- **Tujuan**: Verifikasi pembuatan pesanan sukses
- **Prasyarat**: Pengguna login sebagai pelanggan
- **Input**: Tanggal acara valid, seleksi paket, permintaan opsional
- **Hasil yang Diharapkan**: Pesanan dibuat dengan status "waiting validation"
- **Pasca Kondisi**: Pesanan muncul di riwayat pesanan pengguna

#### TC_ORD_002: Perbarui Status Pesanan

- **Tujuan**: Verifikasi pembaruan status yang sah
- **Prasyarat**: Pengguna login sebagai staf/admin
- **Input**: ID pesanan valid dan status baru
- **Hasil yang Diharapkan**: Status pesanan berhasil diperbarui
- **Pasca Kondisi**: Status diperbarui tercermin di sistem

#### TC_ORD_003: Vendor Menerima Pesanan

- **Tujuan**: Verifikasi vendor dapat menerima pesanan
- **Prasyarat**: Vendor login dan memiliki pesanan tertunda
- **Input**: ID konfirmasi pesanan, harga aktual, catatan opsional
- **Hasil yang Diharapkan**: Pesanan diterima dengan detail vendor
- **Pasca Kondisi**: Vendor lain ditutup, status diperbarui

### Kasus Uji Manajemen Paket

#### TC_PKG_001: Buat Paket dengan Foto

- **Tujuan**: Verifikasi pembuatan paket dengan banyak foto
- **Prasyarat**: Pengguna login sebagai staf/admin
- **Input**: Detail paket dan file foto
- **Hasil yang Diharapkan**: Paket disimpan dengan semua foto
- **Pasca Kondisi**: Paket terlihat oleh pelanggan

#### TC_PKG_002: Edit Informasi Paket

- **Tujuan**: Verifikasi informasi paket dapat dimodifikasi
- **Prasyarat**: Paket ada dan pengguna memiliki hak edit
- **Input**: Informasi paket yang diperbarui
- **Hasil yang Diharapkan**: Informasi paket berhasil diperbarui
- **Pasca Kondisi**: Perubahan tercermin di sistem

### Kasus Uji Keamanan

#### TC_SEC_001: Pencegahan Akses Tanpa Otorisasi

- **Tujuan**: Verifikasi pencegahan akses tanpa otorisasi
- **Prasyarat**: Pengguna tidak login
- **Input**: Akses URL langsung ke sumber daya yang dilindungi
- **Hasil yang Diharapkan**: Dialihkan ke halaman login
- **Pasca Kondisi**: Tidak ada akses tanpa otorisasi diberikan

#### TC_SEC_002: Pencegahan Injeksi SQL

- **Tujuan**: Verifikasi perlindungan terhadap injeksi SQL
- **Prasyarat**: Aplikasi berjalan
- **Input**: Upaya injeksi SQL di bidang formulir
- **Hasil yang Diharapkan**: Input disanitasi, tidak ada kesalahan basis data
- **Pasca Kondisi**: Sistem tetap aman

---

## Strategi Pengujian

### Strategi Pengujian Blackbox

#### 1. Partisi Ekuivalensi

- Partisi input valid/tidak valid untuk semua formulir
- Nilai batas untuk input numerik (harga, jumlah)
- Seleksi kategori untuk paket dan vendor

#### 2. Analisis Nilai Batas

- Rentang tanggal minimum dan maksimum untuk acara
- Ambang harga untuk paket dan penawaran
- Batas karakter untuk bidang teks

#### 3. Pengujian Tabel Keputusan

- Izin akses berbasis peran
- Aturan transisi status pesanan
- Alur kerja konfirmasi vendor

#### 4. Pengujian Transisi Status

- Status sesi pengguna
- Status siklus hidup pesanan
- Status ketersediaan vendor

### Strategi Pengujian Whitebox

#### 1. Cakupan Pernyataan

- Pastikan setiap pernyataan yang dapat dieksekusi dieksekusi
- Identifikasi kode yang tidak dapat dijangkau
- Validasi semua cabang kondisional

#### 2. Cakupan Cabang

- Uji semua kemungkinan hasil keputusan
- Validasi kondisi benar dan salah
- Cakup jalur penanganan pengecualian

#### 3. Cakupan Jalur

- Eksekusi semua jalur eksekusi yang mungkin
- Uji pernyataan kondisional bersarang
- Validasi iterasi loop

#### 4. Cakupan Kondisi

- Uji ekspresi boolean individu
- Validasi logika kondisional kompleks
- Periksa kondisi batas

### Pendekatan Pengujian Otomatis

#### 1. Pengujian Unit

- Validasi metode repositori
- Verifikasi logika bisnis
- Pengujian fungsi utilitas

#### 2. Pengujian Integrasi

- Validasi koneksi basis data
- Pengujian endpoint API
- Integrasi lapisan layanan

#### 3. Pengujian End-to-End

- Validasi perjalanan pengguna lengkap
- Fungsionalitas lintas modul
- Simulasi skenario dunia nyata

### Alat dan Kerangka Pengujian

#### 1. Pengujian Manual

- Pengujian UI berbasis browser
- Kompatibilitas lintas platform
- Penilaian kegunaan

#### 2. Pengujian Otomatis

- NUnit untuk pengujian unit
- Selenium untuk otomasi UI
- Postman untuk pengujian API

#### 3. Pengujian Keamanan

- OWASP ZAP untuk pemindaian kerentanan
- SQLMap untuk pengujian injeksi
- Burp Suite untuk pengujian penetrasi

### Jadwal Eksekusi Pengujian

#### Fase 1: Pengujian Komponen

- Pengujian modul individual
- Eksekusi pengujian unit
- Validasi repositori

#### Fase 2: Pengujian Integrasi

- Pengujian interaksi modul
- Validasi API
- Integrasi basis data

#### Fase 3: Pengujian Sistem

- Pengujian alur kerja end-to-end
- Validasi kinerja
- Penilaian keamanan

#### Fase 4: Pengujian Penerimaan Pengguna

- Validasi persyaratan bisnis
- Pengujian kegunaan
- Persetujuan stakeholder

---

## Kesimpulan

Dokumentasi pengujian komprehensif ini menyediakan pendekatan terstruktur untuk memvalidasi sistem Event Organizer melalui metodologi pengujian blackbox dan whitebox. Kombinasi ini memastikan baik kebenaran fungsional maupun kualitas kode, mengatasi arsitektur multi-tier kompleks dan berbagai peran pengguna dalam sistem.

Pembaruan pengujian rutin harus dilakukan saat fitur baru ditambahkan atau fungsionalitas yang ada dimodifikasi untuk menjaga integritas dan keandalan sistem.
