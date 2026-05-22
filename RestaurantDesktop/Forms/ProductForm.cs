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
using System.IO;

namespace RestaurantDesktop.Forms
{
    public partial class ProductForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly List<CategoryDto> _categories;
        private readonly ProductDto? _existing;
        private readonly int? _existingCatId;

        // ── Controls ──────────────────────────────────────────────────────────────
        private DevExpress.XtraEditors.ComboBoxEdit cmbCategory = null!;
        private TextEdit txtName = null!;
        private MemoEdit txtDescription = null!;
        private TextEdit txtPrice = null!;
        private TextEdit txtDiscount = null!;
        private TextEdit txtPrepTime = null!;
        private TextEdit txtCalories = null!;
        private TextEdit txtImageUrl = null!;
        private SimpleButton btnSave = null!;
        private SimpleButton btnCancel = null!;
        private LabelControl lblError = null!;

        public ProductForm(List<CategoryDto> categories, ProductDto? existing,
                           int? existingCatId = null)
        {
            _categories = categories;
            _existing = existing;
            _existingCatId = existingCatId;
            InitializeComponent();
            if (_existing != null) FillForm();
        }

        private void InitializeComponent()
        {
            Text = _existing == null ? "إضافة منتج جديد" : "تعديل المنتج";
            Size = new Size(500, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = AppTheme.Surface;

            int y = 16;

            Field("القسم", ref y);
            cmbCategory = new DevExpress.XtraEditors.ComboBoxEdit
            {
                Location = new Point(16, y),
                Size = new Size(456, 34)
            };
            cmbCategory.Font = new Font("Tahoma", 10);
            foreach (var c in _categories)
                cmbCategory.Properties.Items.Add(c.Name);
            if (_existingCatId.HasValue)
            {
                var cat = _categories.FirstOrDefault(c => c.Id == _existingCatId);
                if (cat != null) cmbCategory.EditValue = cat.Name;
            }
            Controls.Add(cmbCategory);
            y += 42;

            Field("اسم المنتج *", ref y);
            txtName = MakeTxt(ref y, 34);

            Field("الوصف", ref y);
            txtDescription = new MemoEdit
            {
                Location = new Point(16, y),
                Size = new Size(456, 60)
            };
            txtDescription.Font = new Font("Tahoma", 10);
            Controls.Add(txtDescription);
            y += 68;

            Field("السعر (EGP) *", ref y);
            txtPrice = MakeTxt(ref y, 34);

            Field("السعر بعد الخصم (اتركه فارغ لو مفيش خصم)", ref y);
            txtDiscount = MakeTxt(ref y, 34);

            Field("وقت التحضير (دقيقة)", ref y);
            txtPrepTime = MakeTxt(ref y, 34);
            txtPrepTime.Text = "15";

            Field("السعرات الحرارية (اختياري)", ref y);
            txtCalories = MakeTxt(ref y, 34);

            Field("رابط الصورة (اختياري)", ref y);
            txtImageUrl = MakeTxt(ref y, 34);

            // Error
            lblError = new LabelControl
            {
                Text = string.Empty,
                ForeColor = AppTheme.Danger,
                Font = new Font("Tahoma", 9),
                Location = new Point(16, y),
                Size = new Size(456, 20),
                AutoSizeMode = LabelAutoSizeMode.None,
                BackColor = AppTheme.Surface
            };
            Controls.Add(lblError);
            y += 26;

            // Buttons
            btnSave = new SimpleButton
            {
                Text = _existing == null ? "💾 إضافة" : "💾 حفظ التعديلات",
                Location = new Point(16, y),
                Size = new Size(220, 40),
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                Appearance = { BackColor = AppTheme.Primary, ForeColor = Color.White }
            };
            btnCancel = new SimpleButton
            {
                Text = "إلغاء",
                Location = new Point(252, y),
                Size = new Size(220, 40),
                Font = new Font("Tahoma", 11)
            };

            btnSave.Click += (_, _) => _ = SaveAsync();
            btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { btnSave, btnCancel });

            ClientSize = new Size(492, y + 56);
        }

        private void Field(string label, ref int y)
        {
            Controls.Add(new LabelControl
            {
                Text = label,
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(16, y),
                AutoSize = true,
                BackColor = AppTheme.Surface
            });
            y += 20;
        }

        private TextEdit MakeTxt(ref int y, int height)
        {
            var txt = new TextEdit
            {
                Location = new Point(16, y),
                Size = new Size(456, height)
            };
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
            txtDiscount.Text = _existing.DiscountedPrice?.ToString("F2") ?? string.Empty;
            txtPrepTime.Text = _existing.PreparationTime.ToString();
            txtCalories.Text = _existing.Calories?.ToString() ?? string.Empty;
            txtImageUrl.Text = _existing.ImageUrl ?? string.Empty;
        }

        private async Task SaveAsync()
        {
            lblError.Text = string.Empty;

            var name = txtName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            { lblError.Text = "اسم المنتج مطلوب"; return; }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price <= 0)
            { lblError.Text = "برجاء إدخال سعر صحيح"; return; }

            var catName = cmbCategory.EditValue?.ToString();
            var cat = _categories.FirstOrDefault(c => c.Name == catName);
            if (cat == null) { lblError.Text = "برجاء اختيار القسم"; return; }

            decimal? discount = null;
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text) &&
                decimal.TryParse(txtDiscount.Text.Trim(), out decimal d))
                discount = d;

            int prepTime = int.TryParse(txtPrepTime.Text.Trim(), out int pt) ? pt : 15;
            int? calories = null;
            if (int.TryParse(txtCalories.Text.Trim(), out int cal)) calories = cal;

            var req = new CreateProductRequest
            {
                CategoryId = cat.Id,
                Name = name,
                Description = txtDescription.Text.Trim(),
                Price = price,
                DiscountedPrice = discount,
                PreparationTime = prepTime,
                Calories = calories,
                ImageUrl = txtImageUrl.Text.Trim().NullIfEmpty()
            };

            btnSave.Enabled = false;
            btnSave.Text = "جاري الحفظ...";

            var (ok, error) = _existing == null
                ? await ApiService.CreateProductAsync(req)
                : await ApiService.UpdateProductAsync(_existing.Id, req);

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

    File static class StringExt
    {
        public static string? NullIfEmpty(this string? s) =>
            string.IsNullOrWhiteSpace(s) ? null : s;
    }
}