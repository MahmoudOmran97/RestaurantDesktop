using Newtonsoft.Json;
using RestaurantDesktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDesktop.Services
{
    public class ApiService
    {
        private static readonly HttpClient _http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = delegate { return true; }
        });

        private static void SetAuth()
        {
            if (!string.IsNullOrEmpty(AppSession.Token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AppSession.Token);
            }
        }

        private static StringContent Json(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        private static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            // مهم: الـ API بيرجع camelCase (data, total, pageSize)
            // لكن الـ model بـ PascalCase (Data, Total, PageSize)
            // CamelCasePropertyNamesContractResolver بيعمل match للاتنين
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        // ── Auth ────────────────────────────────────────────────────────────────

        public static async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.Email = email;
                loginRequest.Password = password;

                HttpResponseMessage res = await _http.PostAsync(
                    AppConfig.ApiBaseUrl + "/auth/login",
                    Json(loginRequest));

                string body = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    LoginResult errorResult = new LoginResult();
                    errorResult.Ok = false;
                    errorResult.Data = null;
                    errorResult.Error = "خطأ في تسجيل الدخول";
                    return errorResult;
                }

                LoginResponse data = Deserialize<LoginResponse>(body);
                LoginResult successResult = new LoginResult();
                successResult.Ok = true;
                successResult.Data = data;
                successResult.Error = string.Empty;
                return successResult;
            }
            catch (Exception ex)
            {
                LoginResult errorResult = new LoginResult();
                errorResult.Ok = false;
                errorResult.Data = null;
                errorResult.Error = "لا يمكن الاتصال بالسيرفر: " + ex.Message;
                return errorResult;
            }
        }

        // ── Restaurants ────────────────────────────────────────────────────────

        public static async Task<PagedResult<RestaurantDto>> GetRestaurantsAsync(int page = 1, int pageSize = 50)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/restaurants?page=" + page + "&pageSize=" + pageSize;
            HttpResponseMessage res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<PagedResult<RestaurantDto>>(await res.Content.ReadAsStringAsync());
        }

        public static async Task<RestaurantDto> GetRestaurantAsync(int id)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/restaurants/" + id;
            HttpResponseMessage res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<RestaurantDto>(await res.Content.ReadAsStringAsync());
        }

        // ── Menu ───────────────────────────────────────────────────────────────

        public static async Task<List<CategoryDto>> GetMenuAsync(int restaurantId)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/restaurants/" + restaurantId + "/menu";
            HttpResponseMessage res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<List<CategoryDto>>(await res.Content.ReadAsStringAsync());
        }

        public static async Task<ProductResult> CreateProductAsync(CreateProductRequest req)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/products";
            HttpResponseMessage res = await _http.PostAsync(url, Json(req));

            if (res.IsSuccessStatusCode)
            {
                ProductResult successResult = new ProductResult();
                successResult.Ok = true;
                successResult.Error = string.Empty;
                return successResult;
            }

            ProductResult errorResult = new ProductResult();
            errorResult.Ok = false;
            errorResult.Error = "فشل إضافة المنتج";
            return errorResult;
        }

        public static async Task<ProductResult> UpdateProductAsync(int id, CreateProductRequest req)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/products/" + id;
            HttpResponseMessage res = await _http.PutAsync(url, Json(req));

            if (res.IsSuccessStatusCode)
            {
                ProductResult successResult = new ProductResult();
                successResult.Ok = true;
                successResult.Error = string.Empty;
                return successResult;
            }

            ProductResult errorResult = new ProductResult();
            errorResult.Ok = false;
            errorResult.Error = "فشل تعديل المنتج";
            return errorResult;
        }

        public static async Task<bool> ToggleProductAvailabilityAsync(int id)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/products/" + id + "/toggle-availability";
            HttpResponseMessage res = await _http.PutAsync(url, null);
            return res.IsSuccessStatusCode;
        }

        public static async Task<bool> DeleteProductAsync(int id)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/products/" + id;
            HttpResponseMessage res = await _http.DeleteAsync(url);
            return res.IsSuccessStatusCode;
        }

        public static async Task<ProductResult> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/restaurants/" + id + "/desktop-update";
            HttpResponseMessage res = await _http.PutAsync(url, Json(dto));

            if (res.IsSuccessStatusCode)
            {
                ProductResult successResult = new ProductResult();
                successResult.Ok = true;
                successResult.Error = string.Empty;
                return successResult;
            }

            string body = await res.Content.ReadAsStringAsync();
            ProductResult errorResult = new ProductResult();
            errorResult.Ok = false;
            errorResult.Error = "فشل تحديث بيانات المطعم: " + res.StatusCode;
            return errorResult;
        }

        public static async Task<bool> ToggleRestaurantStatusAsync(int id)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/restaurants/" + id + "/toggle-status";
            HttpResponseMessage res = await _http.PutAsync(url, null);
            return res.IsSuccessStatusCode;
        }

        // ── Orders (المعدلة لجلب كل الأوردرات) ─────────────────────────────────

        public static async Task<List<OrderDetail>> GetRestaurantOrdersAsync(
            int restaurantId, string status = null, int page = 1, int pageSize = 100)
        {
            SetAuth();
            string url = AppConfig.ApiBaseUrl + "/orders/restaurant/" + restaurantId +
                      "?page=" + page + "&pageSize=" + pageSize;

            if (!string.IsNullOrEmpty(status))
                url = url + "&status=" + status;

            HttpResponseMessage res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return new List<OrderDetail>();

            string body = await res.Content.ReadAsStringAsync();
            PagedResult<OrderDetail> paged = Deserialize<PagedResult<OrderDetail>>(body);

            if (paged != null && paged.Data != null)
                return paged.Data;
            else
                return new List<OrderDetail>();
        }

        // دالة جديدة: تجلب كل الأوردرات (بتقسيمها على صفحات)
        public static async Task<List<OrderDetail>> GetAllRestaurantOrdersAsync(
            int restaurantId, string status = null)
        {
            List<OrderDetail> allOrders = new List<OrderDetail>();
            int page = 1;
            int pageSize = 100;

            while (true)
            {
                List<OrderDetail> orders = await GetRestaurantOrdersAsync(restaurantId, status, page, pageSize);

                if (orders == null || orders.Count == 0)
                    break;

                allOrders.AddRange(orders);

                if (orders.Count < pageSize)
                    break;

                page++;
            }

            return allOrders;
        }

        public static async Task<OrderStatusResult> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            // لا حاجة لـ SetAuth — endpoint مخصص للمطعم بدون تسجيل دخول
            string url = AppConfig.ApiBaseUrl + "/orders/" + orderId + "/restaurant-status";

            UpdateStatusRequest statusRequest = new UpdateStatusRequest();
            statusRequest.Status = newStatus;

            HttpResponseMessage res = await _http.PutAsync(url, Json(statusRequest));

            if (res.IsSuccessStatusCode)
            {
                OrderStatusResult successResult = new OrderStatusResult();
                successResult.Ok = true;
                successResult.Error = string.Empty;
                return successResult;
            }

            string body = await res.Content.ReadAsStringAsync();
            OrderStatusResult errorResult = new OrderStatusResult();
            errorResult.Ok = false;
            errorResult.Error = "فشل تحديث الحالة: " + res.StatusCode;
            return errorResult;
        }

        // ── Dashboard ─────────────────────────────────────────────────────────

        public static async Task<DashboardStats> GetDashboardStatsAsync(int restaurantId)
        {
            List<OrderDetail> orders = await GetAllRestaurantOrdersAsync(restaurantId);
            if (orders == null) return new DashboardStats();

            DateTime today = DateTime.Today;

            int todayOrdersCount = 0;
            decimal todayRevenue = 0;
            int pendingOrdersCount = 0;
            int preparingOrdersCount = 0;

            foreach (OrderDetail order in orders)
            {
                if (order.CreatedAt.Date == today)
                {
                    todayOrdersCount++;
                    if (order.Status == "Delivered")
                        todayRevenue = todayRevenue + order.TotalAmount;
                }

                if (order.Status == "Pending")
                    pendingOrdersCount++;

                if (order.Status == "Accepted" || order.Status == "Preparing")
                    preparingOrdersCount++;
            }

            DashboardStats stats = new DashboardStats();
            stats.TodayOrders = todayOrdersCount;
            stats.TodayRevenue = todayRevenue;
            stats.PendingOrders = pendingOrdersCount;
            stats.PreparingOrders = preparingOrdersCount;
            stats.TotalProducts = 0;

            return stats;
        }
    }

    public class LoginResult
    {
        public bool Ok { get; set; }
        public LoginResponse Data { get; set; }
        public string Error { get; set; }
    }

    public class ProductResult
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
    }

    public class OrderStatusResult
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
    }
}