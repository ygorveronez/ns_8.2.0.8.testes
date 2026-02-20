using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Abastecimento;
using Dominio.ObjetosDeValor.WebService.Carga;
using Dominio.ObjetosDeValor.WebService.Rest;
using Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para Atendimentos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AbastecimentoController : BaseService
    {
        #region Construtores

        public AbastecimentoController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }

        #endregion

        #region Métodos Públicos

        [HttpPost("Conecttec/V3/FuelPoint/Status")]
        public Retorno<bool> AtualizarStatusAbastecimentoConecttec(Dominio.ObjetosDeValor.WebService.Abastecimento.AtualizarStatusAbastecimentoConecttec atualizarStatusAbastecimentoConecttec)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);

                Dominio.Entidades.WebService.Integradora integradora = repositorioIntegradora.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Conecttec);

                if (integradora == null || !(integradora?.Ativo ?? false))
                    throw new WebServiceException("Não autorizado. Token inválido.");

                return new Servicos.WebService.Abastecimento.Abastecimento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).AtualizarStatusAbastecimentoConecttec(atualizarStatusAbastecimentoConecttec);

            }
            catch (BaseException ex)
            {
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
        }

        [HttpPost("Conecttec/FuelPoint/Delivery")]
        public Retorno<bool> FinalizarAbastecimentoConecttec(Dominio.ObjetosDeValor.WebService.Abastecimento.FinalizarAbastecimentoConecttec finalizarAbastecimentoConecttec)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(Request, _memoryCache, _configuration, _webHostEnvironment), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);

                Dominio.Entidades.WebService.Integradora integradora = repositorioIntegradora.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Conecttec);

                if (integradora == null || !(integradora?.Ativo ?? false))
                    throw new WebServiceException("Não autorizado. Token inválido.");

                return new Servicos.WebService.Abastecimento.Abastecimento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.AdminStringConexao(_memoryCache, _configuration, _webHostEnvironment)).FinalizarAbastecimentoConecttec(finalizarAbastecimentoConecttec);

            }
            catch (BaseException ex)
            {
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
            finally
            {
                if (unitOfWork != null)
                    unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Metodos Protegidos

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceAbastecimento;
        }

        #endregion Metodos Protegidos
    }
}
