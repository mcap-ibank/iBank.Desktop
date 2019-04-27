using System;

namespace iBank.Operator.Desktop.SpeechRecognition
{
    public class CardSpeechCommandEventArgs : EventArgs
    {
        public CardSpeechCommands Command { get; }

        public CardSpeechCommandEventArgs(CardSpeechCommands command) => Command = command;
    }
}