using System;

namespace LogComponent
{
    public class Log
    {
        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public Log(string text, DateTime timeStamp)
        {
            Text = text;
            Timestamp = timeStamp;
        }
    }
}