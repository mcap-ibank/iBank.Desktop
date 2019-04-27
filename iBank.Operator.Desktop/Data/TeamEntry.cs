using System.ComponentModel;

namespace iBank.Operator.Desktop.Data
{
    public class TeamEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Inventory { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public string PeopleCount { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
    }
}