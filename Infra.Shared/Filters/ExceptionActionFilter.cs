using System;
using System.Net;
using Infra.Shared.Exceptions;
using Infra.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infra.Shared.Filters
{
    public class ExceptionActionFilter : ExceptionFilterAttribute
    {
        public ExceptionActionFilter()
        {
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception != null && isTypeOfAppException(context.Exception))
            {
                var appException = (BadRequestException)context.Exception;
                handleAppException(context, appException);
            }
            else if (context.Exception != null && context.Exception.InnerException != null && isTypeOfAppException(context.Exception.InnerException))
            {
                var appException = (BadRequestException)context.Exception.InnerException;
                handleAppException(context, appException);
            }
            else
            {
                base.OnException(context);
            }
        }

        private void handleAppException(ExceptionContext context, BadRequestException appException)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.HttpContext.Response.ContentType = "application/json";

            context.Result = new JsonResult(new ExceptionResponse()
            {
                Message = appException.Message,
                Result = appException.ResultData
            });
        }

        private bool isTypeOfAppException(Exception exception)
        {
            return exception.GetType() == typeof(BadRequestException) || exception.GetType().IsSubclassOf(typeof(BadRequestException));
        }
    }
}
