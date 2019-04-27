using iBank.Operator.Desktop.Database;
using iBank.Operator.Desktop.Extensions;
using iBank.Operator.Desktop.Utils;

using iBank.Operator.Desktop.Data;
using iBank.Operator.Desktop.ViewModels;

using Ookii.Dialogs.Wpf;

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace iBank.Operator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for ImportPersons.xaml
    /// </summary>
    public partial class ImportView : UserControl
    {
        public ImportViewModel ViewModel => DataContext as ImportViewModel ?? throw new Exception("Шо блэт");

        public ImportView()
        {
            InitializeComponent();
            DataContext = new ImportViewModel();
        }

        private void Button_SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new VistaFolderBrowserDialog();
            if (openFileDialog.ShowDialog() == false)
            {
                MessageBox.Show("Папка не выбрана!", "Ошибка!");
                return;
            }

            ViewModel.Files.Clear();
            foreach (var file in Directory.GetFiles(openFileDialog.SelectedPath).Where(f => !Path.GetFileName(f).StartsWith("~$")))
            {
                ViewModel.Files.Add(new ImportEntry()
                {
                    IsSelected = false,
                    Path = file,
                    Count = ExcelLoader.LoadFileCount(file)
                });
            }
        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.Files.Any(f => f.IsSelected))
            {
                MessageBox.Show("Не выбран файл", "Внимание!");
                return;
            }
            foreach (var file in ViewModel.Files.Where(f => f.IsSelected))
            {
                var hasError = false;
                foreach (var person in ExcelUtils.LoadInputForm(file.Path))
                {
                    var sql = $@"
EXECUTE pr_AddPerson
   '{person.PassportSerial}'
  ,'{person.LastName}'
  ,'{person.FirstName}'
  ,'{person.Patronymic}'
  ,'{person.BirthDate}'
  ,'{person.BirthPlace}'
  ,'{person.PassportIssue}'
  ,'{person.PassportIssueDate}'
  ,'{person.PassportDivisionCode}'
  ,'{person.Address}'
  ,'{person.PhoneHome}'
  ,'{person.PhoneMobile}'
  ,{person.RecruitmentOfficeID}
  ,'{person.Codeword}'
  ,'{person.Comment}'
";
                    using (var sqlComm = new SqlCommandExecutor(sql))
                        if (!sqlComm.TryExecuteNonQuery(out _))
                            hasError = true;
                }
                file.IsImported = !hasError;
                file.HasError = hasError;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_CheckFile_Click(object sender, RoutedEventArgs e)
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
            {
                MessageBox.Show("Файл не выбран!", "Ошибка!");
                return;
            }


            ViewModel.PersonsToCheck.Clear();
            foreach (var entry in ExcelUtils.LoadInputForm(openFileDialog.FileName))
                ViewModel.PersonsToCheck.Add(new PersonExtended(new Person(entry)));
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                foreach (var file in ViewModel.Files)
                    file.IsSelected = checkBox.IsChecked == true;
            }
        }
    }
}
