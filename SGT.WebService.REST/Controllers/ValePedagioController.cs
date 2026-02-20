using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ValePedagioController : BaseService
    {
        #region Construtores
        public ValePedagioController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos
        /// <summary>
        /// Integrao os vale pedagio.
        /// </summary>
        /// <param name="valePedagio">Vale Pedagio</param>
        /// <returns></returns>
        [HttpPost("IntegrarValePedagio")]
        public Retorno<bool> IntegrarValePedagio(Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio valePedagio)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.ValePedagio.ValePedadigo(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).IntegraValePedagio(valePedagio, integradora);
            });
        }

        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceValePedagio;
        }

        #endregion
    }
}
