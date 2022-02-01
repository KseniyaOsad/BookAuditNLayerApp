using log4net;
using Microsoft.Extensions.Logging;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Common.Exceptions.Enum;
using System;

namespace OnlineLibrary.Common.Extensions
{
    public class ExceptionExtensions
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ExceptionExtensions));

        public static void Check<T>(bool condition, string message = "Exception catched", ExceptionType prop = ExceptionType.InternalServerError) where T : OLException
        {
            if (condition)
            {
                T exception = (T)Activator.CreateInstance(typeof(T), message, prop);
                _logger.Error(exception.ToString());
                throw exception;
            }
        }

        public static void Check<T>(bool condition, string message = "Exception catched") where T : Exception
        {
            if (condition)
            {
                T exception = (T)Activator.CreateInstance(typeof(T), message);
                _logger.Error(exception.ToString());
                throw exception;

            }
        }
    }
}
