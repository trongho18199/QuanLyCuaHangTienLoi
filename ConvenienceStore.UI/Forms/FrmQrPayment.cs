using System;
using System.Net.Http;
using ConvenienceStore.UI.Helpers;
using DevExpress.Utils.Svg;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmQrPayment : DevExpress.XtraEditors.XtraForm
    {
        private readonly string qrUrl;

        public FrmQrPayment() : this("", "", 0m) { }

        public FrmQrPayment(string imageUrl, string reference, decimal amount)
        {
            qrUrl = imageUrl;
            InitializeComponent();
            lblAccount.Text = VietQrPayment.AccountDisplay;
            lblAmount.Text = "Số tiền: " + amount.ToString("N0") + " đ";
            txtReference.Text = reference ?? "";
            btnConfirm.Enabled = false;
            chkReceived.CheckedChanged += (s, e) => btnConfirm.Enabled = chkReceived.Checked;
        }

        private async void FrmQrPayment_Load(object sender, EventArgs e)
        {
            if (DesignMode || string.IsNullOrWhiteSpace(qrUrl)) return;
            try
            {
                lblLoadStatus.Text = "Đang tải mã VietQR...";
                using (var client = new HttpClient())
                using (var stream = await client.GetStreamAsync(qrUrl))
                    pictureQr.SvgImage = SvgImage.FromStream(stream);
                lblLoadStatus.Text = "Khách quét mã bằng ứng dụng ngân hàng.";
            }
            catch (Exception)
            {
                lblLoadStatus.Text = "Không tải được VietQR. Hãy kiểm tra kết nối Internet.";
                chkReceived.Enabled = false;
            }
        }
    }
}
