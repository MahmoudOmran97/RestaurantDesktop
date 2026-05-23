using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDesktop
{
    public static class AppConfig
    {
        // يقرأ من App.config تلقائياً — غيّر القيم هناك
        public static string BaseUrl { get; set; } =
            ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://deliveryappapi.runasp.net";

        public static string ApiBaseUrl => $"{BaseUrl}/api";
        public static string HubUrl => $"{BaseUrl}/hubs/tracking";

        public static int RestaurantId
        {
            get
            {
                string val = ConfigurationManager.AppSettings["RestaurantId"] ?? "1";
                return int.TryParse(val, out int id) ? id : 1;
            }
        }
    }
}