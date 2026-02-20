using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace SGT.WebService.Ecommerce.REST.Configuracao
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = new ObjectResult("Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.")
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}