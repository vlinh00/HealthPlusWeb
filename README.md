# HealthPlusWeb
# HealthPlus — Website thương mại điện tử thực phẩm bổ sung

HealthPlus là đồ án website thương mại điện tử được xây dựng bằng ASP.NET Core Web API, Blazor WebAssembly và SQL Server. Website hỗ trợ khách hàng tìm kiếm sản phẩm, quản lý giỏ hàng, đặt hàng và theo dõi đơn hàng; đồng thời cung cấp khu vực quản trị để quản lý sản phẩm, danh mục, đơn hàng, người dùng và báo cáo.

## 1. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Backend | ASP.NET Core Web API .NET 10 |
| Frontend | Blazor WebAssembly .NET 10 |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server 2022 |
| Authentication | JWT Bearer Authentication |
| API documentation | Swagger / OpenAPI |
| Database deployment | Docker hoặc SQL Server cài trực tiếp |
| Image storage | File ảnh trong `wwwroot/uploads/products/` |

## 2. Chức năng chính

### Khách hàng
- Đăng ký, đăng nhập và đăng xuất.
- Xem danh sách sản phẩm và chi tiết sản phẩm.
- Tìm kiếm, lọc theo danh mục và sắp xếp sản phẩm.
- Thêm sản phẩm vào giỏ hàng, thay đổi số lượng và xóa sản phẩm.
- Nhập thông tin nhận hàng và đặt hàng.
- Theo dõi đơn hàng theo các chức năng đã triển khai.

### Quản trị viên
- Dashboard thống kê hoạt động cửa hàng.
- Thêm, sửa và quản lý sản phẩm.
- Upload ảnh sản phẩm.
- Quản lý danh mục sản phẩm.
- Theo dõi đơn hàng và cập nhật trạng thái xử lý.
- Quản lý tài khoản người dùng theo các chức năng đã triển khai.
- Xem báo cáo doanh thu và sản phẩm bán chạy.

> Lưu ý: Phương thức thanh toán thực tế phụ thuộc vào cấu hình và các chức năng đã triển khai trong phiên bản hiện tại. Không mặc định rằng website đã kết nối cổng thanh toán thật.

## 3. Cấu trúc dự án

```text
HealthPlusWeb/
├── HealthPlus.slnx
├── docker-compose.yml
├── Database/
│   ├── 01_CreateDatabase.sql
│   ├── 02_CreateTables.sql
│   └── 03_SeedData.sql
├── HealthPlus.API/
│   ├── Controllers/
│   ├── Common/
│   ├── Data/
│   ├── DTOs/
│   ├── Entities/
│   ├── Repositories/
│   ├── Services/
│   ├── Settings/
│   ├── wwwroot/
│   │   ├── images/
│   │   └── uploads/
│   │       └── products/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── HealthPlus.API.csproj
│   └── Program.cs
└── HealthPlus.Client/
    ├── Pages/
    ├── Shared/
    ├── Services/
    ├── Models/
    ├── wwwroot/
    ├── Program.cs
    └── HealthPlus.Client.csproj
```

**Cơ chế chạy website:** `HealthPlus.Client` là frontend Blazor WebAssembly độc lập về source code. Khi build hoặc publish API theo cấu hình của dự án, frontend được publish và copy vào `HealthPlus.API/wwwroot`. ASP.NET Core phục vụ giao diện và các REST API trên cùng một địa chỉ.

## 4. Yêu cầu môi trường

Cần có:

- .NET 10 SDK.
- Git.
- SQL Server 2022 hoặc phiên bản SQL Server tương thích với database scripts.
- SQL Server Management Studio (SSMS) hoặc công cụ quản trị SQL tương đương.
- Docker Desktop hoặc Docker Engine nếu chọn phương án chạy SQL Server bằng Docker.

Kiểm tra .NET SDK:

```bash
dotnet --version
```

Kiểm tra Git:

```bash
git --version
```

Nếu dùng Docker:

```bash
docker --version
docker compose version
```

## 5. Lấy source code

Clone repository:

```bash
git clone <REPOSITORY_URL>
cd HealthPlusWeb
```

Thay `<REPOSITORY_URL>` bằng URL repository thực tế.

Nếu đã có source code, mở terminal tại thư mục gốc chứa `HealthPlus.slnx`.

## 6. Chuẩn bị database

Có hai phương án. Chỉ cần chọn **một trong hai**.

