using System;

namespace iBank.Operator.Desktop.SpeechRecognition
{
    public class DigitsSpeechCommandEventArgs : EventArgs
    {
        public int Digit { get; }

        public DigitsSpeechCommandEventArgs(int digit) => Digit = digit;
    }
}