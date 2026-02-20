using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para recepção de dados vindos de webhook
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WebhookController : BaseService
    {
        #region Construtores

        public WebhookController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion Construtores

        #region Métodos Publicos

        /// <summary>
        /// Recepção de eventos do Webhook emissor da NSTech
        /// </summary>
        /// <param name="evento">Eventos do emissor de CT-e</param>
        /// <returns></returns>
        [HttpPost("ReceberEventosCTe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ReceberEventosCTe(dynamic evento)
        {
            RetornoEventoCTe retornoEventoCTe = new RetornoEventoCTe();
            retornoEventoCTe.objeto = evento;

            Dominio.ObjetosDeValor.WebService.Retorno<bool> retorno = ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Webhook.Webhook(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ReceberEventoCTe(retornoEventoCTe);
            });

            if (!retorno.Status)
                Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

            return retorno;
        }

        #endregion Métodos Publicos

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceWebhook;
        }

        #endregion Métodos Protegidos
    }
}