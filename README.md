# Clean Architecture — ASP.NET Core + PostgreSQL

Template API dengan pemisahan layer: **Web.Api → Service → Repository → Domain**.
Satu contoh lengkap (CRUD `Category`) yang sudah mencakup pagination, search,
error handling, logging, dan konfigurasi rahasia di luar repo.

## Teknologi

| Bagian | Pilihan |
|---|---|
| Runtime | .NET 10 |
| Database | PostgreSQL |
| Akses data | Dapper (SQL manual, bukan ORM) |
| Logging | Serilog (console + file rolling harian) |
| Dokumentasi API | Swagger / OpenAPI |

## Struktur

```
src/
  CleanArchitecture.Domain/       entity, interface, PagedResult — tanpa dependency luar
  CleanArchitecture.Repository/   Dapper + Npgsql, implementasi repository
  CleanArchitecture.Service/      aturan bisnis, batas transaksi (Unit of Work)
  CleanArchitecture.Web.Api/      controller, DTO request, middleware, Program.cs
database.sql                      skema + data contoh
```

Aturan arah dependency: layer dalam **tidak boleh** tahu layer luar.
`Domain` tidak mereferensi apa pun; `Web.Api` adalah satu-satunya entry point.

## Menjalankan

**1. Siapkan database**

```bash
createdb -U postgres northwind        # atau buat lewat pgAdmin
psql -U postgres -d northwind -f database.sql
```

**2. Siapkan konfigurasi**

```bash
cd src/CleanArchitecture.Web.Api
cp .env.example .env
```

Isi `.env`:

```ini
DB__Host=localhost
DB__Port=5432
DB__Database=northwind
DB__Username=postgres
DB__Password=passwordmu
```

`.env` sudah masuk `.gitignore`. Yang di-commit hanya `.env.example`.
Kalau `DB__Database` kosong, aplikasi berhenti saat startup dengan pesan jelas.

**3. Jalankan**

```bash
dotnet run --project src/CleanArchitecture.Web.Api
```

- Swagger: <http://localhost:5207/swagger>
- Log: `src/CleanArchitecture.Web.Api/logs/app-YYYYMMDD.log`

## Endpoint

| Method | Path | Keterangan |
|---|---|---|
| GET | `/api/Categories/GetRows?page=1&pageSize=10&search=sea` | list + paging + search (nama & deskripsi) |
| GET | `/api/Categories/GetRow/{id}` | detail, 404 kalau tidak ada |
| POST | `/api/Categories/Insert` | buat baru, `categoryId: 0` = id otomatis |
| PUT | `/api/Categories/Update/{id}` | ubah |
| DELETE | `/api/Categories/Delete?ids=1&ids=2` | hapus; semua id harus ada, kalau tidak 404 dan tidak ada yang terhapus |

Contoh request lengkap ada di
[`CleanArchitecture.Web.Api.http`](src/CleanArchitecture.Web.Api/CleanArchitecture.Web.Api.http)
— bisa dijalankan langsung dari Visual Studio atau VS Code (ext. REST Client).

## Konvensi yang dipakai

**Response.** Sukses mengembalikan resource apa adanya; error mengembalikan
`ProblemDetails` (RFC 9457). Tidak ada envelope `{ success, data }`.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Terjadi kesalahan pada server.",
  "status": 500,
  "instance": "/api/Categories/GetRows",
  "traceId": "00-0f1c1956...-00"
}
```

`traceId` sama dengan yang ada di log — grep satu string untuk melihat seluruh
request. Detail exception asli tidak pernah dikirim ke client.

**Pagination.** List mengembalikan metadata di samping data:

```json
{ "items": [ ... ], "page": 1, "pageSize": 10, "total": 8, "totalPages": 1 }
```

`page` < 1 atau `pageSize` > 100 ditolak dengan 400 oleh validasi
[`PagedQuery`](src/CleanArchitecture.Web.Api/Contracts/PagedQuery.cs).

**Nama kolom.** Database memakai `snake_case`, C# memakai `PascalCase`.
Pemetaan dilakukan eksplisit di SQL (`category_id AS CategoryID`), bukan lewat
konvensi global, supaya terlihat di query.

**Transaksi.** `SqlUnitOfWork` membuat satu koneksi per request; semua repository
dalam request itu berbagi koneksi dan transaksi. Contoh batas transaksi ada di
[`CategoryService.Delete`](src/CleanArchitecture.Service/Services/CategoryService.cs):
hitung dulu, hapus hanya kalau semua id ada.

## Yang sengaja belum ada

Bukan lupa — ditunda sampai kebutuhannya nyata:

- **Auth / otorisasi.** Belum ada kebutuhan; `UseAuthorization()` sudah terpasang.
- **CQRS / MediatR.** Service sudah jadi seam-nya; pecah kalau method mulai banyak.
- **Migration tool (EF Migrations/Flyway).** Satu tabel, satu file SQL cukup.
- **Unit test.** Yang layak dites lebih dulu: `Delete` atomik dan `offset` paging.
- **Docker / CI.** Ditambahkan saat deployment mulai dirancang.

## Menambah fitur baru

Contoh menambah `Supplier`:

1. `Domain/Entities/Supplier.cs`
2. `Domain/Interfaces/Repositories/ISupplierRepository.cs`
3. `Repository/Repositories/SupplierRepository.cs` + daftarkan di `Repository/DependencyInjection.cs`
4. `Domain/Interfaces/Services/ISupplierService.cs`
5. `Service/Services/SupplierService.cs` + daftarkan di `Service/DependencyInjection.cs`
6. `Web.Api/Contracts/SaveSupplierRequest.cs`
7. `Web.Api/Controllers/SuppliersController.cs` — polanya sama dengan `CategoriesController`

Tabel masuk ke `database.sql`. Tidak ada file lain yang perlu diubah.
