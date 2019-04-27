using System.ComponentModel;

namespace iBank.Operator.Desktop.Data
{
    public class AssigmentHistory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string PassportSerial { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
    }
}