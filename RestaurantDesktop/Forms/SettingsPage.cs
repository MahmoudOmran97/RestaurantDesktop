using DevExpress.XtraEditors;
using RestaurantDesktop.Models;
using RestaurantDesktop.Services;
using System;
using System.Configuration;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantDesktop.Forms
{
    public partial class SettingsPage : XtraUserControl
    {
        // كل العناصر اللي محتاجها
        private GroupControl grpConnection, grpImages, grpInfo;
        private TextEdit txtRestaurantId, txtApiUrl, txtName, txtPhone, txtAddress, txtDeliveryFee, txtMinOrder, txtEstimatedTime, txtImageUrl, txtCoverUrl;
        private MemoEdit txtDescription;
        private CheckEdit chkIsOpen;
        private SimpleButton btnTest, btnSave, btnLoadInfo, btnSaveInfo, btnPreviewLogo, btnPreviewCover;
        private PictureEdit picRestaurant, picCover;
        private LabelControl lblStatus, lblRestName, lblInfoStatus, lblImageStatus;

        public SettingsPage()
        {
            // ضبط لون الخلفية
            this.BackColor = AppTheme.Background;
            this.Dock = DockStyle.Fill;

            // بناء التصميم
            BuildDesign();

            // تحميل القيم
            LoadCurrentValues();
        }

        private void BuildDesign()
        {
            // حاوية قابلة للتمرير
            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = AppTheme.Background
            };

            // حاوية عمودية
            Panel verticalContainer = new Panel
            {
                Width = 900,
                Height = 1200,
                BackColor = AppTheme.Background
            };

            int currentY = 20;

            // 1. كارت إعدادات الاتصال
            CreateConnectionCard(verticalContainer, ref currentY);
            currentY += 20; // مسافة بين الكروت

            // 2. كارت الصور
            CreateImagesCard(verticalContainer, ref currentY);
            currentY += 20;

            // 3. كارت بيانات المطعم
            CreateInfoCard(verticalContainer, ref currentY);

            scrollPanel.Controls.Add(verticalContainer);
            this.Controls.Add(scrollPanel);
        }

        // ================================================
        // كارت إعدادات الاتصال
        // ================================================
        private void CreateConnectionCard(Panel parent, ref int y)
        {
            GroupControl grp = new GroupControl();
            grp.Text = "🔌 إعدادات الاتصال";
            grp.Size = new Size(860, 180);
            grp.Location = new Point(20, y);
            grp.Font = new Font("Tahoma", 10, FontStyle.Bold);
            grp.BackColor = AppTheme.Surface;

            // معرف المطعم
            LabelControl lblId = new LabelControl();
            lblId.Text = "معرف المطعم (RestaurantId):";
            lblId.Location = new Point(20, 40);
            lblId.Size = new Size(150, 20);
            lblId.BackColor = AppTheme.Surface;

            txtRestaurantId = new TextEdit();
            txtRestaurantId.Location = new Point(180, 35);
            txtRestaurantId.Size = new Size(150, 30);

            lblRestName = new LabelControl();
            lblRestName.Location = new Point(340, 40);
            lblRestName.Size = new Size(200, 20);
            lblRestName.ForeColor = AppTheme.Success;

            // رابط API
            LabelControl lblUrl = new LabelControl();
            lblUrl.Text = "رابط الـ API:";
            lblUrl.Location = new Point(20, 85);
            lblUrl.Size = new Size(150, 20);
            lblUrl.BackColor = AppTheme.Surface;

            txtApiUrl = new TextEdit();
            txtApiUrl.Location = new Point(180, 80);
            txtApiUrl.Size = new Size(500, 30);

            // الأزرار
            btnTest = new SimpleButton();
            btnTest.Text = "🔍 اختبار الاتصال";
            btnTest.Location = new Point(20, 125);
            btnTest.Size = new Size(150, 35);
            btnTest.Appearance.BackColor = AppTheme.Info;
            btnTest.Appearance.ForeColor = Color.White;
            btnTest.Click += BtnTest_Click;

            btnSave = new SimpleButton();
            btnSave.Text = "💾 حفظ الإعدادات";
            btnSave.Location = new Point(180, 125);
            btnSave.Size = new Size(150, 35);
            btnSave.Appearance.BackColor = AppTheme.Success;
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;

            lblStatus = new LabelControl();
            lblStatus.Location = new Point(350, 132);
            lblStatus.Size = new Size(300, 20);

            grp.Controls.AddRange(new Control[] { lblId, txtRestaurantId, lblRestName, lblUrl, txtApiUrl, btnTest, btnSave, lblStatus });
            parent.Controls.Add(grp);
            y = grp.Bottom;
        }

        // ================================================
        // كارت الصور
        // ================================================
        private void CreateImagesCard(Panel parent, ref int y)
        {
            GroupControl grp = new GroupControl();
            grp.Text = "🖼️ صور المطعم";
            grp.Size = new Size(860, 320);
            grp.Location = new Point(20, y);
            grp.Font = new Font("Tahoma", 10, FontStyle.Bold);
            grp.BackColor = AppTheme.Surface;

            // صورة الشعار
            LabelControl lblLogo = new LabelControl();
            lblLogo.Text = "شعار المطعم:";
            lblLogo.Location = new Point(20, 35);
            lblLogo.Size = new Size(100, 20);
            lblLogo.BackColor = AppTheme.Surface;

            picRestaurant = new PictureEdit();
            picRestaurant.Location = new Point(20, 60);
            picRestaurant.Size = new Size(120, 120);
            picRestaurant.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

            LabelControl lblLogoUrl = new LabelControl();
            lblLogoUrl.Text = "رابط الشعار:";
            lblLogoUrl.Location = new Point(160, 40);
            lblLogoUrl.Size = new Size(100, 20);
            lblLogoUrl.BackColor = AppTheme.Surface;

            txtImageUrl = new TextEdit();
            txtImageUrl.Location = new Point(160, 65);
            txtImageUrl.Size = new Size(400, 30);
            txtImageUrl.Properties.NullText = "https://example.com/image.png";

            btnPreviewLogo = new SimpleButton();
            btnPreviewLogo.Text = "👁️ معاينة";
            btnPreviewLogo.Location = new Point(570, 62);
            btnPreviewLogo.Size = new Size(100, 35);
            btnPreviewLogo.Appearance.BackColor = AppTheme.Info;
            btnPreviewLogo.Appearance.ForeColor = Color.White;
            btnPreviewLogo.Click += async (s, e) => await PreviewImageAsync(txtImageUrl.Text, picRestaurant);

            // صورة الغلاف
            LabelControl lblCover = new LabelControl();
            lblCover.Text = "صورة الغلاف:";
            lblCover.Location = new Point(20, 195);
            lblCover.Size = new Size(100, 20);
            lblCover.BackColor = AppTheme.Surface;

            picCover = new PictureEdit();
            picCover.Location = new Point(20, 220);
            picCover.Size = new Size(670, 80);
            picCover.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

            LabelControl lblCoverUrl = new LabelControl();
            lblCoverUrl.Text = "رابط الغلاف:";
            lblCoverUrl.Location = new Point(20, 310);
            lblCoverUrl.Size = new Size(100, 20);
            lblCoverUrl.BackColor = AppTheme.Surface;

            txtCoverUrl = new TextEdit();
            txtCoverUrl.Location = new Point(130, 305);
            txtCoverUrl.Size = new Size(430, 30);
            txtCoverUrl.Properties.NullText = "https://example.com/cover.png";

            btnPreviewCover = new SimpleButton();
            btnPreviewCover.Text = "👁️ معاينة";
            btnPreviewCover.Location = new Point(570, 302);
            btnPreviewCover.Size = new Size(100, 35);
            btnPreviewCover.Appearance.BackColor = AppTheme.Info;
            btnPreviewCover.Appearance.ForeColor = Color.White;
            btnPreviewCover.Click += async (s, e) => await PreviewImageAsync(txtCoverUrl.Text, picCover);

            lblImageStatus = new LabelControl();
            lblImageStatus.Location = new Point(20, 345);
            lblImageStatus.Size = new Size(500, 20);
            lblImageStatus.ForeColor = AppTheme.TextSecondary;

            grp.Controls.AddRange(new Control[] {
                lblLogo, picRestaurant, lblLogoUrl, txtImageUrl, btnPreviewLogo,
                lblCover, picCover, lblCoverUrl, txtCoverUrl, btnPreviewCover, lblImageStatus
            });
            parent.Controls.Add(grp);
            y = grp.Bottom;
        }

        // ================================================
        // كارت بيانات المطعم
        // ================================================
        private void CreateInfoCard(Panel parent, ref int y)
        {
            GroupControl grp = new GroupControl();
            grp.Text = "🏪 بيانات المطعم";
            grp.Size = new Size(860, 500);
            grp.Location = new Point(20, y);
            grp.Font = new Font("Tahoma", 10, FontStyle.Bold);
            grp.BackColor = AppTheme.Surface;

            // صف 1: اسم المطعم ورقم الهاتف
            LabelControl lblName = new LabelControl();
            lblName.Text = "اسم المطعم:";
            lblName.Location = new Point(20, 35);
            lblName.Size = new Size(100, 20);
            lblName.BackColor = AppTheme.Surface;

            txtName = new TextEdit();
            txtName.Location = new Point(130, 30);
            txtName.Size = new Size(250, 30);

            LabelControl lblPhone = new LabelControl();
            lblPhone.Text = "رقم الهاتف:";
            lblPhone.Location = new Point(420, 35);
            lblPhone.Size = new Size(100, 20);
            lblPhone.BackColor = AppTheme.Surface;

            txtPhone = new TextEdit();
            txtPhone.Location = new Point(530, 30);
            txtPhone.Size = new Size(200, 30);

            // صف 2: حالة المطعم
            LabelControl lblOpen = new LabelControl();
            lblOpen.Text = "حالة المطعم:";
            lblOpen.Location = new Point(20, 80);
            lblOpen.Size = new Size(100, 20);
            lblOpen.BackColor = AppTheme.Surface;

            chkIsOpen = new CheckEdit();
            chkIsOpen.Text = "المطعم مفتوح الآن";
            chkIsOpen.Location = new Point(130, 75);
            chkIsOpen.Size = new Size(150, 25);

            // صف 3: العنوان
            LabelControl lblAddress = new LabelControl();
            lblAddress.Text = "العنوان:";
            lblAddress.Location = new Point(20, 120);
            lblAddress.Size = new Size(100, 20);
            lblAddress.BackColor = AppTheme.Surface;

            txtAddress = new TextEdit();
            txtAddress.Location = new Point(130, 115);
            txtAddress.Size = new Size(600, 30);

            // صف 4: الوصف
            LabelControl lblDesc = new LabelControl();
            lblDesc.Text = "الوصف:";
            lblDesc.Location = new Point(20, 160);
            lblDesc.Size = new Size(100, 20);
            lblDesc.BackColor = AppTheme.Surface;

            txtDescription = new MemoEdit();
            txtDescription.Location = new Point(130, 155);
            txtDescription.Size = new Size(600, 70);

            // صف 5: رسوم التوصيل وأقل طلب
            LabelControl lblFee = new LabelControl();
            lblFee.Text = "رسوم التوصيل (EGP):";
            lblFee.Location = new Point(20, 245);
            lblFee.Size = new Size(120, 20);
            lblFee.BackColor = AppTheme.Surface;

            txtDeliveryFee = new TextEdit();
            txtDeliveryFee.Location = new Point(150, 240);
            txtDeliveryFee.Size = new Size(120, 30);

            LabelControl lblMin = new LabelControl();
            lblMin.Text = "أقل طلب (EGP):";
            lblMin.Location = new Point(300, 245);
            lblMin.Size = new Size(100, 20);
            lblMin.BackColor = AppTheme.Surface;

            txtMinOrder = new TextEdit();
            txtMinOrder.Location = new Point(410, 240);
            txtMinOrder.Size = new Size(120, 30);

            // صف 6: وقت التحضير
            LabelControl lblTime = new LabelControl();
            lblTime.Text = "وقت التحضير (دقيقة):";
            lblTime.Location = new Point(20, 290);
            lblTime.Size = new Size(130, 20);
            lblTime.BackColor = AppTheme.Surface;

            txtEstimatedTime = new TextEdit();
            txtEstimatedTime.Location = new Point(160, 285);
            txtEstimatedTime.Size = new Size(100, 30);

            // أزرار التحكم
            btnLoadInfo = new SimpleButton();
            btnLoadInfo.Text = "🔄 تحميل البيانات";
            btnLoadInfo.Location = new Point(20, 340);
            btnLoadInfo.Size = new Size(150, 40);
            btnLoadInfo.Appearance.BackColor = AppTheme.Info;
            btnLoadInfo.Appearance.ForeColor = Color.White;
            btnLoadInfo.Click += BtnLoadInfo_Click;

            btnSaveInfo = new SimpleButton();
            btnSaveInfo.Text = "💾 حفظ التعديلات";
            btnSaveInfo.Location = new Point(190, 340);
            btnSaveInfo.Size = new Size(150, 40);
            btnSaveInfo.Appearance.BackColor = AppTheme.Primary;
            btnSaveInfo.Appearance.ForeColor = Color.White;
            btnSaveInfo.Click += BtnSaveInfo_Click;

            lblInfoStatus = new LabelControl();
            lblInfoStatus.Location = new Point(20, 400);
            lblInfoStatus.Size = new Size(600, 20);

            grp.Controls.AddRange(new Control[] {
                lblName, txtName, lblPhone, txtPhone,
                lblOpen, chkIsOpen,
                lblAddress, txtAddress,
                lblDesc, txtDescription,
                lblFee, txtDeliveryFee, lblMin, txtMinOrder,
                lblTime, txtEstimatedTime,
                btnLoadInfo, btnSaveInfo, lblInfoStatus
            });
            parent.Controls.Add(grp);
            y = grp.Bottom;
        }

        // ================================================
        // الدوال الأساسية
        // ================================================
        private void LoadCurrentValues()
        {
            txtRestaurantId.Text = AppSession.RestaurantId.ToString();
            txtApiUrl.Text = AppConfig.BaseUrl;
        }

        private async Task PreviewImageAsync(string url, PictureEdit pic)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                lblImageStatus.Text = "⚠️ أدخل رابط الصورة أولاً";
                return;
            }

            try
            {
                lblImageStatus.Text = "⏳ جاري التحميل...";
                string fullUrl = url.StartsWith("http") ? url : AppConfig.BaseUrl.TrimEnd('/') + "/" + url.TrimStart('/');

                using (var client = new HttpClient())
                {
                    var data = await client.GetByteArrayAsync(fullUrl);
                    using (var ms = new System.IO.MemoryStream(data))
                        pic.Image = Image.FromStream(ms);
                }
                lblImageStatus.Text = "✅ تم تحميل الصورة";
                lblImageStatus.ForeColor = AppTheme.Success;
            }
            catch
            {
                lblImageStatus.Text = "❌ فشل تحميل الصورة";
                lblImageStatus.ForeColor = AppTheme.Danger;
            }
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;
            lblStatus.Text = "⏳ جاري الاختبار...";

            try
            {
                if (!int.TryParse(txtRestaurantId.Text, out int rid) || rid <= 0)
                {
                    lblStatus.Text = "❌ معرف المطعم غير صحيح";
                    return;
                }

                var r = await ApiService.GetRestaurantAsync(rid);
                if (r != null)
                {
                    lblStatus.Text = "✅ الاتصال ناجح";
                    lblStatus.ForeColor = AppTheme.Success;
                    lblRestName.Text = "→ " + r.Name;
                }
                else
                {
                    lblStatus.Text = "❌ المطعم غير موجود";
                    lblStatus.ForeColor = AppTheme.Danger;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ خطأ: {ex.Message}";
                lblStatus.ForeColor = AppTheme.Danger;
            }
            finally
            {
                btnTest.Enabled = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtRestaurantId.Text, out int rid))
            {
                XtraMessageBox.Show("معرف المطعم غير صحيح");
                return;
            }

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["RestaurantId"].Value = rid.ToString();
                config.AppSettings.Settings["ApiBaseUrl"].Value = txtApiUrl.Text.TrimEnd('/');
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                AppSession.RestaurantId = rid;
                AppConfig.BaseUrl = txtApiUrl.Text.TrimEnd('/');

                lblStatus.Text = "✅ تم حفظ الإعدادات";
                lblStatus.ForeColor = AppTheme.Success;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ فشل الحفظ: {ex.Message}";
                lblStatus.ForeColor = AppTheme.Danger;
            }
        }

        private async void BtnLoadInfo_Click(object sender, EventArgs e)
        {
            btnLoadInfo.Enabled = false;
            lblInfoStatus.Text = "⏳ جاري التحميل...";

            try
            {
                var r = await ApiService.GetRestaurantAsync(AppSession.RestaurantId);
                if (r == null)
                {
                    lblInfoStatus.Text = "❌ فشل تحميل البيانات";
                    return;
                }

                txtName.Text = r.Name ?? "";
                txtPhone.Text = r.Phone ?? "";
                txtAddress.Text = r.Address ?? "";
                txtDescription.Text = r.Description ?? "";
                txtDeliveryFee.Text = r.DeliveryFee.ToString();
                txtMinOrder.Text = r.MinOrderAmount.ToString();
                txtEstimatedTime.Text = r.EstimatedTime.ToString();
                chkIsOpen.Checked = r.IsOpen;
                txtImageUrl.Text = r.ImageUrl ?? "";
                txtCoverUrl.Text = r.CoverImageUrl ?? "";

                if (!string.IsNullOrEmpty(r.ImageUrl))
                    await PreviewImageAsync(r.ImageUrl, picRestaurant);
                if (!string.IsNullOrEmpty(r.CoverImageUrl))
                    await PreviewImageAsync(r.CoverImageUrl, picCover);

                lblInfoStatus.Text = "✅ تم تحميل البيانات";
                lblInfoStatus.ForeColor = AppTheme.Success;
            }
            catch (Exception ex)
            {
                lblInfoStatus.Text = $"❌ خطأ: {ex.Message}";
            }
            finally
            {
                btnLoadInfo.Enabled = true;
            }
        }

        private async void BtnSaveInfo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                XtraMessageBox.Show("اسم المطعم مطلوب");
                return;
            }

            btnSaveInfo.Enabled = false;
            lblInfoStatus.Text = "⏳ جاري الحفظ...";

            try
            {
                var dto = new UpdateRestaurantDto
                {
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    DeliveryFee = decimal.TryParse(txtDeliveryFee.Text, out decimal fee) ? fee : 0,
                    MinOrderAmount = decimal.TryParse(txtMinOrder.Text, out decimal min) ? min : 0,
                    EstimatedTime = int.TryParse(txtEstimatedTime.Text, out int time) ? time : 30,
                    IsOpen = chkIsOpen.Checked,
                    ImageUrl = txtImageUrl.Text.Trim(),
                    CoverImageUrl = txtCoverUrl.Text.Trim()
                };

                var result = await ApiService.UpdateRestaurantAsync(AppSession.RestaurantId, dto);
                lblInfoStatus.Text = result.Ok ? "✅ تم حفظ البيانات" : $"❌ {result.Error}";
                lblInfoStatus.ForeColor = result.Ok ? AppTheme.Success : AppTheme.Danger;
            }
            catch (Exception ex)
            {
                lblInfoStatus.Text = $"❌ خطأ: {ex.Message}";
            }
            finally
            {
                btnSaveInfo.Enabled = true;
            }
        }
    }
}