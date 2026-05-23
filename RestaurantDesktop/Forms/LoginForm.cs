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

        private void InitializeComponent()
        {
            Text = "Restaurant Manager — تسجيل الدخول";
            Size = new Size(480, 620);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = AppTheme.Primary;

            // Card
            pnlCard = new PanelControl();
            pnlCard.Size = new Size(400, 500);
            pnlCard.Location = new Point(40, 60);
            pnlCard.BackColor = AppTheme.Surface;
            pnlCard.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // Logo emoji as label
            LabelControl lblEmoji = new LabelControl();
            lblEmoji.Text = "🍽️";
            lblEmoji.Font = new Font("Segoe UI Emoji", 48);
            lblEmoji.Location = new Point(155, 20);
            lblEmoji.AutoSize = true;
            lblEmoji.BackColor = AppTheme.Surface;

            lblTitle = new LabelControl();
            lblTitle.Text = "إدارة المطعم";
            lblTitle.Font = new Font("Tahoma", 20, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.Primary;
            lblTitle.Location = new Point(130, 100);
            lblTitle.AutoSize = true;
            lblTitle.BackColor = AppTheme.Surface;

            lblSubtitle = new LabelControl();
            lblSubtitle.Text = "سجّل دخولك للمتابعة";
            lblSubtitle.Font = new Font("Tahoma", 10);
            lblSubtitle.ForeColor = AppTheme.TextSecondary;
            lblSubtitle.Location = new Point(140, 140);
            lblSubtitle.AutoSize = true;
            lblSubtitle.BackColor = AppTheme.Surface;

            // API URL
            lblApiUrl = new LabelControl();
            lblApiUrl.Text = "رابط السيرفر";
            lblApiUrl.Font = new Font("Tahoma", 9);
            lblApiUrl.ForeColor = AppTheme.TextSecondary;
            lblApiUrl.Location = new Point(30, 185);
            lblApiUrl.AutoSize = true;
            lblApiUrl.BackColor = AppTheme.Surface;

            txtApiUrl = new TextEdit();
            txtApiUrl.Location = new Point(30, 205);
            txtApiUrl.Size = new Size(340, 34);
            txtApiUrl.Text = AppConfig.BaseUrl;
            txtApiUrl.Properties.NullValuePrompt = "https://localhost:7001";
            txtApiUrl.Font = new Font("Tahoma", 10);

            // Email
            lblEmail = new LabelControl();
            lblEmail.Text = "البريد الإلكتروني";
            lblEmail.Font = new Font("Tahoma", 9);
            lblEmail.ForeColor = AppTheme.TextSecondary;
            lblEmail.Location = new Point(30, 250);
            lblEmail.AutoSize = true;
            lblEmail.BackColor = AppTheme.Surface;

            txtEmail = new TextEdit();
            txtEmail.Location = new Point(30, 270);
            txtEmail.Size = new Size(340, 34);
            txtEmail.Properties.NullValuePrompt = "admin@restaurant.com";
            txtEmail.Font = new Font("Tahoma", 10);

            // Password
            lblPassword = new LabelControl();
            lblPassword.Text = "كلمة المرور";
            lblPassword.Font = new Font("Tahoma", 9);
            lblPassword.ForeColor = AppTheme.TextSecondary;
            lblPassword.Location = new Point(30, 315);
            lblPassword.AutoSize = true;
            lblPassword.BackColor = AppTheme.Surface;

            txtPassword = new TextEdit();
            txtPassword.Location = new Point(30, 335);
            txtPassword.Size = new Size(340, 34);
            txtPassword.Properties.PasswordChar = '●';
            txtPassword.Properties.NullValuePrompt = "••••••••";
            txtPassword.Font = new Font("Tahoma", 10);
            txtPassword.KeyDown += TxtPassword_KeyDown;

            // Error label
            lblError = new LabelControl();
            lblError.Text = string.Empty;
            lblError.Font = new Font("Tahoma", 9);
            lblError.ForeColor = AppTheme.Danger;
            lblError.Location = new Point(30, 382);
            lblError.Size = new Size(340, 20);
            lblError.AutoSizeMode = LabelAutoSizeMode.None;
            lblError.BackColor = AppTheme.Surface;

            // Login button
            btnLogin = new SimpleButton();
            btnLogin.Text = "تسجيل الدخول";
            btnLogin.Location = new Point(30, 408);
            btnLogin.Size = new Size(340, 44);
            btnLogin.Font = new Font("Tahoma", 12, FontStyle.Bold);
            btnLogin.Appearance.BackColor = AppTheme.Primary;
            btnLogin.Appearance.ForeColor = Color.White;
            btnLogin.Appearance.BorderColor = AppTheme.Primary;
            btnLogin.LookAndFeel.UseDefaultLookAndFeel = false;
            btnLogin.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            btnLogin.Click += BtnLogin_Click;

            pnlCard.Controls.AddRange(new Control[]
            {
                lblEmoji, lblTitle, lblSubtitle,
                lblApiUrl, txtApiUrl,
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                lblError, btnLogin
            });

            Controls.Add(pnlCard);

            // Center card
            Resize += LoginForm_Resize;
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