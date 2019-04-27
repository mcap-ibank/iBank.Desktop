#if SPEECHRECOGNITION
using Microsoft.Speech.Recognition;

using System;
using System.Globalization;

namespace iBank.Operator.Desktop.SpeechRecognition
{
    public static class SpeechRecognitionEngineExtensions
    {
        public static void SetInputToDefaultAudioStream(this SpeechRecognitionEngine eng) => eng.SetInputToAudioStream(new SpeechStreamer());
        public static void SetInputToAudioStream(this SpeechRecognitionEngine eng, SpeechStreamer stream) => eng.SetInputToAudioStream(stream, stream.GetSpeechAudioFormatInfo());
    }

    public class SpeechActivator
    {
        private static CultureInfo Russian { get; } = new CultureInfo("ru-RU");
        public static SpeechActivator Instance { get; } = new SpeechActivator();


        public event EventHandler<MainSpeechCommandEventArgs> OnRecognisedMain;
        public event EventHandler<CardSpeechCommandEventArgs> OnRecognisedCard;
        public event EventHandler<TeamSpeechCommandEventArgs> OnRecognisedTeam;
        public event EventHandler<DatabaseSpeechCommandEventArgs> OnRecognisedDatabase;

        public event EventHandler<LetterSpeechCommandEventArgs> OnRecognisedLetter;
        public event EventHandler<DigitsSpeechCommandEventArgs> OnRecognisedDigit;

        private Choices DatabaseListenerChoices { get; } = new Choices(
            "искать человека",
            "сброс");
        private SpeechRecognitionEngine DatabaseListener { get; } = new SpeechRecognitionEngine(Russian);

        private Choices TeamListenerChoices { get; } = new Choices(
            "создать команду",
            "подтвердить",
            "отмена",
            "исходящий",
            "команда",
            "сброс");
        private SpeechRecognitionEngine TeamListener { get; } = new SpeechRecognitionEngine(Russian);

        private Choices LettersListenerChoices { get; } = new Choices();
        private SpeechRecognitionEngine LettersListener { get; } = new SpeechRecognitionEngine(Russian);

        private Choices DigitsListenerChoices { get; } = new Choices(
            //"ноль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"
            );
        private SpeechRecognitionEngine DigitsListener { get; } = new SpeechRecognitionEngine(Russian);


        private Choices AlwaysOnListenerChoices { get; } = new Choices(
            "айбанк карты",
            "айбанк команды",
            "айбанк база данных", "айбанк база", "айбанк бд",
            "айбанк создать команду",
            "айбанк искать человека");
        private SpeechRecognitionEngine AlwaysOnListener { get; } = new SpeechRecognitionEngine(Russian);

        public SpeechActivator()
        {
            DatabaseListener.LoadGrammar(new Grammar(new GrammarBuilder(DatabaseListenerChoices)));
            DatabaseListener.SpeechRecognized += (s, e) =>
            {
                if (e.Result.Confidence < 0.70) return;
                OnRecognisedDatabase?.Invoke(this, new DatabaseSpeechCommandEventArgs(e.Result.Text));
            };
            DatabaseListener.SetInputToDefaultAudioStream();

            TeamListener.LoadGrammar(new Grammar(new GrammarBuilder(TeamListenerChoices)));
            TeamListener.SpeechRecognized += (s, e) =>
            {
                if (e.Result.Confidence < 0.70) return;
                OnRecognisedTeam?.Invoke(this, new TeamSpeechCommandEventArgs(e.Result.Text));
            };
            TeamListener.SetInputToDefaultAudioStream();

            for (var i = 'а'; i <= 'я'; i++)
                LettersListenerChoices.Add(i.ToString());
            var gb0 = new GrammarBuilder(LettersListenerChoices);
            LettersListener.LoadGrammar(new Grammar(gb0));
            LettersListener.SpeechRecognized += (s, e) =>
            {
                if (e.Result.Confidence < 0.70) return;
                OnRecognisedLetter?.Invoke(this, new LetterSpeechCommandEventArgs(e.Result.Text[0]));
            };
            LettersListener.SetInputToDefaultAudioStream();

            for (var i = 0; i <= 9; i++)
                DigitsListenerChoices.Add(i.ToString());
            var gb1 = new GrammarBuilder(DigitsListenerChoices);
            DigitsListener.LoadGrammar(new Grammar(gb1));
            DigitsListener.SpeechRecognized += (s, e) =>
            {
                if (e.Result.Confidence < 0.70) return;
                OnRecognisedDigit?.Invoke(this, new DigitsSpeechCommandEventArgs(int.Parse(e.Result.Text)));
            };
            DigitsListener.SetInputToDefaultAudioStream();

            var gb2 = new GrammarBuilder { Culture = Russian };
            gb2.Append(AlwaysOnListenerChoices);
            AlwaysOnListener.LoadGrammar(new Grammar(gb2));
            AlwaysOnListener.SpeechRecognized += (s, e) =>
            {
                if(e.Result.Confidence < 0.70) return;
                switch (e.Result.Text)
                {
                    case "айбанк карты":
                        OnRecognisedMain?.Invoke(this, new MainSpeechCommandEventArgs(MainSpeechCommands.Card));
                            break;

                    case "айбанк команды":
                        OnRecognisedMain?.Invoke(this, new MainSpeechCommandEventArgs(MainSpeechCommands.Team));
                        break;

                    case "айбанк бд":
                    case "айбанк база":
                    case "айбанк база данных":
                        OnRecognisedMain?.Invoke(this, new MainSpeechCommandEventArgs(MainSpeechCommands.Database));
                        break;
                }
            };
            AlwaysOnListener.SetInputToDefaultAudioStream();
        }

        public void Start()
        {
            AlwaysOnListener.RecognizeAsync(RecognizeMode.Multiple);
            DigitsListener.RecognizeAsync(RecognizeMode.Multiple);
            LettersListener.RecognizeAsync(RecognizeMode.Multiple);
            TeamListener.RecognizeAsync(RecognizeMode.Multiple);
            DatabaseListener.RecognizeAsync(RecognizeMode.Multiple);
        }
    }
}
#endif