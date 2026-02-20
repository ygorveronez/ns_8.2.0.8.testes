using Dominio.ObjetosDeValor.Embarcador.ControleEntrega;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class EntregaController : BaseService
    {
        #region Construtores
        public EntregaController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos
        /// <summary>
        ///  Obter entregas por previsões de entrega e quantidade de pacotes.
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("ObterEntregasPorPeriodo")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>> ObterEntregasPorPeriodo(ObterEntregasPorPeriodo dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ObterEntregasPorPeriodo(dadosRequest);
            });
        }


        /// <summary>
        ///  Consulta de detalhes da entrega por protocolo da carga .
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("ConsultarDetalhesEntregaPorProtocoloCarga")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> ConsultarDetalhesEntregaPorProtocoloCarga(int dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConsultarDetalhesEntregaPorProtocoloCarga(dadosRequest);
            });
        }


        /// <summary>
        ///  Atualizar a previsão de entrega.
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("AtualizarPrevisaoEntrega")]
        public Retorno<bool> AtualizarPrevisaoEntrega(AtualizarPrevisaoEntrega dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarPrevisaoEntrega(dadosRequest);
            });
        }

        /// <summary>
        ///  Atualizar a data de carregamento da carga pelo número do pedido.
        /// </summary>
        /// <param name="dadosRequest">Dados para pesquisa</param>
        /// <returns></returns>
        [HttpPost("ConfirmacaoCarregamentoEntregaPedido")]
        public Retorno<bool> ConfirmacaoCarregamentoEntregaPedido(List<ConfirmacaoCarregamentoEntregaPedido> dadosRequest)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).ConfirmacaoCarregamentoEntregaPedido(dadosRequest);
            });
        }

        /// <summary>
        ///  Atualizar data de Agendamento Sugerida.
        /// </summary>
        /// <param name="AgendamentoEntregaPedido">Dados para atualização da Data de Agenadmento Sugerida</param>
        /// <returns></returns>
        [HttpPost("AgendamentoEntregaPedido")]
        public Retorno<bool> AgendamentoEntregaPedido(AgendamentoEntregaPedido agendamentoEntregaPedido)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment))
                    .AgendamentoEntregaPedido(agendamentoEntregaPedido);
            });
        }



        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceEntrega;
        }

        #endregion
    }
}
