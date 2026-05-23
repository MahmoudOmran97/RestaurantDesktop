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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // DevExpress Skin
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful");

            // ──────────────────────────────────────────────
            // لا يوجد تسجيل دخول — البرنامج خاص بمطعم واحد
            // RestaurantId يُقرأ من App.config
            // ──────────────────────────────────────────────
            AppSession.RestaurantId = AppConfig.RestaurantId;

            if (AppSession.RestaurantId <= 0)
            {
                MessageBox.Show(
                    "برجاء ضبط RestaurantId في ملف App.config",
                    "إعداد مطلوب",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Application.Run(new MainForm());
        }
    }
}