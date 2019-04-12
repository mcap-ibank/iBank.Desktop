using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using iBank.Core.Database;
using iBank.Core.Utils;
using iBank.Desktop.SpeechRecognition;
using iBank.Desktop.ViewModel;

namespace iBank.Desktop.View
{

    /// <summary>
    /// Interaction logic for VerificationView.xaml
    /// </summary>
    public partial class VerificationView
    {
        public VerificationViewModel ViewModel => DataContext as VerificationViewModel ?? throw new Exception("Шо блэт");

#if SPEECHRECOGNITION
        private bool IsSubscribed { get; set; }
#endif


        public VerificationView()
        {
            InitializeComponent();
            DataContext = new VerificationViewModel();

#if SPEECHRECOGNITION
            if(App.IsSpeechAvailable)
                SpeechActivator.Instance.OnRecognisedDatabase += (sender, args) =>
                {
                    switch (args.Command)
                    {
                        case "искать человека":
                            UpdateRecognition();
                            break;

                        case "сброс":
                            UpdateRecognition();
                            break;

                        case "фамилия":
                            break;
                        case "имя":
                            break;
                        case "отчество":
                            break;
                    }
                };
#endif
        }

#if SPEECHRECOGNITION
        private void UpdateRecognition()
        {
            if (IsSubscribed)
            {
                SpeechActivator.Instance.OnRecognisedLetter -= SpeechActivator_OnRecognisedLetters;
                IsSubscribed = false;
            }
            else
            {
                SpeechActivator.Instance.OnRecognisedLetter += SpeechActivator_OnRecognisedLetters;
                IsSubscribed = true;
            }
        }

        private void SpeechActivator_OnRecognisedLetters(object sender, LetterSpeechCommandEventArgs e)
        {
            switch (e.Letter)
            {
                case char n when (n >= 'а' && n <= 'я'):
                    ViewModel.LastName += n;
                    break;
            }
        }
#endif

        #region TextBox filters

        private void WatermarkTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!CommonUtils.FilterText(e.Text[0]))
                e.Handled = true;
        }

        private void WatermarkTextBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!CommonUtils.FilterPassport(e.Text[0]))
                e.Handled = true;
        }

        private void WatermarkTextBox3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!CommonUtils.FilterCardNumber(e.Text[0]))
                e.Handled = true;
        }

        #endregion

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.Persons.Clear();
            foreach (var person in SearchPerson.GetAll(ViewModel.LastName, ViewModel.FirstName, ViewModel.Patronymic, ViewModel.PassportSerial, ViewModel.AccountNumber))
                ViewModel.Persons.Add(person);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is SearchPerson person)
            {
                ViewModel.SelectedPerson.Clear();
                ViewModel.SelectedPerson.Add(person);
                //ViewModel.SelectedPerson = person;
            }
        }

        private void MenuItem_IsOrphan_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
