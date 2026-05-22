using DevExpress.XtraEditors;
using RestaurantDesktop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        private PictureBox picLogo = null!;
        private LabelControl lblTitle = null!;
        private LabelControl lblSubtitle = null!;
        private LabelControl lblEmail = null!;
        private TextEdit txtEmail = null!;
        private LabelControl lblPassword = null!;
        private TextEdit txtPassword = null!;
        private LabelControl lblApiUrl = null!;
        private TextEdit txtApiUrl = null!;
        private SimpleButton btnLogin = null!;
        private LabelControl lblError = null!;
        private PanelControl pnlCard = null!;
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
            pnlCard = new PanelControl
            {
                Size = new Size(400, 500),
                Location = new Point(40, 60),
                BackColor = AppTheme.Surface
            };
            pnlCard.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // Logo emoji as label
            var lblEmoji = new LabelControl
            {
                Text = "🍽️",
                Font = new Font("Segoe UI Emoji", 48),
                Location = new Point(155, 20),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            lblTitle = new LabelControl
            {
                Text = "إدارة المطعم",
                Font = new Font("Tahoma", 20, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Location = new Point(130, 100),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            lblSubtitle = new LabelControl
            {
                Text = "سجّل دخولك للمتابعة",
                Font = new Font("Tahoma", 10),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(140, 140),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            // API URL
            lblApiUrl = new LabelControl
            {
                Text = "رابط السيرفر",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(30, 185),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };
            txtApiUrl = new TextEdit
            {
                Location = new Point(30, 205),
                Size = new Size(340, 34),
                Text = AppConfig.BaseUrl,
                Properties = { NullValuePrompt = "https://localhost:7001" }
            };
            txtApiUrl.Font = new Font("Tahoma", 10);

            // Email
            lblEmail = new LabelControl
            {
                Text = "البريد الإلكتروني",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(30, 250),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };
            txtEmail = new TextEdit
            {
                Location = new Point(30, 270),
                Size = new Size(340, 34),
                Properties = { NullValuePrompt = "admin@restaurant.com" }
            };
            txtEmail.Font = new Font("Tahoma", 10);

            // Password
            lblPassword = new LabelControl
            {
                Text = "كلمة المرور",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(30, 315),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };
            txtPassword = new TextEdit
            {
                Location = new Point(30, 335),
                Size = new Size(340, 34),
                Properties = {
                PasswordChar = '●',
                NullValuePrompt = "••••••••"
            }
            };
            txtPassword.Font = new Font("Tahoma", 10);
            txtPassword.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };

            // Error label
            lblError = new LabelControl
            {
                Text = string.Empty,
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.Danger,
                Location = new Point(30, 382),
                Size = new Size(340, 20),
                AutoSizeMode = LabelAutoSizeMode.None,
                BackColor = AppTheme.Surface
            };

            // Login button
            btnLogin = new SimpleButton
            {
                Text = "تسجيل الدخول",
                Location = new Point(30, 408),
                Size = new Size(340, 44),
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                Appearance =
            {
                BackColor = AppTheme.Primary,
                ForeColor = Color.White,
                BorderColor= AppTheme.Primary
            },
                LookAndFeel = { UseDefaultLookAndFeel = false, Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat }
            };
            btnLogin.Click += (_, _) => DoLogin();

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
            Resize += (_, _) =>
            {
                pnlCard.Location = new Point(
                    (Width - pnlCard.Width) / 2,
                    (Height - pnlCard.Height) / 2);
            };
        }

        private async void DoLogin()
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Text.Trim();
            var apiUrl = txtApiUrl.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "برجاء إدخال البريد الإلكتروني وكلمة المرور";
                return;
            }

            // حدّث الـ base URL لو تغير
            if (!string.IsNullOrEmpty(apiUrl))
                AppConfig.BaseUrl = apiUrl.TrimEnd('/');

            btnLogin.Enabled = false;
            btnLogin.Text = "جاري التسجيل...";
            lblError.Text = string.Empty;

            var (ok, data, error) = await ApiService.LoginAsync(email, password);

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
            var restaurants = await ApiService.GetRestaurantsAsync(pageSize: 5);
            if (restaurants?.Data?.Count > 0)
                AppSession.RestaurantId = restaurants.Data[0].Id;

            // افتح الشاشة الرئيسية
            var main = new MainForm();
            main.Show();
            Hide();
            main.FormClosed += (_, _) => Close();
        }
    }
}