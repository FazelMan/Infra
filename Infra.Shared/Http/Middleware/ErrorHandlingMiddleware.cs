using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Infra.Shared.Enums;
using Infra.Shared.Dtos.Shared;
using Infra.Shared.Exceptions;
using Infra.Shared.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using BadHttpRequestException = Infra.Shared.Exceptions.BadHttpRequestException;

namespace Infra.Shared.Http.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _env;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(
            IHostingEnvironment env,
            ILogger<ErrorHandlingMiddleware> logger,
            RequestDelegate next,
            IServiceProvider serviceProvider)
        {
            _env = env;
            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;

            ErrorDto jsonException;

            switch (exception)
            {
                case UnauthorizedException unauthorizedException:
                    code = HttpStatusCode.Unauthorized;
                    jsonException = new ErrorDto
                    {
                        Code = unauthorizedException.Code,
                        Message = unauthorizedException.Message
                    };
                    break;

                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    jsonException = new ErrorDto
                    {
                        Code = badRequestException.Code,
                        Message = badRequestException.Message
                    };
                    break;

                case PayloadTooLargeException payloadTooLargeException:
                    code = HttpStatusCode.RequestEntityTooLarge;
                    jsonException = new ErrorDto
                    {
                        Code = payloadTooLargeException.Code,
                        Message = payloadTooLargeException.Message
                    };
                    break;

                case Exceptions.BadHttpRequestException badHttpRequestException:
                    code = (HttpStatusCode)typeof(Exceptions.BadHttpRequestException)
                        .GetProperty("StatusCode", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(badHttpRequestException);

                    switch (code)
                    {
                        case HttpStatusCode.RequestEntityTooLarge:
                            code = HttpStatusCode.RequestEntityTooLarge;
                            jsonException = new ErrorDto
                            {
                                Code = badHttpRequestException.Code,
                                Message = badHttpRequestException.Message
                            };
                            break;
                        default:
                            code = HttpStatusCode.BadRequest;
                            jsonException = new ErrorDto
                            {
                                Code = badHttpRequestException.Code,
                                Message = badHttpRequestException.Message
                            };
                            break;
                    }

                    break;

                case ForbiddenException forbiddenException:
                    code = HttpStatusCode.Forbidden;
                    jsonException = new ErrorDto
                    {
                        Code = forbiddenException.Code,
                        Message = forbiddenException.Message
                    };
                    break;

                case NotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    jsonException = new ErrorDto
                    {
                        Message = notFoundException.Message
                    };
                    break;

                case ServiceNotInitializedException serviceNotInitialized:
                    code = HttpStatusCode.NotFound;
                    jsonException = new ErrorDto
                    {
                        Message = serviceNotInitialized.Message
                    };
                    break;

                case InternalServerError internalServerError:
                    code = HttpStatusCode.InternalServerError;
                    if (_env.IsProduction())
                    {
                        jsonException = new ErrorDto
                        {
                            Code = internalServerError.Code,
                            Message = internalServerError.Message
                        };
                    }
                    else
                    {
                        jsonException = new ErrorDto
                        {
                            Code = internalServerError.Code,
                            Message = internalServerError.Message,
                            Extra = internalServerError.StackTrace
                        };
                    }

                    break;
                default:
                {
                    code = HttpStatusCode.InternalServerError;
                    jsonException = new ErrorDto
                    {
                        Message = exception.Message
                    };
                }
                    break;
            }

            _logger.LogError(exception.Message, exception.StackTrace);
            _logger.LogError(JsonConvert.SerializeObject(exception));

            bool.TryParse(Host.Config["ShowDeveloperException"], out var showDeveloperException);
            if (showDeveloperException == false && !_env.IsDevelopment())
            {
                jsonException = new ErrorDto
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "خطای سرور. لطفا با پشتیبانی سایت تماس بگیرید."
                };
            }

            var result = JsonConvert.SerializeObject(
                jsonException,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)code;

            await context.Response.WriteAsync(result);
        }
    }
}