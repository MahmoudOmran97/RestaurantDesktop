using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class MenuPage : DevExpress.XtraEditors.XtraUserControl
    {
        // ── Controls ──────────────────────────────────────────────────────────────
        private DevExpress.XtraEditors.ComboBoxEdit cmbCategory;
        private DevExpress.XtraGrid.GridControl grid;
        private DevExpress.XtraGrid.Views.Grid.GridView view;
        private SimpleButton btnRefresh;
        private SimpleButton btnAdd;
        private SimpleButton btnEdit;
        private SimpleButton btnToggle;
        private SimpleButton btnDelete;
        private LabelControl lblCount;

        private List<CategoryDto> _categories;
        private List<ProductDto> _products;

        public MenuPage()
        {
            _categories = new List<CategoryDto>();
            _products = new List<ProductDto>();
            InitializeComponent();
            Task.Run(async () => await LoadAsync()).Wait();
        }

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

        // ── Event Handlers ────────────────────────────────────────────────────────
        private void CmbCategory_EditValueChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadAsync();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddProduct();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            ShowEditProduct();
        }

        private async void BtnToggle_Click(object sender, EventArgs e)
        {
            await ToggleProductAsync();
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            await DeleteProductAsync();
        }

        private void View_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "IsAvailable" && e.CellValue is bool av)
            {
                if (av)
                    e.Appearance.ForeColor = AppTheme.Success;
                else
                    e.Appearance.ForeColor = AppTheme.Danger;
            }
        }

        private void View_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "IsAvailable" && e.Value is bool av)
            {
                e.DisplayText = av ? "✅ متاح" : "❌ متوقف";
            }
            else if (e.Column.FieldName == "Price" && e.Value is decimal p)
            {
                e.DisplayText = string.Format("{0:F0} EGP", p);
            }
            else if (e.Column.FieldName == "DiscountedPrice")
            {
                if (e.Value == null)
                    e.DisplayText = "—";
                else
                    e.DisplayText = string.Format("{0:F0} EGP", e.Value);
            }
            else if (e.Column.FieldName == "PreparationTime" && e.Value is int t)
            {
                e.DisplayText = t.ToString() + " دقيقة";
            }
            else if (e.Column.FieldName == "Calories")
            {
                if (e.Value == null)
                    e.DisplayText = "—";
                else
                    e.DisplayText = e.Value.ToString() + " cal";
            }
        }

        private void View_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateButtons();
        }

        private SimpleButton MakeBtn(string text, Color color, int x, bool enabled)
        {
            SimpleButton btn = new SimpleButton();
            btn.Text = text;
            btn.Location = new Point(x, 10);
            btn.Size = new Size(136, 36);
            btn.Font = new Font("Tahoma", 9);
            btn.Enabled = enabled;
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

        // ── Load ──────────────────────────────────────────────────────────────────

        public async Task LoadAsync()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(async () => await LoadAsync()));
                return;
            }

            btnRefresh.Enabled = false;
            List<CategoryDto> menu = await ApiService.GetMenuAsync(AppSession.RestaurantId);

            if (menu != null)
                _categories = menu;
            else
                _categories = new List<CategoryDto>();

            // Populate category dropdown
            cmbCategory.Properties.Items.Clear();
            cmbCategory.Properties.Items.Add("كل الأقسام");

            foreach (CategoryDto c in _categories)
            {
                cmbCategory.Properties.Items.Add(c.Name);
            }
            cmbCategory.EditValue = "كل الأقسام";

            FilterProducts();
            btnRefresh.Enabled = true;
        }

        private void FilterProducts()
        {
            string sel = cmbCategory.EditValue?.ToString();

            if (string.IsNullOrEmpty(sel) || sel == "كل الأقسام")
            {
                // جمع كل المنتجات من جميع الأقسام
                _products = new List<ProductDto>();
                foreach (CategoryDto cat in _categories)
                {
                    foreach (ProductDto prod in cat.Products)
                    {
                        _products.Add(prod);
                    }
                }
            }
            else
            {
                CategoryDto cat = null;
                foreach (CategoryDto c in _categories)
                {
                    if (c.Name == sel)
                    {
                        cat = c;
                        break;
                    }
                }

                if (cat != null && cat.Products != null)
                    _products = cat.Products;
                else
                    _products = new List<ProductDto>();
            }

            grid.DataSource = _products;
            lblCount.Text = _products.Count.ToString() + " منتج";
            UpdateButtons();
        }

        private ProductDto SelectedProduct
        {
            get
            {
                int idx = view.FocusedRowHandle;
                if (idx < 0 || idx >= _products.Count) return null;
                return view.GetRow(idx) as ProductDto;
            }
        }

        private void UpdateButtons()
        {
            ProductDto p = SelectedProduct;
            btnEdit.Enabled = (p != null);
            btnToggle.Enabled = (p != null);
            btnDelete.Enabled = (p != null);
        }

        // ── CRUD ──────────────────────────────────────────────────────────────────

        private void ShowAddProduct()
        {
            if (_categories.Count == 0)
            {
                XtraMessageBox.Show(this, "لا يوجد أقسام في المنيو. أضف قسم أولاً.",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (ProductForm dlg = new ProductForm(_categories, null))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(async () => await LoadAsync()).Wait();
                }
            }
        }

        private void ShowEditProduct()
        {
            ProductDto p = SelectedProduct;
            if (p == null) return;

            // ابحث عن الكاتيجوري الخاصة بالمنتج
            CategoryDto cat = null;
            foreach (CategoryDto c in _categories)
            {
                foreach (ProductDto prod in c.Products)
                {
                    if (prod.Id == p.Id)
                    {
                        cat = c;
                        break;
                    }
                }
                if (cat != null) break;
            }

            int? catId = null;
            if (cat != null) catId = cat.Id;

            using (ProductForm dlg = new ProductForm(_categories, p, catId))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Task.Run(async () => await LoadAsync()).Wait();
                }
            }
        }

        private async Task ToggleProductAsync()
        {
            ProductDto p = SelectedProduct;
            if (p == null) return;

            string action = p.IsAvailable ? "إيقاف" : "تفعيل";
            string message = action + " المنتج: " + p.Name + "؟";

            if (XtraMessageBox.Show(this, message,
                "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            bool ok = await ApiService.ToggleProductAvailabilityAsync(p.Id);
            if (ok)
            {
                await LoadAsync();
            }
            else
            {
                XtraMessageBox.Show(this, "فشل تحديث المنتج", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteProductAsync()
        {
            ProductDto p = SelectedProduct;
            if (p == null) return;

            string message = "حذف المنتج: " + p.Name + "؟\nلن تتمكن من التراجع.";

            if (XtraMessageBox.Show(this, message,
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            bool ok = await ApiService.DeleteProductAsync(p.Id);
            if (ok)
            {
                await LoadAsync();
            }
            else
            {
                XtraMessageBox.Show(this, "فشل حذف المنتج", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}