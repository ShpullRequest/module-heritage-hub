using System;

namespace MyApp.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ResourceName { get; } = string.Empty;
        public object Key { get; } = string.Empty;
        public NotFoundException() : base("The requested resource was not found.") { }

        public NotFoundException(string resourceName, object key)
            : base($"The resource '{resourceName}' with key '{key}' was not found.")
        {
            ResourceName = resourceName;
            Key = key;
        }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
