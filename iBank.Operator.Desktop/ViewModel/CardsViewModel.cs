using iBank.Core;
using iBank.Core.Database;
using iBank.Desktop.Data;
using iBank.Desktop.Database;
using iBank.Desktop.Extensions;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace iBank.Desktop.ViewModel
{
    public class CardsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Patronymic { get; set; } = "";
        public string PassportSerial { get; set; } = "";

        public ObservableCollection<SearchPersonToAssignCard> Persons { get; set; } = new ObservableCollection<SearchPersonToAssignCard>();
        public ObservableCollection<AssigmentHistory> AssigmentHistory { get; set; } = new ObservableCollection<AssigmentHistory>();

        public bool ShowHistory { get; set; } = false;
        public bool ShowAssigment { get; set; } = true;

        public string ImportPersonCount { get; set; } = "Найдено: 0";
        public double ImportMaxProgress { get; set; } = double.MaxValue;
        public double ImportCurrentProgress { get; set; } = 0;

        public string CheckPersonCount { get; set; } = "Найдено: 0";
        public double CheckMaxProgress { get; set; } = double.MaxValue;
        public double CheckCurrentProgress { get; set; } = 0;


        public Task ImportDailyReportAsync() => Task.Run(() =>
        {
            var personsFromDatabase = PersonAccountNumber.GetAll().ToList();
            var personsFromBank = BankProviderPerson.GetExecutedByDate(DateTime.Now).ToList();
            ImportPersonCount = $"Найдено: {personsFromBank.Count}";
            ImportMaxProgress = personsFromBank.Count;
            ImportCurrentProgress = 0;
            foreach (var personBank in personsFromBank)
            {
                ImportCurrentProgress++;

                var person = personsFromDatabase.Find(p => p.PassportSerial == personBank.PassportSerial);
                if (person == null)
                {
                    MessageBox.Show($@"Не найден человек!
{personBank.PassportSerial} 
{personBank.LastName} 
{personBank.FirstName}
{personBank.Patronymic}
{personBank.BirthDate}", "Ошибка!", MessageBoxButton.OK);
                    continue;
                }


                void ReportError()
                {
                    MessageBox.Show($@"Обнаружено расхождение! (БД | БАНК)
{person.LastName} | {personBank.LastName}
{person.FirstName} | {personBank.FirstName}
{person.Patronymic} | {personBank.Patronymic}
{person.BirthDateDate.ToShortDateString()} | {personBank.BirthDateDate.ToShortDateString()}
{person.BirthPlace} | {personBank.BirthPlace}
{person.PassportIssueDateDate.ToShortDateString()} | {personBank.PassportIssueDateDate.ToShortDateString()}
{person.PassportDivisionCode} | {personBank.PassportDivisionCode}
{person.AccountNumber} | {personBank.AccountNumber}", "Ошибка!", MessageBoxButton.OK);
                }

                if (!person.LastName.Equals(personBank.LastName))
                    ReportError();
                if (!person.FirstName.Equals(personBank.FirstName))
                    ReportError();
                if (!person.Patronymic.Equals(personBank.Patronymic) && !person.Patronymic.Equals(".") && !personBank.Patronymic.Equals(""))
                    ReportError();

                if (!person.BirthDateDate.Equals(personBank.BirthDateDate))
                    ReportError();

                if (!person.PassportIssueDateDate.Equals(personBank.PassportIssueDateDate))
                   ReportError();

                if (!person.PassportDivisionCode.Equals(personBank.PassportDivisionCode))
                    ReportError();

                if (!person.AccountNumber.Equals(personBank.AccountNumber))
                {
                    var sql = $@"
UPDATE person_card SET account_number = '{personBank.AccountNumber}' WHERE passport_serial = '{person.PassportSerial}'

IF @@ROWCOUNT = 0
   INSERT INTO person_card (passport_serial, account_number) VALUES ('{person.PassportSerial}', '{personBank.AccountNumber}')";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                        sqlcmd.TryExecuteScalar(out _);
                }
            }
        });

        public Task ImportDailyReportAsync(string path) => Task.Run(() =>
        {
            var personsFromDatabase = PersonAccountNumber.GetAll().ToList();
            var personsFromBank = ExcelLoader.LoadF1(path).ToList();
            ImportPersonCount = $"Найдено: {personsFromBank.Count}";
            ImportMaxProgress = personsFromBank.Count;
            ImportCurrentProgress = 0;
            foreach (var personBank in personsFromBank)
            {
                ImportCurrentProgress++;

                var person = personsFromDatabase.Find(p => p.PassportSerial == personBank.PassportSerial);
                if (person == null)
                {
                    MessageBox.Show($@"Не найден человек!
{personBank.PassportSerial} 
{personBank.LastName} 
{personBank.FirstName}
{personBank.Patronymic}
{personBank.BirthDate}", "Ошибка!", MessageBoxButton.OK);
                    continue;
                }


                void ReportError()
                {
                    MessageBox.Show($@"Обнаружено расхождение! (БД | БАНК)
{person.LastName} | {personBank.LastName}
{person.FirstName} | {personBank.FirstName}
{person.Patronymic} | {personBank.Patronymic}
{person.BirthDateDate.ToShortDateString()} | {personBank.BirthDateDate.ToShortDateString()}
{person.BirthPlace} | {personBank.BirthPlace}
{person.PassportIssueDateDate.ToShortDateString()} | {personBank.PassportIssueDateDate.ToShortDateString()}
{person.PassportDivisionCode} | {personBank.PassportDivisionCode}
{person.AccountNumber} | {personBank.AccountNumber}", "Ошибка!", MessageBoxButton.OK);
                }

                if (!person.LastName.Equals(personBank.LastName))
                    ReportError();
                if (!person.FirstName.Equals(personBank.FirstName))
                    ReportError();
                if (!person.Patronymic.Equals(personBank.Patronymic) && !person.Patronymic.Equals(".") && !personBank.Patronymic.Equals(""))
                    ReportError();

                if (!person.BirthDateDate.Equals(personBank.BirthDateDate))
                    ReportError();

                if (!person.PassportIssueDateDate.Equals(personBank.PassportIssueDateDate))
                    ReportError();

                if (!person.PassportDivisionCode.Equals(personBank.PassportDivisionCode))
                    ReportError();

                if (!person.AccountNumber.Equals(personBank.AccountNumber))
                {
                    var sql = $@"
UPDATE person_card SET account_number = '{personBank.AccountNumber}' WHERE passport_serial = '{person.PassportSerial}'

IF @@ROWCOUNT = 0
   INSERT INTO person_card (passport_serial, account_number) VALUES ('{person.PassportSerial}', '{personBank.AccountNumber}')";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                        sqlcmd.TryExecuteScalar(out _);
                }
            }
        });


        public Task CheckTotalReportAsync() => Task.Run(() =>
        {
            var personsFromDatabase = PersonAccountNumber.GetAll().ToList();
            var personsFromBank = BankProviderPerson.GetAll().Where(p => !string.IsNullOrEmpty(p.PassportSerial)).ToList();
            CheckPersonCount = $"Найдено: {personsFromBank.Count}";
            CheckMaxProgress = personsFromDatabase.Count;
            CheckCurrentProgress = 0;
            foreach (var personDatabase in personsFromDatabase)
            {
                CheckCurrentProgress++;

                var personBank = personsFromBank.Find(p => p.PassportSerial == personDatabase.PassportSerial);
                if (personBank == null)
                {
                    /*
                    MessageBox.Show($@"Не найден человек в Банке!
{personDatabase.PassportSerial} 
{personDatabase.LastName} 
{personDatabase.FirstName}
{personDatabase.Patronymic}
{personDatabase.BirthDate}", "Ошибка!", MessageBoxButton.OK);
                    */
                    continue;
                }


                void ReportError()
                {
                    MessageBox.Show($@"Обнаружено расхождение! (БД|БАНК)
{personDatabase.LastName.PadRight(20, ' ')}  | {personBank.LastName}
{personDatabase.FirstName.PadRight(20, ' ')} | {personBank.FirstName}
{personDatabase.Patronymic.PadRight(20, ' ')} | {personBank.Patronymic}
{personDatabase.BirthDateDate.ToShortDateString().PadRight(20, ' ')} | {personBank.BirthDateDate.ToShortDateString()}
{personDatabase.BirthPlace.PadRight(20, ' ')} | {personBank.BirthPlace}
{personDatabase.PassportIssueDateDate.ToShortDateString().PadRight(20, ' ')} | {personBank.PassportIssueDateDate.ToShortDateString()}
{personDatabase.PassportDivisionCode.PadRight(20, ' ')} | {personBank.PassportDivisionCode}
{(string.IsNullOrEmpty(personDatabase.AccountNumber) ? "Не назначена карта" : personDatabase.AccountNumber).PadRight(20, ' ')} | {(string.IsNullOrEmpty(personBank.AccountNumber) ? "Не назначена карта" : personBank.AccountNumber)}
Исполнено: {((personBank.CardGiven ?? false) ? "Да" : "Нет").PadRight(20, ' ')}", "Ошибка!", MessageBoxButton.OK);
                }

                if (!personDatabase.LastName.Equals(personBank.LastName))
                    ReportError();
                if (!personDatabase.FirstName.Equals(personBank.FirstName))
                    ReportError();
                if (!personDatabase.Patronymic.Equals(personBank.Patronymic, StringComparison.OrdinalIgnoreCase) && !personBank.Patronymic.Equals(".") && !personDatabase.Patronymic.Equals(" "))
                    ReportError();

                if (!personDatabase.BirthDateDate.Equals(personBank.BirthDateDate))
                    ReportError();

                if (!personDatabase.PassportIssueDateDate.Equals(personBank.PassportIssueDateDate))
                    ReportError();

                if (!personDatabase.PassportDivisionCode.Equals(personBank.PassportDivisionCode))
                    ReportError();

                if (!personDatabase.AccountNumber.Equals(personBank.AccountNumber))
                {
                    if (!string.IsNullOrEmpty(personBank.AccountNumber))
                        ;

                    var sql = $@"
UPDATE person_card SET account_number = '{personBank.AccountNumber}' WHERE passport_serial = '{personDatabase.PassportSerial}'

IF @@ROWCOUNT = 0
   INSERT INTO person_card (passport_serial, account_number) VALUES ('{personDatabase.PassportSerial}', '{personBank.AccountNumber}')";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                        sqlcmd.TryExecuteScalar(out _);
                }
            }
        });

        public Task CheckTotalReportAsync(string path) => Task.Run(() =>
        {
            var personsFromDatabase = PersonAccountNumber.GetAll().ToList();
            var personsFromBank = ExcelLoader.LoadF(path).ToList();
            CheckPersonCount = $"Найдено: {personsFromBank.Count}";
            CheckMaxProgress = personsFromDatabase.Count;
            CheckCurrentProgress = 0;
            foreach (var personDatabase in personsFromDatabase)
            {
                CheckCurrentProgress++;

                var personBank = personsFromBank.Find(p => p.PassportSerial == personDatabase.PassportSerial);
                if (personBank == null)
                {
                    /*
                    MessageBox.Show($@"Не найден человек в Банке!
{personDatabase.PassportSerial} 
{personDatabase.LastName} 
{personDatabase.FirstName}
{personDatabase.Patronymic}
{personDatabase.BirthDate}", "Ошибка!", MessageBoxButton.OK);
                    */
                    continue;
                }


                void ReportError()
                {
                    MessageBox.Show($@"Обнаружено расхождение! (БД|БАНК)
{personDatabase.LastName}  | {personBank.LastName}
{personDatabase.FirstName} | {personBank.FirstName}
{personDatabase.Patronymic} | {personBank.Patronymic}
{personDatabase.BirthDateDate.ToShortDateString()} | {personBank.BirthDateDate.ToShortDateString()}
{personDatabase.BirthPlace} | {personBank.BirthPlace}
{personDatabase.PassportIssueDateDate.ToShortDateString()} | {personBank.PassportIssueDateDate.ToShortDateString()}
{personDatabase.PassportDivisionCode} | {personBank.PassportDivisionCode}
{personDatabase.AccountNumber} | {personBank.AccountNumber}
Исполнено: {((personBank.CardGiven ?? false) ? "Да" : "Нет")}", "Ошибка!", MessageBoxButton.OK);
                }

                if (!personDatabase.LastName.Equals(personBank.LastName))
                    ReportError();
                if (!personDatabase.FirstName.Equals(personBank.FirstName))
                    ReportError();
                if (!personDatabase.Patronymic.Equals(personBank.Patronymic, StringComparison.OrdinalIgnoreCase) && !personBank.Patronymic.Equals(".") && !personDatabase.Patronymic.Equals(" "))
                    ReportError();

                if (!personDatabase.BirthDateDate.Equals(personBank.BirthDateDate))
                    ReportError();

                if (!personDatabase.PassportIssueDateDate.Equals(personBank.PassportIssueDateDate))
                    ReportError();

                if (!personDatabase.PassportDivisionCode.Equals(personBank.PassportDivisionCode))
                    ReportError();

                if (!personDatabase.AccountNumber.Equals(personBank.AccountNumber))
                {
                    var sql = $"UPDATE person_card SET account_number = '{personBank.AccountNumber}' WHERE passport_serial = '{personDatabase.PassportSerial}'";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                        sqlcmd.TryExecuteScalar(out _);
                }
            }
        });
    }
}