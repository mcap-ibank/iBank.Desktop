using iBank.Core.Database;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace iBank.Operator.Desktop.Database
{
    public class PersonAccountNumber : INotifyPropertyChanged, IPersonData, IPersonPassportData, IPersonCommentData, IPersonBankData
    {
        public static IEnumerable<PersonAccountNumber> GetAll()
        {
            using (var sqlcmd = new SqlCommandExecutor("SELECT person.*, person_card.account_number FROM person LEFT JOIN person_card ON person.passport_serial = person_card.passport_serial"))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                while (sqlReader.Read())
                    yield return new PersonAccountNumber(sqlReader);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime BirthDateDate => iBank.Core.Utils.GetDateTime(BirthDate);
        public DateTime PassportIssueDateDate => iBank.Core.Utils.GetDateTime(PassportIssueDate);

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
        public string RecruitmentOfficeID { get; set; }
        public string Codeword { get; set; }
        public string Comment { get; set; }
        public string AccountNumber { get; set; }

        protected PersonAccountNumber(IDataRecord reader)
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
            RecruitmentOfficeID = reader.GetByte(13).ToString();
            Codeword = reader.GetString(14);
            Comment = reader.IsDBNull(15) ? "" : reader.GetString(15);
            AccountNumber = reader.IsDBNull(17) ? "" : reader.GetString(17);

            if (string.IsNullOrEmpty(Patronymic))
                Patronymic = " ";
        }
    }
}