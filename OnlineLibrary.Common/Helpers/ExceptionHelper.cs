using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Helpers
{
    public class ExceptionHelper
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
