namespace Yatmi.Entities.EventArgs
{
    public class LogEventArgs : TimestampEventArgs
    {
        public string Message { get; }

        public LogEventArgs(string message)
        {
            Message = message;
        }
    }
}
