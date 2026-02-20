using Servicos.DTO;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Middleware
{
    public class RequestInterceptorMiddleware(RequestDelegate next)
    {
        private static bool _isFirstRequestExecuted = false;
        private static readonly object _lock = new object();

        public async Task InvokeAsync(HttpContext context)
        {
            ExecuteOnFirstRequestOnly(context);

            if (context.Request.HasFormContentType && context.Request.Form.Files.Count > 0)
            {
                List<CustomFile> files = new List<CustomFile>();

                foreach (IFormFile formFile in context.Request.Form.Files)
                {
                    var customFile = new CustomFile(formFile.Name, formFile.FileName, formFile.ContentType, formFile.Length, formFile.OpenReadStream());
                    files.Add(customFile);
                }

                context.Items["CustomFiles"] = files;
            }

            await next(context);
        } 

        private static void ExecuteOnFirstRequestOnly(HttpContext context)
        {
            if (!_isFirstRequestExecuted)
            {
                lock (_lock)
                {
                    if (!_isFirstRequestExecuted)
                    {
                        Conexao conexao = context.RequestServices.GetRequiredService<Conexao>();

                        conexao.ConfigureFileStorage();

                        _isFirstRequestExecuted = true;
                    }
                }
            }
        }
    }
}
