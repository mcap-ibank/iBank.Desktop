using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace iBank.Operator.Desktop.Database
{
    public class SearchPersonToAssignCard : INotifyPropertyChanged
    {
        public static IEnumerable<SearchPersonToAssignCard> GetAll(string lastName, string firstName, string patronymic, string passportSerial)
        {
            var sql = $@"EXEC pr_SearchPersonToAssignCard '{lastName}%','{firstName}%','{patronymic}%','{passportSerial}%'";
            using (var sqlcmd = new SqlCommandExecutor(sql))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                while (sqlReader.Read())
                    yield return new SearchPersonToAssignCard(sqlReader);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string PassportSerial { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string BirthDate { get; set; }
        public string Comment { get; set; }

        public SearchPersonToAssignCard(IDataRecord reader)
        {
            PassportSerial = reader.GetString(0);
            LastName = reader.GetString(1);
            FirstName = reader.GetString(2);
            Patronymic = reader.GetString(3);
            BirthDate = reader.GetDateTime(4).ToShortDateString();
            Comment = reader.GetString(5);
        }
    }
}