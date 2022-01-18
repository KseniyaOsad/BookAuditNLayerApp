using System;

namespace OnlineLibrary.BLL.Infrastructure
{
    public class ValidationException : Exception
    {
        public Enum Property { get; protected set; }
        public ValidationException(string message, Enum prop) : base(message)
        {
            Property = prop;
        }
    }
}
