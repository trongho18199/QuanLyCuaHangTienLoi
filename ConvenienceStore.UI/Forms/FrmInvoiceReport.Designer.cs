namespace ConvenienceStore.UI.Forms
{
    partial class FrmInvoiceReport
    {
        private System.ComponentModel.IContainer components = null;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.LocalReport.ReportEmbeddedResource = "ConvenienceStore.UI.Reports.HoaDonBanHang.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(0, 0);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ServerReport.BearerToken = null;
            this.reportViewer.Size = new System.Drawing.Size(984, 711);
            this.reportViewer.TabIndex = 0;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(984, 711);
            this.Controls.Add(this.reportViewer);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FrmInvoiceReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xem trước hóa đơn bán hàng";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmInvoiceReport_Load);
            this.ResumeLayout(false);
        }
    }
}
