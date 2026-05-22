using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDesktop
{
    

/// <summary>
/// ألوان وثيمات التطبيق — لون برتقالي/أبيض يليق بالمطاعم
/// </summary>
public static class AppTheme
    {
        public static Color Primary => Color.FromArgb(255, 87, 34);  // #FF5722
        public static Color PrimaryLight => Color.FromArgb(255, 138, 101); // فاتح
        public static Color PrimaryDark => Color.FromArgb(198, 40, 40);  // داكن
        public static Color Background => Color.FromArgb(245, 245, 245);
        public static Color Surface => Color.White;
        public static Color TextPrimary => Color.FromArgb(33, 33, 33);
        public static Color TextSecondary => Color.FromArgb(117, 117, 117);
        public static Color Success => Color.FromArgb(56, 142, 60);
        public static Color Warning => Color.FromArgb(245, 124, 0);
        public static Color Danger => Color.FromArgb(211, 47, 47);
        public static Color Info => Color.FromArgb(2, 136, 209);

        public static Color StatusColor(string status) => status switch
        {
            "Pending" => Warning,
            "Accepted" => Info,
            "Preparing" => Color.FromArgb(123, 31, 162),
            "ReadyForPickup" => Color.FromArgb(0, 121, 107),
            "OnTheWay" => Primary,
            "Delivered" => Success,
            "Cancelled" => Danger,
            "Rejected" => Danger,
            _ => TextSecondary
        };

        public static string StatusArabic(string status) => status switch
        {
            "Pending" => "⏳ انتظار",
            "Accepted" => "✅ مقبول",
            "Preparing" => "👨‍🍳 جاري التحضير",
            "ReadyForPickup" => "📦 جاهز للتسليم",
            "OnTheWay" => "🛵 في الطريق",
            "Delivered" => "✔️ تم التسليم",
            "Cancelled" => "❌ ملغي",
            "Rejected" => "🚫 مرفوض",
            _ => status
        };

        public static string[] NextStatuses(string current) => current switch
        {
            "Pending" => new[] { "Accepted", "Rejected" },
            "Accepted" => new[] { "Preparing" },
            "Preparing" => new[] { "ReadyForPickup" },
            "ReadyForPickup" => new[] { "OnTheWay" },
            "OnTheWay" => new[] { "Delivered" },
            _ => Array.Empty<string>()
        };
    }
    }
