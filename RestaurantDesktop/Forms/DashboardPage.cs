using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace RestaurantDesktop.Forms
{
    public partial class DashboardPage : DevExpress.XtraEditors.XtraUserControl
    {
        private PanelControl pnlCards;
        private PanelControl pnlOrders;
        private SimpleButton btnRefresh;

        // Stat cards
        private StatCard cardTodayOrders;
        private StatCard cardRevenue;
        private StatCard cardPending;
        private StatCard cardPreparing;

        // Recent orders grid
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;

        public DashboardPage()
        {
            InitializeComponent();
            // نستخدم Load event بدل Task.Run().Wait() لتجنب الـ deadlock
            this.Load += async (s, e) => await LoadAsync();
        }


        // ── Event Handlers ────────────────────────────────────────────────────────
        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadAsync();
        }

        private void View_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Status" && e.CellValue is string st)
            {
                e.Appearance.ForeColor = AppTheme.StatusColor(st);
            }
        }

        private void View_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Status" && e.Value is string st)
            {
                e.DisplayText = AppTheme.StatusArabic(st);
            }
            else if (e.Column.FieldName == "TotalAmount" && e.Value is decimal d)
            {
                e.DisplayText = string.Format("{0:F0} EGP", d);
            }
            else if (e.Column.FieldName == "CreatedAt" && e.Value is DateTime dt)
            {
                e.DisplayText = dt.ToLocalTime().ToString("hh:mm tt  dd/MM");
            }
        }

        private void DashboardPage_Resize(object sender, EventArgs e)
        {
            LayoutCards();
        }

        private static void AddCol(DevExpress.XtraGrid.Views.Grid.GridView v,
            string field, string caption, int width)
        {
            DevExpress.XtraGrid.Columns.GridColumn col = new DevExpress.XtraGrid.Columns.GridColumn();
            col.FieldName = field;
            col.Caption = caption;
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            v.Columns.Add(col);
        }

        private void LayoutCards()
        {
            if (pnlCards.Width <= 0) return;

            int w = (pnlCards.Width - 60) / 4;
            if (w <= 0) return;

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
            if (InvokeRequired)
            {
                Invoke(new Action(async () => await LoadAsync()));
                return;
            }

            btnRefresh.Enabled = false;
            btnRefresh.Text = "جاري التحميل...";

            try
            {
                DashboardStats stats = await ApiService.GetDashboardStatsAsync(AppSession.RestaurantId);
                cardTodayOrders.Value = stats.TodayOrders.ToString();
                cardRevenue.Value = string.Format("{0:F0} EGP", stats.TodayRevenue);
                cardPending.Value = stats.PendingOrders.ToString();
                cardPreparing.Value = stats.PreparingOrders.ToString();

                System.Collections.Generic.List<OrderDetail> orders = await ApiService.GetRestaurantOrdersAsync(
                    AppSession.RestaurantId, null, 1, 20);

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
        private LabelControl _lblValue;

        public string Value
        {
            get { return _lblValue.Text; }
            set { _lblValue.Text = value; }
        }

        public StatCard(string emoji, string title, string value, Color accent)
        {
            BackColor = AppTheme.Surface;
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

            LabelControl lblEmoji = new LabelControl();
            lblEmoji.Text = emoji;
            lblEmoji.Font = new Font("Segoe UI Emoji", 24);
            lblEmoji.Location = new Point(12, 14);
            lblEmoji.AutoSize = true;
            lblEmoji.BackColor = AppTheme.Surface;

            _lblValue = new LabelControl();
            _lblValue.Text = value;
            _lblValue.Font = new Font("Tahoma", 18, FontStyle.Bold);
            _lblValue.ForeColor = accent;
            _lblValue.Location = new Point(60, 12);
            _lblValue.AutoSize = true;
            _lblValue.BackColor = AppTheme.Surface;

            LabelControl lblTitle = new LabelControl();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Tahoma", 9);
            lblTitle.ForeColor = AppTheme.TextSecondary;
            lblTitle.Location = new Point(60, 48);
            lblTitle.AutoSize = true;
            lblTitle.BackColor = AppTheme.Surface;

            // Color bar on top
            Panel pnlBar = new Panel();
            pnlBar.Dock = DockStyle.Top;
            pnlBar.Height = 4;
            pnlBar.BackColor = accent;

            Controls.AddRange(new Control[] { pnlBar, lblEmoji, _lblValue, lblTitle });
        }
    }
}