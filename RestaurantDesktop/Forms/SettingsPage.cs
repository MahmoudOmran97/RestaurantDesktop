using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class SettingsPage : DevExpress.XtraEditors.XtraUserControl
    {
        private TextEdit txtRestaurantId;
        private TextEdit txtApiUrl;
        private SimpleButton btnSave;
        private SimpleButton btnTest;
        private LabelControl lblStatus;
        private LabelControl lblRestName;

        public SettingsPage()
        {
            InitLayout();
            LoadCurrentValues();
        }

        private void InitLayout()
        {
            BackColor = AppTheme.Background;
            Dock = DockStyle.Fill;

            // ── Card ─────────────────────────────────────────────────────────────
            PanelControl card = new PanelControl();
            card.Size = new Size(480, 380);
            card.Location = new Point(40, 40);
            card.BackColor = AppTheme.Surface;
            card.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

            // ── Title ─────────────────────────────────────────────────────────────
            LabelControl lblTitle = new LabelControl();
            lblTitle.Text = "⚙️  إعدادات التطبيق";
            lblTitle.Font = new Font("Tahoma", 14, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.Primary;
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            lblTitle.BackColor = AppTheme.Surface;

            // ── Restaurant ID ────────────────────────────────────────────────────
            LabelControl lblId = new LabelControl();
            lblId.Text = "معرّف المطعم (RestaurantId):";
            lblId.Font = new Font("Tahoma", 10);
            lblId.Location = new Point(20, 80);
            lblId.AutoSize = true;
            lblId.BackColor = AppTheme.Surface;

            txtRestaurantId = new TextEdit();
            txtRestaurantId.Location = new Point(20, 106);
            txtRestaurantId.Size = new Size(200, 36);
            txtRestaurantId.Font = new Font("Tahoma", 11);

            lblRestName = new LabelControl();
            lblRestName.Text = "";
            lblRestName.Font = new Font("Tahoma", 9);
            lblRestName.ForeColor = AppTheme.Success;
            lblRestName.Location = new Point(230, 114);
            lblRestName.AutoSize = true;
            lblRestName.BackColor = AppTheme.Surface;

            // ── API URL ──────────────────────────────────────────────────────────
            LabelControl lblUrl = new LabelControl();
            lblUrl.Text = "رابط الـ API:";
            lblUrl.Font = new Font("Tahoma", 10);
            lblUrl.Location = new Point(20, 160);
            lblUrl.AutoSize = true;
            lblUrl.BackColor = AppTheme.Surface;

            txtApiUrl = new TextEdit();
            txtApiUrl.Location = new Point(20, 186);
            txtApiUrl.Size = new Size(430, 36);
            txtApiUrl.Font = new Font("Tahoma", 10);

            // ── Hint ─────────────────────────────────────────────────────────────
            LabelControl lblHint = new LabelControl();
            lblHint.Text = "💡 بعد الحفظ ستحتاج لإعادة تشغيل البرنامج لتطبيق التغييرات";
            lblHint.Font = new Font("Tahoma", 8);
            lblHint.ForeColor = AppTheme.TextSecondary;
            lblHint.Location = new Point(20, 240);
            lblHint.AutoSize = true;
            lblHint.BackColor = AppTheme.Surface;

            // ── Buttons ───────────────────────────────────────────────────────────
            btnTest = new SimpleButton();
            btnTest.Text = "🔍 اختبار الاتصال";
            btnTest.Location = new Point(20, 280);
            btnTest.Size = new Size(160, 40);
            btnTest.Font = new Font("Tahoma", 10);
            btnTest.Appearance.BackColor = AppTheme.Info;
            btnTest.Appearance.ForeColor = Color.White;
            btnTest.LookAndFeel.UseDefaultLookAndFeel = false;
            btnTest.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            btnTest.Click += BtnTest_Click;

            btnSave = new SimpleButton();
            btnSave.Text = "💾 حفظ الإعدادات";
            btnSave.Location = new Point(200, 280);
            btnSave.Size = new Size(160, 40);
            btnSave.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnSave.Appearance.BackColor = AppTheme.Success;
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
            btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            btnSave.Click += BtnSave_Click;

            lblStatus = new LabelControl();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Tahoma", 9);
            lblStatus.Location = new Point(20, 336);
            lblStatus.AutoSize = true;
            lblStatus.BackColor = AppTheme.Surface;

            card.Controls.AddRange(new Control[]
            {
                lblTitle, lblId, txtRestaurantId, lblRestName,
                lblUrl, txtApiUrl, lblHint,
                btnTest, btnSave, lblStatus
            });

            Controls.Add(card);
        }

        private void LoadCurrentValues()
        {
            txtRestaurantId.Text = AppSession.RestaurantId.ToString();
            txtApiUrl.Text = AppConfig.BaseUrl;
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;
            lblStatus.Text = "جاري الاختبار...";
            lblStatus.ForeColor = AppTheme.TextSecondary;
            lblRestName.Text = "";

            try
            {
                // حاول تجيب بيانات المطعم
                if (!int.TryParse(txtRestaurantId.Text.Trim(), out int rid) || rid <= 0)
                {
                    lblStatus.Text = "❌ معرّف المطعم غير صحيح";
                    lblStatus.ForeColor = AppTheme.Danger;
                    return;
                }

                // تحديث الـ URL مؤقتاً للاختبار
                string oldUrl = AppConfig.BaseUrl;
                if (!string.IsNullOrWhiteSpace(txtApiUrl.Text))
                    AppConfig.BaseUrl = txtApiUrl.Text.TrimEnd('/');

                RestaurantDto r = await ApiService.GetRestaurantAsync(rid);

                AppConfig.BaseUrl = oldUrl; // رجّع القديم

                if (r != null)
                {
                    lblStatus.Text = "✅ الاتصال ناجح";
                    lblStatus.ForeColor = AppTheme.Success;
                    lblRestName.Text = "← " + r.Name;
                }
                else
                {
                    lblStatus.Text = "❌ المطعم غير موجود أو الرابط خاطئ";
                    lblStatus.ForeColor = AppTheme.Danger;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ خطأ: " + ex.Message;
                lblStatus.ForeColor = AppTheme.Danger;
            }
            finally
            {
                btnTest.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtRestaurantId.Text.Trim(), out int rid) || rid <= 0)
            {
                XtraMessageBox.Show(this, "برجاء إدخال معرّف مطعم صحيح", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string apiUrl = txtApiUrl.Text.Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(apiUrl))
            {
                XtraMessageBox.Show(this, "برجاء إدخال رابط الـ API", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // احفظ في App.config
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["RestaurantId"].Value = rid.ToString();
                config.AppSettings.Settings["ApiBaseUrl"].Value = apiUrl;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // حدّث الجلسة الحالية فوراً
                AppSession.RestaurantId = rid;
                AppConfig.BaseUrl = apiUrl;

                lblStatus.Text = "✅ تم الحفظ — سيطبّق في الجلسة الحالية فوراً";
                lblStatus.ForeColor = AppTheme.Success;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ فشل الحفظ: " + ex.Message;
                lblStatus.ForeColor = AppTheme.Danger;
            }
        }
    }
}