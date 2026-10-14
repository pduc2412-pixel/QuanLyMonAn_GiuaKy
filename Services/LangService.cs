namespace DishManagement.Services;

/// <summary>Đa ngôn ngữ tự viết: ngôn ngữ lưu trong cookie "lang" (en | vi). Mặc định: en.</summary>
public class LangService
{
    public const string CookieName = "lang";
    private readonly IHttpContextAccessor _http;

    public LangService(IHttpContextAccessor http) => _http = http;

    public string Current
    {
        get
        {
            var c = _http.HttpContext?.Request.Cookies[CookieName];
            return c == "vi" ? "vi" : "en";
        }
    }

    public bool IsVi => Current == "vi";

    public string this[string key] => T(key);
    public string Cat(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return name ?? "";
        foreach (var (en, vi) in CategoryNames)
            if (string.Equals(name.Trim(), en, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name.Trim(), vi, StringComparison.OrdinalIgnoreCase))
                return IsVi ? vi : en;
        return name;
    }

    private static readonly (string en, string vi)[] CategoryNames =
    {
    ("Main dishes", "Món chính"),
    ("Appetizers", "Khai vị"),
    ("Drinks", "Đồ uống"),
    ("Desserts", "Tráng miệng"),
};

    public string T(string key, params object[] args)
    {
        if (!Dict.TryGetValue(key, out var p)) return key;
        var s = IsVi ? p.vi : p.en;
        return args.Length > 0 ? string.Format(s, args) : s;
    }

