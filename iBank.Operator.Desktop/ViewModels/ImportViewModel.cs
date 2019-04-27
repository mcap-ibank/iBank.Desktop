using iBank.Operator.Desktop.Data;
using iBank.Operator.Desktop.Database;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace iBank.Operator.Desktop.ViewModels
{
    public class ImportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ImportEntry> Files { get; } = new ObservableCollection<ImportEntry>();
        public ObservableCollection<Person> PersonsToCheck { get; } = new ObservableCollection<Person>();

        public ImportViewModel() { }
    }
}