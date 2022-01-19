using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Helpers
{
    public class ExceptionHelper
    {
        public static void Check<T>(bool condition, string message = "Exception catched") where T : Exception
        {
            if (condition)
                throw (T)Activator.CreateInstance(typeof(T), message);
        }
    }
}
