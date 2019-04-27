using System;

namespace iBank.Operator.Desktop.Excel
{
    public class ExcelPerson
    {
        public string PassportSerial { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; } = string.Empty;
        public string PassportIssue { get; set; } = string.Empty;
        public DateTime PassportIssueDate { get; set; }
        public string PassportDivisionCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneHome { get; set; } = string.Empty;
        public string PhoneMobile { get; set; } = string.Empty;
        public int RecruitmentOfficeID { get; set; }
        public string Codeword { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}