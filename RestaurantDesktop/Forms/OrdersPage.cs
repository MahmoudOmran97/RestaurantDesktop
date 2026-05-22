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
    public partial class OrdersPage : DevExpress.XtraEditors.XtraUserControl
    {
        // ── Controls ──────────────────────────────────────────────────────────────
        private DevExpress.XtraGrid.GridControl grid = null!;
        private GridView view = null!;
        private DevExpress.XtraEditors.ComboBoxEdit cmbStatus = null!;
        private SimpleButton btnRefresh = null!;
        private SimpleButton btnAccept = null!;
        private SimpleButton btnPrepare = null!;
        private SimpleButton btnReady = null!;
        private SimpleButton btnDeliver = null!;
        private SimpleButton btnReject = null!;
        private SimpleButton btnDetails = null!;
        private LabelControl lblCount = null!;

        private List<OrderDetail> _orders = new();

        public OrdersPage()
        {
            InitializeComponent();
            _ = RefreshAsync();
        }

        private void InitializeComponent()
        {
            BackColor = AppTheme.Background;
            Dock = DockStyle.Fill;

            // ── Toolbar ───────────────────────────────────────────────────────────
            var pnlToolbar = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 54,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 8, 12, 8)
            };
            pnlToolbar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var lblFilter = new LabelControl
            {
                Text = "فلتر الحالة:",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(12, 16),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            cmbStatus = new DevExpress.XtraEditors.ComboBoxEdit
            {
                Location = new Point(90, 12),
                Size = new Size(160, 30),
            };
            cmbStatus.Font = new Font("Tahoma", 9);
            cmbStatus.Properties.Items.AddRange(new object[]
            {
            "الكل", "Pending", "Accepted", "Preparing",
            "ReadyForPickup", "OnTheWay", "Delivered", "Cancelled"
            });
            cmbStatus.EditValue = "الكل";
            cmbStatus.EditValueChanged += (_, _) => _ = RefreshAsync();

            lblCount = new LabelControl
            {
                Text = "0 أوردر",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(265, 16),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            btnRefresh = new SimpleButton
            {
                Text = "🔄",
                Location = new Point(340, 10),
                Size = new Size(50, 32),
                ToolTip = "تحديث"
            };
            btnRefresh.Click += (_, _) => _ = RefreshAsync();

            pnlToolbar.Controls.AddRange(new Control[]
                { lblFilter, cmbStatus, lblCount, btnRefresh });

            // ── Action buttons ────────────────────────────────────────────────────
            var pnlActions = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 10, 12, 10)
            };
            pnlActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            btnDetails = MakeActionBtn("📄 التفاصيل", AppTheme.Info, 10);
            btnAccept = MakeActionBtn("✅ قبول", AppTheme.Success, 130);
            btnPrepare = MakeActionBtn("👨‍🍳 تحضير", AppTheme.Primary, 230);
            btnReady = MakeActionBtn("📦 جاهز", AppTheme.Info, 310);
            btnDeliver = MakeActionBtn("✔️ تسليم", AppTheme.Success, 390);
            btnReject = MakeActionBtn("❌ رفض", AppTheme.Danger, 470);

            btnDetails.Click += (_, _) => ShowOrderDetails();
            btnAccept.Click += (_, _) => _ = ChangeStatusAsync("Accepted");
            btnPrepare.Click += (_, _) => _ = ChangeStatusAsync("Preparing");
            btnReady.Click += (_, _) => _ = ChangeStatusAsync("ReadyForPickup");
            btnDeliver.Click += (_, _) => _ = ChangeStatusAsync("Delivered");
            btnReject.Click += (_, _) => _ = ChangeStatusAsync("Rejected");

            pnlActions.Controls.AddRange(new Control[]
                { btnDetails, btnAccept, btnPrepare, btnReady, btnDeliver, btnReject });

            // ── Grid ──────────────────────────────────────────────────────────────
            grid = new DevExpress.XtraGrid.GridControl { Dock = DockStyle.Fill };
            view = new GridView();
            grid.MainView = view;
            grid.ViewCollection.Add(view);

            AddCol(view, "Id", "#", 60, false);
            AddCol(view, "Status", "الحالة", 130, false);
            AddCol(view, "TotalAmount", "الإجمالي", 110, false);
            AddCol(view, "PaymentMethod", "طريقة الدفع", 110, false);
            AddCol(view, "PaymentStatus", "حالة الدفع", 110, false);
            AddCol(view, "DeliveryAddress", "عنوان التوصيل", 220, false);
            AddCol(view, "CreatedAt", "وقت الطلب", 150, false);
            AddCol(view, "DeliveryNotes", "ملاحظات", 200, false);

            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.Appearance.OddRow.BackColor = Color.FromArgb(250, 250, 250);
            view.Appearance.SelectedRow.BackColor = AppTheme.PrimaryLight;

            view.RowCellStyle += (s, e) =>
            {
                if (e.Column.FieldName == "Status" && e.CellValue is string st)
                {
                    e.Appearance.ForeColor = AppTheme.StatusColor(st);
                    e.Appearance.Font = new Font("Tahoma", 9, FontStyle.Bold);
                }
            };

            view.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column.FieldName == "Status" && e.Value is string st)
                    e.DisplayText = AppTheme.StatusArabic(st);
                if (e.Column.FieldName == "TotalAmount" && e.Value is decimal d)
                    e.DisplayText = $"{d:F0} EGP";
                if (e.Column.FieldName == "CreatedAt" && e.Value is DateTime dt)
                    e.DisplayText = dt.ToLocalTime().ToString("hh:mm tt  dd/MM/yyyy");
            };

            view.FocusedRowChanged += (_, _) => UpdateActionButtons();

            Controls.Add(grid);
            Controls.Add(pnlActions);
            Controls.Add(pnlToolbar);

            UpdateActionButtons();
        }

        private SimpleButton MakeActionBtn(string text, Color color, int x)
        {
            var btn = new SimpleButton
            {
                Text = text,
                Location = new Point(x, 10),
                Size = new Size(106, 36),
                Font = new Font("Tahoma", 9),
                Enabled = false,
                Appearance =
            {
                BackColor  = color,
                ForeColor  = Color.White,
                BorderColor= color
            },
                LookAndFeel = { UseDefaultLookAndFeel = false, Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat }
            };
            return btn;
        }

        private static void AddCol(GridView v, string field, string caption, int width, bool allowEdit)
        {
            var col = new DevExpress.XtraGrid.Columns.GridColumn
            {
                FieldName = field,
                Caption = caption,
                Width = width,
                OptionsColumn = { AllowEdit = allowEdit }
            };
            v.Columns.Add(col);
        }

        // ── Load / Refresh ────────────────────────────────────────────────────────

        public async Task RefreshAsync()
        {
            if (InvokeRequired) { Invoke(RefreshAsync); return; }

            btnRefresh.Enabled = false;
            var statusFilter = cmbStatus.EditValue?.ToString();
            if (statusFilter == "الكل") statusFilter = null;

            var orders = await ApiService.GetRestaurantOrdersAsync(
                AppSession.RestaurantId, statusFilter, pageSize: 100);

            _orders = orders ?? new();
            grid.DataSource = _orders;
            lblCount.Text = $"{_orders.Count} أوردر";

            UpdateActionButtons();
            btnRefresh.Enabled = true;
        }

        // ── Selected order ────────────────────────────────────────────────────────

        private OrderDetail? SelectedOrder
        {
            get
            {
                int idx = view.FocusedRowHandle;
                if (idx < 0 || idx >= _orders.Count) return null;
                return view.GetRow(idx) as OrderDetail;
            }
        }

        private void UpdateActionButtons()
        {
            var order = SelectedOrder;
            btnDetails.Enabled = order != null;

            if (order == null)
            {
                btnAccept.Enabled = btnPrepare.Enabled = btnReady.Enabled =
                btnDeliver.Enabled = btnReject.Enabled = false;
                return;
            }

            var next = AppTheme.NextStatuses(order.Status);
            btnAccept.Enabled = next.Contains("Accepted");
            btnReject.Enabled = next.Contains("Rejected");
            btnPrepare.Enabled = next.Contains("Preparing");
            btnReady.Enabled = next.Contains("ReadyForPickup");
            btnDeliver.Enabled = next.Contains("Delivered");
        }

        private async Task ChangeStatusAsync(string newStatus)
        {
            var order = SelectedOrder;
            if (order == null) return;

            var arabicStatus = AppTheme.StatusArabic(newStatus);
            if (XtraMessageBox.Show(this,
                $"تحويل الأوردر #{order.Id} إلى\n{arabicStatus}؟",
                "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            var (ok, error) = await ApiService.UpdateOrderStatusAsync(order.Id, newStatus);

            if (ok)
            {
                XtraMessageBox.Show(this, "✅ تم تحديث الحالة بنجاح", "نجاح",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshAsync();
            }
            else
            {
                XtraMessageBox.Show(this, $"❌ {error}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowOrderDetails()
        {
            var order = SelectedOrder;
            if (order == null) return;
            using var dlg = new OrderDetailForm(order);
            dlg.ShowDialog(this);
        }
    }
}
