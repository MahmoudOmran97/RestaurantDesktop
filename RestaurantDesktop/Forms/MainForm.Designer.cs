
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class MainForm
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
            Text = "Restaurant Manager";
            Size = new Size(1280, 800);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            // ── Sidebar ───────────────────────────────────────────────────────────
            pnlSidebar = new PanelControl();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 220;
            pnlSidebar.BackColor = AppTheme.PrimaryDark;
            pnlSidebar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            // Restaurant name at top
            PanelControl pnlTop = new PanelControl();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 90;
            pnlTop.BackColor = AppTheme.Primary;
            pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblEmoji = new LabelControl();
            lblEmoji.Text = "🍽️";
            lblEmoji.Font = new Font("Segoe UI Emoji", 22);
            lblEmoji.Location = new Point(16, 12);
            lblEmoji.AutoSize = true;
            lblEmoji.BackColor = AppTheme.Primary;

            lblRestName = new LabelControl();
            lblRestName.Text = "المطعم";
            lblRestName.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblRestName.ForeColor = Color.White;
            lblRestName.Location = new Point(58, 12);
            lblRestName.Size = new Size(155, 22);
            lblRestName.AutoSizeMode = LabelAutoSizeMode.None;
            lblRestName.BackColor = AppTheme.Primary;

            lblUserName = new LabelControl();
            lblUserName.Text = AppSession.FullName;
            lblUserName.Font = new Font("Tahoma", 8);
            lblUserName.ForeColor = Color.FromArgb(220, 220, 220);
            lblUserName.Location = new Point(58, 40);
            lblUserName.Size = new Size(155, 18);
            lblUserName.AutoSizeMode = LabelAutoSizeMode.None;
            lblUserName.BackColor = AppTheme.Primary;

            pnlTop.Controls.AddRange(new Control[] { lblEmoji, lblRestName, lblUserName });

            // Nav buttons
            int btnY = 10;
            btnDashboard = MakeSidebarBtn("📊  الرئيسية", ref btnY);
            btnOrders = MakeSidebarBtn("📋  الأوردرات", ref btnY);
            btnMenu = MakeSidebarBtn("🍕  المنيو", ref btnY);
            btnSettings = MakeSidebarBtn("⚙️  الإعدادات", ref btnY);

            // Badge للأوردرات الجديدة
            lblNewOrders = new LabelControl();
            lblNewOrders.Text = string.Empty;
            lblNewOrders.Font = new Font("Tahoma", 8, FontStyle.Bold);
            lblNewOrders.ForeColor = Color.White;
            lblNewOrders.BackColor = AppTheme.Danger;
            lblNewOrders.Location = new Point(175, btnOrders.Top + 12);
            lblNewOrders.Size = new Size(26, 18);
            lblNewOrders.AutoSizeMode = LabelAutoSizeMode.None;
            // تم إزالة خاصية TextAlignment لأنها غير موجودة في LabelControl
            lblNewOrders.Visible = false;

            // Spacer + Logout at bottom
            btnLogout = new SimpleButton();
            btnLogout.Text = "🚪  تسجيل الخروج";
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.Height = 46;
            btnLogout.Font = new Font("Tahoma", 10);
            btnLogout.Appearance.BackColor = AppTheme.Danger;
            btnLogout.Appearance.ForeColor = Color.White;
            btnLogout.Appearance.BorderColor = AppTheme.Danger;
            btnLogout.LookAndFeel.UseDefaultLookAndFeel = false;
            btnLogout.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            btnLogout.Click += BtnLogout_Click;

            PanelControl pnlNav = new PanelControl();
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.BackColor = AppTheme.PrimaryDark;
            pnlNav.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlNav.Controls.AddRange(new Control[]
                { btnDashboard, btnOrders, btnMenu, btnSettings, lblNewOrders });

            pnlSidebar.Controls.Add(pnlNav);
            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(pnlTop);

            // ── Header ─────────────────────────────────────────────────────────────
            pnlHeader = new PanelControl();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 52;
            pnlHeader.BackColor = AppTheme.Surface;
            pnlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            LabelControl lblPageTitle = new LabelControl();
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Text = "الرئيسية";
            lblPageTitle.Font = new Font("Tahoma", 14, FontStyle.Bold);
            lblPageTitle.ForeColor = AppTheme.TextPrimary;
            lblPageTitle.Location = new Point(16, 14);
            lblPageTitle.AutoSize = true;
            lblPageTitle.BackColor = AppTheme.Surface;

            LabelControl lblDate = new LabelControl();
            lblDate.Text = DateTime.Now.ToString("dddd، d MMMM yyyy",
                            new System.Globalization.CultureInfo("ar-EG"));
            lblDate.Font = new Font("Tahoma", 9);
            lblDate.ForeColor = AppTheme.TextSecondary;
            lblDate.Dock = DockStyle.Right;
            lblDate.AutoSize = true;
            lblDate.BackColor = AppTheme.Surface;
            lblDate.Padding = new Padding(0, 0, 16, 0);

            pnlHeader.Controls.AddRange(new Control[] { lblPageTitle, lblDate });

            // ── Content ────────────────────────────────────────────────────────────
            pnlContent = new PanelControl();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = AppTheme.Background;
            pnlContent.Padding = new Padding(0);
            pnlContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);

            // Wire nav clicks - تم إزالة discard parameters (_)
            btnDashboard.Click += (sender, e) => { ShowPage(GetDashboardPage(), btnDashboard); SetTitle("الرئيسية"); };
            btnOrders.Click += (sender, e) => { ShowPage(GetOrdersPage(), btnOrders); SetTitle("الأوردرات"); ClearNewOrderBadge(); };
            btnMenu.Click += (sender, e) => { ShowPage(GetMenuPage(), btnMenu); SetTitle("المنيو"); };
            //  btnSettings.Click += (sender, e) => { ShowPage(GetSettingsPage(), btnSettings); SetTitle("الإعدادات"); };

            Shown += OnShown;
        }


        #endregion
    }
}