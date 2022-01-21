using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineLibrary.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibrary.Common.Filters
{
    public class GenericExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            HandleException((dynamic)context.Exception, context);
            context.ExceptionHandled = true;
        }

        private void HandleException(Exception exception, ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                Content = context.Exception.ToString(),
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        private void HandleException(OLException exception, ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                Content = exception.ToString(),
                StatusCode = (int)exception.Property
            };
        }
    }
}
