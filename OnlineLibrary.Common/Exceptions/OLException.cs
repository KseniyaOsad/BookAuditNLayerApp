using OnlineLibrary.Common.Exceptions.Enum;
using System;

namespace OnlineLibrary.Common.Exceptions
{
    public class OLException : Exception
    {
        public ExceptionType Property { get; protected set; }
        public OLException(string message, ExceptionType prop) : base(message)
        {
            Property = prop;
        }
        public override string ToString()
        {
            return $"Message: {this.Message}, Status Code: {this.Property}" ;
        }
    }
}
