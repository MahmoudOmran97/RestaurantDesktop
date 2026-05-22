using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDesktop
{
    public static class AppConfig
    {
        // غيّر الـ URL ده لو الـ API على سيرفر تاني
        public static string BaseUrl { get; set; } = "https://deliveryappapi.runasp.net";
        public static string ApiBaseUrl => $"{BaseUrl}/api";
        public static string HubUrl => $"{BaseUrl}/hubs/tracking";
    }
}
