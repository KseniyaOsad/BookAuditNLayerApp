using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineLibrary.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Filters
{
    public class OLExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Exception exc = context.Exception;
            var olException = exc as OLException;

            context.Result = new ContentResult
            {
                Content = exc.ToString(),
                StatusCode = olException == null ? 500 : (int)olException.Property
            };
        }
    }
}
