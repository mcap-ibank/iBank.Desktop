using System.Windows;

namespace iBank.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if SPEECHRECOGNITION
            if(App.IsSpeechAvailable)
            {
                SpeechRecognition.SpeechActivator.Instance.Start();
                SpeechRecognition.SpeechActivator.Instance.OnRecognisedMain += (s, e) =>
                {
                    switch (e.Command)
                    {
                        case SpeechRecognition.MainSpeechCommands.Database:
                            TabControl.SelectedIndex = 3;
                            break;
                        case SpeechRecognition.MainSpeechCommands.Team:
                            TabControl.SelectedIndex = 2;
                            break;
                        case SpeechRecognition.MainSpeechCommands.Card:
                            TabControl.SelectedIndex = 0;
                            break;
                    }
                };
            }
#endif
        }
    }
}