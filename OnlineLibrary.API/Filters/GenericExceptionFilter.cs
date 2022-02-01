using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OnlineLibrary.Common.Exceptions;
using System;

namespace OnlineLibrary.API.Filters
{
    public class GenericExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<Exception> _logger;

        public GenericExceptionFilter(ILogger<Exception> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            HandleException((dynamic)context.Exception, context);
            context.ExceptionHandled = true;
        }

        private void HandleException(Exception exception, ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                Content = exception.ToString(),
                StatusCode = StatusCodes.Status500InternalServerError
            };
            _logger.LogError(exception.ToString());
        }

        private void HandleException(OLException exception, ExceptionContext context)
        {
            context.Result = new ContentResult
            {
                Content = exception.ToString(),
                StatusCode = (int)exception.Property
            };
            _logger.LogWarning(exception.ToString());
        }
    }
}
