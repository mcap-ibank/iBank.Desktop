using System;
using System.ComponentModel;

namespace iBank.Operator.Desktop.Database
{
    public class PersonBank : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime BirthDateDate => iBank.Core.Utils.GetDateTime(BirthDate);
        public DateTime PassportIssueDateDate => iBank.Core.Utils.GetDateTime(PassportIssueDate);

        public string PassportSerial { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string BirthPlace { get; set; } = string.Empty;
        public string PassportIssue { get; set; } = string.Empty;
        public string PassportIssueDate { get; set; } = string.Empty;
        public string PassportDivisionCode { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public bool? CardGiven { get; set; } = null;
    }
}