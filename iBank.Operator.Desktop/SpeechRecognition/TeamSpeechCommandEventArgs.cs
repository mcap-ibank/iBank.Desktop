using System;

namespace iBank.Operator.Desktop.SpeechRecognition
{
    public class TeamSpeechCommandEventArgs : EventArgs
    {
        public string Command { get; }

        public TeamSpeechCommandEventArgs(string command) => Command = command;
        /*
        public TeamSpeechCommands Command { get; }

        public TeamSpeechCommandEventArgs(TeamSpeechCommands command) => Command = command;
        */
    }
}