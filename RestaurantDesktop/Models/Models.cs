using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestaurantDesktop;

namespace RestaurantDesktop.Models
{
    // ─── Auth ────────────────────────────────────────────────────────────────────

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public LoginRequest()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public LoginResponse()
        {
            Token = string.Empty;
            FullName = string.Empty;
            Email = string.Empty;
            Role = string.Empty;
        }
    }

    // ─── Restaurant ───────────────────────────────────────────────────────────────

    public class RestaurantDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("coverImageUrl")]
        public string CoverImageUrl { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("totalRatings")]
        public int TotalRatings { get; set; }

        [JsonProperty("deliveryFee")]
        public decimal DeliveryFee { get; set; }

        [JsonProperty("minOrderAmount")]
        public decimal MinOrderAmount { get; set; }

        [JsonProperty("estimatedTime")]
        public int EstimatedTime { get; set; }

        [JsonProperty("isOpen")]
        public bool IsOpen { get; set; }

        // الـ full URL للصور (مشتقة من ImageUrl)
        public string FullImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl)) return null;
                if (ImageUrl.StartsWith("http")) return ImageUrl;
                return AppConfig.BaseUrl.TrimEnd('/') + "/" + ImageUrl.TrimStart('/');
            }
        }

        public string FullCoverImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(CoverImageUrl)) return null;
                if (CoverImageUrl.StartsWith("http")) return CoverImageUrl;
                return AppConfig.BaseUrl.TrimEnd('/') + "/" + CoverImageUrl.TrimStart('/');
            }
        }

        public RestaurantDto()
        {
            Name = string.Empty;
            Description = null;
            Address = string.Empty;
            ImageUrl = null;
            CoverImageUrl = null;
            Phone = null;
        }
    }

    public class PagedResult<T> where T : class
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("data")]
        public List<T> Data { get; set; }

        public PagedResult()
        {
            Data = new List<T>();
        }
    }

    // ─── Category & Product ───────────────────────────────────────────────────────

    public class CategoryDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        [JsonProperty("products")]
        public List<ProductDto> Products { get; set; }

        public CategoryDto()
        {
            Name = string.Empty;
            ImageUrl = null;
            Products = new List<ProductDto>();
        }
    }

    public class ProductDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("discountedPrice")]
        public decimal DiscountedPrice { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("preparationTime")]
        public int PreparationTime { get; set; }

        [JsonProperty("calories")]
        public int Calories { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        public ProductDto()
        {
            Name = string.Empty;
            Description = null;
            ImageUrl = null;
            DiscountedPrice = 0;  // القيمة الافتراضية 0 تعني لا يوجد خصم
            Calories = 0;
        }

        public decimal EffectivePrice
        {
            get
            {
                // إذا كان السعر بعد الخصم أكبر من 0 وأقل من السعر الأصلي
                if (DiscountedPrice > 0 && DiscountedPrice < Price)
                    return DiscountedPrice;
                else
                    return Price;
            }
        }
    }

    public class CreateProductRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }  // ملاحظة: أصبحت decimal وليس decimal?
        public string ImageUrl { get; set; }
        public int PreparationTime { get; set; }
        public int Calories { get; set; }

        public CreateProductRequest()
        {
            Name = string.Empty;
            Description = null;
            ImageUrl = null;
            DiscountedPrice = 0;
            PreparationTime = 15;
            Calories = 0;
        }
    }

    // ─── Orders ───────────────────────────────────────────────────────────────────

    public class OrderSummary
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantImage { get; set; }
        public int ItemCount { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryAddress { get; set; }

        public OrderSummary()
        {
            Status = string.Empty;
            RestaurantName = string.Empty;
            RestaurantImage = null;
            CustomerName = null;
            CustomerPhone = null;
            PaymentMethod = string.Empty;
            PaymentStatus = string.Empty;
            DeliveryAddress = string.Empty;
        }
    }

    public class OrderDetail
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("subTotal")]
        public decimal SubTotal { get; set; }

        [JsonProperty("deliveryFee")]
        public decimal DeliveryFee { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonProperty("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonProperty("deliveryAddress")]
        public string DeliveryAddress { get; set; }

        [JsonProperty("deliveryNotes")]
        public string DeliveryNotes { get; set; }

        [JsonProperty("estimatedDelivery")]
        public int EstimatedDelivery { get; set; }

        [JsonProperty("cancellationReason")]
        public string CancellationReason { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("acceptedAt")]
        public DateTime AcceptedAt { get; set; }

        [JsonProperty("pickedUpAt")]
        public DateTime PickedUpAt { get; set; }

        [JsonProperty("deliveredAt")]
        public DateTime DeliveredAt { get; set; }

        [JsonProperty("restaurant")]
        public OrderRestaurantInfo Restaurant { get; set; }

        [JsonProperty("items")]
        public List<OrderItemDto> Items { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        public OrderDetail()
        {
            Status = string.Empty;
            PaymentMethod = string.Empty;
            PaymentStatus = string.Empty;
            DeliveryAddress = string.Empty;
            DeliveryNotes = null;
            CancellationReason = null;
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
            Restaurant = null;
            Items = new List<OrderItemDto>();
        }
    }

    public class OrderRestaurantInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Phone { get; set; }

        public OrderRestaurantInfo()
        {
            Name = string.Empty;
            ImageUrl = null;
            Phone = null;
        }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }

        public OrderItemDto()
        {
            ProductName = string.Empty;
            ProductImage = null;
            Notes = null;
        }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; }

        public UpdateStatusRequest()
        {
            Status = string.Empty;
        }
    }

    // ─── Update Restaurant ────────────────────────────────────────────────────────

    public class UpdateRestaurantDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int EstimatedTime { get; set; }
        public bool IsOpen { get; set; }
        public string ImageUrl { get; set; }
        public string CoverImageUrl { get; set; }

        public UpdateRestaurantDto()
        {
            Name = string.Empty;
            Description = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            ImageUrl = null;
            CoverImageUrl = null;
        }
    }

    // ─── Dashboard Stats ──────────────────────────────────────────────────────────

    public class DashboardStats
    {
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int TotalProducts { get; set; }
        public double AvgRating { get; set; }

        public DashboardStats()
        {
            TodayOrders = 0;
            TodayRevenue = 0;
            PendingOrders = 0;
            PreparingOrders = 0;
            TotalProducts = 0;
            AvgRating = 0;
        }
    }
}