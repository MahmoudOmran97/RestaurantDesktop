using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace RestaurantDesktop.Forms
{
    public partial class OrdersPage : DevExpress.XtraEditors.XtraUserControl
    {
        // ── Controls ──────────────────────────────────────────────────────────────
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;
        private DevExpress.XtraEditors.ComboBoxEdit cmbStatus;
        private SimpleButton btnRefresh;
        private SimpleButton btnAccept;
        private SimpleButton btnPrepare;
        private SimpleButton btnReady;
        private SimpleButton btnDeliver;
        private SimpleButton btnReject;
        private SimpleButton btnDetails;
        private LabelControl lblCount;

        private List<OrderDetail> _orders;

        public OrdersPage()
        {
            _orders = new List<OrderDetail>();
            InitializeComponent();
            Task.Run(async () => await RefreshAsync()).Wait();
        }

       

        // ── Event Handlers ────────────────────────────────────────────────────────
        private async void CmbStatus_EditValueChanged(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private void BtnDetails_Click(object sender, EventArgs e)
        {
            ShowOrderDetails();
        }

        private async void BtnAccept_Click(object sender, EventArgs e)
        {
            await ChangeStatusAsync("Accepted");
        }

        private async void BtnPrepare_Click(object sender, EventArgs e)
        {
            await ChangeStatusAsync("Preparing");
        }

        private async void BtnReady_Click(object sender, EventArgs e)
        {
            await ChangeStatusAsync("ReadyForPickup");
        }

        private async void BtnDeliver_Click(object sender, EventArgs e)
        {
            await ChangeStatusAsync("Delivered");
        }

        private async void BtnReject_Click(object sender, EventArgs e)
        {
            await ChangeStatusAsync("Rejected");
        }

        private void View_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Status" && e.CellValue is string st)
            {
                e.Appearance.ForeColor = AppTheme.StatusColor(st);
                e.Appearance.Font = new Font("Tahoma", 9, FontStyle.Bold);
            }
        }

        private void View_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Status" && e.Value is string st)
                e.DisplayText = AppTheme.StatusArabic(st);
            if (e.Column.FieldName == "TotalAmount" && e.Value is decimal d)
                e.DisplayText = string.Format("{0:F0} EGP", d);
            if (e.Column.FieldName == "CreatedAt" && e.Value is DateTime dt)
                e.DisplayText = dt.ToLocalTime().ToString("hh:mm tt  dd/MM/yyyy");
        }

        private void View_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateActionButtons();
        }

        private SimpleButton MakeActionBtn(string text, Color color, int x)
        {
            SimpleButton btn = new SimpleButton();
            btn.Text = text;
            btn.Location = new Point(x, 10);
            btn.Size = new Size(106, 36);
            btn.Font = new Font("Tahoma", 9);
            btn.Enabled = false;
            btn.Appearance.BackColor = color;
            btn.Appearance.ForeColor = Color.White;
            btn.Appearance.BorderColor = color;
            btn.LookAndFeel.UseDefaultLookAndFeel = false;
            btn.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            return btn;
        }

        private static void AddCol(DevExpress.XtraGrid.Views.Grid.GridView v, string field, string caption, int width)
        {
            DevExpress.XtraGrid.Columns.GridColumn col = new DevExpress.XtraGrid.Columns.GridColumn();
            col.FieldName = field;
            col.Caption = caption;
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            v.Columns.Add(col);
        }

        // ── Load / Refresh ────────────────────────────────────────────────────────

        public async Task RefreshAsync()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(async () => await RefreshAsync()));
                return;
            }

            btnRefresh.Enabled = false;
            string statusFilter = cmbStatus.EditValue?.ToString();
            if (statusFilter == "الكل") statusFilter = null;

            // استخدم الدالة الجديدة التي تجلب كل الأوردرات
            List<OrderDetail> orders = await ApiService.GetAllRestaurantOrdersAsync(
                AppSession.RestaurantId, statusFilter);

            _orders = orders ?? new List<OrderDetail>();
            grid.DataSource = _orders;
            lblCount.Text = _orders.Count.ToString() + " أوردر";

            UpdateActionButtons();
            btnRefresh.Enabled = true;
        }

        // ── Selected order ────────────────────────────────────────────────────────

        private OrderDetail SelectedOrder
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
            OrderDetail order = SelectedOrder;
            btnDetails.Enabled = order != null;

            if (order == null)
            {
                btnAccept.Enabled = false;
                btnPrepare.Enabled = false;
                btnReady.Enabled = false;
                btnDeliver.Enabled = false;
                btnReject.Enabled = false;
                return;
            }

            string[] next = AppTheme.NextStatuses(order.Status);
            btnAccept.Enabled = Contains(next, "Accepted");
            btnReject.Enabled = Contains(next, "Rejected");
            btnPrepare.Enabled = Contains(next, "Preparing");
            btnReady.Enabled = Contains(next, "ReadyForPickup");
            btnDeliver.Enabled = Contains(next, "Delivered");
        }

        private bool Contains(string[] array, string value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                    return true;
            }
            return false;
        }

        private async Task ChangeStatusAsync(string newStatus)
        {
            OrderDetail order = SelectedOrder;
            if (order == null) return;

            string arabicStatus = AppTheme.StatusArabic(newStatus);
            if (XtraMessageBox.Show(this,
                string.Format("تحويل الأوردر #{0} إلى\n{1}؟", order.Id, arabicStatus),
                "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            OrderStatusResult result = await ApiService.UpdateOrderStatusAsync(order.Id, newStatus);

            // التصحيح هنا: استخدام Ok و Error (بحروف كبيرة) بدلاً من ok و error
            bool ok = result.Ok;
            string error = result.Error;

            if (ok)
            {
                XtraMessageBox.Show(this, "✅ تم تحديث الحالة بنجاح", "نجاح",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshAsync();
            }
            else
            {
                XtraMessageBox.Show(this, string.Format("❌ {0}", error), "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowOrderDetails()
        {
            OrderDetail order = SelectedOrder;
            if (order == null) return;
            using (OrderDetailForm dlg = new OrderDetailForm(order))
            {
                dlg.ShowDialog(this);
            }
        }
    }
}