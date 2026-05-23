using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class ProductForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly List<CategoryDto> _categories;
        private readonly ProductDto _existing;
        private readonly int? _existingCatId;

        // ── Controls ──────────────────────────────────────────────────────────────
        private DevExpress.XtraEditors.ComboBoxEdit cmbCategory;
        private TextEdit txtName;
        private MemoEdit txtDescription;
        private TextEdit txtPrice;
        private TextEdit txtDiscount;
        private TextEdit txtPrepTime;
        private TextEdit txtCalories;
        private TextEdit txtImageUrl;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;
        private LabelControl lblError;

        public ProductForm(List<CategoryDto> categories, ProductDto existing,
                           int? existingCatId = null)
        {
            _categories = categories;
            _existing = existing;
            _existingCatId = existingCatId;
            InitializeComponent();
            if (_existing != null) FillForm();
        }

       

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void Field(string label, ref int y)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = label;
            lbl.Font = new Font("Tahoma", 9);
            lbl.ForeColor = AppTheme.TextSecondary;
            lbl.Location = new Point(16, y);
            lbl.AutoSize = true;
            lbl.BackColor = AppTheme.Surface;
            Controls.Add(lbl);
            y += 20;
        }

        private TextEdit MakeTxt(ref int y, int height)
        {
            TextEdit txt = new TextEdit();
            txt.Location = new Point(16, y);
            txt.Size = new Size(456, height);
            txt.Font = new Font("Tahoma", 10);
            Controls.Add(txt);
            y += height + 8;
            return txt;
        }

        private void FillForm()
        {
            if (_existing == null) return;
            txtName.Text = _existing.Name;
            txtDescription.Text = _existing.Description ?? string.Empty;
            txtPrice.Text = _existing.Price.ToString("F2");

            // التحقق من وجود خصم (قيمة 0 تعني لا يوجد خصم)
            if (_existing.DiscountedPrice > 0)
                txtDiscount.Text = _existing.DiscountedPrice.ToString("F2");
            else
                txtDiscount.Text = string.Empty;

            txtPrepTime.Text = _existing.PreparationTime.ToString();

            if (_existing.Calories > 0)
                txtCalories.Text = _existing.Calories.ToString();
            else
                txtCalories.Text = string.Empty;

            txtImageUrl.Text = _existing.ImageUrl ?? string.Empty;
        }

        private async Task SaveAsync()
        {
            lblError.Text = string.Empty;

            string name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                lblError.Text = "اسم المنتج مطلوب";
                return;
            }

            decimal price;
            if (!decimal.TryParse(txtPrice.Text.Trim(), out price) || price <= 0)
            {
                lblError.Text = "برجاء إدخال سعر صحيح";
                return;
            }

            string catName = cmbCategory.EditValue?.ToString();

            CategoryDto cat = null;
            foreach (CategoryDto c in _categories)
            {
                if (c.Name == catName)
                {
                    cat = c;
                    break;
                }
            }

            if (cat == null)
            {
                lblError.Text = "برجاء اختيار القسم";
                return;
            }

            decimal discount = 0;  // 0 تعني لا يوجد خصم
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
            {
                decimal d;
                if (decimal.TryParse(txtDiscount.Text.Trim(), out d))
                    discount = d;
            }

            int prepTime = 15;
            if (!string.IsNullOrWhiteSpace(txtPrepTime.Text))
            {
                int pt;
                if (int.TryParse(txtPrepTime.Text.Trim(), out pt))
                    prepTime = pt;
            }

            int calories = 0;  // 0 تعني لا يوجد سعرات حرارية مدخلة
            if (!string.IsNullOrWhiteSpace(txtCalories.Text))
            {
                int cal;
                if (int.TryParse(txtCalories.Text.Trim(), out cal))
                    calories = cal;
            }

            CreateProductRequest req = new CreateProductRequest();
            req.CategoryId = cat.Id;
            req.Name = name;
            req.Description = txtDescription.Text.Trim();
            req.Price = price;
            req.DiscountedPrice = discount;
            req.PreparationTime = prepTime;
            req.Calories = calories;
            req.ImageUrl = StringExt.NullIfEmpty(txtImageUrl.Text.Trim());

            btnSave.Enabled = false;
            btnSave.Text = "جاري الحفظ...";

            bool ok;
            string error;

            if (_existing == null)
            {
                ProductResult result = await ApiService.CreateProductAsync(req);
                ok = result.Ok;      // ✅ استخدام Ok بدلاً من ok
                error = result.Error; // ✅ استخدام Error بدلاً من error
            }
            else
            {
                ProductResult result = await ApiService.UpdateProductAsync(_existing.Id, req);
                ok = result.Ok;      // ✅ استخدام Ok بدلاً من ok
                error = result.Error; // ✅ استخدام Error بدلاً من error
            }

            if (ok)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                lblError.Text = error;
                btnSave.Enabled = true;
                btnSave.Text = _existing == null ? "💾 إضافة" : "💾 حفظ التعديلات";
            }
        }
    }

    public static class StringExt
    {
        public static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }
    }
}