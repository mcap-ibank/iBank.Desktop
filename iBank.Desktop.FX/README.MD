# Дефайны:  
* **KEEPSQLOPEN** (iBank.Desktop.FX, iBank.Desktop.Core, iBank.Core) - Использование постоянного SQL соединения  
* **SPEECHRECOGNITION** (iBank.Desktop.FX) - Использование голосового управления  

# Библиотеки:  
* **ExcelDataReader** - для чтения xls
* **EPPlus** - для чтения\записи xlsx
* **Fody**, **PropertyChanged.Fody** - используется для генерации XAML **INotifyPropertyChanged** компилятором. Не изменять **FodyWeavers.xml**!
* **NAudio** - для захвата микрофона для распознания голоса
* **SqlTableDependency** - для подписывания на изменения данных БД
* **Ookii.Dialogs.Wpf** - улучшенные диалоги открытия файлов\папок и их сохранения
* **Newtonsoft.Json** - вспомогательная библиотека **ExcelDataReader**
* **WPF.ListViewLayoutManager** - для чего-то там для **ListView**
* **gong-wpf-dragdrop** - для легкой реализации драг-дропа