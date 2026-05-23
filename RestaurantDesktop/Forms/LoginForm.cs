using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        private PictureBox picLogo;
        private LabelControl lblTitle;
        private LabelControl lblSubtitle;
        private LabelControl lblEmail;
        private TextEdit txtEmail;
        private LabelControl lblPassword;
        private TextEdit txtPassword;
        private LabelControl lblApiUrl;
        private TextEdit txtApiUrl;
        private SimpleButton btnLogin;
        private LabelControl lblError;
        private PanelControl pnlCard;

        public LoginForm()
        {
            InitializeComponent();
        }

        
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoLogin();
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void LoginForm_Resize(object sender, EventArgs e)
        {
            pnlCard.Location = new Point(
                (Width - pnlCard.Width) / 2,
                (Height - pnlCard.Height) / 2);
        }

        private async void DoLogin()
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string apiUrl = txtApiUrl.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "برجاء إدخال البريد الإلكتروني وكلمة المرور";
                return;
            }

            // حدّث الـ base URL لو تغير
            if (!string.IsNullOrEmpty(apiUrl))
            {
                string trimmedUrl = apiUrl.TrimEnd('/');
                AppConfig.BaseUrl = trimmedUrl;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "جاري التسجيل...";
            lblError.Text = string.Empty;

            // التصحيح هنا: استخدام الخصائص بدلاً من تفكيك tuple
            LoginResult result = await ApiService.LoginAsync(email, password);
            bool ok = result.Ok;
            LoginResponse data = result.Data;
            string error = result.Error;

            if (!ok || data == null)
            {
                lblError.Text = error;
                btnLogin.Enabled = true;
                btnLogin.Text = "تسجيل الدخول";
                return;
            }

            // حفظ الجلسة
            AppSession.Token = data.Token;
            AppSession.UserId = data.Id;
            AppSession.FullName = data.FullName;
            AppSession.Email = data.Email;
            AppSession.Role = data.Role;

            // اجلب أول مطعم مرتبط بالـ Admin
            PagedResult<RestaurantDto> restaurants = await ApiService.GetRestaurantsAsync(pageSize: 5);
            if (restaurants != null && restaurants.Data != null && restaurants.Data.Count > 0)
            {
                AppSession.RestaurantId = restaurants.Data[0].Id;
            }
            // بعد سطر AppSession.Token = data.Token;
            System.Windows.Forms.MessageBox.Show("تم تسجيل الدخول بنجاح!\nRole: " + data.Role);
            // افتح الشاشة الرئيسية
            MainForm main = new MainForm();
            main.Show();
            Hide();
            main.FormClosed += Main_FormClosed;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}