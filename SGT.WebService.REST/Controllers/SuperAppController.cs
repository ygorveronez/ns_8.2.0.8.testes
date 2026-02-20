using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para Webhooks SuperApp Trizy.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SuperAppController : BaseService
    {
        #region Construtores

        public SuperAppController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Processamento de Eventos do SuperApp da Trizy.
        /// </summary>
        /// <param name="eventoSuperApp">Objeto do Evento Trizy que será processado.</param>
        /// <returns></returns>
        [HttpPost("EventReceiver")]
        public async Task<Retorno<bool>> EventReceiver(Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.EventoSuperApp eventoSuperApp)
        {
            Retorno<bool> resposta = await ProcessarRequisicaoAsync(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.SuperApp.SuperApp(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                    .ProcessarEventoSuperAppAsync(eventoSuperApp, cancellationToken, Auditado);
            });

            HttpContext.Response.StatusCode = resposta.CodigoMensagem;
            return resposta;
        }

        /// <summary>
        /// Processamento de Posições do SuperApp da Trizy.
        /// </summary>
        /// <param name="eventoSendPosition">Objeto do Evento Trizy que será processado.</param>
        /// <returns></returns>
        [HttpPost("PositionReceiver")]
        public Retorno<bool> PositionReceiver(Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.EventoSendPosition eventoSendPosition)
        {
            Retorno<bool> resposta = ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.SuperApp.SuperApp(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                    .ProcessarPosicaoSuperApp(eventoSendPosition);
            });
            this.HttpContext.Response.StatusCode = resposta.CodigoMensagem;
            return resposta;
        }

        #endregion Métodos Públicos

        #region Metodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceSuperApp;
        }

        #endregion Metodos Protegidos
    }
}