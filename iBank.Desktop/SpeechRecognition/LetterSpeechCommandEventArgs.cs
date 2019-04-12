using System;

namespace iBank.Desktop.SpeechRecognition
{
    public class LetterSpeechCommandEventArgs : EventArgs
    {
        public char Letter { get; }

        public LetterSpeechCommandEventArgs(char letter) => Letter = letter;
    }
}