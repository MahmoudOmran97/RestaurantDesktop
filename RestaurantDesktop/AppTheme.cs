using System;
using System.Drawing;

namespace RestaurantDesktop
{
    /// <summary>
    /// ألوان وثيمات التطبيق — لون برتقالي/أبيض يليق بالمطاعم
    /// </summary>
    public static class AppTheme
    {
        public static Color Primary
        {
            get { return Color.FromArgb(255, 87, 34); }  // #FF5722
        }

        public static Color PrimaryLight
        {
            get { return Color.FromArgb(255, 138, 101); } // فاتح
        }

        public static Color PrimaryDark
        {
            get { return Color.FromArgb(198, 40, 40); }  // داكن
        }

        public static Color Background
        {
            get { return Color.FromArgb(245, 245, 245); }
        }

        public static Color Surface
        {
            get { return Color.White; }
        }

        public static Color TextPrimary
        {
            get { return Color.FromArgb(33, 33, 33); }
        }

        public static Color TextSecondary
        {
            get { return Color.FromArgb(117, 117, 117); }
        }

        public static Color Success
        {
            get { return Color.FromArgb(56, 142, 60); }
        }

        public static Color Warning
        {
            get { return Color.FromArgb(245, 124, 0); }
        }

        public static Color Danger
        {
            get { return Color.FromArgb(211, 47, 47); }
        }

        public static Color Info
        {
            get { return Color.FromArgb(2, 136, 209); }
        }

        public static Color StatusColor(string status)
        {
            if (status == "Pending")
                return Warning;
            else if (status == "Accepted")
                return Info;
            else if (status == "Preparing")
                return Color.FromArgb(123, 31, 162);
            else if (status == "ReadyForPickup")
                return Color.FromArgb(0, 121, 107);
            else if (status == "OnTheWay")
                return Primary;
            else if (status == "Delivered")
                return Success;
            else if (status == "Cancelled")
                return Danger;
            else if (status == "Rejected")
                return Danger;
            else
                return TextSecondary;
        }

        public static string StatusArabic(string status)
        {
            if (status == "Pending")
                return "⏳ انتظار";
            else if (status == "Accepted")
                return "✅ مقبول";
            else if (status == "Preparing")
                return "👨‍🍳 جاري التحضير";
            else if (status == "ReadyForPickup")
                return "📦 جاهز للتسليم";
            else if (status == "OnTheWay")
                return "🛵 في الطريق";
            else if (status == "Delivered")
                return "✔️ تم التسليم";
            else if (status == "Cancelled")
                return "❌ ملغي";
            else if (status == "Rejected")
                return "🚫 مرفوض";
            else
                return status;
        }

        public static string[] NextStatuses(string current)
        {
            if (current == "Pending")
                return new string[] { "Accepted", "Rejected" };
            else if (current == "Accepted")
                return new string[] { "Preparing" };
            else if (current == "Preparing")
                return new string[] { "ReadyForPickup" };
            else if (current == "ReadyForPickup")
                return new string[] { "OnTheWay" };
            else if (current == "OnTheWay")
                return new string[] { "Delivered" };
            else
                return new string[0];
        }
    }
}