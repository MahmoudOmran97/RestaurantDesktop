using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;

namespace RestaurantDesktop.Forms
{
    public partial class DashboardPage : DevExpress.XtraEditors.XtraUserControl
    {
        private PanelControl pnlCards = null!;
        private PanelControl pnlOrders = null!;
        private SimpleButton btnRefresh = null!;

        // Stat cards
        private StatCard cardTodayOrders = null!;
        private StatCard cardRevenue = null!;
        private StatCard cardPending = null!;
        private StatCard cardPreparing = null!;

        // Recent orders grid
        private DevExpress.XtraGrid.GridControl grid = null!;
        private DevExpress.XtraGrid.Views.Grid.GridView view = null!;

        public DashboardPage()
        {
            InitializeComponent();
            _ = LoadAsync();
        }

        private void InitializeComponent()
        {
            BackColor = AppTheme.Background;
            Dock = DockStyle.Fill;

            // ── Top bar ───────────────────────────────────────────────────────────
            var pnlTop = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 54,
                BackColor = AppTheme.Background,
                Padding = new Padding(16, 10, 16, 0)
            };
            pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblTitle = new LabelControl
            {
                Text = "مرحباً، " + AppSession.FullName + " 👋",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, 14),
                AutoSize = true,
                BackColor = AppTheme.Background
            };
            btnRefresh = new SimpleButton
            {
                Text = "🔄  تحديث",
                Dock = DockStyle.Right,
                Width = 110,
                Font = new Font("Tahoma", 9),
                Appearance = { BackColor = AppTheme.Primary, ForeColor = Color.White }
            };
            btnRefresh.Click += (_, _) => _ = LoadAsync();
            pnlTop.Controls.AddRange(new Control[] { lblTitle, btnRefresh });

            // ── Stat Cards ────────────────────────────────────────────────────────
            pnlCards = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = AppTheme.Background,
                Padding = new Padding(12, 8, 12, 8)
            };
            pnlCards.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            cardTodayOrders = new StatCard("📋", "أوردرات اليوم", "0", AppTheme.Info);
            cardRevenue = new StatCard("💰", "إيرادات اليوم", "0 EGP", AppTheme.Success);
            cardPending = new StatCard("⏳", "انتظار موافقة", "0", AppTheme.Warning);
            cardPreparing = new StatCard("👨‍🍳", "جاري التحضير", "0", AppTheme.Primary);

            pnlCards.Controls.AddRange(new Control[]
                { cardTodayOrders, cardRevenue, cardPending, cardPreparing });

            // ── Recent Orders Grid ────────────────────────────────────────────────
            pnlOrders = new PanelControl
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Surface,
                Padding = new Padding(16)
            };
            pnlOrders.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblRecent = new LabelControl
            {
                Text = "آخر الأوردرات",
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = AppTheme.Surface
            };

            grid = new DevExpress.XtraGrid.GridControl { Dock = DockStyle.Fill };
            view = new DevExpress.XtraGrid.Views.Grid.GridView();
            grid.MainView = view;
            grid.ViewCollection.Add(view);

            // Columns
            AddCol(view, "Id", "#", 50);
            AddCol(view, "Status", "الحالة", 120);
            AddCol(view, "TotalAmount", "الإجمالي", 110);
            AddCol(view, "PaymentMethod", "الدفع", 90);
            AddCol(view, "DeliveryAddress", "العنوان", 200);
            AddCol(view, "CreatedAt", "التوقيت", 140);

            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.Appearance.OddRow.BackColor = Color.FromArgb(250, 250, 250);

            // Conditional formatting for status
            view.RowCellStyle += (s, e) =>
            {
                if (e.Column.FieldName == "Status" && e.CellValue is string st)
                    e.Appearance.ForeColor = AppTheme.StatusColor(st);
            };

            // Format values
            view.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column.FieldName == "Status" && e.Value is string st)
                    e.DisplayText = AppTheme.StatusArabic(st);
                if (e.Column.FieldName == "TotalAmount" && e.Value is decimal d)
                    e.DisplayText = $"{d:F0} EGP";
                if (e.Column.FieldName == "CreatedAt" && e.Value is DateTime dt)
                    e.DisplayText = dt.ToLocalTime().ToString("hh:mm tt  dd/MM");
            };

            pnlOrders.Controls.Add(grid);
            pnlOrders.Controls.Add(lblRecent);

            Controls.Add(pnlOrders);
            Controls.Add(pnlCards);
            Controls.Add(pnlTop);

            Resize += (_, _) => LayoutCards();
            LayoutCards();
        }

        private static void AddCol(DevExpress.XtraGrid.Views.Grid.GridView v,
            string field, string caption, int width)
        {
            var col = new DevExpress.XtraGrid.Columns.GridColumn
            {
                FieldName = field,
                Caption = caption,
                Width = width,
                OptionsColumn = { AllowEdit = false }
            };
            v.Columns.Add(col);
        }

        private void LayoutCards()
        {
            int w = (pnlCards.Width - 60) / 4;
            int y = 8;
            int x = 12;
            foreach (Control c in pnlCards.Controls)
            {
                c.Location = new Point(x, y);
                c.Size = new Size(w, 106);
                x += w + 12;
            }
        }

        public async Task LoadAsync()
        {
            btnRefresh.Enabled = false;
            btnRefresh.Text = "جاري التحميل...";

            try
            {
                var stats = await ApiService.GetDashboardStatsAsync(AppSession.RestaurantId);
                cardTodayOrders.Value = stats.TodayOrders.ToString();
                cardRevenue.Value = $"{stats.TodayRevenue:F0} EGP";
                cardPending.Value = stats.PendingOrders.ToString();
                cardPreparing.Value = stats.PreparingOrders.ToString();

                var orders = await ApiService.GetRestaurantOrdersAsync(
                    AppSession.RestaurantId, pageSize: 20);
                if (orders != null)
                    grid.DataSource = orders;
            }
            finally
            {
                btnRefresh.Enabled = true;
                btnRefresh.Text = "🔄  تحديث";
            }
        }
    }

    // ── Reusable Stat Card ────────────────────────────────────────────────────────
    public class StatCard : PanelControl
    {
        private LabelControl _lblValue = null!;

        public string Value
        {
            get => _lblValue.Text;
            set => _lblValue.Text = value;
        }

        public StatCard(string emoji, string title, string value, Color accent)
        {
            BackColor = AppTheme.Surface;
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

            var lblEmoji = new LabelControl
            {
                Text = emoji,
                Font = new Font("Segoe UI Emoji", 24),
                Location = new Point(12, 14),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            _lblValue = new LabelControl
            {
                Text = value,
                Font = new Font("Tahoma", 18, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(60, 12),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            var lblTitle = new LabelControl
            {
                Text = title,
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(60, 48),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            // Color bar on top
            var pnlBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = accent
            };

            Controls.AddRange(new Control[] { pnlBar, lblEmoji, _lblValue, lblTitle });
        }
    }
}
