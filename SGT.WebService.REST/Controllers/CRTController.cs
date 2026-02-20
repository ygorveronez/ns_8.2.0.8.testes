using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{

    /// <summary>
    /// API para cadastro e manutenção de CRT/MIC.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CRTController : BaseService
    {
        #region Construtores

        public CRTController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion Construtores

        #region Métodos Publicos

        /// <summary>
        /// Informar CRT por carga.
        /// </summary>
        /// <param name="integracaoCRT">Dados do CRT</param>
        /// <returns></returns>
        [HttpPost("InformarCRTPorCarga")]
        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> InformarCRTPorCarga(Dominio.ObjetosDeValor.WebService.CRT.IntegracaoCRT integracaoCRT)
        {
            return await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.CRT.CRT(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).InformarCRTPorCargaAsync(integracaoCRT);
            });
        }

        #endregion Métodos Publicos

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceCRT;
        }

        #endregion Métodos Protegidos
    }
}
