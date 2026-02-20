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
    public class FilialController : BaseService
    {
        #region Construtores
        public FilialController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Metodos Publicos
        /// <summary>
        /// Confirma as Filiais Integradas
        /// </summary>
        /// <param name="protocolos">Lista dos Protocolos das Filiais integradas</param>
        /// <returns></returns>
        [HttpPost("ConfirmarIntegracaoFilial")]
        public Retorno<bool> ConfirmarIntegracaoFilial(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache,_configuration,_webHostEnvironment)).ConfirmarIntegracaoFilial(protocolos);
            });
        }

        /// <summary>
        /// Retorna todas a filiais que estão pendentes de integração
        /// </summary>
        /// <param name="quantidade">Quantidade de registros que deseja processar(Opcional). Por padrão ja é 50</param>
        /// <returns></returns>
        [HttpPost("BuscarFiliaisPendentesIntegracao")]
        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> BuscarFiliaisPendentesIntegracao(int quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).BuscarFiliaisPendentesIntegracao(quantidade);
            });
        }

        /// <summary>
        /// Cria o tanque caso não exista e vincula a filial ao tanque
        /// </summary>
        /// <param name="filialTanque">Objeto com as informações do tanque e o vinculo com a filial</param>
        /// <returns></returns>
        [HttpPost("InformarVolumesTanques")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarVolumesTanques(Dominio.ObjetosDeValor.WebService.Filial.FilialTanque filialTanque)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).InformarVolumesTanques(filialTanque);
            });
        }

        /// <summary>
        /// Cria o ProdutoEmbarcadorEstoqueArmazem caso já exista atualiza
        /// </summary>
        /// <param name="salvarEstoqueProdutoArmazem">Objeto com as informações do ProdutoEmbarcadorEstoqueArmazem</param>
        /// <returns></returns>
        [HttpPost("SalvarEstoqueProdutoArmazem")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarEstoqueProdutoArmazem(Dominio.ObjetosDeValor.WebService.Filial.ProdutoEmbarcadorEstoqueArmazem salvarEstoqueProdutoArmazem)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).SalvarEstoqueProdutoArmazem(salvarEstoqueProdutoArmazem);
            });
        }

        #endregion


        #region Métodos Protegidos
        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceFilial;
        }

        #endregion
    }
}
