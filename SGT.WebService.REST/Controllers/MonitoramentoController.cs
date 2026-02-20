using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de cargas.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class MonitoramentoController : BaseService
    {
        #region Construtores

        public MonitoramentoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Métodos Públicos

        [HttpPost("ConsultaPosicionamentoStatusDispositivo")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores> ConsultaPosicionamentoStatusDispositivo(Dominio.ObjetosDeValor.WebService.Monitoramento.ConsultaPosicionamentoVeiculo consultaPosicionamentoVeiculo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Monitoramento.Monitoramento(unitOfWork).ConsultaPosicionamentoStatusDispositivo(consultaPosicionamentoVeiculo);
            });
        }


        [HttpPost("AdicionarPosicao")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarPosicao(Dominio.ObjetosDeValor.WebService.Rest.Monitoramento.Posicao Posicao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Monitoramento.Monitoramento(unitOfWork).AdicionarNovaPosicao(Posicao);
            });
        }

        #endregion

        #region Métodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceMonitoriamento;
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}