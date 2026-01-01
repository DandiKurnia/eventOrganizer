---

## 🗄 Database Schema Reference (WAJIB DIPATUHI AI)

Bagian ini menjelaskan **struktur tabel dan kolom database** yang digunakan pada project **EventOrganizer**.  
AI **WAJIB** menggunakan schema ini saat membuat:
- Model
- Query Dapper
- Repository
- ViewModel
- Mapping data ke UI

---

## 📂 Table: Category

Digunakan untuk:

- Kategori paket event
- Kategori vendor
- Services di landing page

| Column Name  | Type             | Description   |
| ------------ | ---------------- | ------------- |
| CategoryId   | UNIQUEIDENTIFIER | Primary Key   |
| CategoryName | NVARCHAR         | Nama kategori |
| CreatedAt    | DATETIME         | Waktu dibuat  |

---

## 📦 Table: eventPackage

Digunakan untuk:

- Pricing di Landing Page
- Package Event (Admin & Staff)
- Relasi Order

| Column Name    | Type             | Description       |
| -------------- | ---------------- | ----------------- |
| PackageEventId | UNIQUEIDENTIFIER | Primary Key       |
| PackageName    | NVARCHAR         | Nama paket        |
| Description    | NVARCHAR(MAX)    | Deskripsi paket   |
| BasePrice      | DECIMAL          | Harga paket       |
| Status         | NVARCHAR         | Active / Inactive |
| MainPhotoId    | UNIQUEIDENTIFIER | Foto utama paket  |

📌 **Catatan AI**

- Untuk Landing Page → tampilkan hanya `Status = 'Active'`
- Sorting default → `BasePrice ASC`

---

## 🛒 Table: Orders

Digunakan untuk:

- Booking client
- Dashboard
- Order management

| Column Name       | Type             | Description                     |
| ----------------- | ---------------- | ------------------------------- |
| OrderId           | UNIQUEIDENTIFIER | Primary Key                     |
| UserId            | UNIQUEIDENTIFIER | Client                          |
| PackageEventId    | UNIQUEIDENTIFIER | Paket yang dipilih              |
| AdditionalRequest | NVARCHAR(MAX)    | Permintaan tambahan             |
| EventDate         | DATETIME         | Tanggal event                   |
| Status            | NVARCHAR         | Pending / Confirmed / Cancelled |
| CreatedAt         | DATETIME         | Tanggal order                   |
| ConfirmClient     | BIT              | Konfirmasi client               |

---

## 🔗 Table: PackageCategory

Relasi **Many-to-Many**:

- eventPackage ↔ Category

| Column Name       | Type             | Description       |
| ----------------- | ---------------- | ----------------- |
| PackageCategoryId | UNIQUEIDENTIFIER | Primary Key       |
| PackageEventId    | UNIQUEIDENTIFIER | FK → eventPackage |
| CategoryId        | UNIQUEIDENTIFIER | FK → Category     |

📌 Digunakan untuk:

- Menampilkan kategori di detail paket
- Filter paket berdasarkan kategori

---

## 🖼 Table: PackagePhoto

Digunakan untuk:

- Gallery
- Detail Package
- Slider / Carousel

| Column Name     | Type             | Description         |
| --------------- | ---------------- | ------------------- |
| PhotoId         | UNIQUEIDENTIFIER | Primary Key         |
| PackageEventId  | UNIQUEIDENTIFIER | FK → eventPackage   |
| PhotoUrl        | NVARCHAR         | URL foto (jika ada) |
| Foto            | VARBINARY(MAX)   | File foto           |
| FotoContentType | NVARCHAR         | MIME type           |
| CreatedAt       | DATETIME         | Upload time         |

---

## 👤 Table: Users

Digunakan untuk:

- Client
- Admin
- Staff
- Vendor (via relasi)

