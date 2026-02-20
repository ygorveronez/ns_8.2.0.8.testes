using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Exportacao
{
    [CustomAuthorize("Cargas/CargaExportacaoIntegracao")]
    public class CargaExportacaoIntegracaoController : BaseController
    {
		#region Construtores

		public CargaExportacaoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao integracao = repositorioIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {integracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();
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
        public async Task<IActionResult> PesquisaHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao integracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                var arquivosTransacaoRetornar = (
                    from arquivoTransacao in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosTransacaoRetornar);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao cargaExportacaoIntegracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: true);

                if (cargaExportacaoIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoExportacaoMarfrig(unitOfWork).IntegrarCargaExportacao(cargaExportacaoIntegracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
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

        #endregion

        #region Métodos Privados

        private dynamic ObterCargaExportacaoIntegracao(Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao cargaExportacaoIntegracao, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, List<(int CodigoCarga, string NumeroEXP)> listaNumeroEXP)
        {
            int codigoCargaFiltrarJanelaCarregamento = cargaExportacaoIntegracao.Carga.CargaAgrupamento?.Codigo ?? cargaExportacaoIntegracao.Carga.Codigo;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();
            string numeroEXP = string.Join(", ", (from o in listaNumeroEXP where o.CodigoCarga == codigoCargaFiltrarJanelaCarregamento select o.NumeroEXP));

            return new
            {
                cargaExportacaoIntegracao.Codigo,
                cargaExportacaoIntegracao.Carga.CodigoCargaEmbarcador,
                cargaExportacaoIntegracao.Numero,
                NumeroEXP = numeroEXP,
                Filial = cargaExportacaoIntegracao.Carga.Filial?.Descricao ?? "",
                Veiculo = (cargaExportacaoIntegracao.Carga.CargaAgrupamento == null) ? cargaExportacaoIntegracao.Carga.RetornarPlacas : cargaExportacaoIntegracao.Carga.CargaAgrupamento.RetornarPlacas,
                InicioCarregamento = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? "",
                SituacaoIntegracao = cargaExportacaoIntegracao.DescricaoSituacaoIntegracao,
                ProblemaIntegracao = cargaExportacaoIntegracao.ProblemaIntegracao,
                NumeroTentativas = cargaExportacaoIntegracao.NumeroTentativas,
                DataIntegracao = cargaExportacaoIntegracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                DT_RowColor = cargaExportacaoIntegracao.SituacaoIntegracao.ObterCorLinha(),
                DT_FontColor = cargaExportacaoIntegracao.SituacaoIntegracao.ObterCorFonte(),
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetListParam<int>("Filial"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                DataInicialAgendamento = Request.GetNullableDateTimeParam("DataInicialAgendamento"),
                DataLimiteAgendamento = Request.GetNullableDateTimeParam("DataLimiteAgendamento"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao"),
                Transportadores = Request.GetListParam<int>("Transportador")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número EXP", "NumeroEXP", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Filial", "Filial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data do Agendamento", "InicioCarregamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorio = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao> listaCargaExportacaoIntegracao;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento;
                List<(int CodigoCarga, string NumeroEXP)> listaNumeroEXP;

                if (totalRegistros > 0)
                {
                    listaCargaExportacaoIntegracao = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                    List<int> codigosCargas = (from o in listaCargaExportacaoIntegracao select o.Carga.CargaAgrupamento?.Codigo ?? o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                    listaNumeroEXP = repositorioCargaPedido.BuscarNumerosEXPPorCarga(codigosCargas);
                }
                else
                {
                    listaCargaExportacaoIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                    listaNumeroEXP = new List<(int CodigoCarga, string NumeroEXP)>();
                }

                var listaCargaExportacaoIntegracaoRetornar = (
                   from cargaExportacaoIntegracao in listaCargaExportacaoIntegracao
                   select ObterCargaExportacaoIntegracao(cargaExportacaoIntegracao, listaCargaJanelaCarregamento, listaNumeroEXP)
               ).ToList();

                grid.AdicionaRows(listaCargaExportacaoIntegracaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Filial")
                return "Carga.Filial.Descricao";

            if (propriedadeOrdenar == "InicioCarregamento")
                return "Carga.JanelaCarregamento.InicioCarregamento";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