    private static readonly Dictionary<string, (string en, string vi)> Dict = new()
    {
        // App / menu
        ["app_title"] = ("Dish Management", "Quản lý món ăn"),
        ["nav_dishes"] = ("Dishes", "Món ăn"),
        ["nav_categories"] = ("Categories", "Loại món"),
        ["footer"] = ("Dish Management - Midterm", "Quản lý món ăn - Giữa kỳ"),
        ["lang_en"] = ("English", "English"),
        ["lang_vi"] = ("Tiếng Việt", "Tiếng Việt"),

        // Titles
        ["title_dishes"] = ("Dish list", "Danh sách món ăn"),
        ["title_create_dish"] = ("Create dish", "Thêm món ăn"),
        ["title_edit_dish"] = ("Edit dish", "Sửa món ăn"),
        ["title_dish_details"] = ("Dish details", "Chi tiết món ăn"),
        ["title_categories"] = ("Dish categories", "Danh sách loại món"),
        ["title_create_category"] = ("Create category", "Thêm loại món"),
        ["title_edit_category"] = ("Edit category", "Sửa loại món"),
        ["title_images"] = ("Images", "Hình ảnh"),
        ["title_not_found"] = ("Not found", "Không tìm thấy"),
        ["title_error"] = ("Something went wrong", "Đã xảy ra lỗi"),

        // Buttons
        ["btn_create_dish"] = ("Add dish", "Thêm món ăn"),
        ["btn_create_category"] = ("Add category", "Thêm loại món"),
        ["btn_create"] = ("Create", "Tạo mới"),
        ["btn_save"] = ("Save", "Lưu"),
        ["btn_edit"] = ("Edit", "Sửa"),
        ["btn_delete"] = ("Delete", "Xóa"),
        ["btn_details"] = ("Details", "Chi tiết"),
        ["btn_back"] = ("Back to list", "Quay lại danh sách"),
        ["btn_search"] = ("Search / Filter", "Tìm kiếm / Lọc"),
        ["btn_reset"] = ("Reset", "Đặt lại"),
        ["btn_add_image"] = ("Add image", "Thêm ảnh"),
        ["btn_set_thumb"] = ("Set as thumbnail", "Đặt làm ảnh đại diện"),
        ["btn_cancel"] = ("Cancel", "Hủy"),

        // Fields
        ["f_name"] = ("Name", "Tên"),
        ["f_price"] = ("Price", "Giá"),
        ["f_prep_time"] = ("Preparation time (min)", "Thời gian chế biến (phút)"),
        ["f_available"] = ("Available", "Đang bán"),
        ["f_description"] = ("Description", "Mô tả"),
        ["f_category"] = ("Category", "Loại món"),
        ["f_image_url"] = ("Image URL", "Đường dẫn ảnh"),
        ["f_thumbnail"] = ("Thumbnail", "Ảnh đại diện"),
        ["f_status"] = ("Status", "Trạng thái"),
        ["f_prep_class"] = ("Preparation type", "Phân loại chế biến"),
        ["f_dish_count"] = ("Dishes", "Số món"),
        ["f_image"] = ("Image", "Ảnh"),
        ["f_actions"] = ("Actions", "Thao tác"),

        // Filters
        ["filter_search"] = ("Search by name", "Tìm theo tên món"),
        ["filter_category"] = ("Category", "Loại món"),
        ["filter_all_categories"] = ("All categories", "Tất cả loại món"),
        ["filter_status"] = ("Status", "Trạng thái"),
        ["filter_all_status"] = ("All", "Tất cả"),
        ["filter_min_price"] = ("Min price", "Giá từ"),
        ["filter_max_price"] = ("Max price", "Giá đến"),
        ["filter_sort"] = ("Sort by", "Sắp xếp"),
        ["sort_default"] = ("Name (A-Z)", "Tên (A-Z)"),
        ["sort_price_asc"] = ("Price: low to high", "Giá: thấp đến cao"),
        ["sort_price_desc"] = ("Price: high to low", "Giá: cao đến thấp"),
        ["sort_time_asc"] = ("Preparation time: shortest first", "Thời gian chế biến: ngắn nhất trước"),

        // Status / classification
        ["status_available"] = ("Available", "Đang bán"),
        ["status_unavailable"] = ("Unavailable", "Ngừng bán"),
        ["prep_quick"] = ("Quick", "Nhanh"),
        ["prep_regular"] = ("Regular", "Thông thường"),
        ["minutes"] = ("min", "phút"),
        ["badge_thumbnail"] = ("Thumbnail", "Ảnh đại diện"),

        // Misc texts
        ["select_category"] = ("-- Select category --", "-- Chọn loại món --"),
        ["no_results"] = ("No dishes match your search.", "Không tìm thấy món ăn phù hợp."),
        ["no_categories"] = ("There are no categories yet.", "Chưa có loại món nào."),
        ["no_image"] = ("No image", "Chưa có ảnh"),
        ["no_images_yet"] = ("This dish has no images yet.", "Món này chưa có ảnh."),
        ["confirm_delete"] = ("Are you sure you want to delete this item?", "Bạn có chắc muốn xóa mục này?"),
        ["back_home"] = ("Back to dish list", "Về danh sách món ăn"),

        // Success messages
        ["msg_dish_created"] = ("Dish created successfully.", "Đã thêm món ăn thành công."),
        ["msg_dish_updated"] = ("Dish updated successfully.", "Đã cập nhật món ăn thành công."),
        ["msg_dish_deleted"] = ("Dish deleted successfully.", "Đã xóa món ăn thành công."),
        ["msg_cat_created"] = ("Category created successfully.", "Đã thêm loại món thành công."),
        ["msg_cat_updated"] = ("Category updated successfully.", "Đã cập nhật loại món thành công."),
        ["msg_cat_deleted"] = ("Category deleted successfully.", "Đã xóa loại món thành công."),
        ["msg_image_added"] = ("Image added successfully.", "Đã thêm ảnh thành công."),
        ["msg_thumb_set"] = ("Thumbnail updated.", "Đã đổi ảnh đại diện."),
        ["msg_image_deleted"] = ("Image deleted.", "Đã xóa ảnh."),

        // Errors
        ["err_cat_in_use"] = ("Cannot delete this category because it still has dishes.", "Không thể xóa loại món vì vẫn còn món ăn thuộc loại này."),
        ["err_dish_nf"] = ("Dish not found.", "Không tìm thấy món ăn."),
        ["err_cat_nf"] = ("Category not found.", "Không tìm thấy loại món."),
        ["err_image_nf"] = ("Image not found.", "Không tìm thấy ảnh."),
        ["err_page_nf"] = ("Page not found.", "Không tìm thấy trang."),
        ["err_generic"] = ("An unexpected error occurred. Please try again.", "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại."),
        ["err_price_range"] = ("Invalid price range: prices must be non-negative numbers and the minimum must not exceed the maximum. The price filter was ignored.",
                               "Khoảng giá không hợp lệ: giá phải là số không âm và giá từ không được lớn hơn giá đến. Bộ lọc giá đã bị bỏ qua."),

        // Validation
        ["val_name_required"] = ("Name is required.", "Tên không được để trống."),
        ["val_price"] = ("Price must be a number greater than 0.", "Giá phải là số lớn hơn 0."),
        ["val_prep"] = ("Preparation time must be a whole number greater than 0.", "Thời gian chế biến phải là số nguyên lớn hơn 0."),
        ["val_category_required"] = ("Please select a category.", "Vui lòng chọn loại món."),
        ["val_category_nf"] = ("The selected category does not exist.", "Loại món đã chọn không tồn tại."),
        ["val_name_dup"] = ("A dish with this name already exists in this category.", "Tên món ăn đã tồn tại trong loại món này."),
        ["val_too_long"] = ("The value is too long.", "Giá trị quá dài."),
        ["val_invalid"] = ("The value is invalid.", "Giá trị không hợp lệ."),
        ["val_image_required"] = ("Image URL is required.", "Đường dẫn ảnh không được để trống."),
        ["val_image_url"] = ("Image URL must be a valid http(s) link or a path starting with '/'.", "Đường dẫn ảnh phải là liên kết http(s) hợp lệ hoặc đường dẫn bắt đầu bằng '/'."),
    };
}
