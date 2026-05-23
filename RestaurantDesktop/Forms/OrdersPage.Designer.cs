using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class OrdersPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BackColor = AppTheme.Background;
            Dock = DockStyle.Fill;

            // ── Toolbar ───────────────────────────────────────────────────────────
            PanelControl pnlToolbar = new PanelControl();
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 54;
            pnlToolbar.BackColor = AppTheme.Surface;
            pnlToolbar.Padding = new Padding(12, 8, 12, 8);
            pnlToolbar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblFilter = new LabelControl();
            lblFilter.Text = "فلتر الحالة:";
            lblFilter.Font = new Font("Tahoma", 9);
            lblFilter.ForeColor = AppTheme.TextSecondary;
            lblFilter.Location = new Point(12, 16);
            lblFilter.AutoSize = true;
            lblFilter.BackColor = AppTheme.Surface;

            cmbStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            cmbStatus.Location = new Point(90, 12);
            cmbStatus.Size = new Size(160, 30);
            cmbStatus.Font = new Font("Tahoma", 9);
            cmbStatus.Properties.Items.AddRange(new object[]
            {
                "الكل", "Pending", "Accepted", "Preparing",
                "ReadyForPickup", "OnTheWay", "Delivered", "Cancelled"
            });
            cmbStatus.EditValue = "الكل";
            cmbStatus.EditValueChanged += CmbStatus_EditValueChanged;

            lblCount = new LabelControl();
            lblCount.Text = "0 أوردر";
            lblCount.Font = new Font("Tahoma", 9);
            lblCount.ForeColor = AppTheme.TextSecondary;
            lblCount.Location = new Point(265, 16);
            lblCount.AutoSize = true;
            lblCount.BackColor = AppTheme.Surface;

            btnRefresh = new SimpleButton();
            btnRefresh.Text = "🔄";
            btnRefresh.Location = new Point(340, 10);
            btnRefresh.Size = new Size(50, 32);
            btnRefresh.ToolTip = "تحديث";
            btnRefresh.Click += BtnRefresh_Click;

            pnlToolbar.Controls.AddRange(new Control[]
                { lblFilter, cmbStatus, lblCount, btnRefresh });

            // ── Action buttons ────────────────────────────────────────────────────
            PanelControl pnlActions = new PanelControl();
            pnlActions.Dock = DockStyle.Bottom;
            pnlActions.Height = 58;
            pnlActions.BackColor = AppTheme.Surface;
            pnlActions.Padding = new Padding(12, 10, 12, 10);
            pnlActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            btnDetails = MakeActionBtn("📄 التفاصيل", AppTheme.Info, 10);
            btnAccept = MakeActionBtn("✅ قبول", AppTheme.Success, 130);
            btnPrepare = MakeActionBtn("👨‍🍳 تحضير", AppTheme.Primary, 230);
            btnReady = MakeActionBtn("📦 جاهز", AppTheme.Info, 310);
            btnDeliver = MakeActionBtn("✔️ تسليم", AppTheme.Success, 390);
            btnReject = MakeActionBtn("❌ رفض", AppTheme.Danger, 470);

            btnDetails.Click += BtnDetails_Click;
            btnAccept.Click += BtnAccept_Click;
            btnPrepare.Click += BtnPrepare_Click;
            btnReady.Click += BtnReady_Click;
            btnDeliver.Click += BtnDeliver_Click;
            btnReject.Click += BtnReject_Click;

            pnlActions.Controls.AddRange(new Control[]
                { btnDetails, btnAccept, btnPrepare, btnReady, btnDeliver, btnReject });

            // ── Grid ──────────────────────────────────────────────────────────────
            grid = new DevExpress.XtraGrid.GridControl();
            grid.Dock = DockStyle.Fill;
            view = new DevExpress.XtraGrid.Views.Grid.GridView();
            grid.MainView = view;
            grid.ViewCollection.Add(view);

            AddCol(view, "Id", "#", 60);
            AddCol(view, "CustomerName", "العميل", 130);
            AddCol(view, "CustomerPhone", "الهاتف", 110);
            AddCol(view, "Status", "الحالة", 130);
            AddCol(view, "TotalAmount", "الإجمالي", 110);
            AddCol(view, "PaymentMethod", "طريقة الدفع", 110);
            AddCol(view, "PaymentStatus", "حالة الدفع", 110);
            AddCol(view, "DeliveryAddress", "عنوان التوصيل", 220);
            AddCol(view, "CreatedAt", "وقت الطلب", 150);
            AddCol(view, "DeliveryNotes", "ملاحظات", 200);

            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.OptionsView.ColumnAutoWidth = false;
            view.Appearance.OddRow.BackColor = Color.FromArgb(250, 250, 250);
            view.Appearance.SelectedRow.BackColor = AppTheme.PrimaryLight;

            view.RowCellStyle += View_RowCellStyle;
            view.CustomColumnDisplayText += View_CustomColumnDisplayText;
            view.FocusedRowChanged += View_FocusedRowChanged;

            Controls.Add(grid);
            Controls.Add(pnlActions);
            Controls.Add(pnlToolbar);

            UpdateActionButtons();
        }

        #endregion
    }
}