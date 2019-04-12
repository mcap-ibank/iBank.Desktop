using iBank.Core;
using iBank.Desktop.Data;

using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iBank.Desktop.ViewModel
{
    public class SPICViewModel : INotifyPropertyChanged
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Entry> Months { get; } = new ObservableCollection<Entry>();
        public Entry SelectedMonth { get; set; }

        public DateTime? SelectedDate { get; set; }

        public SelectedDatesCollection SelectedDates { get; set; }

        public double MaxProgress { get; set; } = double.MaxValue;
        public double CurrentProgress { get; set; } = 0;

        public Task ImportDailyReportAsync(string[] paths) => Task.Run(() =>
        {
            var list = new List<Formirovanie>();
            foreach (var fileName in paths)
            {
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
            }
            var distinctList = DistinctBy(list, p => new { p.Branch, p.DateDispatch, p.LastName, p.FirstName, p.Patronymic, p.BirthDate }).ToList();
            MaxProgress = distinctList.Count;

            foreach (var person in distinctList)
            {
                var sql = $@"
	SELECT person.*, person_card.* 
	FROM person
	LEFT JOIN person_card ON person.passport_serial = person_card.passport_serial
	WHERE person.last_name = '{person.LastName.Trim()}'
	AND person.first_name = '{person.FirstName.Trim()}'
	AND person.patronymic = '{person.Patronymic.Trim()}'
	AND person.birth_date = '{person.BirthDate.Substring(0, 10)}'
    AND person_card.account_number IS NOT NULL";
                using (var sqlcmd = new SqlCommandExecutor(sql))
                using (var sqlReader = sqlcmd.ExecuteReader())
                {
                    if (!sqlReader.Read())
                    {
                        MessageBox.Show($@"Не найден человек!
{person.LastName} 
{person.FirstName}
{person.Patronymic}
{person.BirthDate}", "Ошибка!", MessageBoxButton.OK);
                        CurrentProgress++;
                        continue;
                    }


                    var f = sqlReader.GetString(2); // F
                    var i = sqlReader.GetString(3); // I
                    var o = sqlReader.GetString(4); // O
                    var bd = sqlReader.GetDateTime(5).ToShortDateString(); // BD

                    if (!person.LastName.Trim().Equals(f.Trim(), StringComparison.OrdinalIgnoreCase))
                        ReportError();
                    if (person.FirstName.Trim() != i.Trim())
                        ReportError();
                    if (!person.Patronymic.Trim().Equals(o.Trim(), StringComparison.OrdinalIgnoreCase))
                        ReportError();
                    if (person.BirthDate.Substring(0, 10) != bd)
                        ReportError();

                    CurrentProgress++;
                    void ReportError()
                    {
                        MessageBox.Show($@"Обнаружено расхождение!
{f} {person.LastName}
{i} {person.FirstName}
{o} {person.Patronymic} 
{bd} {person.BirthDate}", "Ошибка!", MessageBoxButton.OK);
                    }
                }
            }
        });
    }
}
