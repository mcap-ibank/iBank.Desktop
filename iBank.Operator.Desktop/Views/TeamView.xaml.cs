using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using iBank.Operator.Desktop;
using iBank.Operator.Desktop.Database;
using iBank.Operator.Desktop.Extensions;
using iBank.Operator.Desktop.SpeechRecognition;
using iBank.Operator.Desktop.ViewModels;

using OfficeOpenXml;

using Ookii.Dialogs.Wpf;

using TableDependency.SqlClient.Base.Enums;
using iBank.Core;

namespace iBank.Operator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TeamView.xaml
    /// </summary>
    public partial class TeamView
    {
        private TeamViewModel ViewModel => DataContext as TeamViewModel ?? throw new Exception("Шо блэт");

#if SPEECHRECOGNITION

        private bool IsSubscribed { get; set; }
        private bool IsInventory { get; set; } = true;
#endif

        public TeamView()
        {
            InitializeComponent();
            DataContext = new TeamViewModel();

            IsVisibleChanged += (_, e) =>
            {
                if(e.NewValue is bool isVisible)
                {
                    if(isVisible && ViewModel.Dependency?.Status != TableDependencyStatus.Started)
                        ViewModel.Dependency?.Start();
                    if (!isVisible && ViewModel.Dependency?.Status == TableDependencyStatus.Started)
                        ViewModel.Dependency?.Stop();
                }
            };

#if SPEECHRECOGNITION
            if (App.IsSpeechAvailable)
                SpeechActivator.Instance.OnRecognisedTeam += (sender, args) =>
                {
                    switch (args.Command)
                    {
                        case "создать команду":
                            UpdateRecognition();
                            break;

                        case "подтвердить":
                            UpdateRecognition();
                            Button_CreateTeam_Click(sender, EventArgs.Empty);
                            break;
                        case "отмена":
                            UpdateRecognition();
                            break;

                        case "исходящий":
                            IsInventory = true;
                            break;
                        case "команда":
                            IsInventory = false;
                            break;

                        case "сброс":
                            if (IsInventory)
                                ViewModel.CreateTeamOutgoing = "";
                            else
                                ViewModel.CreateTeamTeam = "";
                            break;
                    }
                };
#endif
        }

#if SPEECHRECOGNITION
        private void UpdateRecognition()
        {
            if (IsSubscribed)
            {
                SpeechActivator.Instance.OnRecognisedDigit -= SpeechActivator_OnRecognisedDigits;
                IsSubscribed = false;
            }
            else
            {
                SpeechActivator.Instance.OnRecognisedDigit += SpeechActivator_OnRecognisedDigits;
                IsSubscribed = true;
            }
        }
        private void SpeechActivator_OnRecognisedDigits(object sender, DigitsSpeechCommandEventArgs e)
        {
            if (IsInventory)
                ViewModel.CreateTeamOutgoing += e.Digit;
            else
                ViewModel.CreateTeamTeam += e.Digit;
        }
#endif

        //Обновление списка людей с прописанными картами
        private void Button_RefreshPersonToAssignTeamList_Click(object sender, EventArgs e)
        {
            ViewModel.PersonToAssignTeamList.Clear();
            foreach (var person in PersonsWithCardList.GetAll())
                ViewModel.PersonToAssignTeamList.Add(person);

            //Считаем количество народа с картами.
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetAssignedCardCount"))
                ViewModel.AssignedCardCount = $"Люди с присвоенными картами: {sqlcmd.ExecuteScalar()}";

            // -- Считаем количество народа с картами за сегодня.
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetAssignedCardCountToday"))
                ViewModel.AssignedCardTodayCount = $"Присвоено карт за сегодня: {sqlcmd.ExecuteScalar()}";

            // -- Считаем количество народа с картами за сегодня.
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetAssignedCardWithoutTeamCount"))
                ViewModel.AssignedCardTodayWithoutTeamCount = $"Не отправенных в команду: {sqlcmd.ExecuteScalar()}";
        }

        //обновляем список исходящих по кнопке
        private void Button_RefreshTeamList_Click(object sender, EventArgs e) => ViewModel.UpdateTeamList(sender, e);

       
        private void ListView_TeamDetail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.IsMouseOver)
            {
                ViewModel.SelectedPersonFromTeam.IsSelected = true;
                var sql = $"EXEC pr_AccountNumberRegisteredChanged \'{ViewModel.SelectedPersonFromTeam.PassportSerial}\', 1 ";
                using (var sqlcmd = new SqlCommandExecutor(sql))
                    sqlcmd.TryExecuteNonQuery(out _);
            }
        }

        // Создание новой команды
        private void Button_CreateTeam_Click(object sender, EventArgs e)
        {
            if (ViewModel.CreateTeamOutgoing?.Length != 0 && ViewModel.CreateTeamTeam?.Length != 0)
            {
                var sql = $"EXEC pr_CreateTeam \'{ViewModel.CreateTeamOutgoing}\', \'{ViewModel.CreateTeamTeam}\' ";
                using (var sqlcmd = new SqlCommandExecutor(sql))
                {
                    if (sqlcmd.TryExecuteNonQuery(out _))
                    {
                        ViewModel.CreateTeamOutgoing = string.Empty;
                        ViewModel.CreateTeamTeam = string.Empty;

                        Button_RefreshTeamList_Click(sender, EventArgs.Empty);
                    }
                }
            }
        }

        // Выбор команды
        private void ListView_TeamList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.TeamList.Count <= 0|| ViewModel.SelectedTeam == null)
                return;

            ViewModel.PersonFromTeamList.Clear();
            foreach (var person in PersonFromTeam.GetAll(ViewModel.SelectedTeam.Inventory))
                ViewModel.PersonFromTeamList.Add(person);

        }

        // Суем в команду по двойному щелчку
        private void ListView_Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(ViewModel.SelectedPersonToAssignTeam == null || ViewModel.SelectedTeam == null || e.ChangedButton == MouseButton.Right)
                return;

            var sql = $"EXEC pr_AssignTeam \'{ViewModel.SelectedPersonToAssignTeam.PassportSerial}\', \'{ViewModel.SelectedTeam.Inventory}\' ";
            using (var sqlcmd = new SqlCommandExecutor(sql))
            {
                if (sqlcmd.TryExecuteNonQuery(out _))
                {
                    ViewModel.PersonFromTeamList.Add(new PersonFromTeam(ViewModel.SelectedPersonToAssignTeam));
                    ViewModel.PersonToAssignTeamList.Remove(ViewModel.SelectedPersonToAssignTeam);
                }
            }
        }


        // Убираем желтую полоску, т.е, что ему прописали в военнике карту
        private void MenuItem_RevertAccountNumberAssigment_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedPersonFromTeam.IsSelected = false;
            var sql = $"EXEC pr_AccountNumberRegisteredChanged \'{ViewModel.SelectedPersonFromTeam.PassportSerial}\', 0 ";
            using (var sqlcmd = new SqlCommandExecutor(sql))
                sqlcmd.TryExecuteNonQuery(out _);

            if (sender is ListView listView)
                listView.SelectedIndex = -1;
        }

        // Убираем человека из команды
        private void MenuItem_RemovePersonFromTeam_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите убрать из команды призывника?", "Внимание!", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using (var sqlcmd = new SqlCommandExecutor($"EXEC pr_DeassignTeam \'{ViewModel.SelectedPersonFromTeam.PassportSerial}\'"))
                if (sqlcmd.TryExecuteNonQuery(out _))
                    ViewModel.PersonFromTeamList.Remove(ViewModel.SelectedPersonFromTeam);
        }

        // Удаление команды
        private void MenuItem_DeleteTeam_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить команду?", "Внимание!", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using (var sqlcmd = new SqlCommandExecutor($"EXEC pr_GetTeamPersonCount \'{ViewModel.SelectedTeam.Inventory}\'"))
                if (sqlcmd.TryExecuteScalar(out var result) && result is int value && value > 0)
                {
                    MessageBox.Show("В команде остались призывники!", "Ошибка!", MessageBoxButton.OK);
                    return;
                }

            using (var sqlcmd = new SqlCommandExecutor($"EXEC pr_DeleteTeam \'{ViewModel.SelectedTeam.Inventory}\'"))
                if (sqlcmd.TryExecuteNonQuery(out _))
                    ViewModel.TeamList.Remove(ViewModel.SelectedTeam);
        }

        // Изменение исходящего команды
        private void MenuItem_ChangeOutgoing_OnClick(object sender, RoutedEventArgs e)
        {
            var outgoing = TextPopupWindow.Prompt("Введи номер нового исходника", "Внимание!");
            if(outgoing == null)
                return;
            var newOutgoing = int.Parse(outgoing);
            using (var sqlcmd = new SqlCommandExecutor($"EXEC pr_TransferTeam \'{ViewModel.SelectedTeam.Inventory}\', \'{newOutgoing}\'"))
                if (sqlcmd.TryExecuteNonQuery(out _))
                    ViewModel.SelectedTeam.Inventory = newOutgoing.ToString();

            Button_RefreshTeamList_Click(sender, e);
        }

        // Создание отчета для старшего команды
        private void MenuItem_CreateOfficerReport_OnClick(object sender, RoutedEventArgs e)
        {
            var path = $"{Directory.GetCurrentDirectory()}\\Documents\\Старшим Команд.xlsx";
            using (var p = new ExcelPackage(new FileInfo(path)))
            using (var sqlcmd = new SqlCommandExecutor($"EXEC pr_GetTeamOfficerInfo {ViewModel.SelectedTeam.Inventory}"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                //Get the Worksheet created in the previous codesample. 
                var ws = p.Workbook.Worksheets.First();
                var i = 3;
                while (sqlReader.Read())
                {
                    ws.Cells[$"A{i}"].Value = sqlReader.GetString(1);
                    ws.Cells[$"B{i}"].Value = sqlReader.GetString(2);
                    ws.Cells[$"C{i}"].Value = sqlReader.GetString(3);
                    ws.Cells[$"D{i}"].Value = sqlReader.GetString(4);
                    ws.Cells[$"E{i}"].Value = sqlReader.GetString(5);
                    ws.Cells[$"F{i}"].Value = sqlReader.GetString(6);
                    ws.Cells[$"G{i}"].Value = sqlReader.GetString(7);
                    ws.Cells[$"H{i}"].Value = sqlReader.GetString(8);
                    ws.Cells[$"I{i}"].Value = sqlReader.GetString(9);
                    ws.Cells[$"J{i}"].Value = sqlReader.GetString(10);
                    ws.Cells[$"K{i}"].Value = sqlReader.GetString(11);
                    ws.Cells[$"L{i}"].Value = sqlReader.GetString(12);
                    ws.Cells[$"M{i}"].Value = sqlReader.GetString(13);
                    ws.Cells[$"N{i}"].Value = sqlReader.GetString(14);
                    i++;
                }

                var dlg = new VistaSaveFileDialog()
                {
                    Filter = "Office Open XML |*.xlsx",
                    FileName = $"Команда{ViewModel.SelectedTeam.Team}",
                    DefaultExt = ".xlsx",
                    OverwritePrompt = true,
                    CreatePrompt = false,
                    //InitialDirectory = "shell:MyComputerFolder", //""shell:NetworkPlacesFolder""

                };
                if (dlg.ShowDialog() == true)
                {
                    using (var fs = File.Create(dlg.FileName))
                        p.SaveAs(fs);
                }
            }
        }


        // Фильтрация ввода ФИО
        private void TextBox_FilterFIO_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!iBank.Core.Utils.FilterText(e.Text[0]))
                e.Handled = true;
        }

        // Фильтрация ввода паспорта
        private void TextBox_FilterPassport_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!iBank.Core.Utils.FilterPassport(e.Text[0]))
                e.Handled = true;
        }

        // Фильтрация ввода карты
        private void TextBox_FilterAccountNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!iBank.Core.Utils.FilterCardNumber(e.Text[0]))
                e.Handled = true;
        }

        // Фильтрация ввода инвентарного и команды
        private void TextBox_FilterTeam_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsControl(e.Text[0]) && !char.IsDigit(e.Text[0]))
                e.Handled = true;
        }
    }
}
