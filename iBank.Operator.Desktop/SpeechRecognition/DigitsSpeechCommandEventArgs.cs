using System;

namespace iBank.Desktop.SpeechRecognition
{
    public class DigitsSpeechCommandEventArgs : EventArgs
    {
        public int Digit { get; }

        public DigitsSpeechCommandEventArgs(int digit) => Digit = digit;
    }
}