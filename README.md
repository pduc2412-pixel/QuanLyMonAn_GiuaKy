# Quản lý món ăn - ASP.NET Core MVC + EF Core (SQL Server)

## 1. MSSV: BIT240069 (đã được áp dụng sẵn)

## 2. Chạy
Yêu cầu: .NET 10 SDK + SQL Server LocalDB (đi kèm Visual Studio).


    dotnet restore
    dotnet run

Mở http://localhost:5000 (xem địa chỉ in ra ở console).
Lần chạy đầu tiên app tự tạo database `<MSSV>_MID`, 3 bảng, 3 loại món, 7 món mẫu.
Dùng SQL Server khác: sửa `ConnectionStrings:Default` trong `appsettings.json`.
Muốn tạo lại dữ liệu: xóa database `<MSSV>_MID` rồi chạy lại.

## 3. Cấu trúc
- Models/            3 Model có hậu tố MSSV, [Table]/[Column] đặt tên bảng + cột có tiền tố MSSV
- Data/              AppDbContext (quan hệ, unique index tên món theo loại), SeedData
- Services/LangService.cs   Đa ngôn ngữ en/vi (cookie `lang`, mặc định English)
- Controllers/       Dishes (list/lọc/CRUD/ảnh), DishCategories, Home (đổi ngôn ngữ, 404)
- Views/             Razor views

## 4. Đối chiếu đề
| Yêu cầu | Nằm ở đâu |
|---|---|
| Tên DB/bảng/cột/Model có MSSV | Models/*.cs, appsettings.json |
| Validation (Name, Price>0, Prep>0, category tồn tại, tên không trùng trong loại) | attribute trong Models + `ValidateDish` trong DishesController |
| Tìm kiếm / lọc / khoảng giá / sắp xếp, giữ điều kiện, lọc ở DB | `DishesController.Index` (IQueryable) + form GET trong Views/Dishes/Index |
| Quick/Regular tính khi hiển thị | `Dish.PrepClassKey` ([NotMapped]) |
| Ảnh: thêm URL, nhiều ảnh, đổi ảnh đại diện (tối đa 1) | AddImage / SetThumbnail / DeleteImage |
| Không xóa loại đang có món | `DishCategoriesController.Delete` (+ FK Restrict) |
| Món ngừng bán vẫn hiện, lọc Available thì ẩn | Index: Status filter |
| Xử lý Id không tồn tại, khoảng giá sai, món chưa có ảnh, không có kết quả | NotFoundPage, PriceRangeInvalid, các view |
| Đa ngôn ngữ, giữ ngôn ngữ + điều kiện lọc | LangService, Home/SetLanguage (returnUrl kèm query string) |

## 5. Gợi ý quay demo
1. SSMS: cho thấy database/bảng/cột có MSSV; mở thư mục Models.
2. Danh sách: tìm "bún", lọc loại/giá/trạng thái, sắp xếp; bấm đổi ngôn ngữ -> điều kiện lọc vẫn còn.
3. Món "Cơm tấm sườn" (15 phút = Quick) và "Nem rán" (25 phút = Regular); thử Create/Edit.
4. Details: thêm ảnh bằng URL, đổi ảnh đại diện.
5. Xóa loại món đang có món -> báo lỗi; vào /Dishes/Details/999 -> trang 404.
6. Mở lần đầu bằng cửa sổ ẩn danh (English) -> chuyển Tiếng Việt -> submit form rỗng xem validation tiếng Việt.
