using iBank.Operator.Desktop.Data;
using iBank.Operator.Desktop.Database;
using iBank.Operator.Desktop.Extensions;
using iBank.Operator.Desktop.ViewModels;

using OfficeOpenXml;

using Ookii.Dialogs.Wpf;

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iBank.Operator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for Bank.xaml
    /// </summary>
    public partial class BankView : UserControl
    {
        public CardsViewModel ViewModel => DataContext as CardsViewModel ?? throw new Exception("Шо блэт");

        public BankView()
        {
            DataContext = new CardsViewModel();
            InitializeComponent();
        }

        private async void Button_WholePeriod_DB_Click(object sender, RoutedEventArgs e) => await ViewModel.CheckTotalReportAsync();
        private async void Button_WholePeriod_File_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new VistaOpenFileDialog()
            {
                Filter = "Excel |*.xlsx;*.xlsm;*.xlsb;*.xltx;*.xltm;*.xls;*.xlt;*.xls;*.xml;*.xml;*.xlam;*.xla;*.xlw;*.xlr",
                Multiselect = false,
                Title = "Выбор Банковского отчета",
                ShowReadOnly = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,

            };
            if (openFileDialog.ShowDialog() == false)
                return;

            await ViewModel.CheckTotalReportAsync(openFileDialog.FileName);
        }

        private async void Button_Today_DB_Click(object sender, RoutedEventArgs e) => await ViewModel.ImportDailyReportAsync();
        private async void Button_Today_File_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new VistaOpenFileDialog()
            {
                Filter = "Excel |*.xlsx;*.xlsm;*.xlsb;*.xltx;*.xltm;*.xls;*.xlt;*.xls;*.xml;*.xml;*.xlam;*.xla;*.xlw;*.xlr",
                Multiselect = false,
                Title = "Выбор Банковского отчета",
                ShowReadOnly = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,

            };
            if (openFileDialog.ShowDialog() == false)
                return;

            await ViewModel.ImportDailyReportAsync(openFileDialog.FileName);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is SearchPersonToAssignCard person)
            {
                var accountNumber = TextPopupWindow.Prompt("Номер карты", "Внимание!");
                if (accountNumber == null || accountNumber.Length != 16 || !long.TryParse(accountNumber, out _))
                    return;

                var sql = $"INSERT INTO person_card (passport_serial, account_number) VALUES ('{person.PassportSerial}', '{accountNumber}')";
                using (var sqlcmd = new SqlCommandExecutor(sql))
                    if (sqlcmd.TryExecuteScalar(out _))
                    {
                        WatermarkTextBox_TextChanged(sender, null);
                        ViewModel.AssigmentHistory.Add(new AssigmentHistory()
                        {
                            PassportSerial = person.PassportSerial,
                            LastName = person.LastName,
                            FirstName = person.FirstName,
                            Patronymic = person.Patronymic,
                            BirthDate = person.BirthDate,
                            AccountNumber = accountNumber.Substring(7, 8)
                        });
                    }
            }
        }


        // Фильтрация ввода ФИО
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!iBank.Core.Utils.FilterText(e.Text[0]))
                e.Handled = true;
        }

        // Фильтрация ввода паспорта
        private void TextBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!iBank.Core.Utils.FilterPassport(e.Text[0]))
                e.Handled = true;
        }


        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs? e)
        {
            ViewModel.Persons.Clear();
            foreach (var person in SearchPersonToAssignCard.GetAll(ViewModel.LastName, ViewModel.FirstName, ViewModel.Patronymic, ViewModel.PassportSerial))
                ViewModel.Persons.Add(person);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                ViewModel.ShowAssigment = !checkBox.IsChecked ?? true;
                ViewModel.ShowHistory = checkBox.IsChecked ?? false;
            }
        }

        private void Button_Report_Click(object sender, RoutedEventArgs e)
        {
            var path = $"{Directory.GetCurrentDirectory()}\\Documents\\Форма.xlsx";
            using (var p = new ExcelPackage(new FileInfo(path)))
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetAddedPersonsFromYesterday"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                var ws = p.Workbook.Worksheets.First();
                int count = sqlReader.FieldCount;
                for (var i = 4; sqlReader.Read(); i++)
                    for (int j = 1; j <= count; j++)
                        ws.Cells[i, j].Value = sqlReader.GetValue(j - 1);
                // ws.Cells[] starts from 1, 1
                // GetValue() starts from 0

                var dlg = new VistaSaveFileDialog()
                {
                    Filter = "Open XML Spreadsheet |*.xlsx",
                    FileName = $"Залитые за {DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy")}",
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var path = $"{Directory.GetCurrentDirectory()}\\Documents\\Персонализация.xlsm";
            using (var p = new ExcelPackage(new FileInfo(path)))
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetReportForVTB"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                var ws = p.Workbook.Worksheets.First();
                int count = sqlReader.FieldCount;
                for (var i = 5; sqlReader.Read(); i++)
                    for (int j = 1; j <= count; j++)
                        ws.Cells[i, j].Value = sqlReader.GetValue(j - 1);
                // ws.Cells[] starts from 1, 1
                // GetValue() starts from 0

                var dlg = new VistaSaveFileDialog()
                {
                    Filter = "Open XML Macro-Enabled Spreadsheet |*.xlsm",
                    FileName = "Персонализация ВТБ",
                    DefaultExt = ".xlsm",
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
