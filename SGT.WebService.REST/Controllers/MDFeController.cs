using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.MDFe;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de MDFe.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]


    public class MDFeController : BaseService
    {
        #region Constructores
        public MDFeController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion


        #region Enpoints

        /// <summary>
        /// Busca MDFe.
        /// </summary>
        /// <param name="requestMDFe"></param>
        /// <returns></returns>
        [HttpPost("BuscarMDFes")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.MDFe.MDFe>> BuscarMDFes(RequestMDFe requestMDFe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.MDFe.MDFe(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado).BuscarMDFes(requestMDFe);
            });
        }
        #endregion



        #region Metodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceMDFe;
        }
        #endregion

    }
}
