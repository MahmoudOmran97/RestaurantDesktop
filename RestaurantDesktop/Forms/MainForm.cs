using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        // ── Layout ────────────────────────────────────────────────────────────────
        private PanelControl pnlSidebar;
        private PanelControl pnlContent;
        private PanelControl pnlHeader;

        // ── Sidebar Buttons ───────────────────────────────────────────────────────
        private SimpleButton btnDashboard;
        private SimpleButton btnOrders;
        private SimpleButton btnMenu;
        private SimpleButton btnSettings;
        private SimpleButton btnLogout;
        private LabelControl lblRestName;
        private LabelControl lblUserName;
        private LabelControl lblNewOrders; // badge

        // ── Pages (lazy) ──────────────────────────────────────────────────────────
        private DashboardPage _dashboardPage;
        private OrdersPage _ordersPage;
        private MenuPage _menuPage;
      //  private SettingsPage _settingsPage;
        private Control _activePage;
        private SimpleButton _activeBtn;

        // ── Real-time ─────────────────────────────────────────────────────────────
        private readonly HubService _hub = new HubService();
        private int _newOrderCount = 0;

        public MainForm()
        {
            InitializeComponent();
            Task.Run(async () => await InitHubAsync()).Wait();
            ShowPage(GetDashboardPage(), btnDashboard);
        }

        private void InitializeComponent()
        {
            Text = "Restaurant Manager";
            Size = new Size(1280, 800);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            // ── Sidebar ───────────────────────────────────────────────────────────
            pnlSidebar = new PanelControl();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 220;
            pnlSidebar.BackColor = AppTheme.PrimaryDark;
            pnlSidebar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // Restaurant name at top
            PanelControl pnlTop = new PanelControl();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 90;
            pnlTop.BackColor = AppTheme.Primary;
            pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblEmoji = new LabelControl();
            lblEmoji.Text = "🍽️";
            lblEmoji.Font = new Font("Segoe UI Emoji", 22);
            lblEmoji.Location = new Point(16, 12);
            lblEmoji.AutoSize = true;
            lblEmoji.BackColor = AppTheme.Primary;

            lblRestName = new LabelControl();
            lblRestName.Text = "المطعم";
            lblRestName.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblRestName.ForeColor = Color.White;
            lblRestName.Location = new Point(58, 12);
            lblRestName.Size = new Size(155, 22);
            lblRestName.AutoSizeMode = LabelAutoSizeMode.None;
            lblRestName.BackColor = AppTheme.Primary;

            lblUserName = new LabelControl();
            lblUserName.Text = AppSession.FullName;
            lblUserName.Font = new Font("Tahoma", 8);
            lblUserName.ForeColor = Color.FromArgb(220, 220, 220);
            lblUserName.Location = new Point(58, 40);
            lblUserName.Size = new Size(155, 18);
            lblUserName.AutoSizeMode = LabelAutoSizeMode.None;
            lblUserName.BackColor = AppTheme.Primary;

            pnlTop.Controls.AddRange(new Control[] { lblEmoji, lblRestName, lblUserName });

            // Nav buttons
            int btnY = 10;
            btnDashboard = MakeSidebarBtn("📊  الرئيسية", ref btnY);
            btnOrders = MakeSidebarBtn("📋  الأوردرات", ref btnY);
            btnMenu = MakeSidebarBtn("🍕  المنيو", ref btnY);
            btnSettings = MakeSidebarBtn("⚙️  الإعدادات", ref btnY);

            // Badge للأوردرات الجديدة
            lblNewOrders = new LabelControl();
            lblNewOrders.Text = string.Empty;
            lblNewOrders.Font = new Font("Tahoma", 8, FontStyle.Bold);
            lblNewOrders.ForeColor = Color.White;
            lblNewOrders.BackColor = AppTheme.Danger;
            lblNewOrders.Location = new Point(175, btnOrders.Top + 12);
            lblNewOrders.Size = new Size(26, 18);
            lblNewOrders.AutoSizeMode = LabelAutoSizeMode.None;
            // تم إزالة خاصية TextAlignment لأنها غير موجودة في LabelControl
            lblNewOrders.Visible = false;

            // Spacer + Logout at bottom
            btnLogout = new SimpleButton();
            btnLogout.Text = "🚪  تسجيل الخروج";
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 46;
            btnLogout.Font = new Font("Tahoma", 10);
            btnLogout.Appearance.BackColor = AppTheme.Danger;
            btnLogout.Appearance.ForeColor = Color.White;
            btnLogout.Appearance.BorderColor = AppTheme.Danger;
            btnLogout.LookAndFeel.UseDefaultLookAndFeel = false;
            btnLogout.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            btnLogout.Click += BtnLogout_Click;

            PanelControl pnlNav = new PanelControl();
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.BackColor = AppTheme.PrimaryDark;
            pnlNav.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlNav.Controls.AddRange(new Control[]
                { btnDashboard, btnOrders, btnMenu, btnSettings, lblNewOrders });

            pnlSidebar.Controls.Add(pnlNav);
            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(pnlTop);

            // ── Header ─────────────────────────────────────────────────────────────
            pnlHeader = new PanelControl();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 52;
            pnlHeader.BackColor = AppTheme.Surface;
            pnlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblPageTitle = new LabelControl();
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Text = "الرئيسية";
            lblPageTitle.Font = new Font("Tahoma", 14, FontStyle.Bold);
            lblPageTitle.ForeColor = AppTheme.TextPrimary;
            lblPageTitle.Location = new Point(16, 14);
            lblPageTitle.AutoSize = true;
            lblPageTitle.BackColor = AppTheme.Surface;

            LabelControl lblDate = new LabelControl();
            lblDate.Text = DateTime.Now.ToString("dddd، d MMMM yyyy",
                            new System.Globalization.CultureInfo("ar-EG"));
            lblDate.Font = new Font("Tahoma", 9);
            lblDate.ForeColor = AppTheme.TextSecondary;
            lblDate.Dock = DockStyle.Right;
            lblDate.AutoSize = true;
            lblDate.BackColor = AppTheme.Surface;
            lblDate.Padding = new Padding(0, 0, 16, 0);

            pnlHeader.Controls.AddRange(new Control[] { lblPageTitle, lblDate });

            // ── Content ────────────────────────────────────────────────────────────
            pnlContent = new PanelControl();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = AppTheme.Background;
            pnlContent.Padding = new Padding(0);
            pnlContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);

            // Wire nav clicks - تم إزالة discard parameters (_)
            btnDashboard.Click += (sender, e) => { ShowPage(GetDashboardPage(), btnDashboard); SetTitle("الرئيسية"); };
            btnOrders.Click += (sender, e) => { ShowPage(GetOrdersPage(), btnOrders); SetTitle("الأوردرات"); ClearNewOrderBadge(); };
            btnMenu.Click += (sender, e) => { ShowPage(GetMenuPage(), btnMenu); SetTitle("المنيو"); };
          //  btnSettings.Click += (sender, e) => { ShowPage(GetSettingsPage(), btnSettings); SetTitle("الإعدادات"); };

            Shown += OnShown;
        }

        private async void OnShown(object sender, EventArgs e)
        {
            await LoadRestaurantNameAsync();
        }

        // ── Sidebar button factory ────────────────────────────────────────────────
        private SimpleButton MakeSidebarBtn(string text, ref int y)
        {
            SimpleButton btn = new SimpleButton();
            btn.Text = text;
            btn.Location = new Point(0, y);
            btn.Size = new Size(220, 46);
            btn.Font = new Font("Tahoma", 11);
            btn.Appearance.BackColor = AppTheme.PrimaryDark;
            btn.Appearance.ForeColor = Color.FromArgb(220, 220, 220);
            btn.Appearance.BorderColor = AppTheme.PrimaryDark;
            btn.LookAndFeel.UseDefaultLookAndFeel = false;
            btn.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            // تم إزالة خاصية ImageAlignToText لأنها غير موجودة في SimpleButton
            y += 50;
            return btn;
        }

        private void SetTitle(string title)
        {
            LabelControl lbl = pnlHeader.Controls["lblPageTitle"] as LabelControl;
            if (lbl != null) lbl.Text = title;
        }

        // ── Page switching ────────────────────────────────────────────────────────
        private void ShowPage(Control page, SimpleButton btn)
        {
            if (_activePage != null)
            {
                _activePage.Visible = false;
                pnlContent.Controls.Remove(_activePage);
            }
            if (_activeBtn != null)
                _activeBtn.Appearance.BackColor = AppTheme.PrimaryDark;

            page.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(page);
            page.BringToFront();
            page.Visible = true;

            btn.Appearance.BackColor = AppTheme.Primary;
            _activePage = page;
            _activeBtn = btn;
        }

        // ── Lazy page getters (Modified for C# 7.3 compatibility) ─────────────────
        private DashboardPage GetDashboardPage()
        {
            if (_dashboardPage == null)
                _dashboardPage = new DashboardPage();
            return _dashboardPage;
        }

        private OrdersPage GetOrdersPage()
        {
            if (_ordersPage == null)
                _ordersPage = new OrdersPage();
            return _ordersPage;
        }

        private MenuPage GetMenuPage()
        {
            if (_menuPage == null)
                _menuPage = new MenuPage();
            return _menuPage;
        }

       /* private SettingsPage GetSettingsPage()
        {
            if (_settingsPage == null)
                _settingsPage = new SettingsPage();
            return _settingsPage;
        }*/

        // ── Restaurant name ───────────────────────────────────────────────────────
        private async Task LoadRestaurantNameAsync()
        {
            if (AppSession.RestaurantId <= 0) return;
            RestaurantDto r = await ApiService.GetRestaurantAsync(AppSession.RestaurantId);
            if (r != null)
                lblRestName.Text = r.Name;
        }

        // ── SignalR ───────────────────────────────────────────────────────────────
        private async Task InitHubAsync()
        {
            _hub.OnNewOrder += OnNewOrderHandler;
            _hub.OnOrderStatusChanged += OnOrderStatusChangedHandler;
            await _hub.StartAsync();
        }

        private void OnNewOrderHandler(int orderId)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(OnNewOrderHandler), orderId);
                return;
            }

            _newOrderCount++;
            lblNewOrders.Text = _newOrderCount.ToString();
            lblNewOrders.Visible = true;
            // Refresh orders page لو مفتوحة
            if (_ordersPage != null)
                _ordersPage.RefreshAsync();
            SystemSounds.Beep_Play();
        }

        private void OnOrderStatusChangedHandler(int orderId, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, string>(OnOrderStatusChangedHandler), orderId, status);
                return;
            }

            if (_ordersPage != null)
                _ordersPage.RefreshAsync();
        }

        private static class SystemSounds
        {
            public static void Beep_Play()
            {
                System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private void ClearNewOrderBadge()
        {
            _newOrderCount = 0;
            lblNewOrders.Visible = false;
        }

        // ── Logout ────────────────────────────────────────────────────────────────
        private async void BtnLogout_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show(this,
                "هل تريد تسجيل الخروج؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            await _hub.StopAsync();
            AppSession.Clear();
            LoginForm login = new LoginForm();
            login.Show();
            Close();
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            await _hub.DisposeAsync();
            base.OnFormClosing(e);
        }
    }
}