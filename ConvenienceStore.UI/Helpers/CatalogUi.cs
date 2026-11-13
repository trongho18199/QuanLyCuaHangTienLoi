using System;
using System.Drawing;
using System.Windows.Forms;
namespace ConvenienceStore.UI.Helpers
{
    
    
    
    
    internal static class CatalogUi
    {
        
        public static bool IsDesigner(Control form) { return TradeUi.IsDesigner(form); }

        
        public static bool Run(IWin32Window owner, Action action) { return TradeUi.Run(owner, action); }

        
        public static void Grid(DataGridView grid, params string[] specs) { TradeUi.Grid(grid, specs); }

        
        public static void ExportCsv(IWin32Window owner, DataGridView grid, string filename) { TradeUi.ExportCsv(owner, grid, filename); }

        
        public static void LiveSearch(Form owner, TextBox search, Action refresh)
        {
            var timer = new Timer { Interval = 250 };
            search.TextChanged += (s, e) => { timer.Stop(); timer.Start(); };
            timer.Tick += (s, e) => { timer.Stop(); if (!owner.IsDisposed && !IsDesigner(owner)) Run(owner, refresh); };
            owner.Disposed += (s, e) => timer.Dispose();
        }

        
        public static string PromptText(IWin32Window owner, string title, string prompt, int maxLength)
        {
            using (var form = new Form())
            {
                form.Text = title; form.FormBorderStyle = FormBorderStyle.FixedDialog; form.StartPosition = FormStartPosition.CenterParent;
                form.MaximizeBox = false; form.MinimizeBox = false; form.ShowInTaskbar = false;
                form.Font = new Font("Segoe UI", 10F); form.ClientSize = new Size(470, 200);
                var label = new Label { Text = prompt, Location = new Point(15, 14), Size = new Size(440, 44) };
                var box = new TextBox { Location = new Point(15, 62), Size = new Size(440, 70), Multiline = true, MaxLength = maxLength, ScrollBars = ScrollBars.Vertical };
                var ok = new Button { Text = "Đồng ý", DialogResult = DialogResult.OK, Location = new Point(255, 148), Size = new Size(95, 34) };
                var cancel = new Button { Text = "Bỏ qua", DialogResult = DialogResult.Cancel, Location = new Point(360, 148), Size = new Size(95, 34) };
                form.Controls.AddRange(new Control[] { label, box, ok, cancel });
                form.AcceptButton = ok; form.CancelButton = cancel;
                return form.ShowDialog(owner) == DialogResult.OK ? box.Text.Trim() : null;
            }
        }
    }
}
