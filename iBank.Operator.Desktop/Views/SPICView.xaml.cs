using iBank.Core.Extensions;
using iBank.Operator.Desktop.Data;
using iBank.Operator.Desktop.ViewModels;

using OfficeOpenXml;

using Ookii.Dialogs.Wpf;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace iBank.Operator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for SPIC.xaml
    /// </summary>
    public partial class SPICView : UserControl
    {
        public SPICViewModel ViewModel => DataContext as SPICViewModel ?? throw new Exception("Шо блэт");

        public SPICView()
        {
            InitializeComponent();
            DataContext = new SPICViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new VistaOpenFileDialog()
            {
                Filter = "Excel |*.xlsx;*.xlsm;*.xlsb;*.xltx;*.xltm;*.xls;*.xlt;*.xls;*.xml;*.xml;*.xlam;*.xla;*.xlw;*.xlr",
                Multiselect = true,
                Title = "Выбор Форумного отчета",
                ShowReadOnly = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
            };
            if (openFileDialog.ShowDialog() == false) return;

            foreach (var fileName in openFileDialog.FileNames)
            {
                var list = new List<Formirovanie>();
                using (var p = new ExcelPackage(new FileInfo(fileName)))
                {
                    foreach (var sheet in p.Workbook.Worksheets)
                    {
                        var i = 2;
                        while (sheet.Cells[$"A{i}"].Value != null && sheet.Cells[$"B{i}"].Value != null)
                        {
                            list.Add(new Formirovanie
                            {
                                ID = sheet.Cells[$"A{i}"].Value?.ToString() ?? "",
                                LastName = sheet.Cells[$"B{i}"].Value?.ToString() ?? "",
                                FirstName = sheet.Cells[$"C{i}"].Value?.ToString() ?? "",
                                Patronymic = sheet.Cells[$"D{i}"].Value?.ToString() ?? "",
                                BirthDate = sheet.Cells[$"E{i}"].Value?.ToString() ?? "",
                                Address = sheet.Cells[$"F{i}"].Value?.ToString() ?? "",
                                DogTag = sheet.Cells[$"G{i}"].Value?.ToString() ?? "",
                                Branch = sheet.Cells[$"H{i}"].Value?.ToString() ?? "",
                                Unit = sheet.Cells[$"I{i}"].Value?.ToString() ?? "",
                                Station = sheet.Cells[$"J{i}"].Value?.ToString() ?? "",
                                DateDispatch = sheet.Cells[$"K{i}"].Value?.ToString() ?? "",
                                District = sheet.Cells[$"L{i}"].Value?.ToString() ?? "",
                                MilitaryIDSerial = sheet.Cells[$"M{i}"].Value?.ToString() ?? "",
                                MilitaryIDNumber = sheet.Cells[$"N{i}"].Value?.ToString() ?? "",
                                Team = sheet.Cells[$"O{i}"].Value?.ToString() ?? ""
                            });

                            i++;
                        }

                    }
                }

                var distinctList = list.DistinctBy(p => new { p.LastName, p.FirstName, p.Patronymic, p.BirthDate }).ToList();
                for (var i = 1; i <= 12; i++)
                {
                    var people = distinctList.Where(p => int.TryParse(p.DateDispatch, out var OADate) ? DateTime.FromOADate(OADate).Month == i : DateTime.Parse(p.DateDispatch).Month == i).ToList();
                    ViewModel.Months.Add(new Entry()
                    {
                        People = people,
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        Count = people.Count.ToString(),
                        MonthInt = i,
                    });
                }
            }
        }

        private void MenuItem_SPICDays_OnClick(object sender, RoutedEventArgs e)
        {
            var month = ViewModel.SelectedMonth;
            var days = HH.SelectedDates;

            var path = $"{Directory.GetCurrentDirectory()}\\Documents\\ЕРЦ.xlsx";
            using (var p = new ExcelPackage(new FileInfo(path)))
            {
                var list = new List<FullFormirovanie>();
                foreach (var person in ViewModel.SelectedMonth.People.Where(pp => days.Contains(DateTime.Parse(pp.DateDispatch))))
                {
                    var sql = $@"
	SELECT person.*, person_card.* 
	FROM person
	LEFT JOIN person_card ON person.passport_serial = person_card.passport_serial
	WHERE person.last_name = '{person.LastName}'
	AND person.first_name = '{person.FirstName}'
	AND person.patronymic = '{person.Patronymic}'
	AND person.birth_date = '{person.BirthDate.Substring(0, 10)}'
    AND person_card.account_number IS NOT NULL";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                    using (var sqlReader = sqlcmd.ExecuteReader())
                    {
                        if (sqlReader.Read())
                        {
                            list.Add(new FullFormirovanie()
                            {
                                LastName = sqlReader.GetString(2),
                                FirstName = sqlReader.GetString(3),
                                Patronymic = sqlReader.GetString(4),
                                BirthDate = sqlReader.GetDateTime(5).ToShortDateString(),
                                BirthPlace = sqlReader.GetString(6),
                                DocumentType = "2",
                                PassportSerial = sqlReader.GetString(1).Substring(0, 5).Remove(2, 1),
                                PassportNumber = sqlReader.GetString(1).Substring(6, 6),
                                PassportIssueDate = sqlReader.GetDateTime(8).ToShortDateString(),
                                PassportIssue = sqlReader.GetString(7),
                                AccountNumber = sqlReader.GetString(18),
                                BankBIK = "044525716",
                                DateDispatch = person.DateDispatch.Substring(0, 10),
                                DispatchPlace = "ВК г. Москвы",
                                DogTag = person.DogTag,
                                Rank = "Рядовой",
                                Unit = person.Unit,
                                DispatchPlaceTo = $"{person.District}, {person.Branch}, {person.Station}",
                                Address = sqlReader.GetString(10),

                                Branch = person.Branch,
                                District = person.District,
                                Station = person.Station,
                            });
                        }
                        else
                            MessageBox.Show($"Не найден призывник\n{person.LastName}\n{person.FirstName}\n{person.Patronymic}\n{person.BirthDate.Substring(0, 10)}");
                    }
                }

                var distinctList = list.DistinctBy(pp => new { pp.LastName, pp.FirstName, pp.Patronymic, pp.BirthDate }).ToList();

                var orderedList = distinctList
                    .OrderBy(pp => pp.DateDispatch)
                    .ThenBy(pp => pp.Unit)
                    .ThenBy(pp => pp.LastName)
                    .ThenBy(pp => pp.FirstName)
                    .ThenBy(pp => pp.Patronymic)
                    .ThenBy(pp => pp.BirthDate)
                    .ToList();

                //Get the Worksheet created in the previous codesample. 
                var ws = p.Workbook.Worksheets.First();
                var i = 6;
                foreach (var person in orderedList)
                {
                    ws.Cells[$"A{i}"].Value = i - 5; // ID
                    ws.Cells[$"B{i}"].Value = person.LastName; // F
                    ws.Cells[$"C{i}"].Value = person.FirstName; // I
                    ws.Cells[$"D{i}"].Value = person.Patronymic.Equals("") ? " " : person.Patronymic; // O
                    ws.Cells[$"E{i}"].Value = person.BirthDate; // BD
                    ws.Cells[$"F{i}"].Value = person.BirthPlace; // BP
                    ws.Cells[$"G{i}"].Value = person.DocumentType; // Паспорт   
                    ws.Cells[$"H{i}"].Value = person.PassportSerial; // PS1
                    ws.Cells[$"I{i}"].Value = person.PassportNumber; // PS2
                    ws.Cells[$"J{i}"].Value = person.PassportIssueDate; // PID
                    ws.Cells[$"K{i}"].Value = person.PassportIssue; // PI
                    ws.Cells[$"L{i}"].Value = person.AccountNumber; // AN
                    ws.Cells[$"M{i}"].Value = person.BankBIK;
                    ws.Cells[$"N{i}"].Value = person.DateDispatch;
                    ws.Cells[$"O{i}"].Value = person.DispatchPlace;
                    ws.Cells[$"P{i}"].Value = person.DogTag;
                    ws.Cells[$"Q{i}"].Value = person.Rank;
                    ws.Cells[$"R{i}"].Value = person.Unit;
                    ws.Cells[$"U{i}"].Value = person.DispatchPlaceTo;
                    ws.Cells[$"V{i}"].Value = person.Address;
                    ws.Cells[$"W{i}"].Value = person.Address;
                    i++;
                }



                var dlg = new VistaSaveFileDialog()
                {
                    Filter = "Office Open XML |*.xlsx",
                    FileName = "ЕРЦ",
                    DefaultExt = ".xlsx",

                };
                if (dlg.ShowDialog() == true)
                {
                    using (var fs = File.Create(dlg.FileName))
                        p.SaveAs(fs);
                }
            }
        }

        private void MenuItem_SPICMonth_OnClick(object sender, RoutedEventArgs e)
        {
            var month = ViewModel.SelectedMonth;

            var path = $"{Directory.GetCurrentDirectory()}\\Documents\\ЕРЦ.xlsx";
            using (var p = new ExcelPackage(new FileInfo(path)))
            {
                //Get the Worksheet created in the previous codesample. 
                var ws = p.Workbook.Worksheets.First();
                var i = 6;
                foreach(var person in ViewModel.SelectedMonth.People)
                {
                    var sql = $@"
	SELECT person.*, person_card.* 
	FROM person
	LEFT JOIN person_card ON person.passport_serial = person_card.passport_serial
	WHERE person.last_name = '{person.LastName}'
	AND person.first_name = '{person.FirstName}'
	AND person.patronymic = '{person.Patronymic}'
	AND person.birth_date = '{person.BirthDate.Substring(0, 10)}'
    AND person_card.account_number IS NOT NULL";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                    using (var sqlReader = sqlcmd.ExecuteReader())
                    {
                        sqlReader.Read();

                        ws.Cells[$"A{i}"].Value = i - 5; // ID
                        ws.Cells[$"B{i}"].Value = sqlReader.GetString(2); // F
                        ws.Cells[$"C{i}"].Value = sqlReader.GetString(3); // I
                        ws.Cells[$"D{i}"].Value = sqlReader.GetString(4); // O
                        ws.Cells[$"E{i}"].Value = sqlReader.GetDateTime(5).ToShortDateString(); // BD
                        ws.Cells[$"F{i}"].Value = sqlReader.GetString(6); // B{
                        ws.Cells[$"G{i}"].Value = "2"; // Паспорт   
                        ws.Cells[$"H{i}"].Value = sqlReader.GetString(1).Substring(0, 5).Remove(2, 1); // PS1
                        ws.Cells[$"I{i}"].Value = sqlReader.GetString(1).Substring(6, 6); // PS2
                        ws.Cells[$"J{i}"].Value = sqlReader.GetDateTime(8).ToShortDateString(); // PID
                        ws.Cells[$"K{i}"].Value = sqlReader.GetString(7); // PI
                        ws.Cells[$"L{i}"].Value = sqlReader.GetString(18); // AN
                        ws.Cells[$"M{i}"].Value = "044525716";
                        ws.Cells[$"N{i}"].Value = person.DateDispatch.Substring(0, 10);
                        ws.Cells[$"O{i}"].Value = "ВК г. Москвы";
                        ws.Cells[$"P{i}"].Value = person.DogTag;
                        ws.Cells[$"Q{i}"].Value = "Рядовой";
                        ws.Cells[$"R{i}"].Value = person.Unit;
                        ws.Cells[$"U{i}"].Value = $"{person.District}, {person.Branch}, {person.Station}";
                        ws.Cells[$"V{i}"].Value = sqlReader.GetString(10);
                        ws.Cells[$"W{i}"].Value = sqlReader.GetString(10);
                    }

                    i++;
                }



                var dlg = new VistaSaveFileDialog()
                {
                    Filter = "Office Open XML |*.xlsx",
                    FileName = "ЕРЦ",
                    DefaultExt = ".xlsx",

                };
                if (dlg.ShowDialog() == true)
                {
                    using (var fs = File.Create(dlg.FileName))
                        p.SaveAs(fs);
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedDate = new DateTime(DateTime.Now.Year, ViewModel.SelectedMonth.MonthInt, 1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new VistaOpenFileDialog()
            {
                Filter = "Excel |*.xlsx;*.xlsm;*.xlsb;*.xltx;*.xltm;*.xls;*.xlt;*.xls;*.xml;*.xml;*.xlam;*.xla;*.xlw;*.xlr",
                Multiselect = true,
                Title = "Выбор Форумного отчета",
                ShowReadOnly = false,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
            };
            if (openFileDialog.ShowDialog() == false) return;

            ViewModel.ImportDailyReportAsync(openFileDialog.FileNames);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) => ViewModel.Months.Clear();
    }
}
