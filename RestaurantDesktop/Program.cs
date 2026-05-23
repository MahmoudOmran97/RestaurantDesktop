using RestaurantDesktop.Forms;
using System;
using System.Windows.Forms;

namespace RestaurantDesktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // تطبيق إعدادات التطبيق الأساسية
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // DevExpress Skin
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful");

            Application.Run(new LoginForm());
        }
    }
}