| Column Name  | Type             | Description                     |
| ------------ | ---------------- | ------------------------------- |
| UserId       | UNIQUEIDENTIFIER | Primary Key                     |
| Name         | NVARCHAR         | Nama user                       |
| Email        | NVARCHAR         | Email                           |
| PasswordHash | NVARCHAR         | Password hash                   |
| Role         | NVARCHAR         | Admin / Staff / Client / Vendor |
| PhoneNumber  | NVARCHAR         | Nomor HP                        |
| IsActive     | BIT              | Status user                     |
| CreatedAt    | DATETIME         | Waktu daftar                    |

---

## 🏢 Table: Vendor

Digunakan untuk:

- Data vendor
- Relasi ke order
- Vendor confirmation

| Column Name | Type             | Description       |
| ----------- | ---------------- | ----------------- |
| VendorId    | UNIQUEIDENTIFIER | Primary Key       |
| UserId      | UNIQUEIDENTIFIER | FK → Users        |
| CompanyName | NVARCHAR         | Nama perusahaan   |
| Address     | NVARCHAR         | Alamat            |
| Status      | NVARCHAR         | Active / Inactive |
| CreatedAt   | DATETIME         | Tanggal dibuat    |

---

## 🔗 Table: VendorCategory

Relasi **Vendor ↔ Category**

| Column Name      | Type             | Description   |
| ---------------- | ---------------- | ------------- |
| VendorCategoryId | UNIQUEIDENTIFIER | Primary Key   |
| VendorId         | UNIQUEIDENTIFIER | FK → Vendor   |
| CategoryId       | UNIQUEIDENTIFIER | FK → Category |

---

## ✅ Table: VendorConfirmation

Digunakan untuk:

- Vendor menerima / menolak order
- Harga final vendor

| Column Name          | Type             | Description                   |
| -------------------- | ---------------- | ----------------------------- |
| VendorConfirmationId | UNIQUEIDENTIFIER | Primary Key                   |
| OrderId              | UNIQUEIDENTIFIER | FK → Orders                   |
| VendorId             | UNIQUEIDENTIFIER | FK → Vendor                   |
| ActualPrice          | DECIMAL          | Harga final vendor            |
| Notes                | NVARCHAR(MAX)    | Catatan vendor                |
| VendorStatus         | NVARCHAR         | Pending / Accepted / Rejected |
| CreatedAt            | DATETIME         | Tanggal konfirmasi            |

---

## 🤖 AI Development Rules (DATABASE)

- Gunakan **foreign key logic** sesuai tabel di atas
- Jangan menambah kolom tanpa instruksi
- Semua query harus **explicit column mapping**
- Hindari `SELECT *`
- Gunakan JOIN sesuai relasi tabel

---

---

🤖 AI Prompt – UI Improvement (AdminEventOrganizer)

Prompt:

Anda adalah AI Frontend Engineer yang berpengalaman dalam ASP.NET Core MVC, Razor View, Bootstrap 5, dan UI/UX profesional.

Saya memiliki dua project:

AdminEventOrganizer

EventOrganizer

Tugas Anda adalah:

Mengubah tampilan (UI/UX) tabel pada halaman Index di project AdminEventOrganizer

Tampilan harus menyerupai halaman Vendor/Index pada project EventOrganizer

⚠️ Aturan Penting (WAJIB DIIKUTI):

JANGAN mengubah, menghapus, atau menambah column

JANGAN mengubah nama property Model

JANGAN mengubah Controller, Query, Repository, atau Logic Backend

HANYA mengubah bagian View (.cshtml)

Struktur data dan binding @model harus tetap sama

🎨 Yang boleh diubah:

Layout visual tabel

Penggunaan Bootstrap (card, badge, icon, spacing)

Styling (hover, alignment, warna, font)

Penempatan tombol action agar lebih rapi

🎯 Target tampilan:

Clean

Profesional

Konsisten dengan UI di project EventOrganizer

Mudah dibaca dan nyaman untuk admin

📌 Output yang saya inginkan:

Full code View (Index.cshtml)

Siap langsung dipakai (copy–paste)

Tanpa placeholder fiktif

Ingat: Fokus hanya pada tampilan, bukan data.
