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