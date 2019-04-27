using System.Windows;

namespace iBank.Operator.Desktop
{
    /// <summary>
    /// Interaction logic for TextPopup.xaml
    /// </summary>
    public partial class TextPopupWindow : Window
    {
        public TextPopupWindow()
        {
            InitializeComponent();
        }

        public TextPopupWindow(string question, string title, string defaultValue = "")
        {
            InitializeComponent();
            Loaded += PromptDialog_Loaded;
            txtQuestion.Text = question;
            Title = title;
            txtResponse.Text = defaultValue;
        }

        private void PromptDialog_Loaded(object sender, RoutedEventArgs e) => txtResponse.Focus();

        public static string? Prompt(string question, string title, string defaultValue = "")
        {
            var inst = new TextPopupWindow(question, title, defaultValue);
            inst.ShowDialog();
            return inst.DialogResult == true ? inst.ResponseText : null;
        }

        public string ResponseText => txtResponse.Text;

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
