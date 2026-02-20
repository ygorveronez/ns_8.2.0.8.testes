using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.CargaEntregaEventoIntegracao
{
    [CustomAuthorize("Cargas/CargaEntregaEventoIntegracao")]
    public class CargaEntregaEventoIntegracaoController : BaseController
    {
		#region Construtores

		public CargaEntregaEventoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = ObterGridPesquisa(unitOfWork);
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repCargaEntregaEventoIntegracao.BuscarPorCodigo(codigo, auditavel: true);

                if (pedidoOcorrenciaColetaEntregaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left);

                var arquivosRetornar = (
                    from arquivoTransacao in pedidoOcorrenciaColetaEntregaIntegracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosRetornar);
                grid.setarQuantidadeTotal(arquivosRetornar.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico de integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repCargaEntregaEventoIntegracao.BuscarPorCodigo(codigo, auditavel: true);
                if (pedidoOcorrenciaColetaEntregaIntegracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                if (pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Apenas integrações com problema podem ser reenviadas.");

                pedidoOcorrenciaColetaEntregaIntegracao.Initialize();

                Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventoIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
                await servicoCargaEntregaEventoIntegracao.ProcessarIntegracaoPendenteAsync(pedidoOcorrenciaColetaEntregaIntegracao.Codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoOcorrenciaColetaEntregaIntegracao, pedidoOcorrenciaColetaEntregaIntegracao.GetChanges(), "Solicitou o reenvio da integração.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodasIntegracoesComFalha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repositorioCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);

                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");

                if (!dataInicial.HasValue || !dataFinal.HasValue)
                    return new JsonpResult(false, true, "É preciso informar os filtros de data e a diferença entre as datas precisa ser de, no máximo, 1 dia.");

                if ((dataFinal.Value.Date - dataInicial.Value.Date).TotalDays > 1)
                    return new JsonpResult(false, true, "As datas precisam ter, no máximo, 1 dia de diferença.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> listaOcorrenciasComFalha = repositorioCargaEntregaEventoIntegracao.BuscarIntegracoesPorSituacaoEData(SituacaoIntegracao.ProblemaIntegracao, dataInicial.Value, dataFinal.Value);

                if (listaOcorrenciasComFalha.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma integração com falha foi encontrada.");

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao pedidoOcorrenciaColetaEntregaIntegracao in listaOcorrenciasComFalha)
                {
                    pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    repositorioCargaEntregaEventoIntegracao.Atualizar(pedidoOcorrenciaColetaEntregaIntegracao, Auditado, null, "Clicou em ReenviarTodasIntegracoesComFalha");
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar as integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repCargaEntregaEventoIntegracao.BuscarPorCodigoArquivo(codigo);

                if (pedidoOcorrenciaColetaEntregaIntegracao == null)
                    return new JsonpResult(false, "Histórico de integração não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = pedidoOcorrenciaColetaEntregaIntegracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();
                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });
                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {pedidoOcorrenciaColetaEntregaIntegracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoSituacaoIntegracao", false);
            grid.AdicionarCabecalho("CodigoTipoDeOcorrencia", false);
            grid.AdicionarCabecalho("CodigoTipoIntegracao", false);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Entrega", "ClienteEntrega", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de ocorrência", "DescricaoTipoDeOcorrencia", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integradora", "DescricaoTipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 20, Models.Grid.Align.left, false);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaCargaEntregaEventoIntegracao filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
            int totalRegistros = repCargaEntregaEventoIntegracao.ContarConsulta(filtroPesquisa);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao> lista = totalRegistros > 0 ? repCargaEntregaEventoIntegracao.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao>();

            var listaRetornar = (
                from row in lista
                select new
                {
                    row.Codigo,
                    CodigoSituacaoIntegracao = (int)row.SituacaoIntegracao,
                    CodigoTipoDeOcorrencia = row.CargaEntregaEvento.TipoDeOcorrencia.Codigo,
                    CodigoTipoIntegracao = row.TipoIntegracao.Codigo,
                    CodigoCargaEmbarcador = row.CargaEntregaEvento.Carga.CodigoCargaEmbarcador,
                    ClienteEntrega = row.CargaEntregaEvento.CargaEntrega?.Cliente?.CPF_CNPJ_SemFormato,
                    DescricaoTipoDeOcorrencia = row.CargaEntregaEvento.TipoDeOcorrencia.Descricao,
                    Transportador = row.CargaEntregaEvento.Carga.Empresa?.Descricao,
                    DescricaoTipoIntegracao = row.TipoIntegracao.Descricao,
                    DataIntegracao = row.DataIntegracao.ToDateTimeString(),
                    NumeroTentativas = row.NumeroTentativas,
                    SituacaoIntegracao = row.DescricaoSituacaoIntegracao,
                    ProblemaIntegracao = row.ProblemaIntegracao,
                    DT_RowColor = row.SituacaoIntegracao.ObterCorLinha(),
                    DT_FontColor = row.SituacaoIntegracao.ObterCorFonte()
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaCargaEntregaEventoIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaCargaEntregaEventoIntegracao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoTipoDeOcorrencia = Request.GetIntParam("TipoDeOcorrencia"),
                CodigoTipoIntegracao = Request.GetIntParam("TipoIntegracao"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao")
            };
        }

        #endregion
    }
}
