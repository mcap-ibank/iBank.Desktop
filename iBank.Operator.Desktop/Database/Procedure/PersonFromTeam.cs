using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace iBank.Operator.Desktop.Database
{
    public class PersonFromTeam : INotifyPropertyChanged
    {
        public static IEnumerable<PersonFromTeam> GetAll(string inventory)
        {
            using (var sqlcmd = new SqlCommandExecutor($"EXEC pr_GetPersonsFromTeam {inventory}"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                while (sqlReader.Read()) //Выводим в список
                    yield return new PersonFromTeam(sqlReader);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string PassportSerial { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string BirthDate { get; set; }
        public string AccountNumber { get; set; }

        internal bool? _isSelected;
        public bool IsSelected
        {
            get
            {
                if (_isSelected == null)
                {
                    var sql = $"SELECT account_number_registered_in_military_id FROM person_team_metadata WHERE passport_serial = \'{PassportSerial}\'";
                    using (var sqlcmd = new SqlCommandExecutor(sql))
                        _isSelected = Convert.ToBoolean(sqlcmd.ExecuteScalar());
                }
                return _isSelected.Value;
            }

            set => _isSelected = value;
        }

        public PersonFromTeam(PersonsWithCardList source)
        {
            PassportSerial = source.PassportSerial;
            LastName = source.LastName;
            FirstName = source.FirstName;
            Patronymic = source.Patronymic;
            BirthDate = source.BirthDate;
            AccountNumber = source.AccountNumber;
        }
        protected PersonFromTeam(IDataRecord reader)
        {
            PassportSerial = reader.GetString(0);
            LastName = reader.GetString(1);
            FirstName = reader.GetString(2);
            Patronymic = reader.GetString(3);
            BirthDate = reader.GetDateTime(4).ToShortDateString();
            AccountNumber = reader.GetString(7);
        }
    }
}