using iBank.Operator.Desktop.ViewModels;

using OfficeOpenXml;

using Ookii.Dialogs.Wpf;

using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace iBank.Operator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for Bank.xaml
    /// </summary>
    public partial class DactyloscopyView : UserControl
    {
        public DactyloscopyViewModel ViewModel => DataContext as DactyloscopyViewModel ?? throw new Exception("Шо блэт");

        public DactyloscopyView()
        {
            DataContext = new CardsViewModel();
            InitializeComponent();
        }

        private void Button_Today_DB_Click(object sender, RoutedEventArgs e)
        {
            using (var p = new ExcelPackage())
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetDactyloscopyReport"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                var ws = p.Workbook.Worksheets.Add("BANK");
                int count = sqlReader.FieldCount;
                for (var i = 1; sqlReader.Read(); i++)
                    for (int j = 1; j <= count; j++)
                    {
                        ws.Cells[i, j].Value = sqlReader.GetValue(j - 1);
                        ws.Cells[i, j].Style.Font.SetFromFont(new Font("Times New Roman", 12));
                    }

                var dlg = new VistaSaveFileDialog()
                {
                    Filter = "Open XML Spreadsheet |*.xlsx",
                    FileName = "Отчет для дактилоскопии",
                    DefaultExt = ".xlsx",
                    OverwritePrompt = true,
                    CreatePrompt = false,
                };
                if (dlg.ShowDialog() == true)
                {
                    using (var fs = File.Create(dlg.FileName))
                        p.SaveAs(fs);
                }
            }
        }
    }
}