using System.ComponentModel;
using System.Data;

using iBank.Core.Database;
using iBank.Operator.Desktop.Data;
using iBank.Operator.Desktop.Excel;
using iBank.Operator.Desktop.Utils;

namespace iBank.Operator.Desktop.Database
{

    public class Person : INotifyPropertyChanged, IPersonData, IPersonPassportData, IPersonCommentData
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string PassportSerial { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Patronymic { get; set; } = "";
        public string BirthDate { get; set; } = "";
        public string BirthPlace { get; set; } = "";
        public string PassportIssue { get; set; } = "";
        public string PassportIssueDate { get; set; } = "";
        public string PassportDivisionCode { get; set; } = "";
        public string Address { get; set; } = "";
        public string PhoneHome { get; set; } = "";
        public string PhoneMobile { get; set; } = "";
        public string RecruitmentOfficeID { get; set; } = "";
        public string Codeword { get; set; } = "";
        public string Comment { get; set; } = "";
        public ErrorEnum WarningEnum
        {
            get
            {
                CommonUtils.GetWarnings(this, this, out var @enum);
                return @enum;
            }
        }
        public ErrorEnum ErrorEnum
        {
            get
            {
                CommonUtils.GetErrors(this, this, out var @enum);
                return @enum;
            }
        }

        public Person() { }
        public Person(IDataRecord reader)
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

            if (string.IsNullOrEmpty(Patronymic))
                Patronymic = " ";
        }
        public Person(ExcelPerson excelPerson)
        {
            PassportSerial = excelPerson.PassportSerial;
            LastName = excelPerson.LastName;
            FirstName = excelPerson.FirstName;
            Patronymic = excelPerson.Patronymic;
            BirthDate = excelPerson.BirthDate.ToShortDateString();
            BirthPlace = excelPerson.BirthPlace;
            PassportIssue = excelPerson.PassportIssue;
            PassportIssueDate = excelPerson.PassportIssueDate.ToShortDateString();
            PassportDivisionCode = excelPerson.PassportDivisionCode;
            Address = excelPerson.Address;
            PhoneHome = excelPerson.PhoneHome;
            PhoneMobile = excelPerson.PhoneMobile;
            RecruitmentOfficeID = excelPerson.RecruitmentOfficeID.ToString();
            Codeword = excelPerson.Codeword;
            Comment = excelPerson.Comment;

            if (string.IsNullOrEmpty(Patronymic))
                Patronymic = " ";
        }
    }
}