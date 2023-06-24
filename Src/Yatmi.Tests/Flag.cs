namespace Yatmi.Tests;

public class Flag
{
    private readonly AutoResetEvent _wait;
    private readonly int _target;
    private int _counter;


    /// <summary>
    /// Creates a new instance of <see cref="Flag"/>
    /// </summary>
    /// <param name="target">How many times <see cref="Set"/> must be called</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when target is lower than 1</exception>
    public Flag(int target = 1)
    {
        if (target < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(target), "Value cannot be lower than 1");
        }

        _wait = new AutoResetEvent(false);
        _target = target;
    }


    /// <summary>
    /// Sets the flag
    /// </summary>
    public void Set()
    {
        if (++_counter == _target)
        {
            _wait.Set();
        }
    }


    /// <summary>
    /// Waits for 500 ms until all the flags has been set.
    /// </summary>
    public bool Wait() => _wait.WaitOne(500);
}
