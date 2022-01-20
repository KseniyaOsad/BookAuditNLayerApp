using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Exceptions.Enum
{
    public enum ExceptionType
    {
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500
    }
}
