using System.ComponentModel;

namespace iBank.Operator.Desktop.Data
{
    public class ImportEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelected { get; set; }
        public bool IsImported { get; set; }

        public bool HasError { get; set; }

        public string Name => System.IO.Path.GetFileName(Path);
        public int Count { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}
