using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    partial class SettingsPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

     /*   private void InitLayout()
        {
            BackColor = AppTheme.Background;
            Dock = DockStyle.Fill;

            Panel scrollWrapper = new Panel();
            scrollWrapper.Dock = DockStyle.Fill;
            scrollWrapper.AutoScroll = true;
            scrollWrapper.BackColor = AppTheme.Background;

            // ── كارت إعدادات الاتصال ────────────────────────────────────────────
            PanelControl cardConn = MakeCard(40, 40, 580, 340, "🔌  إعدادات الاتصال");

            LabelControl lblId = MakeLabel("معرّف المطعم (RestaurantId):", 20, 80);
            txtRestaurantId = MakeTextEdit(20, 106, 180, 36);

            lblRestName = new LabelControl();
            lblRestName.Text = "";
            lblRestName.Font = new Font("Tahoma", 9);
            lblRestName.ForeColor = AppTheme.Success;
            lblRestName.Location = new Point(210, 114);
            lblRestName.AutoSize = true;
            lblRestName.BackColor = AppTheme.Surface;

            LabelControl lblUrl = MakeLabel("رابط الـ API:", 20, 160);
            txtApiUrl = MakeTextEdit(20, 186, 530, 36);

            LabelControl lblHint = new LabelControl();
            lblHint.Text = "💡 التغييرات تُطبَّق فوراً";
            lblHint.Font = new Font("Tahoma", 8);
            lblHint.ForeColor = AppTheme.TextSecondary;
            lblHint.Location = new Point(20, 236);
            lblHint.AutoSize = true;
            lblHint.BackColor = AppTheme.Surface;

            btnTest = MakeBtn("🔍 اختبار الاتصال", AppTheme.Info, 20, 268, 160, 40);
            btnSave = MakeBtn("💾 حفظ الإعدادات", AppTheme.Success, 196, 268, 160, 40, true);
            btnTest.Click += BtnTest_Click;
            btnSave.Click += BtnSave_Click;

            lblStatus = new LabelControl();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Tahoma", 9);
            lblStatus.Location = new Point(20, 316);
            lblStatus.AutoSize = true;
            lblStatus.BackColor = AppTheme.Surface;

            cardConn.Controls.AddRange(new Control[]
            { lblId, txtRestaurantId, lblRestName, lblUrl, txtApiUrl, lblHint, btnTest, btnSave, lblStatus });

            // ── كارت الصور ──────────────────────────────────────────────────────
            PanelControl cardImages = MakeCard(640, 40, 500, 340, "🖼️  صور المطعم");

            // صورة الشعار
            LabelControl lblLogoLbl = MakeLabel("شعار المطعم (Logo):", 20, 80);

            picRestaurant = new PictureEdit();
            picRestaurant.Location = new Point(20, 106);
            picRestaurant.Size = new Size(140, 140);
            picRestaurant.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

            LabelControl lblLogoUrl = MakeLabel("رابط الصورة:", 175, 106);
            txtImageUrl = MakeTextEdit(175, 128, 290, 36);
            txtImageUrl.Properties.NullText = "https://... أو مسار نسبي";

            SimpleButton btnPreviewLogo = MakeBtn("👁️ معاينة", AppTheme.Info, 175, 174, 100, 32);
            btnPreviewLogo.Click += async (s, e) => await PreviewImageAsync(txtImageUrl.Text, picRestaurant);

            // صورة الغلاف
            LabelControl lblCoverLbl = MakeLabel("صورة الغلاف (Cover):", 20, 262);

            picCover = new PictureEdit();
            picCover.Location = new Point(20, 288);
            picCover.Size = new Size(440, 100); // عرضها أكبر
            picCover.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

            // حجم cardImages محتاج يكون أكبر
            cardImages.Size = new Size(500, 420);

            LabelControl lblCoverUrl = MakeLabel("رابط الغلاف:", 20, 396);
            txtCoverUrl = MakeTextEdit(20, 422, 360, 36);
            txtCoverUrl.Properties.NullText = "https://... أو مسار نسبي";

            SimpleButton btnPreviewCover = MakeBtn("👁️ معاينة", AppTheme.Info, 390, 422, 90, 32);
            btnPreviewCover.Click += async (s, e) => await PreviewImageAsync(txtCoverUrl.Text, picCover);

            lblImageStatus = new LabelControl();
            lblImageStatus.Text = "";
            lblImageStatus.Font = new Font("Tahoma", 8);
            lblImageStatus.ForeColor = AppTheme.TextSecondary;
            lblImageStatus.Location = new Point(20, 462);
            lblImageStatus.AutoSize = true;
            lblImageStatus.BackColor = AppTheme.Surface;

            cardImages.Size = new Size(500, 490);

            cardImages.Controls.AddRange(new Control[]
            {
                lblLogoLbl, picRestaurant, lblLogoUrl, txtImageUrl, btnPreviewLogo,
                lblCoverLbl, picCover, lblCoverUrl, txtCoverUrl, btnPreviewCover, lblImageStatus
            });

            // ── كارت بيانات المطعم ──────────────────────────────────────────────
            PanelControl cardInfo = MakeCard(40, 400, 1100, 430, "🏪  بيانات المطعم");

            LabelControl lblName = MakeLabel("اسم المطعم:", 20, 80);
            txtName = MakeTextEdit(20, 106, 400, 36);

            LabelControl lblPhone = MakeLabel("رقم الهاتف:", 440, 80);
            txtPhone = MakeTextEdit(440, 106, 200, 36);

            chkIsOpen = new CheckEdit();
            chkIsOpen.Text = "المطعم مفتوح الآن ✅";
            chkIsOpen.Location = new Point(660, 106);
            chkIsOpen.Size = new Size(200, 36);
            chkIsOpen.Font = new Font("Tahoma", 10, FontStyle.Bold);
            chkIsOpen.BackColor = AppTheme.Surface;

            LabelControl lblAddr = MakeLabel("العنوان:", 20, 158);
            txtAddress = MakeTextEdit(20, 184, 840, 36);

            LabelControl lblDesc = MakeLabel("وصف المطعم:", 20, 236);
            txtDescription = new MemoEdit();
            txtDescription.Location = new Point(20, 262);
            txtDescription.Size = new Size(840, 70);
            txtDescription.Font = new Font("Tahoma", 10);

            LabelControl lblFee = MakeLabel("رسوم التوصيل (EGP):", 20, 352);
            txtDeliveryFee = MakeTextEdit(20, 378, 160, 36);

            LabelControl lblMin = MakeLabel("أقل طلب (EGP):", 200, 352);
            txtMinOrder = MakeTextEdit(200, 378, 160, 36);

            LabelControl lblEst = MakeLabel("وقت التحضير (دقيقة):", 380, 352);
            txtEstimatedTime = MakeTextEdit(380, 378, 130, 36);

            btnLoadInfo = MakeBtn("🔄 تحميل البيانات", AppTheme.Info, 20, 340, 160, 40);
            btnLoadInfo.Location = new Point(540, 375);
            btnSaveInfo = MakeBtn("💾 حفظ التعديلات", AppTheme.Primary, 720, 375, 160, 40, true);
            btnLoadInfo.Click += BtnLoadInfo_Click;
            btnSaveInfo.Click += BtnSaveInfo_Click;

            lblInfoStatus = new LabelControl();
            lblInfoStatus.Text = "";
            lblInfoStatus.Font = new Font("Tahoma", 9);
            lblInfoStatus.Location = new Point(20, 425);
            lblInfoStatus.AutoSize = true;
            lblInfoStatus.BackColor = AppTheme.Surface;

            cardInfo.Controls.AddRange(new Control[]
            {
                lblName, txtName, lblPhone, txtPhone, chkIsOpen,
                lblAddr, txtAddress,
                lblDesc, txtDescription,
                lblFee, txtDeliveryFee, lblMin, txtMinOrder, lblEst, txtEstimatedTime,
                btnLoadInfo, btnSaveInfo, lblInfoStatus
            });

            scrollWrapper.Controls.Add(cardInfo);
            scrollWrapper.Controls.Add(cardImages);
            scrollWrapper.Controls.Add(cardConn);
            Controls.Add(scrollWrapper);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private PanelControl MakeCard(int x, int y, int w, int h, string title)
        {
            PanelControl card = new PanelControl();
            card.Size = new Size(w, h);
            card.Location = new Point(x, y);
            card.BackColor = AppTheme.Surface;
            card.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;

            LabelControl lbl = new LabelControl();
            lbl.Text = title;
            lbl.Font = new Font("Tahoma", 12, FontStyle.Bold);
            lbl.ForeColor = AppTheme.Primary;
            lbl.Location = new Point(20, 20);
            lbl.AutoSize = true;
            lbl.BackColor = AppTheme.Surface;
            card.Controls.Add(lbl);
            return card;
        }

        private LabelControl MakeLabel(string text, int x, int y)
        {
            LabelControl lbl = new LabelControl();
            lbl.Text = text;
            lbl.Font = new Font("Tahoma", 9);
            lbl.Location = new Point(x, y);
            lbl.AutoSize = true;
            lbl.BackColor = AppTheme.Surface;
            return lbl;
        }

        private TextEdit MakeTextEdit(int x, int y, int w, int h)
        {
            TextEdit txt = new TextEdit();
            txt.Location = new Point(x, y);
            txt.Size = new Size(w, h);
            txt.Font = new Font("Tahoma", 10);
            return txt;
        }

        private SimpleButton MakeBtn(string text, Color color, int x, int y, int w, int h, bool bold = false)
        {
            SimpleButton btn = new SimpleButton();
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, h);
            btn.Font = new Font("Tahoma", 9, bold ? FontStyle.Bold : FontStyle.Regular);
            btn.Appearance.BackColor = color;
            btn.Appearance.ForeColor = Color.White;
            btn.LookAndFeel.UseDefaultLookAndFeel = false;
            btn.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            return btn;
        }*/
    }
}
