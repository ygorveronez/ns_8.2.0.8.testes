using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SGT.WebService.Ecommerce.REST.Base;

namespace SGT.WebService.Ecommerce.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : AbstractControllerBase
    {

        public WeatherForecastController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) : base(memoryCache, configuration, webHostEnvironment, httpContextAccessor)
        {

        }

        [HttpGet("Teste")]
        [Authorize]
        public ActionResult Teste()
        {
            throw new Exception("tesarsar");
            return MResult("erro");
        }

        #region Metodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.GerenciadorApp;
        }

        #endregion Metodos Protegidos
    }
}
