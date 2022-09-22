using System;

namespace Yatmi.Core
{
    internal class Refresher
    {
        private readonly TimeSpan _resetAfter;
        private readonly int _start;
        private readonly int _max;
        private int _value;
        private DateTime _lastReset;

        /// <summary>
        /// Creates a new instance of <see cref="Refresher"/>
        /// </summary>
        /// <param name="resetAfter">Time after which the value is refreshed back to value of <paramref name="start"/></param>
        /// <param name="start">The starting value</param>
        /// <param name="max">Maximum value</param>
        public Refresher(TimeSpan resetAfter, int start = 0, int max = 1)
        {
            _resetAfter = resetAfter;
            _start = start;
            _value = start;
            _max = max;
            _lastReset = DateTime.Now;
        }


        /// <summary>
        /// Value is refreshed when specified time has passed
        /// </summary>
        public int GetAndIncrease()
        {
            var now = DateTime.Now;

            if (now - _lastReset > _resetAfter)
            {
                _value = _start;
                _lastReset = now;
            }

            if (_value > _max)
            {
                _value = _max;
            }

            return _value++;
        }
    }
}