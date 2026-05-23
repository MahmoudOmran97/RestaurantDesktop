
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class MenuPage
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

            LabelControl lblCat = new LabelControl();
            lblCat.Text = "القسم:";
            lblCat.Font = new Font("Tahoma", 9);
            lblCat.ForeColor = AppTheme.TextSecondary;
            lblCat.Location = new Point(12, 16);
            lblCat.AutoSize = true;
            lblCat.BackColor = AppTheme.Surface;

            cmbCategory = new DevExpress.XtraEditors.ComboBoxEdit();
            cmbCategory.Location = new Point(70, 12);
            cmbCategory.Size = new Size(200, 30);
            cmbCategory.Font = new Font("Tahoma", 9);
            cmbCategory.EditValueChanged += CmbCategory_EditValueChanged;

            lblCount = new LabelControl();
            lblCount.Text = "0 منتج";
            lblCount.Font = new Font("Tahoma", 9);
            lblCount.ForeColor = AppTheme.TextSecondary;
            lblCount.Location = new Point(285, 16);
            lblCount.AutoSize = true;
            lblCount.BackColor = AppTheme.Surface;

            btnRefresh = new SimpleButton();
            btnRefresh.Text = "🔄";
            btnRefresh.Location = new Point(360, 10);
            btnRefresh.Size = new Size(46, 32);
            btnRefresh.Click += BtnRefresh_Click;

            pnlToolbar.Controls.AddRange(new Control[]
                { lblCat, cmbCategory, lblCount, btnRefresh });

            // ── Action buttons ────────────────────────────────────────────────────
            PanelControl pnlActions = new PanelControl();
            pnlActions.Dock = DockStyle.Bottom;
            pnlActions.Height = 58;
            pnlActions.BackColor = AppTheme.Surface;
            pnlActions.Padding = new Padding(12, 10, 12, 10);
            pnlActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            btnAdd = MakeBtn("➕ إضافة منتج", AppTheme.Success, 10, true);
            btnEdit = MakeBtn("✏️ تعديل", AppTheme.Info, 160, false);
            btnToggle = MakeBtn("🔄 تفعيل/إيقاف", AppTheme.Warning, 280, false);
            btnDelete = MakeBtn("🗑️ حذف", AppTheme.Danger, 430, false);

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnToggle.Click += BtnToggle_Click;
            btnDelete.Click += BtnDelete_Click;

            pnlActions.Controls.AddRange(new Control[]
                { btnAdd, btnEdit, btnToggle, btnDelete });

            // ── Grid ──────────────────────────────────────────────────────────────
            grid = new DevExpress.XtraGrid.GridControl();
            grid.Dock = DockStyle.Fill;
            view = new DevExpress.XtraGrid.Views.Grid.GridView();
            grid.MainView = view;
            grid.ViewCollection.Add(view);

            AddCol(view, "Id", "#", 60);
            AddCol(view, "Name", "المنتج", 180);
            AddCol(view, "Price", "السعر", 90);
            AddCol(view, "DiscountedPrice", "بعد الخصم", 90);
            AddCol(view, "PreparationTime", "وقت التحضير", 110);
            AddCol(view, "Calories", "سعرات", 80);
            AddCol(view, "IsAvailable", "متاح", 70);
            AddCol(view, "Description", "الوصف", 280);

            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.Appearance.OddRow.BackColor = Color.FromArgb(250, 250, 250);

            view.RowCellStyle += View_RowCellStyle;
            view.CustomColumnDisplayText += View_CustomColumnDisplayText;
            view.FocusedRowChanged += View_FocusedRowChanged;

            Controls.Add(grid);
            Controls.Add(pnlActions);
            Controls.Add(pnlToolbar);

            UpdateButtons();
        }

        #endregion
    }
}
