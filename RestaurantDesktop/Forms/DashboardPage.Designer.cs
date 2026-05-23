using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class DashboardPage
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

            // ── Top bar ───────────────────────────────────────────────────────────
            PanelControl pnlTop = new PanelControl();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 54;
            pnlTop.BackColor = AppTheme.Background;
            pnlTop.Padding = new Padding(16, 10, 16, 0);
            pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblTitle = new LabelControl();
            lblTitle.Text = "مرحباً، " + AppSession.FullName + " 👋";
            lblTitle.Font = new Font("Tahoma", 14, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.TextPrimary;
            lblTitle.Location = new Point(16, 14);
            lblTitle.AutoSize = true;
            lblTitle.BackColor = AppTheme.Background;

            btnRefresh = new SimpleButton();
            btnRefresh.Text = "🔄  تحديث";
            btnRefresh.Dock = DockStyle.Right;
            btnRefresh.Width = 110;
            btnRefresh.Font = new Font("Tahoma", 9);
            btnRefresh.Appearance.BackColor = AppTheme.Primary;
            btnRefresh.Appearance.ForeColor = Color.White;
            btnRefresh.Click += BtnRefresh_Click;

            pnlTop.Controls.AddRange(new Control[] { lblTitle, btnRefresh });

            // ── Stat Cards ────────────────────────────────────────────────────────
            pnlCards = new PanelControl();
            pnlCards.Dock = DockStyle.Top;
            pnlCards.Height = 130;
            pnlCards.BackColor = AppTheme.Background;
            pnlCards.Padding = new Padding(12, 8, 12, 8);
            pnlCards.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            cardTodayOrders = new StatCard("📋", "أوردرات اليوم", "0", AppTheme.Info);
            cardRevenue = new StatCard("💰", "إيرادات اليوم", "0 EGP", AppTheme.Success);
            cardPending = new StatCard("⏳", "انتظار موافقة", "0", AppTheme.Warning);
            cardPreparing = new StatCard("👨‍🍳", "جاري التحضير", "0", AppTheme.Primary);

            pnlCards.Controls.AddRange(new Control[]
                { cardTodayOrders, cardRevenue, cardPending, cardPreparing });

            // ── Recent Orders Grid ────────────────────────────────────────────────
            pnlOrders = new PanelControl();
            pnlOrders.Dock = DockStyle.Fill;
            pnlOrders.BackColor = AppTheme.Surface;
            pnlOrders.Padding = new Padding(16);
            pnlOrders.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblRecent = new LabelControl();
            lblRecent.Text = "آخر الأوردرات";
            lblRecent.Font = new Font("Tahoma", 12, FontStyle.Bold);
            lblRecent.ForeColor = AppTheme.TextPrimary;
            lblRecent.Dock = DockStyle.Top;
            lblRecent.Height = 36;
            lblRecent.BackColor = AppTheme.Surface;

            grid = new DevExpress.XtraGrid.GridControl();
            grid.Dock = DockStyle.Fill;
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
            view.OptionsView.ColumnAutoWidth = false;
            view.Appearance.OddRow.BackColor = Color.FromArgb(250, 250, 250);

            // Conditional formatting for status
            view.RowCellStyle += View_RowCellStyle;

            // Format values
            view.CustomColumnDisplayText += View_CustomColumnDisplayText;

            pnlOrders.Controls.Add(grid);
            pnlOrders.Controls.Add(lblRecent);

            Controls.Add(pnlOrders);
            Controls.Add(pnlCards);
            Controls.Add(pnlTop);

            Resize += DashboardPage_Resize;
            LayoutCards();
        }


        #endregion
    }
}