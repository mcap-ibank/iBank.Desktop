using System;

namespace iBank.Desktop.SpeechRecognition
{
    public class CardSpeechCommandEventArgs : EventArgs
    {
        public CardSpeechCommands Command { get; }

        public CardSpeechCommandEventArgs(CardSpeechCommands command) => Command = command;
    }
}