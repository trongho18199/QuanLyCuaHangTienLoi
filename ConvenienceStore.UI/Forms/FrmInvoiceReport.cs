using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using Microsoft.Reporting.WinForms;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmInvoiceReport : DevExpress.XtraEditors.XtraForm
    {
        private readonly InvoiceDetail invoice;

        public FrmInvoiceReport() : this(null) { }

        public FrmInvoiceReport(InvoiceDetail value)
        {
            invoice = value;
            InitializeComponent();
        }

        private void FrmInvoiceReport_Load(object sender, EventArgs e)
        {
            if (DesignMode || invoice == null) return;

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(
                new ReportDataSource("InvoiceLines", invoice.Lines ?? new List<InvoiceLineRow>()));
            reportViewer.LocalReport.SetParameters(new[]
            {
                Param("InvoiceNumber", invoice.Number),
                Param("CreatedAt", invoice.CreatedAt.ToString("dd/MM/yyyy HH:mm")),
                Param("CustomerName", invoice.CustomerName),
                Param("Cashier", invoice.Cashier),
                Param("PaymentMethod", invoice.PaymentMethod),
                Param("PaymentReference", invoice.PaymentMethod == PaymentMethods.Transfer ? invoice.PaymentReference : ""),
                Param("CancelledText", invoice.IsCancelled ? "*** HÓA ĐƠN ĐÃ HỦY ***" : ""),
                Param("Subtotal", Money(invoice.Subtotal)),
                Param("MemberDiscount", Money(invoice.MemberDiscountAmount)),
                Param("Promotion", invoice.PromotionAmount > 0m
                    ? Money(invoice.PromotionAmount) + (string.IsNullOrEmpty(invoice.PromotionCode) ? "" : " (" + invoice.PromotionCode + ")")
                    : Money(0m)),
                Param("Total", Money(invoice.Total)),
                Param("CashReceived", Money(invoice.CashReceived)),
                Param("Change", Money(invoice.Change))
            });
            reportViewer.RefreshReport();
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer.ZoomMode = ZoomMode.PageWidth;
        }

        private static ReportParameter Param(string name, string value)
        {
            return new ReportParameter(name, value ?? "");
        }

        private static string Money(decimal value)
        {
            return value.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("vi-VN")) + " đ";
        }
    }
}
