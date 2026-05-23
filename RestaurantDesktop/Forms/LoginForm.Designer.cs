
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class LoginForm
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
        /// 
        private void InitializeComponent()
        {
            this.pnlCard = new DevExpress.XtraEditors.PanelControl();
            this.lblEmoji = new DevExpress.XtraEditors.LabelControl();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblSubtitle = new DevExpress.XtraEditors.LabelControl();
            this.lblApiUrl = new DevExpress.XtraEditors.LabelControl();
            this.txtApiUrl = new DevExpress.XtraEditors.TextEdit();
            this.lblEmail = new DevExpress.XtraEditors.LabelControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblError = new DevExpress.XtraEditors.LabelControl();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCard)).BeginInit();
            this.pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtApiUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Appearance.BackColor = System.Drawing.Color.White;
            this.pnlCard.Appearance.Options.UseBackColor = true;
            this.pnlCard.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlCard.Controls.Add(this.lblEmoji);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.lblApiUrl);
            this.pnlCard.Controls.Add(this.txtApiUrl);
            this.pnlCard.Controls.Add(this.lblEmail);
            this.pnlCard.Controls.Add(this.txtEmail);
            this.pnlCard.Controls.Add(this.lblPassword);
            this.pnlCard.Controls.Add(this.txtPassword);
            this.pnlCard.Controls.Add(this.lblError);
            this.pnlCard.Controls.Add(this.btnLogin);
            this.pnlCard.Location = new System.Drawing.Point(40, 60);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(400, 500);
            this.pnlCard.TabIndex = 0;
            // 
            // lblEmoji
            // 
            this.lblEmoji.Appearance.BackColor = System.Drawing.Color.White;
            this.lblEmoji.Appearance.Font = new System.Drawing.Font("Segoe UI Emoji", 48F);
            this.lblEmoji.Appearance.Options.UseBackColor = true;
            this.lblEmoji.Appearance.Options.UseFont = true;
            this.lblEmoji.Location = new System.Drawing.Point(155, 20);
            this.lblEmoji.Name = "lblEmoji";
            this.lblEmoji.Size = new System.Drawing.Size(88, 85);
            this.lblEmoji.TabIndex = 0;
            this.lblEmoji.Text = "🍽️";
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.BackColor = System.Drawing.Color.White;
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(34)))));
            this.lblTitle.Appearance.Options.UseBackColor = true;
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.Appearance.Options.UseForeColor = true;
            this.lblTitle.Location = new System.Drawing.Point(130, 100);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 33);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "إدارة المطعم";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Appearance.BackColor = System.Drawing.Color.White;
            this.lblSubtitle.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblSubtitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblSubtitle.Appearance.Options.UseBackColor = true;
            this.lblSubtitle.Appearance.Options.UseFont = true;
            this.lblSubtitle.Appearance.Options.UseForeColor = true;
            this.lblSubtitle.Location = new System.Drawing.Point(140, 140);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(125, 17);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "سجّل دخولك للمتابعة";
            // 
            // lblApiUrl
            // 
            this.lblApiUrl.Appearance.BackColor = System.Drawing.Color.White;
            this.lblApiUrl.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblApiUrl.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblApiUrl.Appearance.Options.UseBackColor = true;
            this.lblApiUrl.Appearance.Options.UseFont = true;
            this.lblApiUrl.Appearance.Options.UseForeColor = true;
            this.lblApiUrl.Location = new System.Drawing.Point(30, 185);
            this.lblApiUrl.Name = "lblApiUrl";
            this.lblApiUrl.Size = new System.Drawing.Size(60, 14);
            this.lblApiUrl.TabIndex = 3;
            this.lblApiUrl.Text = "رابط السيرفر";
            // 
            // txtApiUrl
            // 
            this.txtApiUrl.EditValue = "https://deliveryappapi.runasp.net";
            this.txtApiUrl.Location = new System.Drawing.Point(30, 205);
            this.txtApiUrl.Name = "txtApiUrl";
            this.txtApiUrl.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtApiUrl.Properties.Appearance.Options.UseFont = true;
            this.txtApiUrl.Properties.NullValuePrompt = "https://localhost:7001";
            this.txtApiUrl.Size = new System.Drawing.Size(340, 22);
            this.txtApiUrl.TabIndex = 4;
            // 
            // lblEmail
            // 
            this.lblEmail.Appearance.BackColor = System.Drawing.Color.White;
            this.lblEmail.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblEmail.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblEmail.Appearance.Options.UseBackColor = true;
            this.lblEmail.Appearance.Options.UseFont = true;
            this.lblEmail.Appearance.Options.UseForeColor = true;
            this.lblEmail.Location = new System.Drawing.Point(30, 250);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(77, 14);
            this.lblEmail.TabIndex = 5;
            this.lblEmail.Text = "البريد الإلكتروني";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(30, 270);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtEmail.Properties.Appearance.Options.UseFont = true;
            this.txtEmail.Properties.NullValuePrompt = "admin@restaurant.com";
            this.txtEmail.Size = new System.Drawing.Size(340, 22);
            this.txtEmail.TabIndex = 6;
            // 
            // lblPassword
            // 
            this.lblPassword.Appearance.BackColor = System.Drawing.Color.White;
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblPassword.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(117)))), ((int)(((byte)(117)))));
            this.lblPassword.Appearance.Options.UseBackColor = true;
            this.lblPassword.Appearance.Options.UseFont = true;
            this.lblPassword.Appearance.Options.UseForeColor = true;
            this.lblPassword.Location = new System.Drawing.Point(30, 315);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(55, 14);
            this.lblPassword.TabIndex = 7;
            this.lblPassword.Text = "كلمة المرور";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(30, 335);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.NullValuePrompt = "••••••••";
            this.txtPassword.Properties.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(340, 22);
            this.txtPassword.TabIndex = 8;
            // 
            // lblError
            // 
            this.lblError.Appearance.BackColor = System.Drawing.Color.White;
            this.lblError.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblError.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.lblError.Appearance.Options.UseBackColor = true;
            this.lblError.Appearance.Options.UseFont = true;
            this.lblError.Appearance.Options.UseForeColor = true;
            this.lblError.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblError.Location = new System.Drawing.Point(30, 382);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(340, 20);
            this.lblError.TabIndex = 9;
            // 
            // btnLogin
            // 
            this.btnLogin.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(34)))));
            this.btnLogin.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(34)))));
            this.btnLogin.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Appearance.Options.UseBackColor = true;
            this.btnLogin.Appearance.Options.UseBorderColor = true;
            this.btnLogin.Appearance.Options.UseFont = true;
            this.btnLogin.Appearance.Options.UseForeColor = true;
            this.btnLogin.Location = new System.Drawing.Point(30, 408);
            this.btnLogin.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.btnLogin.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(340, 44);
            this.btnLogin.TabIndex = 10;
            this.btnLogin.Text = "تسجيل الدخول";
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(478, 588);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Restaurant Manager — تسجيل الدخول";
            ((System.ComponentModel.ISupportInitialize)(this.pnlCard)).EndInit();
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtApiUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        /* private void InitializeComponent()
         {
             this.components = new System.ComponentModel.Container();
             this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
             this.Text = "LoginForm";
         }*/

        #endregion

        private LabelControl lblEmoji;
    }
}