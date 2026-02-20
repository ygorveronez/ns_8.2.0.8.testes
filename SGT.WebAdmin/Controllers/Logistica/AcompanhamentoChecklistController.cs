using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Logistica.AcompanhamentoChecklist;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AcompanhamentoChecklist")]
    public class AcompanhamentoChecklistController : BaseController
    {
        #region Construtores

        public AcompanhamentoChecklistController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Metodos Publicos

        public async Task<IActionResult> Pesquisar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Grid grid = await ObterGridPesquisaAsync(unitOfWork, cancellationToken);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisarDetalhes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamentoTransportador = Request.GetIntParam("CodigoJanelaCarregamentoTransportador");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Grid gridDetalhes = await ObterGridPesquisaDetalhesAsync(unitOfWork, cancellationToken, codigoJanelaCarregamentoTransportador, codigoCarga);
                return new JsonpResult(gridDetalhes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Grid grid = await ObterGridPesquisaAsync(unitOfWork, cancellationToken);

                List<dynamic> linhasCombinadas = new List<dynamic>();

                List<dynamic> dadosPrincipais = grid.ObterDataRows();

                foreach (dynamic row in dadosPrincipais)
                {
                    int codigoJanelaCarregamentoTransportador = Convert.ToInt32(row.Codigo);
                    int codigoCarga = Convert.ToInt32(row.CodigoCarga);

                    Grid gridDetalhes = await ObterGridPesquisaDetalhesAsync(unitOfWork, cancellationToken, codigoJanelaCarregamentoTransportador, codigoCarga);
                    List<dynamic> dadosDetalhes = gridDetalhes.ObterDataRows();

                    foreach (dynamic detalhe in dadosDetalhes)
                    {
                        dynamic linhaCombinada = new
                        {
                            row.Carga,
                            row.Filial,
                            row.Transportador,
                            row.SituacaoFormatada,
                            row.Veiculos,
                            detalhe.OrdemChecklistFormatado,
                            detalhe.Placa,
                            detalhe.Produto,
                            detalhe.RegimeLimpezaCargaFormatado
                        };
                        linhasCombinadas.Add(linhaCombinada);
                    }
                }

                grid.AdicionaRows(linhasCombinadas);

                List<Head> novosCabecalhos = grid.header.ToList();
                if (!novosCabecalhos.Any(h => h.data == "OrdemChecklistFormatado"))
                    novosCabecalhos.Add(new Head { title = "Ordem do Checklist", visible = true, data = "OrdemChecklistFormatado", className = "text-start", width = "15" });
                if (!novosCabecalhos.Any(h => h.data == "Placa"))
                    novosCabecalhos.Add(new Head { title = "Placa", visible = true, data = "Placa", className = "text-start", width = "10" });
                if (!novosCabecalhos.Any(h => h.data == "Produto"))
                    novosCabecalhos.Add(new Head { title = "Produto", visible = true, data = "Produto", className = "text-start", width = "20" });
                if (!novosCabecalhos.Any(h => h.data == "RegimeLimpezaCargaFormatado"))
                    novosCabecalhos.Add(new Head { title = "Regime de Limpeza", visible = true, data = "RegimeLimpezaCargaFormatado", className = "text-start", width = "20" });

                grid.header = novosCabecalhos;
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

                return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarVisualizacaoChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

                int codigoCargaJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamentoTransportador");
                bool situacaoChecklist = Request.GetBoolParam("Situacao");

                if (codigoCargaJanelaCarregamento > 0 && !situacaoChecklist)
                    repCargaJanelaCarregamentoTransportador.MarcarChecklistComoVisualizado(codigoCargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist()
            {
                Filial = Request.GetIntParam("Filial"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                DataCarregamento = Request.GetDateTimeParam("DataCarregamento"),
                Transportador = Request.GetIntParam("Transportador"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
                CodigosCarga = Request.GetListParam<int>("Carga")
            };
        }

        private Grid ObterGrid()
        {
            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("CodigoCarga", false);
            grid.AdicionarCabecalho("Carga", "Carga", 5, Align.left, false);
            grid.AdicionarCabecalho("Filial", "Filial", 15, Align.left, false);
            grid.AdicionarCabecalho("Transportador ", "Transportador", 20, Align.left, false);
            grid.AdicionarCabecalho("Situação", "SituacaoFormatada", 5, Align.center, false);
            grid.AdicionarCabecalho("Veículos", "Veiculos", 10, Align.left, false);

            return grid;
        }

        private Grid ObterGridDetalhesChecklist()
        {
            Grid gridDetalhes = new Grid(Request)
            {
                header = new List<Head>()
            };

            gridDetalhes.AdicionarCabecalho("Ordem do Checklist", "OrdemChecklistFormatado", 5, Align.left, false);
            gridDetalhes.AdicionarCabecalho("Placa", "Placa", 10, Align.left, false);
            gridDetalhes.AdicionarCabecalho("Produto", "Produto", 20, Align.left, false);
            gridDetalhes.AdicionarCabecalho("Regime de Limpeza", "RegimeLimpezaCargaFormatado", 20, Align.left, false);

            return gridDetalhes;
        }


        private async Task<Grid> ObterGridPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador =
                new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork, cancellationToken);

            Grid grid = ObterGrid();
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoChecklist filtroPequisa = ObterFiltrosPesquisa();
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegitros = await repCargaJanelaCarregamentoTransportador.ContarPorChecklistAsync(filtroPequisa);
            IList<AcompanhamentoChecklist> acompanhamentoChecklist = totalRegitros > 0
                ? repCargaJanelaCarregamentoTransportador.Consultar(filtroPequisa, parametrosConsulta)
                : new List<AcompanhamentoChecklist>();

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AcompanhamentoChecklist/Pesquisar", "grid-acompanhamento-checklist");
            Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);

            grid.AplicarPreferenciasGrid(preferenciasGrid);
            grid.setarQuantidadeTotal(totalRegitros);
            grid.AdicionaRows(acompanhamentoChecklist);

            return grid;
        }


        private async Task<Grid> ObterGridPesquisaDetalhesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken, int codigoJanelaCarregamentoTransportador, int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork, cancellationToken);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist servicoCargaJanelaCarregamentoTransportadorChecklist = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist(unitOfWork);

            Grid grid = ObterGridDetalhesChecklist();

            if (codigoCarga == 0)
            {
                grid.AdicionaRows(new List<dynamic>());
                return grid;
            }

            List<int> codigosVeiculo = repCargaJanelaCarregamentoTransportador.BuscarVeiculosPorCodigoCarga(codigoCarga);
            if (codigosVeiculo == null || !codigosVeiculo.Any())
            {
                grid.AdicionaRows(new List<dynamic>());
                return grid;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> cargaJanelaCarregamentoTransportadorChecklist = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist>();

            foreach (int codigoVeiculo in codigosVeiculo)
            {
                List<CargaJanelaCarregamentoTransportadorChecklist> checklists = servicoCargaJanelaCarregamentoTransportadorChecklist.ObterChecklist(codigoJanelaCarregamentoTransportador, codigoVeiculo);
                cargaJanelaCarregamentoTransportadorChecklist.AddRange(checklists);
            }

            if (cargaJanelaCarregamentoTransportadorChecklist.Count == 0)
            {
                grid.AdicionaRows(new List<dynamic>());
                return grid;
            }

            IList<AcompanhamentoChecklistDetalhes> acompanhamentoChecklistDetalhes = MapearAcompanhamentoChecklistDetalhes(cargaJanelaCarregamentoTransportadorChecklist);

            grid.setarQuantidadeTotal(acompanhamentoChecklistDetalhes.Count);
            grid.AdicionaRows(acompanhamentoChecklistDetalhes);

            return grid;
        }

        private IList<AcompanhamentoChecklistDetalhes> MapearAcompanhamentoChecklistDetalhes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> checklists)
        {
            List<AcompanhamentoChecklistDetalhes> detalhes = new List<AcompanhamentoChecklistDetalhes>();
            foreach (CargaJanelaCarregamentoTransportadorChecklist checklist in checklists)
            {
                detalhes.Add(new AcompanhamentoChecklistDetalhes
                {
                    OrdemChecklist = checklist.OrdemCargaChecklist,
                    Placa = checklist.Placa,
                    Produto = checklist.GrupoProduto.Descricao,
                    RegimeLimpezaCarga = checklist.RegimeLimpeza
                });
            }
            return detalhes;
        }

        #endregion
    }
}
