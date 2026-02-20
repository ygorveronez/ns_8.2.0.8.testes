using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IntegradoraIntegracaoRetorno")]
    public class IntegradoraIntegracaoRetornoController : BaseController
    {
		#region Construtores

		public IntegradoraIntegracaoRetornoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracaoRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosIntegracaoRetorno.Count());

                var retorno = (from obj in integracao.ArquivosIntegracaoRetorno.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> DownloadArquivosIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetIntParam("Codigo");

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integradoraIntegracaoRetorno = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(codigo, false);

                if (integradoraIntegracaoRetorno == null)
                    return new JsonpResult(true, false, "Registro não encontrado.");

                if (integradoraIntegracaoRetorno.ArquivoRequisicao == null && integradoraIntegracaoRetorno.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para esta integração.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integradoraIntegracaoRetorno.ArquivoRequisicao, integradoraIntegracaoRetorno.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", $"{integradoraIntegracaoRetorno.NumeroIdentificacao}_{integradoraIntegracaoRetorno.Data:yyyyMMddHHmmss}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigoArquivoRetorno(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo arquivoIntegracao = integracao.ArquivosIntegracaoRetorno.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Retorno " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracaoRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar averbar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Integradora", "Integradora", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Ident. / Carga", "NumeroIdentificacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Checkin", "TipoCheckin", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Sucesso", "Sucesso", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 30, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoUnilever = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Unilever);
                if (tipoIntegracaoUnilever != null)
                {
                    grid.AdicionarCabecalho("Situação Retorno", "DescricaoSituacaoRetorno", 10, Models.Grid.Align.center, false);
                    //grid.AdicionarCabecalho("Mensagem Retorno", "MensagemRetorno", 30, Models.Grid.Align.left, false);
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno filtroPesquisa = ObterFiltroPesquisa();

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                List<Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno> integracoesRetorno = repIntegradoraIntegracaoRetorno.Consultar(filtroPesquisa, ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data), grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repIntegradoraIntegracaoRetorno.ContarConsulta(filtroPesquisa);

                grid.AdicionaRows(integracoesRetorno.Select(o => new
                {
                    o.Codigo,
                    Data = o.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                    NumeroIdentificacao = !string.IsNullOrWhiteSpace(o.NumeroIdentificacao) ? o.NumeroIdentificacao : o.Carga?.CodigoCargaEmbarcador,
                    Integradora = o.Integradora?.Descricao,
                    Sucesso = o.Sucesso ? "Sim" : "Não",
                    Situacao = o.DescricaoSituacao,
                    o.Mensagem,
                    Filial = o.Carga?.Filial?.Descricao ?? string.Empty,
                    TipoOperacao = o.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                    TipoCheckin = o.Carga?.TipoCheckin?.ObterDescricao() ?? string.Empty,
                    o.DescricaoSituacaoRetorno,
                    //o.MensagemRetorno,
                    DT_RowColor = o.Sucesso && o.Situacao == SituacaoIntegracao.Integrado 
                                    ? CorGrid.Verde 
                                    : o.Sucesso && o.Situacao == SituacaoIntegracao.AgIntegracao
                                    ? CorGrid.Azul
                                    : CorGrid.Vermelho,
                    DT_FontColor = o.Sucesso ? "" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco,
                }).ToList()); ;

                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno ObterFiltroPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegradoraIntegracaoRetorno()
            {
                CodigoIntegradora = Request.GetIntParam("Integradora"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                NumeroIdentificacao = Request.GetStringParam("NumeroIdentificacao"),
                Sucesso = Request.GetNullableBoolParam("Sucesso"),
                PossuiCarga = Request.GetNullableBoolParam("PossuiCarga")
            };
        }

        #endregion
    }
}
