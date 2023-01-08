using Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace Domain
{
    public class MyExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public MyExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            int statusCode;
            _logger.Error(exception, "An exception occurred while processing a request");
            switch (true)
            {
                case bool _ when exception is RecordNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;


                case bool _ when exception is UnauthorizedUserException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case bool _ when exception is AlreadyExistingRecordException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            context.Result = new ObjectResult(exception.Message)
            {
                StatusCode = statusCode
            };
        }
    }
}
