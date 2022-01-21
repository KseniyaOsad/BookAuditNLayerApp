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
            switch (context.Exception)
            {
                case OLException exc:
                    context.Result = new ContentResult
                    {
                        Content = exc.ToString(),
                        StatusCode = (int)exc.Property
                    };
                    break;
                default:
                    context.Result = new ContentResult
                    {
                        Content = context.Exception.ToString(),
                        StatusCode = 505
                    };
                    break;
            }
        }
    }
}