### Phương án A — Chạy SQL Server bằng Docker

#### Bước 1: Kiểm tra Docker Compose

Mở `docker-compose.yml` tại thư mục gốc dự án.

Ví dụ cấu hình SQL Server:

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: healthplus-sqlserver
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "${MSSQL_SA_PASSWORD}"
    ports:
      - "1433:1433"
    volumes:
      - healthplus-sql-data:/var/opt/mssql

  dbeaver:
    image: dbeaver/cloudbeaver:latest
    container_name: healthplus-dbeaver
    ports:
      - "8978:8978"
    depends_on:
      - sqlserver

volumes:
  healthplus-sql-data:
```

Nếu file Compose hiện tại của repository đã có cấu hình tương đương, có thể giữ nguyên file đó.

#### Bước 2: Cấu hình mật khẩu

Tạo file `.env` tại thư mục gốc:

```dotenv
MSSQL_SA_PASSWORD=Replace_With_Your_Strong_Password_123!
```

Thay bằng mật khẩu mạnh phù hợp với chính sách của SQL Server. Không commit file `.env` chứa mật khẩu lên Git.

Nếu `docker-compose.yml` hiện tại đang ghi mật khẩu trực tiếp trong `MSSQL_SA_PASSWORD`, hãy đồng bộ lại cấu hình hoặc tiếp tục sử dụng đúng biến môi trường mà file hiện tại yêu cầu.

#### Bước 3: Khởi động SQL Server

```bash
docker compose up -d sqlserver
```

Kiểm tra container:

```bash
docker compose ps
```

Xem log nếu SQL Server chưa khởi động:

```bash
docker compose logs -f sqlserver
```

Đợi SQL Server sẵn sàng trước khi chạy các script tạo database.

#### Bước 4: Tạo database và dữ liệu mẫu

Mở SSMS hoặc công cụ quản trị SQL Server.

Kết nối đến SQL Server:

- Server: `localhost,1433` nếu kết nối qua cổng được map trên máy đang chạy Docker.
- Authentication: SQL Server Authentication.
- Login: `sa`.
- Password: mật khẩu đã cấu hình.

Trong GitHub Codespaces, cần dùng cổng `1433` đã được forward nếu kết nối từ máy cá nhân. Địa chỉ kết nối từ máy cá nhân có thể khác `localhost` bên trong Codespaces.

Chạy lần lượt các script tại thư mục `Database/`:

1. `01_CreateDatabase.sql`
2. `02_CreateTables.sql`
3. `03_SeedData.sql`

Không chạy script tạo database nhiều lần một cách tùy tiện trên database đang có dữ liệu. Kiểm tra nội dung script trước khi chạy lại.

#### Bước 5: Cấu hình connection string

Khi API chạy trực tiếp trên máy hoặc trong Codespaces host, connection string có thể dùng:

```text
Server=localhost,1433;Database=HealthPlus;User Id=sa;Password=<YOUR_SQL_PASSWORD>;Encrypt=True;TrustServerCertificate=True;
```

Thay `<YOUR_SQL_PASSWORD>` bằng mật khẩu tương ứng.

Nếu API cũng chạy trong một container cùng Docker network với SQL Server, hostname thường là tên service `sqlserver`, không phải `localhost`:

```text
Server=sqlserver,1433;Database=HealthPlus;User Id=sa;Password=<YOUR_SQL_PASSWORD>;Encrypt=True;TrustServerCertificate=True;
```

### Phương án B — Dùng SQL Server cài trực tiếp trên máy, không dùng Docker

Phương án này phù hợp khi máy đã cài SQL Server Developer, Express hoặc một phiên bản SQL Server tương thích.

#### Bước 1: Khởi động SQL Server

Mở SQL Server Configuration Manager hoặc Services, bảo đảm SQL Server instance đang chạy.

Các tên instance phổ biến:

- `localhost` — SQL Server default instance.
- `localhost\SQLEXPRESS` — SQL Server Express.
- `localhost,1433` — nếu SQL Server được cấu hình lắng nghe cổng TCP 1433.

Dùng SSMS để kết nối và kiểm tra instance trước khi tạo database.

#### Bước 2: Tạo database

Trong SSMS, kết nối đến instance SQL Server của máy.

Mở và chạy lần lượt:

1. `Database/01_CreateDatabase.sql`
2. `Database/02_CreateTables.sql`
3. `Database/03_SeedData.sql`

Nếu sử dụng Windows Authentication, hãy kiểm tra tài khoản Windows có quyền tạo database và bảng. Nếu dùng SQL Server Authentication, hãy bảo đảm tài khoản có đủ quyền.

#### Bước 3: Cấu hình connection string

Mở `HealthPlus.API/appsettings.Development.json`.

Ví dụ dùng SQL Server default instance với SQL Server Authentication:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HealthPlus;User Id=YOUR_SQL_USER;Password=YOUR_SQL_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

Ví dụ dùng SQL Server Express:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HealthPlus;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Ví dụ dùng Windows Authentication với default instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HealthPlus;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Chọn đúng một connection string** phù hợp với SQL Server trên máy. Không sử dụng `localhost,1433` nếu instance của bạn đang chạy ở một cổng khác.

Nếu project đang sử dụng cấu hình `DefaultConnection` trong `appsettings.json`, hãy đồng bộ giá trị tương ứng ở cấu hình Development. Không đưa mật khẩu thật vào Git.

#### Bước 4: Kiểm tra kết nối

Sau khi cấu hình, chạy API ở phần 7. Nếu gặp lỗi kết nối database, kiểm tra:

- Tên server hoặc instance.
- Database `HealthPlus` đã được tạo chưa.
- Tài khoản có quyền truy cập database không.
- SQL Server Authentication hoặc Windows Authentication có đúng không.
- Cổng TCP và firewall nếu kết nối qua mạng.

## 7. Build và chạy toàn bộ website bằng API

Đây là cách chạy được khuyến nghị khi demo hoặc trình bày đồ án.

### Bước 1: Mở terminal tại thư mục gốc

```bash
cd HealthPlusWeb
```

Đảm bảo cấu trúc có cả `HealthPlus.API` và `HealthPlus.Client`.

### Bước 2: Restore dependencies

```bash
dotnet restore HealthPlus.API/HealthPlus.API.csproj
```

### Bước 3: Build API và frontend

```bash
dotnet build HealthPlus.API/HealthPlus.API.csproj
```

Trong cấu hình hiện tại, target MSBuild của API sẽ publish Blazor Client và copy các file frontend vào `HealthPlus.API/wwwroot`.

Sau khi build, thư mục `HealthPlus.API/wwwroot` phải có `index.html`, `_framework/` và các asset frontend bên cạnh thư mục `images/` và `uploads/`.

Nếu build thất bại, xử lý lỗi build trước khi chạy ứng dụng.

### Bước 4: Chạy API

```bash
dotnet run --project HealthPlus.API/HealthPlus.API.csproj --urls http://localhost:5204
```

Mở trình duyệt:

- Website: `http://localhost:5204/`
- Swagger: `http://localhost:5204/swagger` trong môi trường Development.
- Danh sách sản phẩm: `http://localhost:5204/api/products`.

