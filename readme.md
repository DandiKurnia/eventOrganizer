# 📘 EventOrganizer – AI Development Guide (.NET)

## 🎯 Tujuan README

README ini dibuat sebagai **panduan untuk AI / developer assistant** agar memahami:

- Struktur solution
- Arsitektur yang digunakan
- Permasalahan utama
- Aturan saat membuat fitur CRUD di **StaffEventOrganizer**

Target utama:

> Membuat fitur CRUD di project **StaffEventOrganizer** yang **setara dengan AdminEventOrganizer**,  
> dengan tetap menggunakan **shared Models, Interface, dan Repository pattern**.

---

## 🏗 Struktur Solution

EventOrganizer
│
├── AdminEventOrganizer # Admin area (FULL CRUD – reference utama)
├── StaffEventOrganizer # Staff area (CRUD harus dibuat di sini)
├── EventOrganizer # Client / customer web
├── Models # Shared Models & ViewModels
│
├── EventOrganizer.sln

yaml
Copy code

---

## ⚙️ Tech Stack

- ASP.NET Core MVC
- .NET
- Dapper
- SQL Server
- Razor View
- Dependency Injection
- Repository Pattern

---

## 🧠 Arsitektur Aplikasi

Pola arsitektur yang digunakan:

Controller
↓
Interface (IRepository)
↓
Repository (Dapper)
↓
Database

markdown
Copy code

### 📌 Shared Layer

- Semua **Entity & ViewModel** berada di:
  /Models

yaml
Copy code

- **TIDAK BOLEH** menduplikasi model di AdminEventOrganizer atau StaffEventOrganizer

---

## 🚨 Permasalahan Utama

Project **StaffEventOrganizer**:

- Belum memiliki CRUD lengkap
- Harus memiliki **fitur yang sama** dengan **AdminEventOrganizer**
- Namun:
- Akses & UI disesuaikan untuk **role staff**
- Logic bisnis tetap konsisten

---

## 📋 Fitur yang WAJIB Ada di StaffEventOrganizer

Fitur berikut **SUDAH ADA** di AdminEventOrganizer dan harus direplikasi:

### 1️⃣ Dashboard

- Total order
- Booking confirmed
- Summary data (read-only)

### 2️⃣ Category

- View category
- CRUD (opsional, sesuai role staff)

### 3️⃣ Package Event

- View package
- Detail package
- Kategori paket
- Foto paket
- Harga & status

### 4️⃣ Order

- View order
- Detail order
- Event date
- Additional request
- Status order

---

## 🧱 Aturan Pengembangan (WAJIB DIIKUTI)

### ❌ DILARANG

- ❌ Mengakses database langsung dari Controller
- ❌ Menulis SQL di Controller
- ❌ Menduplikasi Model di StaffEventOrganizer
- ❌ Mengubah struktur Models tanpa kebutuhan jelas

### ✅ WAJIB

- ✅ Gunakan **Models/** (shared)
- ✅ Gunakan **Interface + Repository**
- ✅ Ikuti struktur dan query dari AdminEventOrganizer
- ✅ Gunakan Dependency Injection
- ✅ Konsisten dengan naming & folder structure

---

## 📂 Struktur Ideal StaffEventOrganizer

StaffEventOrganizer
│
├── Controllers
│ ├── DashboardController.cs
│ ├── CategoryController.cs
│ ├── PackageEventController.cs
│ ├── OrderController.cs
│
├── Interface
│ ├── ICategory.cs
│ ├── IPackageEvent.cs
│ ├── IOrder.cs
│
├── Repository
│ ├── CategoryRepository.cs
│ ├── PackageEventRepository.cs
│ ├── OrderRepository.cs
│
├── Views
│ ├── Dashboard
│ ├── Category
│ ├── PackageEvent
│ ├── Order

yaml
Copy code

---

## 🔁 Workflow Pembuatan CRUD (UNTUK AI)

Setiap pembuatan fitur **HARUS mengikuti alur ini**:

1. Cek fitur di **AdminEventOrganizer**
2. Analisis:
   - Controller
   - Interface
   - Repository
   - Query SQL
3. Gunakan **Model yang sama** dari `/Models`
4. Buat versi **StaffEventOrganizer**
5. Sesuaikan:
   - Hak akses
   - Tampilan UI
   - Action yang diizinkan

---

## 🧪 Contoh Prompt ke AI

Buatkan CRUD Package Event di StaffEventOrganizer.
Ikuti struktur AdminEventOrganizer.
Gunakan Models yang sudah ada.
Lengkapi Controller, Interface, Repository, dan View.
Gunakan Dapper dan Repository Pattern.

yaml
Copy code

---

## 🎯 Output yang Diharapkan

- Code clean & konsisten
- Compile tanpa error
- Mengikuti arsitektur existing
- Mudah dikembangkan
- Tidak breaking change

---

## 📝 Catatan Akhir

- Admin = full access
- Staff = operational access
- Semua logic bisnis harus **selaras**
- AdminEventOrganizer adalah **reference utama**
