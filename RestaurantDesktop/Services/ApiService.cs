using Newtonsoft.Json;
using RestaurantDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDesktop.Services
{
    
public class ApiService
    {
        private static readonly HttpClient _http = new(new HttpClientHandler
        {
            // تجاهل SSL في التطوير — شيله في Production
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });

        private static void SetAuth()
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AppSession.Token);
        }

        private static StringContent Json(object obj) =>
            new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        private static T? Deserialize<T>(string json) =>
            JsonConvert.DeserializeObject<T>(json);

        // ── Auth ──────────────────────────────────────────────────────────────────

        public static async Task<(bool ok, LoginResponse? data, string error)> LoginAsync(
            string email, string password)
        {
            try
            {
                var res = await _http.PostAsync(
                    $"{AppConfig.ApiBaseUrl}/auth/login",
                    Json(new LoginRequest { Email = email, Password = password }));

                var body = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    dynamic? err = Deserialize<dynamic>(body);
                    return (false, null, (string?)err?.message ?? "خطأ في تسجيل الدخول");
                }

                return (true, Deserialize<LoginResponse>(body), string.Empty);
            }
            catch (Exception ex)
            {
                return (false, null, $"لا يمكن الاتصال بالسيرفر: {ex.Message}");
            }
        }

        // ── Restaurants ───────────────────────────────────────────────────────────

        public static async Task<PagedResult<RestaurantDto>?> GetRestaurantsAsync(
            int page = 1, int pageSize = 50)
        {
            SetAuth();
            var res = await _http.GetAsync(
                $"{AppConfig.ApiBaseUrl}/restaurants?page={page}&pageSize={pageSize}");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<PagedResult<RestaurantDto>>(
                await res.Content.ReadAsStringAsync());
        }

        public static async Task<RestaurantDto?> GetRestaurantAsync(int id)
        {
            SetAuth();
            var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/restaurants/{id}");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<RestaurantDto>(await res.Content.ReadAsStringAsync());
        }

        public static async Task<bool> ToggleRestaurantStatusAsync(int id)
        {
            SetAuth();
            var res = await _http.PutAsync(
                $"{AppConfig.ApiBaseUrl}/restaurants/{id}/toggle-status", null);
            return res.IsSuccessStatusCode;
        }

        // ── Menu ──────────────────────────────────────────────────────────────────

        public static async Task<List<CategoryDto>?> GetMenuAsync(int restaurantId)
        {
            SetAuth();
            var res = await _http.GetAsync(
                $"{AppConfig.ApiBaseUrl}/restaurants/{restaurantId}/menu");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<List<CategoryDto>>(await res.Content.ReadAsStringAsync());
        }

        public static async Task<(bool ok, string error)> CreateProductAsync(
            CreateProductRequest req)
        {
            SetAuth();
            var res = await _http.PostAsync(
                $"{AppConfig.ApiBaseUrl}/products", Json(req));
            if (res.IsSuccessStatusCode) return (true, string.Empty);
            var body = await res.Content.ReadAsStringAsync();
            dynamic? err = Deserialize<dynamic>(body);
            return (false, (string?)err?.message ?? "فشل إضافة المنتج");
        }

        public static async Task<(bool ok, string error)> UpdateProductAsync(
            int id, CreateProductRequest req)
        {
            SetAuth();
            var res = await _http.PutAsync(
                $"{AppConfig.ApiBaseUrl}/products/{id}", Json(req));
            if (res.IsSuccessStatusCode) return (true, string.Empty);
            return (false, "فشل تعديل المنتج");
        }

        public static async Task<bool> ToggleProductAvailabilityAsync(int id)
        {
            SetAuth();
            var res = await _http.PutAsync(
                $"{AppConfig.ApiBaseUrl}/products/{id}/toggle-availability", null);
            return res.IsSuccessStatusCode;
        }

        public static async Task<bool> DeleteProductAsync(int id)
        {
            SetAuth();
            var res = await _http.DeleteAsync(
                $"{AppConfig.ApiBaseUrl}/products/{id}");
            return res.IsSuccessStatusCode;
        }

        // ── Orders ────────────────────────────────────────────────────────────────

        /// <summary>
        /// جيب أوردرات المطعم — الـ API بيرجع my orders للـ customer
        /// هنا بنجيب كل الأوردرات الخاصة بالمطعم ده عبر /orders بالفلترة
        /// </summary>
        public static async Task<List<OrderDetail>?> GetRestaurantOrdersAsync(
            int restaurantId, string? status = null,
            int page = 1, int pageSize = 50)
        {
            SetAuth();
            var url = $"{AppConfig.ApiBaseUrl}/orders/restaurant/{restaurantId}" +
                      $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";

            var res = await _http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return new List<OrderDetail>();

            var body = await res.Content.ReadAsStringAsync();
            var paged = Deserialize<PagedResult<OrderDetail>>(body);
            return paged?.Data ?? new List<OrderDetail>();
        }

        public static async Task<OrderDetail?> GetOrderDetailAsync(int orderId)
        {
            SetAuth();
            var res = await _http.GetAsync($"{AppConfig.ApiBaseUrl}/orders/{orderId}");
            if (!res.IsSuccessStatusCode) return null;
            return Deserialize<OrderDetail>(await res.Content.ReadAsStringAsync());
        }

        public static async Task<(bool ok, string error)> UpdateOrderStatusAsync(
            int orderId, string newStatus)
        {
            SetAuth();
            var res = await _http.PutAsync(
                $"{AppConfig.ApiBaseUrl}/orders/{orderId}/status",
                Json(new UpdateStatusRequest { Status = newStatus }));

            if (res.IsSuccessStatusCode) return (true, string.Empty);
            var body = await res.Content.ReadAsStringAsync();
            dynamic? err = Deserialize<dynamic>(body);
            return (false, (string?)err?.message ?? "فشل تحديث الحالة");
        }

        // ── Dashboard (نحسبها من بيانات الأوردرات) ───────────────────────────────

        public static async Task<DashboardStats> GetDashboardStatsAsync(int restaurantId)
        {
            var orders = await GetRestaurantOrdersAsync(restaurantId, pageSize: 200);
            if (orders == null) return new DashboardStats();

            var today = DateTime.Today;
            var todayOrders = orders.Where(o => o.CreatedAt.Date == today).ToList();

            return new DashboardStats
            {
                TodayOrders = todayOrders.Count,
                TodayRevenue = todayOrders
                                    .Where(o => o.Status == "Delivered")
                                    .Sum(o => o.TotalAmount),
                PendingOrders = orders.Count(o => o.Status == "Pending"),
                PreparingOrders = orders.Count(o =>
                                    o.Status == "Accepted" || o.Status == "Preparing"),
                TotalProducts = 0, // بيتحسب من المنيو
            };
        }
    }
    }