Nếu dùng GitHub Codespaces, mở cổng `5204` trong tab **Ports** và truy cập bằng URL được Codespaces cung cấp.

### Bước 5: Kiểm tra các chức năng

1. Mở trang chủ HealthPlus.
2. Truy cập danh sách sản phẩm.
3. Đăng nhập bằng tài khoản demo hợp lệ.
4. Kiểm tra giỏ hàng và đặt hàng.
5. Đăng nhập Admin và kiểm tra các trang quản trị.
6. Thử thêm sản phẩm và upload ảnh.

Không cần chạy thêm `HealthPlus.Client` khi dùng cách host chung này.

## 8. Publish để chạy bản demo

Tại thư mục gốc:

```bash
dotnet publish HealthPlus.API/HealthPlus.API.csproj -c Release -o ./publish
```

Sau khi publish thành công, thư mục `publish/wwwroot` phải có `index.html`, `_framework/` và các file frontend.

Chạy bản publish:

```bash
dotnet ./publish/HealthPlus.API.dll --urls http://localhost:5204
```

Đảm bảo cấu hình connection string và các thiết lập môi trường cần thiết vẫn được cung cấp cho bản publish.

Nếu cần phân phối cho máy khác, không gửi mật khẩu database trong repository. Cấu hình connection string riêng trên máy chạy ứng dụng.

## 9. Upload và lưu ảnh sản phẩm

Ảnh sản phẩm được upload qua API, lưu trong:

```text
HealthPlus.API/wwwroot/uploads/products/
```

