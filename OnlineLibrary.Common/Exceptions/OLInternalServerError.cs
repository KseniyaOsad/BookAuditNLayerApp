using OnlineLibrary.Common.Exceptions.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Exceptions
{
    public class OLInternalServerError: OLException
    {
        public OLInternalServerError(string message) : base(message)
        {
            Property = ExceptionType.InternalServerError;
        }
    }
}
