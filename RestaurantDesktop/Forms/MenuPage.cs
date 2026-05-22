using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
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
    public partial class MenuPage : DevExpress.XtraEditors.XtraUserControl
    {
        // ── Controls ──────────────────────────────────────────────────────────────
        private DevExpress.XtraEditors.ComboBoxEdit cmbCategory = null!;
        private DevExpress.XtraGrid.GridControl grid = null!;
        private GridView view = null!;
        private SimpleButton btnRefresh = null!;
        private SimpleButton btnAdd = null!;
        private SimpleButton btnEdit = null!;
        private SimpleButton btnToggle = null!;
        private SimpleButton btnDelete = null!;
        private LabelControl lblCount = null!;

        private List<CategoryDto> _categories = new();
        private List<ProductDto> _products = new();

        public MenuPage()
        {
            InitializeComponent();
            _ = LoadAsync();
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

            var lblCat = new LabelControl
            {
                Text = "القسم:",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(12, 16),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            cmbCategory = new DevExpress.XtraEditors.ComboBoxEdit
            {
                Location = new Point(70, 12),
                Size = new Size(200, 30)
            };
            cmbCategory.Font = new Font("Tahoma", 9);
            cmbCategory.EditValueChanged += (_, _) => FilterProducts();

            lblCount = new LabelControl
            {
                Text = "0 منتج",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(285, 16),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };

            btnRefresh = new SimpleButton
            {
                Text = "🔄",
                Location = new Point(360, 10),
                Size = new Size(46, 32)
            };
            btnRefresh.Click += (_, _) => _ = LoadAsync();

            pnlToolbar.Controls.AddRange(new Control[]
                { lblCat, cmbCategory, lblCount, btnRefresh });

            // ── Action buttons ────────────────────────────────────────────────────
            var pnlActions = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                BackColor = AppTheme.Surface,
                Padding = new Padding(12, 10, 12, 10)
            };
            pnlActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            btnAdd = MakeBtn("➕ إضافة منتج", AppTheme.Success, 10, true);
            btnEdit = MakeBtn("✏️ تعديل", AppTheme.Info, 160, false);
            btnToggle = MakeBtn("🔄 تفعيل/إيقاف", AppTheme.Warning, 280, false);
            btnDelete = MakeBtn("🗑️ حذف", AppTheme.Danger, 430, false);

            btnAdd.Click += (_, _) => ShowAddProduct();
            btnEdit.Click += (_, _) => ShowEditProduct();
            btnToggle.Click += (_, _) => _ = ToggleProductAsync();
            btnDelete.Click += (_, _) => _ = DeleteProductAsync();

            pnlActions.Controls.AddRange(new Control[]
                { btnAdd, btnEdit, btnToggle, btnDelete });

            // ── Grid ──────────────────────────────────────────────────────────────
            grid = new DevExpress.XtraGrid.GridControl { Dock = DockStyle.Fill };
            view = new GridView();
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

            view.RowCellStyle += (s, e) =>
            {
                if (e.Column.FieldName == "IsAvailable" && e.CellValue is bool av)
                    e.Appearance.ForeColor = av ? AppTheme.Success : AppTheme.Danger;
            };

            view.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column.FieldName == "IsAvailable" && e.Value is bool av)
                    e.DisplayText = av ? "✅ متاح" : "❌ متوقف";
                if (e.Column.FieldName == "Price" && e.Value is decimal p)
                    e.DisplayText = $"{p:F0} EGP";
                if (e.Column.FieldName == "DiscountedPrice")
                    e.DisplayText = e.Value == null ? "—" : $"{e.Value:F0} EGP";
                if (e.Column.FieldName == "PreparationTime" && e.Value is int t)
                    e.DisplayText = $"{t} دقيقة";
                if (e.Column.FieldName == "Calories")
                    e.DisplayText = e.Value == null ? "—" : $"{e.Value} cal";
            };

            view.FocusedRowChanged += (_, _) => UpdateButtons();

            Controls.Add(grid);
            Controls.Add(pnlActions);
            Controls.Add(pnlToolbar);

            UpdateButtons();
        }

        private SimpleButton MakeBtn(string text, Color color, int x, bool enabled)
        {
            return new SimpleButton
            {
                Text = text,
                Location = new Point(x, 10),
                Size = new Size(136, 36),
                Font = new Font("Tahoma", 9),
                Enabled = enabled,
                Appearance = { BackColor = color, ForeColor = Color.White, BorderColor = color },
                LookAndFeel = { UseDefaultLookAndFeel = false, Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat }
            };
        }

        private static void AddCol(GridView v, string field, string caption, int width)
        {
            v.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
            {
                FieldName = field,
                Caption = caption,
                Width = width,
                OptionsColumn = { AllowEdit = false }
            });
        }

        // ── Load ──────────────────────────────────────────────────────────────────

        public async Task LoadAsync()
        {
            btnRefresh.Enabled = false;
            _categories = await ApiService.GetMenuAsync(AppSession.RestaurantId)
                          ?? new List<CategoryDto>();

            // Populate category dropdown
            cmbCategory.Properties.Items.Clear();
            cmbCategory.Properties.Items.Add("كل الأقسام");
            foreach (var c in _categories)
                cmbCategory.Properties.Items.Add(c.Name);
            cmbCategory.EditValue = "كل الأقسام";

            FilterProducts();
            btnRefresh.Enabled = true;
        }

        private void FilterProducts()
        {
            var sel = cmbCategory.EditValue?.ToString();
            if (string.IsNullOrEmpty(sel) || sel == "كل الأقسام")
                _products = _categories.SelectMany(c => c.Products).ToList();
            else
            {
                var cat = _categories.FirstOrDefault(c => c.Name == sel);
                _products = cat?.Products ?? new();
            }

            grid.DataSource = _products;
            lblCount.Text = $"{_products.Count} منتج";
            UpdateButtons();
        }

        private ProductDto? SelectedProduct
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
            var p = SelectedProduct;
            btnEdit.Enabled = p != null;
            btnToggle.Enabled = p != null;
            btnDelete.Enabled = p != null;
        }

        // ── CRUD ──────────────────────────────────────────────────────────────────

        private void ShowAddProduct()
        {
            if (!_categories.Any())
            {
                XtraMessageBox.Show(this, "لا يوجد أقسام في المنيو. أضف قسم أولاً.",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new ProductForm(_categories, null);
            if (dlg.ShowDialog(this) == DialogResult.OK)
                _ = LoadAsync();
        }

        private void ShowEditProduct()
        {
            var p = SelectedProduct;
            if (p == null) return;

            // ابحث عن الكاتيجوري الخاصة بالمنتج
            var cat = _categories.FirstOrDefault(c =>
                c.Products.Any(pr => pr.Id == p.Id));

            using var dlg = new ProductForm(_categories, p, cat?.Id);
            if (dlg.ShowDialog(this) == DialogResult.OK)
                _ = LoadAsync();
        }

        private async Task ToggleProductAsync()
        {
            var p = SelectedProduct;
            if (p == null) return;

            var action = p.IsAvailable ? "إيقاف" : "تفعيل";
            if (XtraMessageBox.Show(this, $"{action} المنتج: {p.Name}؟",
                "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            var ok = await ApiService.ToggleProductAvailabilityAsync(p.Id);
            if (ok) await LoadAsync();
            else XtraMessageBox.Show(this, "فشل تحديث المنتج", "خطأ",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task DeleteProductAsync()
        {
            var p = SelectedProduct;
            if (p == null) return;

            if (XtraMessageBox.Show(this, $"حذف المنتج: {p.Name}؟\nلن تتمكن من التراجع.",
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            var ok = await ApiService.DeleteProductAsync(p.Id);
            if (ok) await LoadAsync();
            else XtraMessageBox.Show(this, "فشل حذف المنتج", "خطأ",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
