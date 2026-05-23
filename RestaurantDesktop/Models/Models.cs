using System;
using System.Collections.Generic;

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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
        public string CoverImageUrl { get; set; }
        public string Phone { get; set; }
        public double Rating { get; set; }
        public int TotalRatings { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int EstimatedTime { get; set; }
        public bool IsOpen { get; set; }

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
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }

        public PagedResult()
        {
            Data = new List<T>();
        }
    }

    // ─── Category & Product ───────────────────────────────────────────────────────

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }  // ملاحظة: أصبحت decimal وليس decimal?
        public string ImageUrl { get; set; }
        public int PreparationTime { get; set; }
        public int Calories { get; set; }
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
        public int Id { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryNotes { get; set; }
        public int EstimatedDelivery { get; set; }
        public string CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime AcceptedAt { get; set; }
        public DateTime PickedUpAt { get; set; }
        public DateTime DeliveredAt { get; set; }
        public OrderRestaurantInfo Restaurant { get; set; }
        public List<OrderItemDto> Items { get; set; }

        public OrderDetail()
        {
            Status = string.Empty;
            PaymentMethod = string.Empty;
            PaymentStatus = string.Empty;
            DeliveryAddress = string.Empty;
            DeliveryNotes = null;
            CancellationReason = null;
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