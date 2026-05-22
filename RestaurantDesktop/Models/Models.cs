using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDesktop.Models
{
   

// ─── Auth ────────────────────────────────────────────────────────────────────

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // ─── Restaurant ───────────────────────────────────────────────────────────────

    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Phone { get; set; }
        public double Rating { get; set; }
        public int TotalRatings { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal MinOrderAmount { get; set; }
        public int EstimatedTime { get; set; }
        public bool IsOpen { get; set; }
    }

    public class PagedResult<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new();
    }

    // ─── Category & Product ───────────────────────────────────────────────────────

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTime { get; set; }
        public int? Calories { get; set; }
        public bool IsAvailable { get; set; }

        public decimal EffectivePrice => DiscountedPrice ?? Price;
    }

    public class CreateProductRequest
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int PreparationTime { get; set; } = 15;
        public int? Calories { get; set; }
    }

    // ─── Orders ───────────────────────────────────────────────────────────────────

    public class OrderSummary
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string? RestaurantImage { get; set; }
        public int ItemCount { get; set; }

        // حقول إضافية لما بنجيب الأوردرات بتاعت المطعم
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string? DeliveryNotes { get; set; }
        public int? EstimatedDelivery { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public OrderRestaurantInfo? Restaurant { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderRestaurantInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
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
    }
 }
