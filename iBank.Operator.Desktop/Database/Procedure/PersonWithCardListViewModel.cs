using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace iBank.Operator.Desktop.Database
{
    public class PersonsWithCardList : INotifyPropertyChanged
    {
        public static IEnumerable<PersonsWithCardList> GetAll()
        {
            using (var sqlcmd = new SqlCommandExecutor("EXEC pr_GetPersonsWithCardList"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                while (sqlReader.Read())
                    yield return new PersonsWithCardList(sqlReader);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string PassportSerial { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string BirthDate { get; set; }
        public string RecruitmentOffice { get; set; }
        public string RecruitmentOfficeID { get; set; }
        public string AccountNumber { get; set; }

        public PersonsWithCardList(IDataRecord reader)
        {
            PassportSerial = reader.GetString(0);
            LastName = reader.GetString(1);
            FirstName = reader.GetString(2);
            Patronymic = reader.GetString(3);
            BirthDate = reader.GetDateTime(4).ToShortDateString();
            RecruitmentOfficeID = reader.GetByte(5).ToString();
            RecruitmentOffice = reader.GetString(6);
            AccountNumber = reader.GetString(7);
        }
    }
}