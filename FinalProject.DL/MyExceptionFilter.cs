using Domain.Exceptions;
using FinalProject.DL.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.DL
{
    public class MyExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            int statusCode;

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
