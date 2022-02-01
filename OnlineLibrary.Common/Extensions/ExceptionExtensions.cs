using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using System;

namespace OnlineLibrary.Common.Extensions
{
    public class ExceptionExtensions
    {
        public static void Check<T>(bool condition, string message = "Exception catched", ExceptionType prop = ExceptionType.InternalServerError) where T : OLException
        {
            if (condition)
                throw (T)Activator.CreateInstance(typeof(T), message, prop);
        }

        public static void Check<T>(bool condition, string message = "Exception catched") where T : Exception
        {
            if (condition)
                throw (T)Activator.CreateInstance(typeof(T), message);
        }
    }
}
