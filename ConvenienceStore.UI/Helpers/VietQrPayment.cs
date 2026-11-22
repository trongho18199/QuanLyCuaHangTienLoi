using System;
using System.Configuration;
using System.Globalization;

namespace ConvenienceStore.UI.Helpers
{
    internal static class VietQrPayment
    {
        public static string Reference(Guid operationId)
        {
            return "TT" + operationId.ToString("N").Substring(0, 12).ToUpperInvariant();
        }

        public static string CreateImageUrl(decimal amount, string reference)
        {
            if (amount <= 0m || amount != decimal.Truncate(amount))
                throw new InvalidOperationException("Số tiền chuyển khoản phải là số nguyên VND lớn hơn 0.");

            string bankId = Required("VietQrBankId");
            string accountNo = Required("VietQrAccountNo");
            string template = ConfigurationManager.AppSettings["VietQrTemplate"] ?? "qr_only";
            return "https://img.vietqr.io/image/" + Uri.EscapeDataString(bankId) + "-" +
                Uri.EscapeDataString(accountNo) + "-" + Uri.EscapeDataString(template) + ".svg?amount=" +
                amount.ToString("0", CultureInfo.InvariantCulture) + "&addInfo=" + Uri.EscapeDataString(reference);
        }

        public static string AccountDisplay
        {
            get { return "Vietcombank - " + Required("VietQrAccountNo"); }
        }

        private static string Required(string key)
        {
            string value = (ConfigurationManager.AppSettings[key] ?? "").Trim();
            if (value.Length == 0) throw new InvalidOperationException("Thiếu cấu hình " + key + " trong App.config.");
            return value;
        }
    }
}
