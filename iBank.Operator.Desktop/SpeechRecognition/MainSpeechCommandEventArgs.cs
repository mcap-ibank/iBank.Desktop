using System;

namespace iBank.Operator.Desktop.SpeechRecognition
{
    public class MainSpeechCommandEventArgs : EventArgs
    {
        public MainSpeechCommands Command { get; }

        public MainSpeechCommandEventArgs(MainSpeechCommands command) => Command = command;
    }
}