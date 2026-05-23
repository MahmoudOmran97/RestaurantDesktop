using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class OrderDetailForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly OrderDetail _order;

        public OrderDetailForm(OrderDetail order)
        {
            _order = order;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "تفاصيل الأوردر #" + _order.Id;
            Size = new Size(600, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = AppTheme.Surface;

            Panel scroll = new Panel();
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            int y = 16;

            // Status badge
            Panel pnlStatus = new Panel();
            pnlStatus.Location = new Point(16, y);
            pnlStatus.Size = new Size(560, 40);
            pnlStatus.BackColor = AppTheme.StatusColor(_order.Status);

            LabelControl lblStatus = new LabelControl();
            lblStatus.Text = AppTheme.StatusArabic(_order.Status);
            lblStatus.Font = new Font("Tahoma", 14, FontStyle.Bold);
            lblStatus.ForeColor = Color.White;
            lblStatus.Dock = DockStyle.Fill;
            // تم إزالة خاصية TextAlignment - استخدام Location و Size بدلاً من ذلك
            lblStatus.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblStatus.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            lblStatus.BackColor = Color.Transparent;

            pnlStatus.Controls.Add(lblStatus);
            scroll.Controls.Add(pnlStatus);
            y += 52;

            // Info rows
            AddRow(scroll, ref y, "رقم الأوردر", "#" + _order.Id);
            AddRow(scroll, ref y, "وقت الطلب", _order.CreatedAt.ToLocalTime().ToString("hh:mm tt  dd/MM/yyyy"));
            AddRow(scroll, ref y, "طريقة الدفع", _order.PaymentMethod);
            AddRow(scroll, ref y, "حالة الدفع", _order.PaymentStatus);
            AddRow(scroll, ref y, "عنوان التوصيل", _order.DeliveryAddress);

            if (!string.IsNullOrEmpty(_order.DeliveryNotes))
                AddRow(scroll, ref y, "ملاحظات", _order.DeliveryNotes);

            // Separator
            Panel separator1 = new Panel();
            separator1.Location = new Point(16, y);
            separator1.Size = new Size(560, 1);
            separator1.BackColor = Color.FromArgb(224, 224, 224);
            scroll.Controls.Add(separator1);
            y += 14;

            // Items
            LabelControl lblItems = new LabelControl();
            lblItems.Text = "المنتجات:";
            lblItems.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblItems.ForeColor = AppTheme.TextPrimary;
            lblItems.Location = new Point(16, y);
            lblItems.AutoSize = true;
            lblItems.BackColor = AppTheme.Surface;
            scroll.Controls.Add(lblItems);
            y += 28;

            foreach (OrderItemDto item in _order.Items)
            {
                PanelControl pnlItem = new PanelControl();
                pnlItem.Location = new Point(16, y);
                pnlItem.Size = new Size(560, 36);
                pnlItem.BackColor = Color.FromArgb(248, 248, 248);
                pnlItem.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

                LabelControl lblName = new LabelControl();
                lblName.Text = item.ProductName;
                lblName.Font = new Font("Tahoma", 9, FontStyle.Bold);
                lblName.ForeColor = AppTheme.TextPrimary;
                lblName.Location = new Point(8, 8);
                lblName.AutoSize = true;
                lblName.BackColor = Color.Transparent;

                LabelControl lblQty = new LabelControl();
                lblQty.Text = "× " + item.Quantity;
                lblQty.Font = new Font("Tahoma", 9);
                lblQty.ForeColor = AppTheme.TextSecondary;
                lblQty.Location = new Point(200, 8);
                lblQty.AutoSize = true;
                lblQty.BackColor = Color.Transparent;

                LabelControl lblPrice = new LabelControl();
                lblPrice.Text = string.Format("{0:F0} EGP", item.TotalPrice);
                lblPrice.Font = new Font("Tahoma", 9, FontStyle.Bold);
                lblPrice.ForeColor = AppTheme.Primary;
                lblPrice.Location = new Point(460, 8);
                lblPrice.AutoSize = true;
                lblPrice.BackColor = Color.Transparent;

                pnlItem.Controls.AddRange(new Control[] { lblName, lblQty, lblPrice });
                scroll.Controls.Add(pnlItem);
                y += 42;
            }

            // Separator
            Panel separator2 = new Panel();
            separator2.Location = new Point(16, y);
            separator2.Size = new Size(560, 1);
            separator2.BackColor = Color.FromArgb(224, 224, 224);
            scroll.Controls.Add(separator2);
            y += 14;

            // Totals
            AddRow(scroll, ref y, "المجموع الجزئي", string.Format("{0:F0} EGP", _order.SubTotal));
            AddRow(scroll, ref y, "رسوم التوصيل", string.Format("{0:F0} EGP", _order.DeliveryFee));
            if (_order.Discount > 0)
                AddRow(scroll, ref y, "الخصم", string.Format("- {0:F0} EGP", _order.Discount));

            // Total highlighted
            PanelControl pnlTotal = new PanelControl();
            pnlTotal.Location = new Point(16, y);
            pnlTotal.Size = new Size(560, 42);
            pnlTotal.BackColor = AppTheme.Primary;
            pnlTotal.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblTotalLabel = new LabelControl();
            lblTotalLabel.Text = "الإجمالي";
            lblTotalLabel.Font = new Font("Tahoma", 12, FontStyle.Bold);
            lblTotalLabel.ForeColor = Color.White;
            lblTotalLabel.Location = new Point(12, 10);
            lblTotalLabel.AutoSize = true;
            lblTotalLabel.BackColor = Color.Transparent;

            LabelControl lblTotalVal = new LabelControl();
            lblTotalVal.Text = string.Format("{0:F0} EGP", _order.TotalAmount);
            lblTotalVal.Font = new Font("Tahoma", 13, FontStyle.Bold);
            lblTotalVal.ForeColor = Color.White;
            lblTotalVal.Location = new Point(430, 10);
            lblTotalVal.AutoSize = true;
            lblTotalVal.BackColor = Color.Transparent;

            pnlTotal.Controls.AddRange(new Control[] { lblTotalLabel, lblTotalVal });
            scroll.Controls.Add(pnlTotal);
            y += 56;

            // bottom padding
            Panel bottomPadding = new Panel();
            bottomPadding.Height = 20;
            bottomPadding.Top = y;
            bottomPadding.Width = 1;
            scroll.Controls.Add(bottomPadding);

            // Close button
            SimpleButton btnClose = new SimpleButton();
            btnClose.Text = "إغلاق";
            btnClose.Dock = DockStyle.Bottom;
            btnClose.Height = 44;
            btnClose.Font = new Font("Tahoma", 11);
            btnClose.Appearance.BackColor = AppTheme.Primary;
            btnClose.Appearance.ForeColor = Color.White;
            btnClose.Click += BtnClose_Click;

            Controls.Add(scroll);
            Controls.Add(btnClose);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddRow(Panel parent, ref int y, string label, string value)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = label + ":";
            lbl.Font = new Font("Tahoma", 9);
            lbl.ForeColor = AppTheme.TextSecondary;
            lbl.Location = new Point(16, y);
            lbl.Size = new Size(140, 22);
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.BackColor = AppTheme.Surface;

            LabelControl val = new LabelControl();
            val.Text = value;
            val.Font = new Font("Tahoma", 9, FontStyle.Bold);
            val.ForeColor = AppTheme.TextPrimary;
            val.Location = new Point(160, y);
            val.Size = new Size(420, 22);
            val.AutoSizeMode = LabelAutoSizeMode.None;
            val.BackColor = AppTheme.Surface;

            parent.Controls.AddRange(new Control[] { lbl, val });
            y += 28;
        }
    }
}