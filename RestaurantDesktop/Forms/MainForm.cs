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