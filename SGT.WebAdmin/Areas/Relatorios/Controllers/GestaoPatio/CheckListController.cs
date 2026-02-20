using Dominio.Entidades.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.GestaoPatio
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/GestaoPatio/CheckList")]
    public class CheckListController : BaseController
    {
		#region Construtores

		public CheckListController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R226_CheckList;
        private readonly int _limiteColunasDinamicas = 60;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);

                int codigoFilial = Request.GetIntParam("CodigoFilial");
                int Codigo = Request.GetIntParam("Codigo");

                if (codigoFilial <= 0)
                    return new JsonpResult(false, "Filial não encontrada.");

                List<CheckListOpcoes> listaPerguntas = repositorioChecklistOpcoes.BuscarPerguntasPorFilial(codigoFilial);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CheckList", "GestaoPatio", "CheckListRelatorio.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, false, true);

                Models.Grid.Grid grid = ObterGridPadrao(listaPerguntas);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = new Models.Grid.Relatorio().RetornoGridPadraoRelatorio(grid, relatorio);

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorio = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga> listaChecklist = (totalRegistros > 0) ? repositorio.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga>();

                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistCargaPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(unitOfWork);

                List<CheckListOpcoes> listaPerguntas = repositorioChecklistOpcoes.BuscarPerguntasPorFilial(filtrosPesquisa.CodigoFilial);

                foreach (Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga dataSource in listaChecklist)
                {
                    List<CheckListCargaPergunta> listaRespostas = repositorioChecklistCargaPergunta.BuscarPorCheckList(dataSource.Codigo);
                    List<CheckListCargaPerguntaAlternativa> listaAlternativas = repositorioCheckListCargaPerguntaAlternativa.BuscarPorCheckList(dataSource.Codigo);

                    int contador = 0;

                    for (int i = 0; i < listaPerguntas.Count; i++)
                    {
                        if (contador >= _limiteColunasDinamicas) break;

                        string resposta = "";

                        CheckListCargaPergunta pergunta = listaRespostas.Where(o => o.Descricao == listaPerguntas[i].Descricao).FirstOrDefault();

                        if (pergunta != null)
                        {
                            switch (pergunta.Tipo)
                            {
                                case TipoOpcaoCheckList.SimNao:
                                    if (pergunta?.Resposta == CheckListResposta.Aprovada)
                                        resposta = CheckListResposta.Aprovada.ObterDescricaoSimNao();
                                    else if (pergunta?.Resposta == CheckListResposta.Reprovada)
                                        resposta = CheckListResposta.Reprovada.ObterDescricaoSimNao();
                                    else
                                        resposta = string.Empty;
                                    break;

                                case TipoOpcaoCheckList.Aprovacao:
                                    if (pergunta?.Resposta == CheckListResposta.Aprovada)
                                        resposta = CheckListResposta.Aprovada.ObterDescricao();
                                    else if (pergunta?.Resposta == CheckListResposta.Reprovada)
                                        resposta = CheckListResposta.Reprovada.ObterDescricao();
                                    else
                                        resposta = string.Empty;
                                    break;

                                case TipoOpcaoCheckList.Opcoes:
                                    List<CheckListCargaPerguntaAlternativa> selecionadas = listaAlternativas.Where(o => o.CheckListCargaPergunta == pergunta && o.Marcado).ToList();
                                    resposta = String.Join(", ", selecionadas?.Select(o => o.Descricao));
                                    break;

                                case TipoOpcaoCheckList.Selecoes:
                                    CheckListCargaPerguntaAlternativa selecao = listaAlternativas.Where(o => o.CheckListCargaPergunta == pergunta && o.Marcado).FirstOrDefault();
                                    resposta = selecao?.Descricao;
                                    break;

                                case TipoOpcaoCheckList.Informativo:
                                    resposta = pergunta.Observacao;
                                    break;
                            }
                        }

                        dataSource.GetType().GetProperty($"ColunaDinamica{(contador)}")?.SetValue(dataSource, resposta);

                        contador++;
                    }
                }

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaChecklist);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.GestaoPatio.CheckListCarga checkListCarga = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga> dataSourceCheckList = checkListCarga.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta);

                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta repositorioChecklistCargaPergunta = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPergunta(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa(unitOfWork);

                List<CheckListOpcoes> listaPerguntas = repositorioChecklistOpcoes.BuscarPerguntasPorFilial(filtrosPesquisa.CodigoFilial);

                foreach (Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RelatorioCheckListCarga dataSource in dataSourceCheckList)
                {
                    List<CheckListCargaPergunta> listaRespostas = repositorioChecklistCargaPergunta.BuscarPorCheckList(dataSource.Codigo);
                    List<CheckListCargaPerguntaAlternativa> listaAlternativas = repositorioCheckListCargaPerguntaAlternativa.BuscarPorCheckList(dataSource.Codigo);

                    int contador = 0;

                    for (int i = 0; i < listaPerguntas.Count; i++)
                    {
                        if (contador >= _limiteColunasDinamicas) break;

                        string resposta = "";

                        CheckListCargaPergunta pergunta = listaRespostas.Where(o => o.Descricao == listaPerguntas[i].Descricao).FirstOrDefault();

                        if (pergunta != null)
                        {
                            switch (pergunta.Tipo)
                            {
                                case TipoOpcaoCheckList.SimNao:
                                    resposta = pergunta?.Resposta == CheckListResposta.Aprovada ? "Sim" : "Não";
                                    break;

                                case TipoOpcaoCheckList.Aprovacao:
                                    resposta = pergunta?.Resposta == CheckListResposta.Aprovada ? "Aprovada" : "Reprovada";
                                    break;

                                case TipoOpcaoCheckList.Opcoes:
                                    List<CheckListCargaPerguntaAlternativa> selecionadas = listaAlternativas.Where(o => o.CheckListCargaPergunta == pergunta && o.Marcado).ToList();
                                    resposta = String.Join(", ", selecionadas?.Select(o => o.Descricao));
                                    break;

                                case TipoOpcaoCheckList.Selecoes:
                                    CheckListCargaPerguntaAlternativa selecao = listaAlternativas.Where(o => o.CheckListCargaPergunta == pergunta && o.Marcado).FirstOrDefault();
                                    resposta = selecao?.Descricao;
                                    break;

                                case TipoOpcaoCheckList.Informativo:
                                    resposta = pergunta.Observacao;
                                    break;
                            }
                        }

                        dataSource.GetType().GetProperty($"ColunaDinamica{(contador)}")?.SetValue(dataSource, resposta);

                        contador++;
                    }
                }

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/GestaoPatio/CheckList", new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>(), relatorioControleGeracao, relatorioTemporario, dataSourceCheckList, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioCheckList
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Carga = Request.GetStringParam("Carga"),
                Situacao = Request.GetNullableEnumParam<SituacaoCheckList>("Situacao")
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.EndsWith("Formatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            if (propriedadeOrdenar.EndsWith("Descricao"))
                return propriedadeOrdenar.Replace("Descricao", "");

            return propriedadeOrdenar;
        }

        private Models.Grid.Grid ObterGridPadrao(List<CheckListOpcoes> listaPerguntas)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", 6, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "Carga", 6, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Status Checklist Anterior", "StatusChecklistAnteriorFormatada", 5, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Status", "StatusDescricao", 5, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Motivo de Reprova do Checklist Anterior", "MotivoReprovaChecklistAnterior", 4, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Checklist Anterior", "DataChecklistAnteriorFormatada", 6, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Checklist Atual", "DataChecklistAtualFormatada", 6, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Placa", "Placa", 4, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Observações Gerais", "ObservacoesGerais", 4, Models.Grid.Align.left, false, false, false, false, true);

            int count = 0;

            foreach (CheckListOpcoes pergunta in listaPerguntas)
            {
                if (count == _limiteColunasDinamicas) break;
                grid.AdicionarCabecalho(pergunta.Descricao, $"ColunaDinamica{count}", 10, Models.Grid.Align.left, false, false, false, false, false);
                count++;
            }

            return grid;
        }

        #endregion
    }
}
