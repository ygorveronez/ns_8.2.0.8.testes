using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.Frete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// Endpoints Tabela Frete
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FreteController : BaseService
    {
        #region Construtores
        public FreteController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Buscar Documento Entrada Pendentes
        /// </summary>
        /// <param name="request">Lista de Objetos Requisição</param>
        /// <returns></returns>
        /// 
        [HttpPost("RetornoContratoTransportador")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoContratoTransportador(List<RetornoContratoTransportador> request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Frete.Frete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoContratoTransportador(request);
            });
        }

        /// <summary>
        /// Retorno para atualizar o status da tabela
        /// </summary>
        /// <param name="request">Lista de Objetos Requisição</param>
        /// <returns></returns>
        /// 
        [HttpPost("RetornoAprovacaoTabela")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoAprovacaoTabela(List<RetornoTabelaFrete> request)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Frete.Frete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoAprovacaoTabela(request, integradora);
            });
        }

        /// <summary>
        /// Calcular Frete
        /// </summary>
        /// <param name="dadosCalculoFrete">Dados Calculo Frete</param>
        /// <returns></returns>
        /// 
        [HttpPost("CalcularFrete")]
        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>> CalcularFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.Embarcador.Frete.CalculoFrete(unitOfWork, TipoServicoMultisoftware).CalcularFreteVariacao(dadosCalculoFrete);
            });
        }

        /// <summary>
        /// Informar Valor do Frete Operador
        /// </summary>
        /// <param name="ProtocoloIntegracaoCarga">Protocolo Integração Carga</param>
        /// <param name="ValorFreteOperador">Valor Frete Operador</param>
        /// <returns></returns>
        /// 
        [HttpPost("InformarValorFreteOperador")]
        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> InformarValorFreteOperador(int ProtocoloIntegracaoCarga, decimal ValorFreteOperador)
        {
            return await ProcessarRequisicaoAsync<bool>(async (Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) =>
            {
                return await new Servicos.WebService.Frete.Frete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).RetornoValorFreteOperador(ProtocoloIntegracaoCarga, ValorFreteOperador);
            });
        }
        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceFretes;
        }

        #endregion
    }
}