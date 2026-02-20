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
    public class ProdutoController : BaseService
    {
        #region Construtores
        public ProdutoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion


        #region Metodos Publicos
        /// <summary>
        /// Integrar PO Data
        /// </summary>
        /// <param name="ordeCompra">Ordem de Compra</param>
        /// <returns></returns>
        [HttpPost("IntegrarPOData")]
        public Retorno<bool> IntegrarPOData(Dominio.ObjetosDeValor.Embarcador.Produtos.OrdemDeCompra ordeCompra)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Produtos.Produto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).IntegrarPOData(ordeCompra);
            });
        }

        /// <summary>
        /// Integrar History Data
        /// </summary>
        /// <param name="historialPrincipal">Historial de integração</param>
        /// <returns></returns>
        [HttpPost("IntegrarHistoryData")]
        public Retorno<bool> IntegrarHistoryData(Dominio.ObjetosDeValor.Embarcador.Produtos.HistorioalOrdemPrincipal historialPrincipal)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Produtos.Produto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).IntegrarHistoryData(historialPrincipal);
            });
        }


        #endregion

        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceProduto;
        }

        #endregion
    }
}
