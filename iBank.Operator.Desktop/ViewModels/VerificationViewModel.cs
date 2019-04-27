using iBank.Operator.Desktop.Database;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace iBank.Operator.Desktop.ViewModels
{
    public class VerificationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Patronymic { get; set; } = "";
        public string PassportSerial { get; set; } = "";
        public string AccountNumber { get; set; } = "";

        public ObservableCollection<SearchPerson> Persons { get; } = new ObservableCollection<SearchPerson>();
        public ObservableCollection<SearchPerson> SelectedPerson { get; } = new ObservableCollection<SearchPerson>();
        //public PersonSearchViewModel SelectedPerson { get; set; }
    }
}
