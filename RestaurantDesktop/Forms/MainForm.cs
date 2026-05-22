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
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        // ── Layout ────────────────────────────────────────────────────────────────
        private PanelControl pnlSidebar = null!;
        private PanelControl pnlContent = null!;
        private PanelControl pnlHeader = null!;

        // ── Sidebar Buttons ───────────────────────────────────────────────────────
        private SimpleButton btnDashboard = null!;
        private SimpleButton btnOrders = null!;
        private SimpleButton btnMenu = null!;
        private SimpleButton btnSettings = null!;
        private SimpleButton btnLogout = null!;
        private LabelControl lblRestName = null!;
        private LabelControl lblUserName = null!;
        private LabelControl lblNewOrders = null!; // badge

        // ── Pages (lazy) ──────────────────────────────────────────────────────────
        private DashboardPage? _dashboardPage;
        private OrdersPage? _ordersPage;
        private MenuPage? _menuPage;
        private SettingsPage? _settingsPage;
        private Control? _activePage;
        private SimpleButton? _activeBtn;

        // ── Real-time ─────────────────────────────────────────────────────────────
        private readonly HubService _hub = new();
        private int _newOrderCount = 0;

        public MainForm()
        {
            InitializeComponent();
            _ = InitHubAsync();
            ShowPage(GetDashboardPage(), btnDashboard);
        }

        private void InitializeComponent()
        {
            Text = "Restaurant Manager";
            Size = new Size(1280, 800);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            // ── Sidebar ───────────────────────────────────────────────────────────
            pnlSidebar = new PanelControl
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = AppTheme.PrimaryDark
            };
            pnlSidebar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // Restaurant name at top
            var pnlTop = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = AppTheme.Primary
            };
            pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblEmoji = new LabelControl
            {
                Text = "🍽️",
                Font = new Font("Segoe UI Emoji", 22),
                Location = new Point(16, 12),
                AutoSize = true,
                BackColor = AppTheme.Primary
            };
            lblRestName = new LabelControl
            {
                Text = "المطعم",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(58, 12),
                Size = new Size(155, 22),
                AutoSizeMode = LabelAutoSizeMode.None,
                BackColor = AppTheme.Primary
            };
            lblUserName = new LabelControl
            {
                Text = AppSession.FullName,
                Font = new Font("Tahoma", 8),
                ForeColor = Color.FromArgb(220, 220, 220),
                Location = new Point(58, 40),
                Size = new Size(155, 18),
                AutoSizeMode = LabelAutoSizeMode.None,
                BackColor = AppTheme.Primary
            };
            pnlTop.Controls.AddRange(new Control[] { lblEmoji, lblRestName, lblUserName });

            // Nav buttons
            int btnY = 10;
            btnDashboard = MakeSidebarBtn("📊  الرئيسية", ref btnY);
            btnOrders = MakeSidebarBtn("📋  الأوردرات", ref btnY);
            btnMenu = MakeSidebarBtn("🍕  المنيو", ref btnY);
            btnSettings = MakeSidebarBtn("⚙️  الإعدادات", ref btnY);

            // Badge للأوردرات الجديدة
            lblNewOrders = new LabelControl
            {
                Text = string.Empty,
                Font = new Font("Tahoma", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = AppTheme.Danger,
                Location = new Point(175, btnOrders.Top + 12),
                Size = new Size(26, 18),
                AutoSizeMode = LabelAutoSizeMode.None,
                TextAlignment = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Spacer + Logout at bottom
            btnLogout = new SimpleButton
            {
                Text = "🚪  تسجيل الخروج",
                Dock = DockStyle.Bottom,
                Height = 46,
                Font = new Font("Tahoma", 10),
                Appearance =
            {
                BackColor  = AppTheme.Danger,
                ForeColor  = Color.White,
                BorderColor= AppTheme.Danger
            },
                LookAndFeel = { UseDefaultLookAndFeel = false, Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat }
            };
            btnLogout.Click += BtnLogout_Click;

            var pnlNav = new PanelControl
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.PrimaryDark
            };
            pnlNav.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlNav.Controls.AddRange(new Control[]
                { btnDashboard, btnOrders, btnMenu, btnSettings, lblNewOrders });

            pnlSidebar.Controls.Add(pnlNav);
            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(pnlTop);

            // ── Header ─────────────────────────────────────────────────────────────
            pnlHeader = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = AppTheme.Surface
            };
            pnlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblPageTitle = new LabelControl
            {
                Name = "lblPageTitle",
                Text = "الرئيسية",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 14),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };
            var lblDate = new LabelControl
            {
                Text = DateTime.Now.ToString("dddd، d MMMM yyyy",
                                new System.Globalization.CultureInfo("ar-EG")),
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Right,
                AutoSize = true,
                BackColor = AppTheme.Surface,
                Padding = new Padding(0, 0, 16, 0)
            };
            pnlHeader.Controls.AddRange(new Control[] { lblPageTitle, lblDate });

            // ── Content ────────────────────────────────────────────────────────────
            pnlContent = new PanelControl
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Background,
                Padding = new Padding(0)
            };
            pnlContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);

            // Wire nav clicks
            btnDashboard.Click += (_, _) => { ShowPage(GetDashboardPage(), btnDashboard); SetTitle("الرئيسية"); };
            btnOrders.Click += (_, _) => { ShowPage(GetOrdersPage(), btnOrders); SetTitle("الأوردرات"); ClearNewOrderBadge(); };
            btnMenu.Click += (_, _) => { ShowPage(GetMenuPage(), btnMenu); SetTitle("المنيو"); };
            btnSettings.Click += (_, _) => { ShowPage(GetSettingsPage(), btnSettings); SetTitle("الإعدادات"); };

            Shown += async (_, _) => await LoadRestaurantNameAsync();
        }

        // ── Sidebar button factory ────────────────────────────────────────────────
        private SimpleButton MakeSidebarBtn(string text, ref int y)
        {
            var btn = new SimpleButton
            {
                Text = text,
                Location = new Point(0, y),
                Size = new Size(220, 46),
                Font = new Font("Tahoma", 11),
                Appearance =
            {
                BackColor  = AppTheme.PrimaryDark,
                ForeColor  = Color.FromArgb(220, 220, 220),
                BorderColor= AppTheme.PrimaryDark
            },
                LookAndFeel = { UseDefaultLookAndFeel = false, Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat },
                ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter
            };
            y += 50;
            return btn;
        }

        private void SetTitle(string title)
        {
            var lbl = pnlHeader.Controls["lblPageTitle"] as LabelControl;
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

        // ── Lazy page getters ─────────────────────────────────────────────────────
        private DashboardPage GetDashboardPage() => _dashboardPage ??= new DashboardPage();
        private OrdersPage GetOrdersPage() => _ordersPage ??= new OrdersPage();
        private MenuPage GetMenuPage() => _menuPage ??= new MenuPage();
        private SettingsPage GetSettingsPage() => _settingsPage ??= new SettingsPage();

        // ── Restaurant name ───────────────────────────────────────────────────────
        private async Task LoadRestaurantNameAsync()
        {
            if (AppSession.RestaurantId <= 0) return;
            var r = await ApiService.GetRestaurantAsync(AppSession.RestaurantId);
            if (r != null)
                lblRestName.Text = r.Name;
        }

        // ── SignalR ───────────────────────────────────────────────────────────────
        private async Task InitHubAsync()
        {
            _hub.OnNewOrder += orderId => Invoke(() =>
            {
                _newOrderCount++;
                lblNewOrders.Text = _newOrderCount.ToString();
                lblNewOrders.Visible = true;
                // Refresh orders page لو مفتوحة
                _ordersPage?.RefreshAsync();
                SystemSounds.Beep_Play();
            });

            _hub.OnOrderStatusChanged += (orderId, status) => Invoke(() =>
                _ordersPage?.RefreshAsync());

            await _hub.StartAsync();
        }

        private static class SystemSounds
        {
            public static void Beep_Play() =>
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void ClearNewOrderBadge()
        {
            _newOrderCount = 0;
            lblNewOrders.Visible = false;
        }

        // ── Logout ────────────────────────────────────────────────────────────────
        private async void BtnLogout_Click(object? sender, EventArgs e)
        {
            if (XtraMessageBox.Show(this,
                "هل تريد تسجيل الخروج؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            await _hub.StopAsync();
            AppSession.Clear();
            var login = new LoginForm();
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