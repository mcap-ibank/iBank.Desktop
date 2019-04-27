using System;
using System.Windows;

namespace iBank.Operator.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    MessageBox.Show(ex.ToString(), e.IsTerminating ? "Критическая ошибка! Закрытие программы!" : "Ошибка!");
            };
        }


#if SPEECHRECOGNITION
        [System.Runtime.InteropServices.DllImport("winmm.dll", EntryPoint = "waveInGetNumDevs")]
        public static extern int WaveInGetNumDevs();

        private static System.Globalization.CultureInfo Russian { get; } = new System.Globalization.CultureInfo("ru-RU");

        public static bool IsSpeechAvailable
        {
            get
            {
                try
                {
                    if (!System.Linq.Enumerable.Any(Microsoft.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers(), r => r.Culture.Equals(Russian)))
                        return false;

                    if (WaveInGetNumDevs() == 0)
                        return false;

                    new Microsoft.Speech.Recognition.SpeechRecognitionEngine(Russian).Dispose();
                }
                catch (Exception e) when (e is System.Runtime.InteropServices.COMException)
                {
                    // x64 and x86 Runtime required
                    return false;
                }
                catch (Exception e) when (e is NullReferenceException)
                {
                    // Russian language required
                    return false;
                }
                return true;
            }
        }
#endif
    }
}