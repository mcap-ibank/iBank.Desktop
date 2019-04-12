using System;

namespace iBank.Desktop.SpeechRecognition
{
    public class MainSpeechCommandEventArgs : EventArgs
    {
        public MainSpeechCommands Command { get; }

        public MainSpeechCommandEventArgs(MainSpeechCommands command) => Command = command;
    }
}