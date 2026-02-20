using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de frotas.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FrotaController : BaseService
    {
        #region Construtores

        public FrotaController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adicionar Multa.
        /// </summary>
        /// <param name="multaIntegracao">Multa que será adicionada.</param>
        /// <returns></returns>
        [HttpPost("AdicionarMulta")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarMulta(Dominio.ObjetosDeValor.WebService.Frota.Multa multaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Frota.Infracao(unitOfWork).AdicionarMulta(multaIntegracao, Auditado);
            });
        }

        /// <summary>
        /// Adicionar Notificação.
        /// </summary>
        /// <param name="notificacaoIntegracao">Notificação que será adicionada.</param>
        /// <returns></returns>
        [HttpPost("AdicionarNotificacao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarNotificacao(Dominio.ObjetosDeValor.WebService.Frota.Notificacao notificacaoIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Frota.Infracao(unitOfWork).AdicionarNotificacao(notificacaoIntegracao, Auditado);
            });
        }

        #endregion Métodos Públicos

        #region Metodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceFrota;
        }

        #endregion Metodos Protegidos
    }
}
