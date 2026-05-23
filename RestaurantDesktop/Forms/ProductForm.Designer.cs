
using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class ProductForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
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
            cmbCategory = new DevExpress.XtraEditors.ComboBoxEdit();
            cmbCategory.Location = new Point(16, y);
            cmbCategory.Size = new Size(456, 34);
            cmbCategory.Font = new Font("Tahoma", 10);

            foreach (CategoryDto c in _categories)
                cmbCategory.Properties.Items.Add(c.Name);

            if (_existingCatId.HasValue)
            {
                CategoryDto cat = null;
                foreach (CategoryDto c in _categories)
                {
                    if (c.Id == _existingCatId.Value)
                    {
                        cat = c;
                        break;
                    }
                }
                if (cat != null) cmbCategory.EditValue = cat.Name;
            }
            Controls.Add(cmbCategory);
            y += 42;

            Field("اسم المنتج *", ref y);
            txtName = MakeTxt(ref y, 34);

            Field("الوصف", ref y);
            txtDescription = new MemoEdit();
            txtDescription.Location = new Point(16, y);
            txtDescription.Size = new Size(456, 60);
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
            lblError = new LabelControl();
            lblError.Text = string.Empty;
            lblError.ForeColor = AppTheme.Danger;
            lblError.Font = new Font("Tahoma", 9);
            lblError.Location = new Point(16, y);
            lblError.Size = new Size(456, 20);
            lblError.AutoSizeMode = LabelAutoSizeMode.None;
            lblError.BackColor = AppTheme.Surface;
            Controls.Add(lblError);
            y += 26;

            // Buttons
            btnSave = new SimpleButton();
            btnSave.Text = _existing == null ? "💾 إضافة" : "💾 حفظ التعديلات";
            btnSave.Location = new Point(16, y);
            btnSave.Size = new Size(220, 40);
            btnSave.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnSave.Appearance.BackColor = AppTheme.Primary;
            btnSave.Appearance.ForeColor = Color.White;

            btnCancel = new SimpleButton();
            btnCancel.Text = "إلغاء";
            btnCancel.Location = new Point(252, y);
            btnCancel.Size = new Size(220, 40);
            btnCancel.Font = new Font("Tahoma", 11);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            Controls.AddRange(new Control[] { btnSave, btnCancel });

            ClientSize = new Size(492, y + 56);
        }
        #endregion
    }
}