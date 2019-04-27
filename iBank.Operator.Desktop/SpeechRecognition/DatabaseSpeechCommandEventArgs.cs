using System;

namespace iBank.Operator.Desktop.SpeechRecognition
{
    public class DatabaseSpeechCommandEventArgs : EventArgs
    {
        public string Command { get; }

        public DatabaseSpeechCommandEventArgs(string command) => Command = command;
        /*
        public DatabaseSpeechCommands Command { get; }

        public DatabaseSpeechCommandEventArgs(DatabaseSpeechCommands command) => Command = command;
        */
    }
}