using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Integracao
{
    [CustomAuthorize(new string[] { "DownloadArquivosIntegracao", "DownloadArquivosHistoricoIntegracao", "ConsultarHistoricoIntegracao" }, "Integracoes/IntegracaoGhost")]
    public class IntegracaoGhostController : BaseController
    {
		#region Construtores

		public IntegracaoGhostController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGrid();

                Repositorio.Embarcador.Integracao.IntegracaoGhost repositorioIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoGhost(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost filtroPequisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegitrosIntegracao = repositorioIntegracao.ContarConsulta(filtroPequisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Integracao.IntegracaoGhost> integracoes = totalRegitrosIntegracao > 0 ? repositorioIntegracao.Consultar(filtroPequisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Integracao.IntegracaoGhost>();

                grid.setarQuantidadeTotal(totalRegitrosIntegracao);
                grid.AdicionaRows(FormatarRetorno(integracoes));
                
                return new JsonpResult(grid);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu um erro ao processar a consulta");
            }
        }
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivo = repIntegracaoArquivo.BuscarPorCodigo(codigo, false);

                if (arquivo == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                if (arquivo.ArquivoRequisicao == null && arquivo.ArquivoResposta == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.NaoHaRegistrosParaEsseHistorico);

                byte[] arquivos = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivo.ArquivoRequisicao, arquivo.ArquivoResposta });

                return Arquivo(arquivos, "application/zip", $"Integração Ghost {arquivo.Data}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivo = repIntegracaoArquivo.BuscarPorCodigo(codigo, false);

                if (arquivo == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                byte[] arquivos = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivo.ArquivoRequisicao, arquivo.ArquivoResposta }, unitOfWork);

                return Arquivo(arquivos, "application/zip", $"Integração Ghost {arquivo.Data}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoGhost repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoGhost(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao = repIntegracao.BuscarPorCodigo(codigo, false);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoGhost repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoGhost(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao = repIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, false, "Integração não encontrada");
            
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.ProblemaIntegracao = "";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenvio de integração", unitOfWork);

                repIntegracao.Atualizar(integracao);
                

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoGhost()
            {
                SituacaoIntegracao = Request.GetListEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                TipoDestino = Request.GetListEnumParam<TipoDestinoGhost>("TipoDestino"),
                DataInicial = Request.GetNullableDateTimeParam("DataIntegracaoInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataIntegracaoFinal"),
                Chave = Request.GetStringParam("Chave"),
            };
        }

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracaoDescricao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo Destino", "TipoDestinoDescricao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Driver Ticket", "Chave", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Integração", "DataIntegracaoDescricao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 10, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic FormatarRetorno(IList<Dominio.ObjetosDeValor.Embarcador.Integracao.IntegracaoGhost> integracoes)
        {
            return from obj in integracoes
                   select new
                   {
                       obj.Codigo,
                       obj.Retorno,
                       obj.Chave,
                       SituacaoIntegracaoDescricao = obj.SituacaoIntegracao.ObterDescricao(),
                       TipoDestinoDescricao = obj.TipoDestino.ObterDescricao(),
                       DataIntegracaoDescricao = obj.DataIntegracao.ToString("G"),
                       DT_RowColor = obj.SituacaoIntegracao.ObterCorLinha(),
                       DT_FontColor = obj.SituacaoIntegracao.ObterCorFonte(),
                   };
        }

        #endregion
    }
}
