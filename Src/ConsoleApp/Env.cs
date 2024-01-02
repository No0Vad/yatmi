using System;

namespace Yatmi.Example;

public static class Env
{
    public static EnvEntity<string> Channel => new("YATMI_CHANNEL", a => a);
    public static EnvEntity<string> DataFolder => new("YATMI_DATA_FOLDER", a => a);


    public class EnvEntity<T>
    {
        public string Name { get; init; }
        public T Value { get; init; }
        public bool Exists { get; init; }

        public EnvEntity(string name, Func<string, T> parser, T defaultValue = default)
        {
            Name = name;

            var value = Environment.GetEnvironmentVariable(name);

            if (value != null)
            {
                Exists = true;
                Value = parser(value);
            }
            else
            {
                Value = defaultValue;
            }
        }
    }
}