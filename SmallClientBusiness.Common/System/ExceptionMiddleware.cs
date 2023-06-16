using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.System
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (IncorrectDataException e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new ErrorResponse { Status = "400", Message = e.Message });
            }
            catch (ItemNotFoundException e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new ErrorResponse { Status = "404", Message = e.Message });
            }
            catch (NoPermissionException e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new ErrorResponse { Status = "403", Message = e.Message });
            }
            catch (FailedLoadAvatarException e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new ErrorResponse { Status = "400", Message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.ToString());
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new ErrorResponse { Status = "500", Message = e.Message });
            }
        }
    }
}
