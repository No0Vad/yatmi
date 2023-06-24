using System;

namespace Yatmi.Entities.EventArgs;

public class ExceptionEventArgs : TimestampEventArgs
{
    /// <summary>
    /// The caught exception
    /// </summary>
    public Exception Exception { get; }


    public ExceptionEventArgs(Exception exception)
    {
        Exception = exception;
    }
}