Database chỉ lưu URL ảnh trong cột `ImageUrl`, ví dụ:

```text
http://localhost:5204/uploads/products/<generated-file-name>.jpg
```

Khi API phục vụ frontend trên cùng origin, client nên dùng địa chỉ gốc hiện tại để gọi API.

Lưu ý:

- Thư mục upload cần có quyền ghi.
- Không xóa thư mục `uploads/products` khi build hoặc cập nhật frontend.
- Nếu triển khai trên container hoặc máy chủ khác, cần bảo đảm thư mục upload được lưu bền vững và được sao lưu khi cần.
- URL ảnh chứa hostname của môi trường phát triển có thể không hoạt động khi chuyển sang môi trường khác; cần cập nhật cách tạo URL hoặc cấu hình URL ảnh phù hợp.

## 10. Tài khoản demo

Sử dụng tài khoản được tạo bởi `03_SeedData.sql` hoặc tài khoản đã đăng ký qua giao diện.

Nếu script seed không tạo sẵn tài khoản Admin, hãy tạo hoặc cập nhật tài khoản bằng quy trình được hỗ trợ trong dự án.

Không nên ghi mật khẩu cá nhân hoặc thông tin đăng nhập production trong README công khai.

## 11. Xử lý lỗi thường gặp

### Không kết nối được SQL Server

Kiểm tra connection string, trạng thái SQL Server, tên instance, quyền truy cập và cổng kết nối.

### Lỗi `Login failed for user`

Kiểm tra username/password, chế độ authentication và quyền của tài khoản trên database `HealthPlus`.

### Lỗi không tìm thấy database hoặc bảng

Chạy các script tạo database, bảng và dữ liệu mẫu theo đúng thứ tự. Xác nhận database đang được tạo trên đúng SQL Server instance mà API kết nối đến.

### Website đứng ở 97% khi khởi động Blazor

Kiểm tra các request trong DevTools → Network, đặc biệt các file trong `_framework/`.

Blazor WebAssembly cần tải được các file runtime và manifest với đúng nội dung. Với cấu hình hiện tại, ASP.NET Core static files cần hỗ trợ các đuôi runtime như `.dat`, `.blat`, `.webcil` và `.wasm`.

### Ảnh sản phẩm không hiển thị

Kiểm tra `ImageUrl`, URL ảnh trên trình duyệt và file thực tế trong `wwwroot/uploads/products/` hoặc `wwwroot/images/`.

### Trang Admin không truy cập được

Đăng nhập bằng tài khoản có role `Admin`. Kiểm tra JWT, claim role và authorization policy của frontend/backend.

### Build không tìm thấy project hoặc solution

Đảm bảo terminal đang ở thư mục gốc chứa `HealthPlus.API/HealthPlus.API.csproj` và `HealthPlus.Client/HealthPlus.Client.csproj`.

### Docker Compose báo `version is obsolete`

Có thể xóa thuộc tính `version:` ở đầu `docker-compose.yml` với các phiên bản Docker Compose hiện đại. Cảnh báo này thường không phải nguyên nhân khiến SQL Server không khởi động.

## 12. Git và các file không nên commit

Không commit:

- File `.env` chứa mật khẩu.
- Mật khẩu database hoặc JWT signing key thật.
- Các file build sinh ra như `bin/` và `obj/`.
- Thông tin bí mật của môi trường cá nhân.

Nên giữ trong repository:

- Source code API và Client.
- Các script SQL tạo database, bảng và seed data.
- `docker-compose.yml` không chứa secret cố định.
- README và hướng dẫn cấu hình môi trường.

## 13. Quy trình demo nhanh

Trước khi trình bày:

1. Khởi động SQL Server hoặc Docker SQL Server.
2. Xác nhận database `HealthPlus` đã có bảng và dữ liệu mẫu.
3. Kiểm tra connection string.
4. Chạy `dotnet build HealthPlus.API/HealthPlus.API.csproj`.
5. Chạy API tại cổng `5204`.
6. Mở website và kiểm tra sản phẩm.
7. Đăng nhập khách hàng, thử giỏ hàng và đặt hàng.
8. Đăng nhập Admin, thử quản lý sản phẩm, upload ảnh và xử lý đơn hàng.
9. Kiểm tra các số liệu trên dashboard.

---

**HealthPlus** — Đồ án website thương mại điện tử thực phẩm bổ sung.
