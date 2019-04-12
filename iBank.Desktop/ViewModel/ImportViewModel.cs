using iBank.Core.Database;
using iBank.Desktop.Data;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace iBank.Desktop.ViewModel
{
    public class ImportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ImportEntry> Files { get; } = new ObservableCollection<ImportEntry>();
        public ObservableCollection<Person> PersonsToCheck { get; } = new ObservableCollection<Person>();

        public ImportViewModel() { }
    }
}