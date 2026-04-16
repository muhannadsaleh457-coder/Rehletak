using Rehletak.Domain.Exceptions.UnAuthorize;
using Rehletak.Shared.Dtos.Exceptions;

namespace Rehletak.Web.Middlewares
{
    public class HandleExceptions(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                context.Response.StatusCode = ex switch
                {
                    UnAuthorizeException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };


               await context.Response.WriteAsJsonAsync(new ExceptionResponse
                {
                   message = ex.Message,
                   StatusCode = context.Response.StatusCode
               });

            }
        }
    }
}
