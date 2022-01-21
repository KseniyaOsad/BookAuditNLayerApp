using Microsoft.AspNetCore.Http;
using OnlineLibrary.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineLibrary.Common.ErrorMiddleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (ex)
                {
                    case OLException e:
                        response.StatusCode = (int)e.Property;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var result = JsonSerializer.Serialize(new { StatusCode = response.StatusCode, message = ex?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
