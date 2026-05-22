using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
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
            Text = $"تفاصيل الأوردر #{_order.Id}";
            Size = new Size(600, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = AppTheme.Surface;

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            int y = 16;

            // Status badge
            var pnlStatus = new Panel
            {
                Location = new Point(16, y),
                Size = new Size(560, 40),
                BackColor = AppTheme.StatusColor(_order.Status)
            };
            var lblStatus = new LabelControl
            {
                Text = AppTheme.StatusArabic(_order.Status),
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlignment = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlStatus.Controls.Add(lblStatus);
            scroll.Controls.Add(pnlStatus);
            y += 52;

            // Info rows
            AddRow(scroll, ref y, "رقم الأوردر", $"#{_order.Id}");
            AddRow(scroll, ref y, "وقت الطلب", _order.CreatedAt.ToLocalTime().ToString("hh:mm tt  dd/MM/yyyy"));
            AddRow(scroll, ref y, "طريقة الدفع", _order.PaymentMethod);
            AddRow(scroll, ref y, "حالة الدفع", _order.PaymentStatus);
            AddRow(scroll, ref y, "عنوان التوصيل", _order.DeliveryAddress);

            if (!string.IsNullOrEmpty(_order.DeliveryNotes))
                AddRow(scroll, ref y, "ملاحظات", _order.DeliveryNotes);

            // Separator
            scroll.Controls.Add(new Panel { Location = new Point(16, y), Size = new Size(560, 1), BackColor = Color.FromArgb(224, 224, 224) });
            y += 14;

            // Items
            var lblItems = new LabelControl
            {
                Text = "المنتجات:",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(16, y),
                AutoSize = true,
                BackColor = AppTheme.Surface
            };
            scroll.Controls.Add(lblItems);
            y += 28;

            foreach (var item in _order.Items)
            {
                var pnlItem = new PanelControl
                {
                    Location = new Point(16, y),
                    Size = new Size(560, 36),
                    BackColor = Color.FromArgb(248, 248, 248)
                };
                pnlItem.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

                var lblName = new LabelControl
                {
                    Text = item.ProductName,
                    Font = new Font("Tahoma", 9, FontStyle.Bold),
                    ForeColor = AppTheme.TextPrimary,
                    Location = new Point(8, 8),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                var lblQty = new LabelControl
                {
                    Text = $"× {item.Quantity}",
                    Font = new Font("Tahoma", 9),
                    ForeColor = AppTheme.TextSecondary,
                    Location = new Point(200, 8),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                var lblPrice = new LabelControl
                {
                    Text = $"{item.TotalPrice:F0} EGP",
                    Font = new Font("Tahoma", 9, FontStyle.Bold),
                    ForeColor = AppTheme.Primary,
                    Location = new Point(460, 8),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                pnlItem.Controls.AddRange(new Control[] { lblName, lblQty, lblPrice });
                scroll.Controls.Add(pnlItem);
                y += 42;
            }

            // Separator
            scroll.Controls.Add(new Panel { Location = new Point(16, y), Size = new Size(560, 1), BackColor = Color.FromArgb(224, 224, 224) });
            y += 14;

            // Totals
            AddRow(scroll, ref y, "المجموع الجزئي", $"{_order.SubTotal:F0} EGP");
            AddRow(scroll, ref y, "رسوم التوصيل", $"{_order.DeliveryFee:F0} EGP");
            if (_order.Discount > 0)
                AddRow(scroll, ref y, "الخصم", $"- {_order.Discount:F0} EGP");

            // Total highlighted
            var pnlTotal = new PanelControl
            {
                Location = new Point(16, y),
                Size = new Size(560, 42),
                BackColor = AppTheme.Primary
            };
            pnlTotal.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            var lblTotalLabel = new LabelControl
            {
                Text = "الإجمالي",
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            var lblTotalVal = new LabelControl
            {
                Text = $"{_order.TotalAmount:F0} EGP",
                Font = new Font("Tahoma", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(430, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlTotal.Controls.AddRange(new Control[] { lblTotalLabel, lblTotalVal });
            scroll.Controls.Add(pnlTotal);
            y += 56;

            scroll.Controls.Add(new Panel { Height = 20, Top = y, Width = 1 }); // bottom padding

            // Close button
            var btnClose = new SimpleButton
            {
                Text = "إغلاق",
                Dock = DockStyle.Bottom,
                Height = 44,
                Font = new Font("Tahoma", 11),
                Appearance = { BackColor = AppTheme.Primary, ForeColor = Color.White }
            };
            btnClose.Click += (_, _) => Close();

            Controls.Add(scroll);
            Controls.Add(btnClose);
        }

        private void AddRow(Panel parent, ref int y, string label, string value)
        {
            var lbl = new LabelControl
            {
                Text = label + ":",
                Font = new Font("Tahoma", 9),
                ForeColor = AppTheme.TextSecondary,
                Location = new Point(16, y),
                Size = new Size(140, 22),
                AutoSizeMode = LabelAutoSizeMode.None,
                BackColor = AppTheme.Surface
            };
            var val = new LabelControl
            {
                Text = value,
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(160, y),
                Size = new Size(420, 22),
                AutoSizeMode = LabelAutoSizeMode.None,
                BackColor = AppTheme.Surface
            };
            parent.Controls.AddRange(new Control[] { lbl, val });
            y += 28;
        }
    }
}