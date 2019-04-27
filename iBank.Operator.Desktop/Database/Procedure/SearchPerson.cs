using iBank.Core.Database;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace iBank.Operator.Desktop.Database
{
    public class SearchPerson : INotifyPropertyChanged, IPersonData, IPersonPassportData, IPersonCommentData, IPersonBankData
    {
        public static IEnumerable<SearchPerson> GetAll(string lastName, string firstName, string patronymic, string passportSerial, string accountNumber)
        {
            var sql = $@"EXEC pr_SearchPerson '{lastName}%','{firstName}%','{patronymic}%','{passportSerial}%','%{accountNumber}'";
            using (var sqlcmd = new SqlCommandExecutor(sql))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                while (sqlReader.Read())
                    yield return new SearchPerson(sqlReader);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string PassportSerial { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string PassportIssue { get; set; }
        public string PassportIssueDate { get; set; }
        public string PassportDivisionCode { get; set; }
        public string Address { get; set; }
        public string PhoneHome { get; set; }
        public string PhoneMobile { get; set; }
        public string RecruitmentOffice { get; set; }
        public string Codeword { get; set; }
        public string Comment { get; set; }
        public string AccountNumber { get; set; }
        public string IsOrphan { get; set; }

        public bool IsPassportInvalid => !iBank.Core.Utils.PassportIsValid(DateTime.ParseExact(BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(PassportIssueDate, "dd.MM.yyyy", CultureInfo.InvariantCulture));

        public bool IsPassportExpiringSoon
        {
            get
            {
                var bd = DateTime.ParseExact(BirthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                var days = (bd.AddYears(20) - DateTime.Now).Days + 1;
                if (days < 0)
                    return false;
                if (days > 0 && days < 14)
                    return true;
                return false;
            }
        }

        public string PassportDaysLeft
        {
            get
            {
                switch ((DateTime.Parse(BirthDate).AddYears(20) - DateTime.Now).Days + 1)
                {
                    case int d when d > 0:
                        return $"{d} дней";

                    case int d when d == 0:
                        return "Сегодня";

                    case int d when d < 0:
                        return "Продлен";

                    default:
                        return "ОШИБКА";
                }
            }
        }

        protected SearchPerson(IDataRecord reader)
        {
            PassportSerial = reader.GetString(1);
            LastName = reader.GetString(2);
            FirstName = reader.GetString(3);
            Patronymic = reader.GetString(4);
            BirthDate = reader.GetDateTime(5).ToShortDateString();
            BirthPlace = reader.GetString(6);
            PassportIssue = reader.GetString(7);
            PassportIssueDate = reader.GetDateTime(8).ToShortDateString();
            PassportDivisionCode = reader.GetString(9);
            Address = reader.GetString(10);
            PhoneHome = reader.GetString(11);
            PhoneMobile = reader.GetString(12);
            Codeword = reader.GetString(14);
            Comment = reader.IsDBNull(15) ? "" : reader.GetString(15);
            RecruitmentOffice = reader.GetString(17);
            AccountNumber = reader.IsDBNull(18) ? "Не назначена" : reader.GetString(18);
            IsOrphan = (reader.IsDBNull(19) ? false : reader.GetBoolean(19)) ? "Да" : "";
        }
    }
}