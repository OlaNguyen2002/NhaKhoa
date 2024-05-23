using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DentalClinic.Models
{
    public class JsonResult
    {
        public string status { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public override string ToString()
        {
            string content = "{\"status\":\"" + this.status + "\",\"data\":null ,\"message\":\"" + this.message + "\"  }";
            return content;
        }
        public static class Status
        {
            public const string SUCCESS = "success";
            public const string ERROR = "error";
            public const string UNAUTHORIZED = "unauthorized";
            public const string UNAUTHENTICATED = "unauthenticated";
        }
        public static class Message
        {
            public const string ERROR_SYSTEM = "Có lỗi xảy ra trong quá trình xử lý.";
            public const string TOKEN_EXPIRED = "Token đã hết hạn.";
            public const string UPDATED_SUCCESS = "Cập nhật thành công.";
            public const string DELETED_SUCCESS = "Xóa thành công.";
            public const string LOGIN_ACCOUNT_OR_PASSWORD_EMPTY = "Số điện thoại/Email/Tên tài khoản hoặc mật khẩu không được để trống.";
            public const string LOGIN_ACCOUNT_OR_PASSWORD_INCORRECT = "Số điện thoại/Email/Tên tài khoản hoặc mật khẩu không chính xác.";
            public const string NO_PERMISSION = "Không có quyền truy cập.";
        }
    }
    public class SearchModel
    {
        public int CurrentPage { get; set; }
        public string OrderBy { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
    public class Constant
    {
        public const int PAGE_NEWS_SIZE = 6;
        public const int PAGE_SIZE = 25;
        public const int ADMIN_PAGE_SIZE = 20;
        public const int USER_PAGE_SIZE = 20;

        public const int MINIMUM_BUY_PRODUCT_QUANTITY_FOR_REBATE = 80;
        public const int TOTAL_CASH_REBATE = 100000000;

        public const string PRODUCT_THUMBNAIL_URL = "/files/product/thumbnail/";
        public const string PRODUCT_THUMBNAIL_PATH = "~/files/product/thumbnail/";
        public const string PRODUCT_IMAGE_URL = "/files/product/productimage/";
        public const string PRODUCT_IMAGE_PATH = "~/files/product/productimage/";


        public const string AVATAR_USER_URL = "/files/avatar/";
        public const string AVATAR_USER_PATH = "~/files/avatar/";

        public const string AVATAR_STAFF_URL = "/files/avatar/staffhonor/";
        public const string AVATAR_STAFF_PATH = "~/files/avatar/staffhonor/";

        public const string SLIDE_IMAGE_URL = "/files/slides/";
        public const string SLIDE_IMAGE_PATH = "~/files/slides/";

        public const string NEWS_THUMBNAIL_URL = "/files/news/";
        public const string NEWS_THUMBNAIL_PATH = "~/files/news/";

        public const string DOCTOR_THUMBNAIL_URL = "/files/doctor/";
        public const string DOCTOR_THUMBNAIL_PATH = "~/files/doctor/";

        public const string SERVICE_THUMBNAIL_URL = "/files/service/";
        public const string SERVICE_THUMBNAIL_PATH = "~/files/service/";

        public const string IDENTITY_THUMBNAIL_URL = "/files/identity/";
        public const string IDENTITY_THUMBNAIL_PATH = "~/files/identity/";
    }
}