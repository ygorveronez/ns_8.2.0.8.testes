using Dominio.Entidades.Embarcador.Logistica;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Extensions;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Dynamic;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "VerificarPossibilidadeModificacaoJanela", "ObterEncaixesDisponiveis", "ObterHorariosDisponiveis", "ObterInformacoes", "ObterInformacoesCargas", "ObterGradeTipoOperacaoDoPeriodo", "BuscarDataCarga", "ObterDisponibilidadeCarregamento", "ObterCapacidadeCarregamentoDocas", "ObterCapacidadeCarregamentoDados", "ObterCargasExcedentes", "ObterCargasPendentes", "ObterCargasEmReserva", "ObterTransportadoresOfertados", "ObterTransportadoresOfertadosHistorico", "ObterTransportadoresOfertadosHistoricoOferta", "ObterAreasVeiculos", "ObterInteressadosCarga", "PesquisaInformacoesCargas" }, "Logistica/JanelaCarregamento", "Cargas/ControleEntrega", "Logistica/Monitoramento", "Cargas/MontagemCarga", "Cargas/MontagemCargaMapa", "Cargas/Carga")]
    public class JanelaCarregamentoController : BaseController
    {
        #region Construtores

        public JanelaCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Configuracoes.ConfiguracaoLegenda servicoConfiguracaoLegenda = new Servicos.Embarcador.Configuracoes.ConfiguracaoLegenda(unitOfWork);
                List<int> situacoesPadraoPesquisa = new List<int>();

                if (this.ConfiguracaoEmbarcador?.SituacaoJanelaCarregamentoPadraoPesquisa.HasValue ?? false)
                    situacoesPadraoPesquisa.Add(this.ConfiguracaoEmbarcador.SituacaoJanelaCarregamentoPadraoPesquisa.Value);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorioJanelaCarregamentoSituacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais = repositorioJanelaCarregamentoSituacao.BuscarTodos();

                var legendasDinamicas = (
                    from o in situacoesAdicionais
                    select new
                    {
                        o.Descricao,
                        Cores = new
                        {
                            Fonte = Utilidades.Cores.ObterCorPorPencentual(o.Cor, percentual: 40),
                            Fundo = o.Cor
                        }
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    SituacoesPadraoPesquisa = situacoesPadraoPesquisa,
                    Legendas = servicoConfiguracaoLegenda.ObterPorGrupoCodigoControle(GrupoCodigoControleLegenda.JanelaCarregamento),
                    LegendasDinamicas = legendasDinamicas
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarPossibilidadeModificacaoJanela()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoJanelaCarregamento = Request.GetIntParam("JanelaCarregamento");
                DateTime data = Request.GetDateTimeParam("Data");
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null;

                if (codigoJanelaCarregamento > 0)
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);
                else
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.JanelaDeCarregamentoNaoEncontrada);

                bool permiteNoShow = cargaJanelaCarregamento.CentroCarregamento.PermiteMarcarCargaComoNaoComparecimento;

                int tempoToleranciaChegadaAtraso = cargaJanelaCarregamento.CentroCarregamento.TempoToleranciaChegadaAtraso;
                DateTime dataToleranciaNovoHorario = DateTime.Now.AddMinutes(tempoToleranciaChegadaAtraso);
                DateTime dataToleranciaNoShow = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(-tempoToleranciaChegadaAtraso);

                bool permiteModificarHorario = !permiteNoShow || data >= dataToleranciaNovoHorario;

                return new JsonpResult(new
                {
                    PermiteModificarHorario = permiteModificarHorario,
                    MensagemPermiteModificarHorario = !permiteModificarHorario ? Localization.Resources.Cargas.Carga.NaoPossivelInformarUmaDataMenorDoQueTolerancia : "",
                    PossibilidadeNoShow = permiteNoShow && !cargaJanelaCarregamento.Excedente && DateTime.Now >= dataToleranciaNoShow
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsInformacaesParaModificacaoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterEncaixesDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDeOperacao, propriedade: "TipoOperacao", tamanho: 30, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Quantidade, propriedade: "Quantidade", tamanho: 20, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);

                DateTime dia = Request.GetDateTimeParam("DataCarregamento");

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = PreencherConfiguracaoDisponibilidadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = BuscarCentroCarregamentoPorParametro(unitOfWork);

                if (centroCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CentroDeCarregamentoNaoEncontrada);

                if (dia == DateTime.MinValue)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.DataObrigatoria);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, ConfiguracaoEmbarcador, configuracaoDisponibilidadeCarregamento);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.GradeCarregamento> encaixes = servicoDisponibilidadeCarregamento.ObterEncaixesCarregamentosDisponiveis(centroCarregamento, dia);

                grid.setarQuantidadeTotal(encaixes.Count);
                grid.AdicionaRows((from encaixe in encaixes
                                   select new
                                   {
                                       Codigo = encaixe.CodigoTipoOperacao,
                                       encaixe.TipoOperacao,
                                       encaixe.Quantidade
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsInformacoesDoCentroDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterHorariosDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "HoraInicio", visivel: false);
                grid.AdicionarCabecalho(propriedade: "HoraTermino", visivel: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Horario, propriedade: "Horario", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                DateTime dia = Request.GetDateTimeParam("DataCarregamentoDisponibilidade");

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = PreencherConfiguracaoDisponibilidadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = BuscarCentroCarregamentoPorParametro(unitOfWork);

                if (centroCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CentroDeCarregamentoNaoEncontrada);

                if (dia == DateTime.MinValue)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.DataObrigatoria);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, ConfiguracaoEmbarcador, configuracaoDisponibilidadeCarregamento);

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> horarios = servicoDisponibilidadeCarregamento.ObterPeriodosCarregamentosDisponiveis(centroCarregamento, dia);

                grid.setarQuantidadeTotal(horarios.Count);
                grid.AdicionaRows((from periodo in horarios
                                   select new
                                   {
                                       periodo.Codigo,
                                       Horario = $"{periodo.HoraInicio:hh\\:mm} - {periodo.HoraTermino:hh\\:mm}",
                                       periodo.HoraInicio,
                                       periodo.HoraTermino,
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsInformacoesDoCentroDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterInformacoes()
        {
            var unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao repositorioJanelaCarregamentoHistoricoCotacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorioJanelaCarregamentoSituacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioJanelaCarregamentoTransportadorHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(unidadeDeTrabalho);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
                Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unidadeDeTrabalho);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                List<int> cargas = new List<int>();

                if (cargaJanelaCarregamento != null)
                {
                    if (cargaJanelaCarregamento.Carga != null)
                        cargas.Add(cargaJanelaCarregamento.Carga.Codigo);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(cargaJanelaCarregamento.Carga?.Codigo ?? 0);
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosSumarizados = repCargaPedido.ObterDadosResumidosPorCargas(cargas);
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosInteressados = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento>();
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosVisualizacoes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento>();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais = repositorioJanelaCarregamentoSituacao.BuscarTodos();
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoPorCentro(cargaJanelaCarregamento.CentroCarregamento, cargaJanelaCarregamento.InicioCarregamento, unidadeDeTrabalho);
                    bool possuiHistoricoCotacao = repositorioJanelaCarregamentoHistoricoCotacao.ExistePorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);
                    bool permitirInformarObservacaoFluxoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ExisteSequenciaGestaoPatio(cargaJanelaCarregamento.CentroCarregamento?.Filial?.Codigo ?? 0, TipoFluxoGestaoPatio.Origem);
                    int numeroTransportadoresInteressados = repositorioJanelaCarregamentoTransportador.ContarPorCargaJanelaCarregamentoESituacao(cargaJanelaCarregamento.Codigo, SituacaoCargaJanelaCarregamentoTransportador.ComInteresse);
                    int numeroTransportadoresVisualizacoes = repositorioJanelaCarregamentoTransportadorHistorico.ContarPorCargaJanelaCarregamentoESituacao(cargaJanelaCarregamento.Codigo, TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga);

                    cargaJanelaCarregamentosInteressados.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento()
                    {
                        Codigo = cargaJanelaCarregamento.Codigo,
                        NumeroInteressados = numeroTransportadoresInteressados
                    });
                    cargaJanelaCarregamentosVisualizacoes.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento()
                    {
                        Codigo = cargaJanelaCarregamento.Codigo,
                        NumeroVisualizacoes = numeroTransportadoresVisualizacoes
                    });

                    var retorno = ObterDadosCargaJanelaCarregamento(
                        cargaJanelaCarregamento,
                        configuracaoEmbarcador,
                        unidadeDeTrabalho,
                        cargaPedidosSumarizados,
                        cargaMotoristas,
                        cargaJanelaCarregamentosInteressados,
                        periodosCarregamento,
                        situacoesAdicionais,
                        permitirInformarObservacaoFluxoPatio,
                        operador,
                        dataAtualJanelaCarregamento: null,
                        possuiHistoricoCotacao: possuiHistoricoCotacao,
                        cargaJanelaCarregamentosVisualizacoes
                    );

                    return new JsonpResult(retorno);
                }
                else
                {
                    return new JsonpResult(true, false, Localization.Resources.Cargas.Carga.NaoFoiLocalizadoUmRegistroParaCargaInformadaAtualizePaginaTenteNovamentePoisEsseRegistroJaPodeTerSidoExcluido);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterInformacoesCargas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho, TipoJanelaCarregamento.Calendario);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centro = repCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento);

                if (centro == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CentroDeCarregamentoObrigatorio);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = ObterJanelaCarregamentosPorFiltros(filtrosPesquisa, unidadeDeTrabalho);
                dynamic cargasRetornar = (centro.ExibirVisualizacaoDosTiposDeOperacao && Request.GetBoolParam("ExibirSomenteGradesLivres")) ? new List<dynamic>() : ObterDadosCargaJanelaCarregamento(cargasJanelaCarregamento, unidadeDeTrabalho, filtrosPesquisa.DataCarregamento);

                return new JsonpResult(new
                {
                    Cargas = cargasRetornar,
                    PeriodosCarregamento = ObterPeriodosCarregamentoCargaJanelaCarregamento(centro, cargasJanelaCarregamento, unidadeDeTrabalho, filtrosPesquisa),
                    PeriodosBloqueios = ObterPeriodosComBloqueiosCargaJanelaCarregamento(centro, unidadeDeTrabalho, filtrosPesquisa)
                });
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

        public async Task<IActionResult> ObterGradeTipoOperacaoDoPeriodo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unidadeDeTrabalho);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unidadeDeTrabalho, ConfiguracaoEmbarcador);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho, TipoJanelaCarregamento.Calendario);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centro = repCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento);

                if (centro == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CentroDeCarregamentoObrigatorio);

                DateTime diaDoPeriodo = Request.GetDateTimeParam("InicioPeriodo");
                TimeSpan inicioPeriodo = diaDoPeriodo.TimeOfDay;
                var periodos = ObterPeriodosCarregamentoPorCentro(centro, filtrosPesquisa.DataCarregamento, unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo = periodos.Where(o => o.HoraInicio == inicioPeriodo).FirstOrDefault();

                if (periodo == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.PeriodoNaoEncontrado);

                filtrosPesquisa.PeriodoInicial = periodo.HoraInicio;
                filtrosPesquisa.PeriodoFinal = periodo.HoraTermino;

                DateTime dataPeriodoInicial = diaDoPeriodo.Date.Add(periodo.HoraInicio);
                DateTime dataPeriodoFinal = diaDoPeriodo.Date.Add(periodo.HoraTermino);

                List<(double? Destinatario, int Carga)> cargasEDestinatarios = repCargaJanelaCarregamento.BuscarCargasEDestinatarioPorIncidenciaDeHorario(0, centro.Filial.Codigo, centro.Codigo, dataPeriodoInicial, dataPeriodoFinal);
                List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradaCentros = servicoDisponibilidadeCarregamento.ObterParadaCentroCarregamento(centro, filtrosPesquisa.DataCarregamento.Value);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = ObterJanelaCarregamentosPorFiltros(filtrosPesquisa, unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> tiposOperacaoPeriodo = periodo.TipoOperacaoSimultaneo.ToList();
                List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades = repExclusividadeCarregamento.BuscarExclusividadePorPeriodo(centro.Codigo, filtrosPesquisa.DataCarregamento.Value, DiaSemanaHelper.ObterDiaSemana(filtrosPesquisa.DataCarregamento.Value));
                bool permitirRetornarTipoOperacaoLivre = filtrosPesquisa.CodigoTipoOperacao == 0;

                List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodos = cargasJanelaCarregamento.Select(obj =>
                    new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo
                    {
                        Codigo = obj.Codigo,
                        TipoCarga = (obj.Carga.TipoDeCarga != null ? obj.Carga.TipoDeCarga.Codigo : 0),
                        TipoOperacao = (obj.Carga.TipoOperacao != null ? obj.Carga.TipoOperacao.Codigo : 0),
                        Transportador = (obj.Carga.Empresa != null ? obj.Carga.Empresa.Codigo : 0),
                        ModeloVeicularCarga = (obj.Carga.ModeloVeicularCarga != null ? obj.Carga.ModeloVeicularCarga.Codigo : 0),
                        Destinatario = cargasEDestinatarios.Where(o => o.Carga == obj.Carga?.Codigo).FirstOrDefault().Destinatario ?? 0,
                        Encaixe = obj.HorarioEncaixado,
                        TipoOperacaoEncaixe = obj.TipoOperacaoEncaixe?.Codigo ?? 0,
                        DataInicio = obj.InicioCarregamento,
                        DataFim = obj.TerminoCarregamento
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    PeriodoCarregamento = centro.ExibirVisualizacaoDosTiposDeOperacao ? ObterListaDisponbilidadePorTipoOperacao(filtrosPesquisa.DataCarregamento.Value, periodo, cargasPeriodos, tiposOperacaoPeriodo, paradaCentros, exclusividades, permitirRetornarTipoOperacaoLivre, unidadeDeTrabalho) : null,
                    periodo.HoraInicio,
                    periodo.HoraTermino,
                });
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

        public async Task<IActionResult> ExportarProdutosAgendados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosCargas = Request.GetListParam<int>("Cargas");

                if (codigosCargas.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoExisteNenhumaCargaNaDataSelecionadaParaCentroDeCarregamento);

                Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtos = repositorioCargaPedidoProduto.BuscarPorCargas(codigosCargas);

                if (produtos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NenhumProdutoFoiEncontradoParaGerarArquivo);

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.tituloExportacao = Localization.Resources.Cargas.Carga.ProdutosAgendados;

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DescricaoCarga, "Carga", 8, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CodigoProduto, "CodigoProduto", 8, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Produto, "Produto", 8, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Quantidade, "Quantidade", 8, Models.Grid.Align.right, false, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.PesoUnitario, "PesoUnitario", 8, Models.Grid.Align.right, false, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.PesoEmbalagem, "PesoEmbalagem", 8, Models.Grid.Align.right, false, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.PesoTotal, "PesoTotal", 8, Models.Grid.Align.right, false, false, false, false, true);

                var retorno = (from obj in produtos
                               select new
                               {
                                   Carga = obj.CargaPedido.Carga.CodigoCargaEmbarcador,
                                   CodigoProduto = obj.Produto.CodigoProdutoEmbarcador,
                                   Produto = obj.Produto.Descricao,
                                   Quantidade = obj.Quantidade.ToString("n2"),
                                   PesoUnitario = obj.PesoUnitario.ToString("n3"),
                                   PesoEmbalagem = obj.PesoTotalEmbalagem.ToString("n2"),
                                   PesoTotal = obj.PesoTotal.ToString("n2")
                               });

                grid.AdicionaRows(retorno);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os produtos agendados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterInformacoesCentroCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoRota = Request.GetIntParam("Rota");
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);
                var rotas = codigoRota == 0 ? (from obj in centroCarregamento.PrevisoesCarregamento select obj.Rota).Distinct() : (from obj in centroCarregamento.PrevisoesCarregamento where obj.Rota.Codigo == codigoRota select obj.Rota).Distinct();

                var informacoesRotas = (
                    from obj in rotas
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao,
                        Estados = (from estado in obj.Estados select estado.Sigla).ToList(),
                        PrevisoesCarregamento = (
                            from previsao in centroCarregamento.PrevisoesCarregamento
                            where previsao.Rota.Codigo == obj.Codigo
                            select new
                            {
                                previsao.Dia,
                                ModelosVeiculos = (
                                    from modelo in previsao.ModelosVeiculos
                                    select new
                                    {
                                        Codigo = modelo.Codigo,
                                        Descricao = modelo.Descricao
                                    }
                                ).ToList(),
                                previsao.QuantidadeCargas,
                                previsao.QuantidadeCargasExcedentes
                            }
                        ).ToList(),
                        ExcecoesPrevisoesCarregamento = (
                            from excecao in centroCarregamento.ExcecoesCapacidadeCarregamento
                            select (
                                from previsao in excecao.PrevisoesCarregamento
                                where previsao.Rota.Codigo == obj.Codigo
                                select new
                                {
                                    Data = excecao.Data.ToString("dd/MM/yyyy"),
                                    ModelosVeiculos = (
                                        from modelo in previsao.ModelosVeiculos
                                        select new
                                        {
                                            Codigo = modelo.Codigo,
                                            Descricao = modelo.Descricao
                                        }
                                    ).ToList(),
                                    previsao.QuantidadeCargas,
                                    previsao.QuantidadeCargasExcedentes
                                }
                            )
                        ).SelectMany(o => o).ToList()
                    }
                ).ToList();

                var informacoesCentroCarregamento = new
                {
                    centroCarregamento.Codigo,
                    centroCarregamento.Descricao,
                    centroCarregamento.PermitirLiberarCargaTransportadorExclusivo,
                    centroCarregamento.LiberarCargaManualmenteParaTransportadores,
                    centroCarregamento.TipoJanelaCarregamento,
                    centroCarregamento.TipoMontagemCarregamentoVRP,
                    centroCarregamento.TipoOcupacaoMontagemCarregamentoVRP,
                    centroCarregamento.ExibirSomenteJanelaCarregamento,
                    centroCarregamento.TipoOrdenacaoJanelaCarregamento,
                    centroCarregamento.LimiteCarregamentos,
                    centroCarregamento.GerarJanelaCarregamentoDestino,
                    PermitirInformarAreaVeiculo = centroCarregamento.PermitirInformarAreaVeiculoJanelaCarregamento,

                    Configuracao = new
                    {
                        centroCarregamento.JanelaCarregamentoAbaPendentes,
                        centroCarregamento.JanelaCarregamentoAbaExcedentes,
                        centroCarregamento.JanelaCarregamentoAbaReservas,
                        centroCarregamento.JanelaCarregamentoExibirSituacaoPatio,
                        centroCarregamento.PermiteMarcarCargaComoNaoComparecimento,
                        centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao,
                        ExibirComboVolumeCubagem = centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.VolumeECubagem,
                    },

                    ExibirSituacaoAguardandoAceiteTransportador = centroCarregamento.LiberarCargaAutomaticamenteParaTransportadoras && (centroCarregamento.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente > 0),
                    ExibirTransportadoresOfertadosComMenorValorFreteTabela = configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela,
                    ExibirTransportadoresOfertadosPorPrioridadeDeRota = (
                        centroCarregamento.LiberarCargaAutomaticamenteParaTransportadoras &&
                        centroCarregamento.TipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota &&
                        (centroCarregamento.LiberarCargaAutomaticamenteParaTransportadorasForaRota || centroCarregamento.TipoTransportadorSecundario.HasValue)
                    ),
                    PeriodosCarregamento = (
                        from periodo in centroCarregamento.PeriodosCarregamento
                        where periodo.ExcecaoCapacidadeCarregamento == null && periodo.ExclusividadeCarregamento == null
                        select new
                        {
                            periodo.Codigo,
                            periodo.Dia,
                            periodo.HoraInicio,
                            periodo.HoraTermino,
                            periodo.ToleranciaExcessoTempo,
                            periodo.CapacidadeCarregamentoSimultaneo,
                            periodo.CapacidadeCarregamentoVolume
                        }
                    ).ToList(),
                    Rotas = informacoesRotas,
                };

                return new JsonpResult(informacoesCentroCarregamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsInformacoesDoCentroDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDataCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorio
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                // Converte parametros
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                string codigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
                string codigoPedidoCliente = Request.Params("CodigoPedidoEmbarcador");

                // Busca dados
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarga = repCargaJanelaCarregamento.BuscarDataCarga(codigoCentroCarregamento, codigoCargaEmbarcador, codigoPedidoCliente);
                //Dominio.Entidades.Embarcador.Cargas.Carga cargaJanela = janelaCarga != null ? janelaCarga.CargaJanelaCarregamentoAgrupador == null ? janelaCarga.Carga : janelaCarga.CargaJanelaCarregamentoAgrupador.Carga : null;
                Dominio.Entidades.Embarcador.Cargas.CargaBase cargaJanela = janelaCarga != null ? janelaCarga.CargaJanelaCarregamentoAgrupador == null ? janelaCarga.Carga : janelaCarga.CargaJanelaCarregamentoAgrupador.Carga : null;

                return new JsonpResult(new
                {
                    Codigo = janelaCarga?.Carga.Codigo ?? 0,
                    //Numero = servicoCarga.ObterNumeroCarga(cargaJanela, configuracaoEmbarcador),
                    Numero = cargaJanela?.IsCarga() ?? false ? ((Dominio.Entidades.Embarcador.Cargas.Carga)cargaJanela)?.CodigoCargaEmbarcador ?? string.Empty : ((Dominio.Entidades.Embarcador.PreCargas.PreCarga)cargaJanela)?.NumeroPreCarga ?? string.Empty,
                    DataCarregamento = janelaCarga?.InicioCarregamento.ToString("dd/MM/yyyy") ?? string.Empty,
                    HoraCarregamento = janelaCarga?.InicioCarregamento.ToString("HH:mm:ss") ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarPorCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDisponibilidadeCarregamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota, codigoCentroCarregamento;
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unidadeDeTrabalho);

                List<object> retorno = new List<object>();

                for (var dia = dataInicial; dia <= dataFinal; dia = dia.AddDays(1))
                {
                    DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dia);
                    List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> previsoesCarregamentoDia = null;
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(codigoCentroCarregamento, dia);

                    if (excecao != null)
                        previsoesCarregamentoDia = repPrevisaoCarregamento.BuscarPorExcecao(excecao.Codigo, codigoRota);
                    else
                        previsoesCarregamentoDia = repPrevisaoCarregamento.BuscarPorCentroCarregamento(codigoCentroCarregamento, codigoRota, diaSemana);

                    retorno.Add(new
                    {
                        Dia = dia.ToString("dd/MM/yyyy"),
                        Previsoes = (from obj in previsoesCarregamentoDia
                                     select new
                                     {
                                         obj.Descricao,
                                         obj.QuantidadeCargas,
                                         obj.QuantidadeCargasExcedentes,
                                         QuantidadeCargasUtilizadas = repCargaJanelaCarregamento.ContarCargasPorRota(codigoCentroCarregamento, codigoRota, dia, obj.ModelosVeiculos.Select(o => o.Codigo).ToArray(), 0, false, codigoCargaJanelaCarregamentoDesconsiderar: 0)
                                     }).ToList()
                    });
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsDisponibilidadesDeCarregamento);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterCapacidadeCarregamentoDocas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                DateTime? dataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento");
                int codigoTransportador = Request.GetIntParam("Transportador");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                if (!dataCarregamento.HasValue)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

                if (codigoCentroCarregamento == 0)
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(codigoTipoCarga, codigoFilial, ativo: true);

                if (centroCarregamento == null || centroCarregamento.LimiteCarregamentos != LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                    return new JsonpResult(false);

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento
                {
                    DataCarregamento = dataCarregamento,
                    CodigoCentroCarregamento = centroCarregamento.Codigo,
                    CodigoTransportador = codigoTransportador,
                    CodigoTipoOperacao = codigoTipoOperacao
                };

                List<(decimal PesoCarga, bool PossuiVeiculo, int QuantidadeAdicionalVagasOcupadas)> cargasJanelaCarregamentoPeriodo = repCargaJanelaCarregamento.BuscarDadosCargasAlocadasPorDoca(filtrosPesquisa);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoDia = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, dataCarregamento.Value.Date);
                int quantidadeDocasJanela = 0;
                double porcentagemAgendamentoRelacaoDocasTotaisNoDia = 0;

                if (excecaoDia != null)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> periodosCarregamentoTipoOperacoes = repPeriodoCarregamentoTipoOperacaoSimultaneo.BuscarPorExcecao(excecaoDia.Codigo);

                    if ((periodosCarregamentoTipoOperacoes.Count > 0) && (codigoTipoOperacao > 0))
                        quantidadeDocasJanela = periodosCarregamentoTipoOperacoes.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao).Sum(o => o.CapacidadeCarregamentoSimultaneo);
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentos = repPeriodoCarregamento.BuscarPorExcecao(excecaoDia.Codigo);
                        quantidadeDocasJanela = periodosCarregamentos.Sum(x => x.CapacidadeCarregamentoSimultaneo);
                    }

                    if (quantidadeDocasJanela > 0)
                    {
                        int quantidadeDocasOcupadas = cargasJanelaCarregamentoPeriodo.Count + cargasJanelaCarregamentoPeriodo.Sum(o => o.QuantidadeAdicionalVagasOcupadas);

                        porcentagemAgendamentoRelacaoDocasTotaisNoDia = (quantidadeDocasOcupadas * 100) / (double)quantidadeDocasJanela;
                    }
                }

                return new JsonpResult(new
                {
                    PercentualAgendado = quantidadeDocasJanela > 0 ? $"{(decimal)porcentagemAgendamentoRelacaoDocasTotaisNoDia:n2} %" : "--",
                    ToneladasAlocadas = $"{cargasJanelaCarregamentoPeriodo.Sum(o => o.PesoCarga):n2}",
                    VeiculosInformados = $"{cargasJanelaCarregamentoPeriodo.Count(o => o.PossuiVeiculo)}/{cargasJanelaCarregamentoPeriodo.Count}"
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarAosDadosDeCapacidadeDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCapacidadeCarregamentoDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoTipoCarga = Request.GetIntParam("TipoCarga");
                DateTime? dataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento");
                bool buscarPeriodoAtivo = false;

                TipoVisualizacaoCapacidadeJanela TipoVisualizacaoCapacidadeJanela = Request.GetEnumParam<TipoVisualizacaoCapacidadeJanela>("TipoVisualizacaoCapacidadeJanela", TipoVisualizacaoCapacidadeJanela.Volume);

                if (!dataCarregamento.HasValue)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

                if (codigoCentroCarregamento == 0)
                {
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(codigoTipoCarga, codigoFilial, ativo: true);
                    buscarPeriodoAtivo = true;
                }

                if ((centroCarregamento == null) || !centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorioCapacidadeCarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade repositorioCentroCarregamentoProdutividade = new Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataCarregamento.Value);
                List<dynamic> listaCapacidadeCarregamentoPeriodo = new List<dynamic>();
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoCapacidadeCarregamento = servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, dataCarregamento.Value.Date);

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade> centroCarregamentoProdutividades = repositorioCentroCarregamentoProdutividade.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo);

                if (centroCarregamento.TipoCapacidadeCarregamentoPorPeso == TipoCapacidadeCarregamentoPorPeso.DiaSemana)
                {
                    int capacidadeCarregamento = excecaoCapacidadeCarregamento?.CapacidadeCarregamento ?? centroCarregamento.ObterCapacidadeCarregamento(diaSemana, TipoVisualizacaoCapacidadeJanela == TipoVisualizacaoCapacidadeJanela.Cubagem);
                    int capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamento(centroCarregamento.Codigo, dataCarregamento.Value);
                    int capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarPesoTotalCarregamentoDia(0, centroCarregamento.Codigo, dataCarregamento.Value);

                    if (centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
                    {
                        capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoDia(0, centroCarregamento.Codigo, dataCarregamento.Value); //BUSCAR VOLUMES
                        capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoVolume(centroCarregamento.Codigo, dataCarregamento.Value);
                    }
                    else if (centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.VolumeECubagem)
                    {
                        //tem q ver como veio no tipo visualizacao
                        if (TipoVisualizacaoCapacidadeJanela == TipoVisualizacaoCapacidadeJanela.Volume)
                        {
                            capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoDia(0, centroCarregamento.Codigo, dataCarregamento.Value); //BUSCAR VOLUMES
                            capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoVolume(centroCarregamento.Codigo, dataCarregamento.Value);
                        }
                        else if (TipoVisualizacaoCapacidadeJanela == TipoVisualizacaoCapacidadeJanela.Cubagem)
                        {
                            capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarCubagemTotalCarregamentoDia(0, centroCarregamento.Codigo, dataCarregamento.Value); //BUSCAR CUBAGEM
                            capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoCubagem(centroCarregamento.Codigo, dataCarregamento.Value);
                        }
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargaJanelaCarregamentos = repositorioCargaJanelaCarregamento.BuscarCarregamentoDia(0, centroCarregamento.Codigo, dataCarregamento.Value);
                    string produtividadeUtilizadaTexto = string.Empty;
                    decimal produtividadeUtilizada = CalculoProdutividadeUtilizada(centroCarregamento, cargaJanelaCarregamentos, centroCarregamentoProdutividades, unitOfWork, ref produtividadeUtilizadaTexto);

                    int capacidadeCarregamentoTotal = capacidadeCarregamento + capacidadeCarregamentoAdicional;
                    int capacidadeDisponivel = capacidadeCarregamentoTotal - capacidadeUtilizada;
                    listaCapacidadeCarregamentoPeriodo.Add(new
                    {
                        CapacidadeAdicional = capacidadeCarregamentoAdicional.ToString("n0"),
                        CapacidadeCarregamento = capacidadeCarregamento.ToString("n0"),
                        CapacidadeDisponivel = capacidadeDisponivel.ToString("n0"),
                        CapacidadeUtilizada = capacidadeUtilizada.ToString("n0"),
                        Periodo = "Capacidade Diária",
                        PeriodoAtivo = true,
                        PossuiProdutividade = centroCarregamentoProdutividades.Count > 0,
                        HorasProdutividade = centroCarregamento.HorasTrabalho,
                        ProdutividadeUtilizada = produtividadeUtilizadaTexto
                    });
                }
                else if (centroCarregamento.TipoCapacidadeCarregamentoPorPeso == TipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento)
                {
                    Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
                    ICollection<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = null;
                    int capacidadeDiariaCarregamento = 0;
                    int capacidadeDiariaCarregamentoAdicional = 0;
                    int capacidadeDiariaUtilizada = 0;
                    bool encontrouPeriodoAtivo = false;

                    if (excecaoCapacidadeCarregamento != null)
                        periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorExcecao(excecaoCapacidadeCarregamento.Codigo);
                    else if (!centroCarregamento.EscolherHorarioCarregamentoPorLista)
                        periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(centroCarregamento.Codigo, diaSemana);

                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento in periodosCarregamento)
                    {
                        int capacidadeCarregamentoVolume = TipoVisualizacaoCapacidadeJanela == TipoVisualizacaoCapacidadeJanela.Cubagem ? periodoCarregamento.CapacidadeCarregamentoCubagem : periodoCarregamento.CapacidadeCarregamentoVolume;
                        int capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoPorPeriodo(centroCarregamento.Codigo, dataCarregamento.Value, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);
                        int capacidadeCarregamentoTotal = capacidadeCarregamentoVolume + capacidadeCarregamentoAdicional;
                        int capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarPesoTotalCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento.Value, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);

                        if (centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
                        {
                            capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento.Value, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino); //BUSCAR VOLUMES
                            capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoVolume(centroCarregamento.Codigo, dataCarregamento.Value);
                        }
                        else if (centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.VolumeECubagem)
                        {
                            //tem q ver como veio no tipo visualizacao
                            if (TipoVisualizacaoCapacidadeJanela == TipoVisualizacaoCapacidadeJanela.Volume)
                            {
                                capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento.Value, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino); //BUSCAR VOLUMES
                                capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoVolume(centroCarregamento.Codigo, dataCarregamento.Value);
                            }
                            else if (TipoVisualizacaoCapacidadeJanela == TipoVisualizacaoCapacidadeJanela.Cubagem)
                            {
                                capacidadeUtilizada = (int)repositorioCargaJanelaCarregamento.BuscarCubagemTotalCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento.Value, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino); //BUSCAR VOLUMES //BUSCAR CUBAGEM
                                capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoCubagem(centroCarregamento.Codigo, dataCarregamento.Value);
                            }
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargaJanelaCarregamentos = repositorioCargaJanelaCarregamento.BuscarCarregamentoPeriodo(0, centroCarregamento.Codigo, dataCarregamento.Value, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);
                        string produtividadeUtilizadaTexto = string.Empty;
                        decimal produtividadeUtilizada = CalculoProdutividadeUtilizada(centroCarregamento, cargaJanelaCarregamentos, centroCarregamentoProdutividades, unitOfWork, ref produtividadeUtilizadaTexto);

                        int capacidadeDisponivel = capacidadeCarregamentoTotal - capacidadeUtilizada;
                        bool periodoAtivo = (
                            (periodosCarregamento.Count == 1) ||
                            (buscarPeriodoAtivo && (periodoCarregamento.HoraInicio <= dataCarregamento.Value.TimeOfDay) && (periodoCarregamento.HoraTermino >= dataCarregamento.Value.TimeOfDay))
                        );

                        if (periodoAtivo)
                            encontrouPeriodoAtivo = true;

                        listaCapacidadeCarregamentoPeriodo.Add(new
                        {
                            CapacidadeAdicional = capacidadeCarregamentoAdicional.ToString("n0"),
                            CapacidadeCarregamento = capacidadeCarregamentoVolume.ToString("n0"),
                            CapacidadeDisponivel = capacidadeDisponivel.ToString("n0"),
                            CapacidadeUtilizada = capacidadeUtilizada.ToString("n0"),
                            Periodo = $"{periodoCarregamento.HoraInicio.ToString(@"hh\:mm")} a {periodoCarregamento.HoraTermino.ToString(@"hh\:mm")}",
                            PeriodoAtivo = periodoAtivo,
                            PossuiProdutividade = centroCarregamentoProdutividades.Count > 0 && produtividadeUtilizada > 0,
                            HorasProdutividade = centroCarregamento.HorasTrabalho,
                            ProdutividadeUtilizada = produtividadeUtilizadaTexto
                        });

                        capacidadeDiariaCarregamento += capacidadeCarregamentoVolume;
                        capacidadeDiariaCarregamentoAdicional += capacidadeCarregamentoAdicional;
                        capacidadeDiariaUtilizada += capacidadeUtilizada;
                    }

                    if (periodosCarregamento.Count > 1)
                    {
                        int capacidadeDiariaCarregamentoTotal = capacidadeDiariaCarregamento + capacidadeDiariaCarregamentoAdicional;
                        int capacidadeDiariaDisponivel = capacidadeDiariaCarregamentoTotal - capacidadeDiariaUtilizada;

                        listaCapacidadeCarregamentoPeriodo.Add(new
                        {
                            CapacidadeAdicional = capacidadeDiariaCarregamentoAdicional.ToString("n0"),
                            CapacidadeCarregamento = capacidadeDiariaCarregamento.ToString("n0"),
                            CapacidadeDisponivel = capacidadeDiariaDisponivel.ToString("n0"),
                            CapacidadeUtilizada = capacidadeDiariaUtilizada.ToString("n0"),
                            Periodo = "Capacidade Diária",
                            PeriodoAtivo = !encontrouPeriodoAtivo,
                            PossuiProdutividade = centroCarregamentoProdutividades.Count > 0,
                            HorasProdutividade = centroCarregamento.HorasTrabalho,
                            ProdutividadeUtilizada = 0
                        });
                    }
                }

                return new JsonpResult(listaCapacidadeCarregamentoPeriodo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarAosDadosDeCapacidadeDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCargasExcedentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool? excedente = true;
                bool? emReserva = false;
                bool apenasCargaNaoEmitidas = true;
                bool somenteCargasPendentes = false;
                int countCargasPendentes = 0;

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasPendentes = ObterCargasPorSituacao(ref countCargasPendentes, excedente, emReserva, apenasCargaNaoEmitidas, somenteCargasPendentes, unitOfWork);

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargaJanelaCarregamento(cargasPendentes, unitOfWork),
                    Total = countCargasPendentes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarAsCargasExcedentes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCargasPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool? excedente = null;
                bool? emReserva = null;
                bool apenasCargaNaoEmitidas = false;
                bool somenteCargasPendentes = true;
                int countCargasPendentes = 0;

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasPendentes = ObterCargasPorSituacao(ref countCargasPendentes, excedente, emReserva, apenasCargaNaoEmitidas, somenteCargasPendentes, unitOfWork);

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargaJanelaCarregamento(cargasPendentes, unitOfWork),
                    Total = countCargasPendentes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarAsCargasPendentes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCargasEmReserva()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool? excedente = true;
                bool? emReserva = true;
                bool apenasCargaNaoEmitidas = true;
                bool somenteCargasPendentes = false;
                int countCargasPendentes = 0;

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasPendentes = ObterCargasPorSituacao(ref countCargasPendentes, excedente, emReserva, apenasCargaNaoEmitidas, somenteCargasPendentes, unitOfWork);

                return new JsonpResult(new
                {
                    Cargas = ObterDadosCargaJanelaCarregamento(cargasPendentes, unitOfWork),
                    Total = countCargasPendentes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarAsCargasEmReserva);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTransportadoresOfertados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCargaJanelaCarregamento);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia = cargaJanelaCarregamento?.CargaJanelaCarregamentoAgrupador ?? cargaJanelaCarregamento;
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamentoReferencia?.CentroCarregamento;

                bool exibirValorFrete = !(
                    centroCarregamento != null &&
                    centroCarregamento.LiberarCargaAutomaticamenteParaTransportadoras &&
                    centroCarregamento.TipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota &&
                    centroCarregamento.LiberarCargaAutomaticamenteParaTransportadorasForaRota
                );

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Transportador, propriedade: "Descricao", tamanho: 30, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                if (exibirValorFrete)
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.ValorDoFrete, propriedade: "ValorFreteTransportador", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                if (configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                {
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TabelaDeFrete, propriedade: "CodigoIntegracaoTabelaFrete", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDoFrete, propriedade: "TipoFreteTabelaFrete", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: Localization.Resources.Gerais.Geral.Situacao, propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                if (cargaJanelaCarregamentoReferencia == null)
                {
                    grid.AdicionaRows(new List<dynamic>());
                    grid.setarQuantidadeTotal(0);

                    return new JsonpResult(grid);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaTransportadoresOfertados = null;
                int totalRegistros = 0;

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

                if (configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                    totalRegistros = repositorioCargaJanelaCarregamentoTransportador.ContarConsultaPorCargaJanelaCarregamentoDesbloqueada(cargaJanelaCarregamentoReferencia.Codigo);

                if ((totalRegistros > 0) || !configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                    totalRegistros = repositorioCargaJanelaCarregamentoTransportador.ContarConsultaPorCargaJanelaCarregamento(cargaJanelaCarregamentoReferencia.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaTransportadoresOfertadosSemOrdenacao = (totalRegistros > 0) ? repositorioCargaJanelaCarregamentoTransportador.ConsultarPorCargaJanelaCarregamento(cargaJanelaCarregamentoReferencia.Codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

                if (configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                {
                    listaTransportadoresOfertados = (
                        from o in listaTransportadoresOfertadosSemOrdenacao
                        where o.TabelaFreteCliente?.TabelaFrete?.TipoFreteTabelaFrete == TipoFreteTabelaFrete.Terceiro
                        orderby o.Tipo descending, o.Bloqueada descending, o.ObterValorFreteTransportador() ascending
                        select o
                    ).ToList().Concat((
                        from o in listaTransportadoresOfertadosSemOrdenacao
                        where o.TabelaFreteCliente?.TabelaFrete?.TipoFreteTabelaFrete == TipoFreteTabelaFrete.Spot
                        orderby o.Tipo descending, o.Bloqueada descending, o.ObterValorFreteTransportador() ascending
                        select o
                    ).ToList()).Concat((
                        from o in listaTransportadoresOfertadosSemOrdenacao
                        where o.TabelaFreteCliente == null || o.TabelaFreteCliente.TabelaFrete.TipoFreteTabelaFrete == TipoFreteTabelaFrete.NaoInformado
                        orderby o.Tipo descending, o.Bloqueada descending, o.ObterValorFreteTransportador() ascending
                        select o
                    ).ToList()).ToList();
                }
                else if (exibirValorFrete)
                    listaTransportadoresOfertados = listaTransportadoresOfertadosSemOrdenacao.OrderByDescending(o => o.Tipo).ThenByDescending(o => o.Bloqueada).ThenBy(o => o.ObterValorFreteTransportador()).ToList();
                else
                    listaTransportadoresOfertados = listaTransportadoresOfertadosSemOrdenacao.OrderByDescending(o => o.Tipo).ThenByDescending(o => o.Bloqueada).ThenBy(o => o.Codigo).ToList();

                var listaTransportadoresOfertadosRetornar = (
                    from o in listaTransportadoresOfertados
                    select new
                    {
                        o.Codigo,
                        o.Transportador.Descricao,
                        Situacao = o.Situacao.ObterDescricao(),
                        ValorFreteTransportador = o.ObterValorFreteTransportador().ToString("n2"),
                        CodigoIntegracaoTabelaFrete = o.TabelaFreteCliente?.TabelaFrete?.CodigoIntegracao ?? "",
                        TipoFreteTabelaFrete = o.TabelaFreteCliente?.TabelaFrete?.TipoFreteTabelaFrete.ObterDescricao() ?? TipoFreteTabelaFrete.NaoInformado.ObterDescricao(),
                        DT_FontColor = "#666",
                        DT_RowColor = o.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaTransportadoresOfertadosRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarOsTransportadoresOfertados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTransportadoresOfertadosHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaCarregamentoTransportador = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico> historicos = repositorioHistorico.BuscarPorCargaJanelaCarregamentoTransportador(codigoCargaJanelaCarregamentoTransportador);

                var historicosRetornar = (
                    from historico in historicos
                    select new
                    {
                        historico.Codigo,
                        historico.Tipo,
                        Data = historico.Data.ToDateTimeString(),
                        historico.Descricao,
                        Usuario = historico.Usuario?.Descricao ?? ""
                    }
                ).ToList();

                return new JsonpResult(historicosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarHistoricoDoTransportadorOfertado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTransportadoresOfertadosHistoricoOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoHistorico = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta repositorioHistoricoOferta = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta> historicosOferta = repositorioHistoricoOferta.BuscarPorCargaJanelaCarregamentoTransportadorHistorico(codigoHistorico);

                var historicosRetornar = (
                    from historicoOferta in historicosOferta
                    select new
                    {
                        historicoOferta.Codigo,
                        historicoOferta.Ordem,
                        historicoOferta.Descricao,
                        Empresa = historicoOferta.Empresa.Descricao,
                        PercentualCargas = historicoOferta.PercentualCargas.ToString("n2"),
                        PercentualConfigurado = historicoOferta.PercentualConfigurado.ToString("n2"),
                        Prioridade = historicoOferta.Prioridade.ToString("n0"),
                        DT_FontColor = historicoOferta.Tipo.ObterCorFonte(),
                        DT_RowColor = historicoOferta.Tipo.ObterCorLinha()
                    }
                ).ToList();

                return new JsonpResult(historicosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarHistoricoDoTransportadorOfertado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> MandarCargaExcedentesCarregamento()
        {
            var unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoJanelaCarregamento = Request.GetIntParam("JanelaCarregamento");
                bool naoComparecimento = Request.GetBoolParam("NaoComparecimento");
                bool horarioDesencaixado = Request.GetBoolParam("HorarioDesencaixado");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null;

                if (codigoJanelaCarregamento > 0)
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);
                else
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.JanelaDeCarregamentoNaoEncontrada);

                if (horarioDesencaixado)
                {
                    cargaJanelaCarregamento.HorarioDesencaixado = horarioDesencaixado;

                    repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                }

                MandarCargaExcedentesCarregamento(cargaJanelaCarregamento, naoComparecimento, unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoRetornarCargaParaExcedente);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AlocarHorarioCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorCodigo(cargaJanelaCarregamento?.CentroCarregamento?.Codigo ?? 0);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain servicoIntegracaoSaintGobain = new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
                {
                    BloquearJanelaCarregamentoExcedente = true
                };
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, Auditado, configuracaoDisponibilidadeCarregamento);
                DateTime novoHorario = Request.GetDateTimeParam("NovoHorario");

                servicoDisponibilidadeCarregamento.ValidarPermissaoAlterarHorarioCarregamento(cargaJanelaCarregamento);

                cargaJanelaCarregamento.Initialize();
                cargaJanelaCarregamento.HorarioEncaixado = false;
                cargaJanelaCarregamento.NaoComparecido = Request.GetBoolParam("NaoComparecimento") ? TipoNaoComparecimento.NaoCompareceu : TipoNaoComparecimento.Compareceu;

                AtualizarOperadorCarga(cargaJanelaCarregamento.Carga, unitOfWork);
                servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(cargaJanelaCarregamento, novoHorario, TipoServicoMultisoftware, centroCarregamento.NaoBloquearCapacidadeExcedida);
                servicoIntegracaoSaintGobain.ReenviarIntegrarCarregamento(cargaJanelaCarregamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), Localization.Resources.Cargas.Carga.AlteradoHorarioDeCarregamentoPara + cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"), unitOfWork);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(new
                {
                    InicioCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                    PermiteExceder = centroCarregamento?.NaoBloquearCapacidadeExcedida ?? false
                });
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
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarHorarioDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlocarHorarioCarregamentoPorPeriodo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/MontagemCarga");
            bool usuarioPossuiPermissaoSobreporRegrasCarregamento = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.JanelaDescarga_SobreporRegras);

            try
            {
                unitOfWork.Start();

                int codigoJanelaCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
                {
                    BloquearJanelaCarregamentoExcedente = true,
                    PermitirHorarioCarregamentoComLimiteAtingido = Request.GetBoolParam("PermitirHorarioCarregamentoComLimiteAtingido"),
                    PermitirHorarioCarregamentoInferiorAoAtual = Request.GetBoolParam("PermitirHorarioCarregamentoInferiorAoAtual")
                };
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, Auditado, configuracaoDisponibilidadeCarregamento);

                servicoDisponibilidadeCarregamento.ValidarPermissaoAlterarHorarioCarregamento(cargaJanelaCarregamento);

                int codigoPeriodoCarregamento = Request.GetIntParam("PeriodoCarregamento");
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento = repositorioPeriodoCarregamento.BuscarPorCodigo(codigoPeriodoCarregamento);

                if (periodoCarregamento == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PeriodoDeCarregamentoInformadoNaoFoiEncontrado);

                DateTime dataCarregamento = Request.GetDateTimeParam("DataCarregamento");

                cargaJanelaCarregamento.Initialize();
                cargaJanelaCarregamento.CentroCarregamento = periodoCarregamento.CentroCarregamento;

                AtualizarOperadorCarga(cargaJanelaCarregamento.Carga, unitOfWork);
                servicoDisponibilidadeCarregamento.DefinirHorarioCarregamentoPorPeriodo(cargaJanelaCarregamento, periodoCarregamento, dataCarregamento, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), Localization.Resources.Cargas.Carga.AlteradoHorarioDeCarregamentoPara + cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"), unitOfWork);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (usuarioPossuiPermissaoSobreporRegrasCarregamento)
                {
                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioCarregamentoInferiorAtual)
                        return new JsonpResult(new { HorarioCarregamentoInferiorAtual = true }, true, excecao.Message);

                    if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.HorarioLimiteCarregamentoAtingido)
                        return new JsonpResult(new { HorarioLimiteCarregamentoAtingido = true }, true, excecao.Message);
                }

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarHorarioDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterAreasVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento?.Carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Cargas.CargaAreaVeiculo repositorioCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> listaAreaVeiculo = repositorioCargaAreaVeiculo.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                var listaAreaVeiculoRetornar = (
                    from o in listaAreaVeiculo
                    select new
                    {
                        o.AreaVeiculo.Codigo,
                        o.AreaVeiculo.Descricao
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    PermitirEditar = IsPermitirEditarAreasVeiculos(cargaJanelaCarregamento),
                    AreasVeiculos = listaAreaVeiculoRetornar
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultarAsAreasDeVeiculos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterInteressadosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                bool interesseAtualOuHistorico = Request.GetBoolParam("InteresseAtualOuHistorico");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasVinculadas = repositorioCargaVinculada.BuscarCargasPorCarga(cargaReferencia.Codigo);
                List<int> codigosCargasVinculadas = cargasVinculadas.Select(o => o.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportadorVinculadas = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

                if (interesseAtualOuHistorico)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador> situacoes = new List<SituacaoCargaJanelaCarregamentoTransportador>()
                    {
                        SituacaoCargaJanelaCarregamentoTransportador.Disponivel,
                        SituacaoCargaJanelaCarregamentoTransportador.ComInteresse
                    };

                    cargasJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaESituacoesDiferente(cargaReferencia.Codigo, situacoes);
                    cargasJanelaCarregamentoTransportadorVinculadas = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargasESituacoesDiferente(codigosCargasVinculadas, situacoes);
                }
                else
                {
                    cargasJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaESituacao(cargaReferencia.Codigo, SituacaoCargaJanelaCarregamentoTransportador.ComInteresse);
                    cargasJanelaCarregamentoTransportadorVinculadas = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargasESituacao(codigosCargasVinculadas, SituacaoCargaJanelaCarregamentoTransportador.ComInteresse);
                }

                List<int> codigosCargaJanelaCarregamentoTransportador = cargasJanelaCarregamentoTransportador.Select(o => o.Codigo).Concat(cargasJanelaCarregamentoTransportadorVinculadas.Select(o => o.Codigo)).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargasJanelaCarregamentoTransportador(codigosCargaJanelaCarregamentoTransportador);
                bool exibirColunaValorComponentesFrete = !(cargasJanelaCarregamentoTransportador.Where(o => o.CargaJanelaCarregamento.CentroCarregamento != null).FirstOrDefault()?.CargaJanelaCarregamento?.CentroCarregamento?.BloquearComponentesDeFreteJanelaCarregamentoTransportador ?? true);
                bool exibirColunaValorTotalFrete = (exibirColunaValorComponentesFrete || (cargasVinculadas.Count > 0));
                List<dynamic> interessadosCargaRetornar = new List<dynamic>();

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoTransportador", visivel: false);
                grid.AdicionarCabecalho(descricao: "Posição", propriedade: "Posicao", tamanho: 5, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "CNPJ", propriedade: "CnpjTransportador", tamanho: 8, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Razão Social", propriedade: "NomeTransportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                if (configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga)
                {
                    grid.AdicionarCabecalho(descricao: "Placa", propriedade: "Veiculo", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicular", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data Interesse", propriedade: "DataInteresse", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Última Posição Veículo", propriedade: "UltimaPosicaoVeiculo", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: "Horário de Carregamento", propriedade: "HorarioCarregamento", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: $"Valor do Frete{(cargasVinculadas.Count > 0 ? $" ({carga.CodigoCargaEmbarcador})" : "")}", propriedade: $"ValorFrete{carga.Codigo}", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);

                if (exibirColunaValorComponentesFrete)
                    grid.AdicionarCabecalho(descricao: $"Valor dos Componentes{(cargasVinculadas.Count > 0 ? $" ({carga.CodigoCargaEmbarcador})" : "")}", propriedade: $"ValorComponenteFrete{carga.Codigo}", tamanho: 12, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaVinculada in cargasVinculadas)
                {
                    grid.AdicionarCabecalho(descricao: $"Valor do Frete ({cargaVinculada.CodigoCargaEmbarcador})", propriedade: $"ValorFrete{cargaVinculada.Codigo}", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);

                    if (!(cargasJanelaCarregamentoTransportadorVinculadas.Where(o => o.CargaJanelaCarregamento.Carga.Codigo == cargaVinculada.Codigo && o.CargaJanelaCarregamento.CentroCarregamento != null).FirstOrDefault()?.CargaJanelaCarregamento?.CentroCarregamento?.BloquearComponentesDeFreteJanelaCarregamentoTransportador ?? true))
                        grid.AdicionarCabecalho(descricao: $"Valor dos Componentes ({cargaVinculada.CodigoCargaEmbarcador})", propriedade: $"ValorComponenteFrete{cargaVinculada.Codigo}", tamanho: 12, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);
                }

                if (exibirColunaValorTotalFrete)
                    grid.AdicionarCabecalho(descricao: $"Valor Total de Frete", propriedade: "ValorTotalFrete", tamanho: 8, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga> interessesCarga = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga interesseCarga = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga();

                    interesseCarga.Codigo = cargaJanelaCarregamentoTransportador.Codigo;
                    interesseCarga.CodigoCarga = carga.Codigo;
                    interesseCarga.CodigoTransportador = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador.Terceiro.CPF_CNPJ : (double)cargaJanelaCarregamentoTransportador.Transportador.Codigo;
                    interesseCarga.CnpjTransportador = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador.Terceiro.CPF_CNPJ_Formatado : cargaJanelaCarregamentoTransportador.Transportador.CNPJ_Formatado;
                    interesseCarga.NomeTransportador = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? cargaJanelaCarregamentoTransportador.Terceiro.Nome : cargaJanelaCarregamentoTransportador.Transportador.RazaoSocial;
                    interesseCarga.HorarioCarregamento = cargaJanelaCarregamentoTransportador.HorarioCarregamento;
                    interesseCarga.ValorFrete = cargaJanelaCarregamentoTransportador.ObterValorFreteTransportador();
                    interesseCarga.ValorComponentesFrete = componentesFrete.Where(o => o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador.Codigo).Select(o => o.ValorComponente).Sum();
                    interesseCarga.ValorTotalFrete += (interesseCarga.ValorFrete + interesseCarga.ValorComponentesFrete);
                    interesseCarga.DataLance = cargaJanelaCarregamentoTransportador.DataInteresse;

                    if (configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga)
                    {
                        Dominio.Entidades.Embarcador.Logistica.PosicaoAtual localAtual = repositorioPosicaoAtual.BuscarPorVeiculo(cargaJanelaCarregamentoTransportador.DadosTransporte?.Veiculo?.Codigo ?? 0);

                        interesseCarga.Veiculo = cargaJanelaCarregamentoTransportador.DadosTransporte?.Veiculo?.Placa ?? "";
                        interesseCarga.ModeloVeicular = cargaJanelaCarregamentoTransportador.DadosTransporte?.Veiculo?.ModeloVeicularCarga?.Descricao ?? "";
                        interesseCarga.DataInteresse = cargaJanelaCarregamentoTransportador.DataInteresse;
                        interesseCarga.UltimaPosicaoVeiculo = localAtual?.Descricao ?? "";
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaVinculada in cargasVinculadas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCargaVinculada interesseCargaVinculada = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCargaVinculada();
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorPorCargaVinculada = cargasJanelaCarregamentoTransportadorVinculadas.Where(o => o.Transportador.Codigo == cargaJanelaCarregamentoTransportador.Transportador.Codigo && o.CargaJanelaCarregamento.Carga.Codigo == cargaVinculada.Codigo).FirstOrDefault();

                        interesseCargaVinculada.CodigoCarga = cargaVinculada.Codigo;

                        if (cargaJanelaCarregamentoTransportadorPorCargaVinculada != null)
                        {
                            interesseCargaVinculada.ValorFrete = cargaJanelaCarregamentoTransportadorPorCargaVinculada.ObterValorFreteTransportador();
                            interesseCargaVinculada.ValorComponentesFrete = componentesFrete.Where(o => o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportadorPorCargaVinculada.Codigo).Select(o => o.ValorComponente).Sum();
                            interesseCarga.ValorTotalFrete += (interesseCargaVinculada.ValorFrete + interesseCargaVinculada.ValorComponentesFrete);
                        }

                        interesseCarga.CargasVinculadas.Add(interesseCargaVinculada);
                    }

                    interessesCarga.Add(interesseCarga);
                }

                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga> interessesCargaOrdenados = interessesCarga.OrderBy(o => o.ValorTotalFrete).ThenBy(o => o.DataLance).ToList();
                int posicaoAtual = 0;

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga interesseCarga in interessesCargaOrdenados)
                {
                    ExpandoObject interessadoCarga = new ExpandoObject();
                    IDictionary<string, object> dicionarioInteressadoCarga = (IDictionary<string, object>)interessadoCarga;

                    dicionarioInteressadoCarga.Add("Codigo", interesseCarga.Codigo);
                    dicionarioInteressadoCarga.Add("CodigoTransportador", interesseCarga.CodigoTransportador);
                    dicionarioInteressadoCarga.Add("Posicao", ++posicaoAtual);
                    dicionarioInteressadoCarga.Add("CnpjTransportador", interesseCarga.CnpjTransportador);
                    dicionarioInteressadoCarga.Add("NomeTransportador", interesseCarga.NomeTransportador);

                    if (configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga)
                    {
                        dicionarioInteressadoCarga.Add("Veiculo", interesseCarga.Veiculo);
                        dicionarioInteressadoCarga.Add("ModeloVeicular", interesseCarga.ModeloVeicular);
                        dicionarioInteressadoCarga.Add("DataInteresse", interesseCarga.DataInteresse?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty);
                        dicionarioInteressadoCarga.Add("UltimaPosicaoVeiculo", interesseCarga.UltimaPosicaoVeiculo);
                    }

                    dicionarioInteressadoCarga.Add("HorarioCarregamento", interesseCarga.HorarioCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty);
                    dicionarioInteressadoCarga.Add($"ValorFrete{interesseCarga.CodigoCarga}", interesseCarga.ValorFrete.ToString("n2"));
                    dicionarioInteressadoCarga.Add($"ValorComponenteFrete{interesseCarga.CodigoCarga}", interesseCarga.ValorComponentesFrete.ToString("n2"));

                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCargaVinculada cargaVinculada in interesseCarga.CargasVinculadas)
                    {
                        dicionarioInteressadoCarga.Add($"ValorFrete{cargaVinculada.CodigoCarga}", cargaVinculada.ValorFrete.ToString("n2"));
                        dicionarioInteressadoCarga.Add($"ValorComponenteFrete{cargaVinculada.CodigoCarga}", cargaVinculada.ValorComponentesFrete.ToString("n2"));
                    }

                    dicionarioInteressadoCarga.Add("ValorTotalFrete", interesseCarga.ValorTotalFrete.ToString("n2"));

                    interessadosCargaRetornar.Add(interessadoCarga);
                }

                grid.AdicionaRows(interessadosCargaRetornar);
                grid.setarQuantidadeTotal(interessadosCargaRetornar.Count);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultarOsInteressados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterVisualizacoesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repCargaJanelaCarregamentoTransportadorHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                int codigoCarga = Request.GetIntParam("Carga");

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoTransportador", visivel: false);
                grid.AdicionarCabecalho(descricao: "CNPJ", propriedade: "CnpjTransportador", tamanho: 8, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Razão Social", propriedade: "NomeTransportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Data Visualização", propriedade: "DataVisualizacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Usuário", propriedade: "Usuario", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico> listaHistorico = repCargaJanelaCarregamentoTransportadorHistorico.ConsultarPorCargaESituacao(codigoCarga, TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga);
                grid.setarQuantidadeTotal(repCargaJanelaCarregamentoTransportadorHistorico.ContarPorCargaESituacao(codigoCarga, TipoCargaJanelaCarregamentoTransportadorHistorico.VisualizouCarga));

                var lista = (from p in listaHistorico
                             select new
                             {
                                 p.Codigo,
                                 CodigoTransportador = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? p.CargaJanelaCarregamentoTransportador.Terceiro.CPF_CNPJ : (double)p.CargaJanelaCarregamentoTransportador.Transportador.Codigo,
                                 CnpjTransportador = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? p.CargaJanelaCarregamentoTransportador.Terceiro.CPF_CNPJ_Formatado : p.CargaJanelaCarregamentoTransportador.Transportador.CNPJ_Formatado,
                                 NomeTransportador = configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros ? p.CargaJanelaCarregamentoTransportador.Terceiro.Nome : p.CargaJanelaCarregamentoTransportador.Transportador.RazaoSocial,
                                 DataVisualizacao = p.Data.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 Usuario = p.Usuario?.Nome ?? string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultarOsInteressados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> ObterHistoricoCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao> historicos = repositorioHistorico.BuscarPorCargaJanelaCarregamento(codigoCargaJanelaCarregamento);

                var historicosRetornar = (
                    from historico in historicos
                    select new
                    {
                        historico.Codigo,
                        Data = historico.Data.ToDateTimeString(),
                        historico.Descricao
                    }
                ).ToList();

                return new JsonpResult(historicosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarHistoricoDaCotacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaInformacoesCargas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Grid grid = ObterGridPesquisaInformacoesCargas(unidadeDeTrabalho);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirValoresCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                bool confirmado = Request.GetBoolParam("Confirmado");

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportador = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

                if (confirmado)
                {
                    listaCargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCargaESituacaoDiferente(codigoCarga, SituacaoCargaJanelaCarregamentoTransportador.Disponivel);
                }
                else
                {
                    listaCargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCargaComInteresse(codigoCarga);
                }

                if (listaCargaJanelaCarregamentoTransportador.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NenhumaCotacaoEncontrada);

                byte[] arquivo = ReportRequest.WithType(ReportType.ValoresCotacao)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", codigoCarga)
                    .AddExtraData("Confirmado", confirmado)
                    .CallReport()
                    .GetContentFile();

                return Arquivo(arquivo, "application/pdf", "ValoresCotacao.pdf");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoGerarRelatorioDosValoresDaCotacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInteressadoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                double codigoTransportador = Request.GetDoubleParam("Transportador");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Empresa transportador = null;
                Dominio.Entidades.Cliente transportadorTerceiro = null;
                if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                {
                    transportadorTerceiro = repositorioCliente.BuscarPorCPFCNPJ(codigoTransportador);

                    if (transportadorTerceiro == null)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);
                }
                else
                {
                    transportador = repositorioEmpresa.BuscarPorCodigo((int)codigoTransportador);

                    if (transportador == null)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga.CargaAgrupamento ?? carga;
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao servicoCargaJanelaCarregamentoCotacaoAprovacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacaoAprovacao(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
                Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracaoEmbarcador);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = null;
                if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta.ObterCargasJanelaCarregamentoTransportadorTerceiro(cargaReferencia.Codigo, transportadorTerceiro, retornarCargasOriginais: true);
                else
                    cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(cargaReferencia.Codigo, transportador, retornarCargasOriginais: true);

                if (cargasJanelaCarregamentoTransportador.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == cargaReferencia.Codigo select o).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento;

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento;

                    if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.SemTransportador)
                        throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelConfirmarInteressePoisEssaCargaJaTeveOutroTransportadorComInteresseConfirmado);

                    if ((cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Cancelada) || (cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                        throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelConfirmarInteressePoisEssaCargaPoisMesmaJaFoiCancelada);

                    if ((cargaJanelaCarregamento.Carga.CargaAgrupamento == null) && (cargaJanelaCarregamento.Carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.NaoInformada) && (cargaJanelaCarregamento.Carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.Aprovada))
                        throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelConfirmarUmaTransportadoraAntesDaAprovacaoDoValorDoFreteDaCarga);

                    if (cargaJanelaCarregamentoTransportador.HorarioCarregamento.HasValue)
                    {
                        if (!cargaJanelaCarregamento.Excedente && cargaJanelaCarregamentoTransportador.HorarioCarregamento.Value > cargaJanelaCarregamento.InicioCarregamento)
                            throw new ControllerException(Localization.Resources.Cargas.Carga.ParaSelecionarEsseTransportadorHoraDoCarregamentoDeveSerIgualOuSuperiorHoraQueElePodeAtender);
                    }

                    if (cargaJanelaCarregamento.DataSituacaoAtual.HasValue && cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.TempoBloqueioEscolhaTransportador > 0)
                    {
                        double minutosSituacao = (DateTime.Now - cargaJanelaCarregamento.DataSituacaoAtual.Value).TotalMinutes;

                        if (minutosSituacao < cargaJanelaCarregamento.CentroCarregamento.TempoBloqueioEscolhaTransportador)
                            throw new ControllerException(Localization.Resources.Cargas.Carga.NaopossivelSetarTransportadorDaCargaPoisCargaDeveFicarDisponivelPorMais + (cargaJanelaCarregamento.CentroCarregamento.TempoBloqueioEscolhaTransportador - minutosSituacao).ToString("n0") + Localization.Resources.Cargas.Carga.MinutosParaVisualizacaoDosTransportadoresNaJanelaDeCarregamento);
                    }

                    if (cargaJanelaCarregamento.Carga.CargaAgrupamento == null)
                    {
                        if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                        {
                            servicoCargaJanelaCarregamentoTransportadorTerceiro.ValidarPermissaoMarcarInteresseCarga(cargaJanelaCarregamentoTransportador.Terceiro, carga);
                            servicoCargaJanelaCarregamentoTransportadorTerceiro.ValidarCargasInteressadas(cargaJanelaCarregamentoTransportador.Terceiro, carga);
                        }
                        else
                        {
                            servicoCargaJanelaCarregamentoTransportador.ValidarPermissaoMarcarInteresseCarga(cargaJanelaCarregamentoTransportador.Transportador, carga);
                            servicoCargaJanelaCarregamentoTransportador.ValidarCargasInteressadas(cargaJanelaCarregamentoTransportador.Transportador, carga);
                        }
                    }

                    if (cargaJanelaCarregamento.Codigo == cargaJanelaCarregamentoReferencia.Codigo)
                        servicoCargaJanelaCarregamentoCotacaoAprovacao.CriarAprovacao(cargaJanelaCarregamentoReferencia, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, null, Localization.Resources.Cargas.Carga.AdicionouInteresseNaCarga, unitOfWork);

                    if (cargaJanelaCarregamentoReferencia.SituacaoCotacao.IsPendenteAprovacao())
                        continue;

                    if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                    {
                        if (((cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.PermitirTransportadorInformarValorFrete) || cargaJanelaCarregamento.CargaLiberadaCotacao) && cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0)
                            servicoCargaJanelaCarregamentoTransportadorTerceiro.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                        else
                            servicoCargaJanelaCarregamentoTransportadorTerceiro.DefinirTransportadorSemValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);

                        if (configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga)
                        {
                            SalvarDadosTransporte(carga, unitOfWork, cargaJanelaCarregamentoTransportador.DadosTransporte?.Veiculo, cargaJanelaCarregamentoTransportador.DadosTransporte?.Motorista);
                            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento.Carga, TipoServicoMultisoftware);
                        }

                    }
                    else
                    {
                        if (((cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.PermitirTransportadorInformarValorFrete) || cargaJanelaCarregamento.CargaLiberadaCotacao) && cargaJanelaCarregamentoTransportador.ValorFreteTransportador > 0)
                            servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                        else
                            servicoCargaJanelaCarregamentoTransportador.DefinirTransportadorSemValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, TipoServicoMultisoftware, Usuario);
                    }

                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
                }

                if (!cargaJanelaCarregamentoReferencia.SituacaoCotacao.IsPendenteAprovacao())
                {
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaSemTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento);
                    servicoCargaJanelaCarregamentoNotificacao.NotificarCotacaoComTransportadorEscolhido(cargaJanelaCarregamentoTransportadorReferencia, cargasJanelaCarregamentoTransportador, TipoServicoMultisoftware);
                }

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
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultarOsInteressados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarObservacaoFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamentoReferencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                string observacaoFluxoPatio = Request.GetNullableStringParam("Observacao");
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoComCargasAgrupadas(cargaJanelaCarregamentoReferencia);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                {
                    cargaJanelaCarregamento.Initialize();
                    cargaJanelaCarregamento.ObservacaoFluxoPatio = observacaoFluxoPatio;

                    repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento, Auditado);
                }

                unitOfWork.CommitChanges();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAtualizarObservacaoDoFluxoDePatio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarObservacaoTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<(string NumeroCarga, string MensagemRetorno, bool Sucesso)> retornos = new List<(string NumeroPreCarga, string MensagemRetorno, bool Sucesso)>();

            try
            {
                List<int> codigosJanelasCarregamento = Request.GetListParam<int>("Codigos");
                string observacaoTransportador = Request.GetNullableStringParam("Observacao");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                HashSet<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamentoAtualizadas = new HashSet<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                foreach (int codigoJanelaCarregamento in codigosJanelasCarregamento)
                {
                    string numeroCarga = string.Empty;

                    try
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);
                        numeroCarga = cargaJanelaCarregamentoReferencia.CargaBase.Numero;

                        if (!IsCargaJanelaCarregamentoEditavel(cargaJanelaCarregamentoReferencia, operadorLogistica))
                            throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelAlterarNaAtualSituacao);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoComCargasAgrupadas(cargaJanelaCarregamentoReferencia);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                        {
                            cargaJanelaCarregamento.Initialize();

                            cargaJanelaCarregamento.ObservacaoTransportador = servicoCargaJanelaCarregamento.VerificarEGerarObservacaoFinal(observacaoTransportador, cargaJanelaCarregamento.Carga.ObservacaoTransportador);
                            cargaJanelaCarregamento.ObservacaoTransportadorInformadaManualmente = true;
                            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento, Auditado);
                            cargasJanelaCarregamentoAtualizadas.Add(cargaJanelaCarregamento);
                        }

                        unitOfWork.CommitChanges();

                        retornos.Add(ValueTuple.Create(numeroCarga, Localization.Resources.Cargas.Carga.ObservacaoAoTransportadorAlteradaComSucesso, true));
                    }
                    catch (ControllerException excecao)
                    {
                        unitOfWork.Rollback();
                        retornos.Add(ValueTuple.Create(numeroCarga, excecao.Message, false));
                    }
                    catch (Exception excecao)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecao);
                        retornos.Add(ValueTuple.Create(numeroCarga, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAtualizarObservacaoAoTransportador, false));
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamentoAtualizadas)
                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAtualizarObservacaoAoTransportador);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new JsonpResult((
                from retorno in retornos
                select new
                {
                    retorno.NumeroCarga,
                    retorno.MensagemRetorno,
                    retorno.Sucesso,
                    DT_RowColor = retorno.Sucesso ? "#99ff99" : "#ffb3b3"
                }
            ).ToList());
        }

        public async Task<IActionResult> AlterarPrioridade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                int novaPrioridade = Request.GetIntParam("NovaPrioridade");
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoPrioridade servicoCargaJanelaCarregamentoPrioridade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoPrioridade(unitOfWork, Auditado);

                servicoCargaJanelaCarregamentoPrioridade.AlterarPrioridade(cargaJanelaCarregamento, novaPrioridade);
                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarPrioridadeDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarAreasVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<(string NumeroCarga, string MensagemRetorno, bool Sucesso)> retornos = new List<(string NumeroPreCarga, string MensagemRetorno, bool Sucesso)>();

            try
            {
                List<int> codigosJanelasCarregamento = Request.GetListParam<int>("Codigos");
                List<int> codigosAreasVeiculos = Request.GetListParam<int>("AreasVeiculos");
                Repositorio.Embarcador.Cargas.CargaAreaVeiculo repositorioCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                foreach (int codigoJanelaCarregamento in codigosJanelasCarregamento)
                {
                    string numeroCarga = string.Empty;

                    try
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);
                        numeroCarga = cargaJanelaCarregamento.CargaBase.Numero;

                        if (!IsCargaJanelaCarregamentoEditavel(cargaJanelaCarregamento, operadorLogistica) || !IsPermitirEditarAreasVeiculos(cargaJanelaCarregamento))
                            throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelAlterarNaAtualSituacao);

                        repositorioCargaAreaVeiculo.DeletarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                        foreach (int codigoAreaVeiculo in codigosAreasVeiculos)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo cargaAreaVeiculo = new Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo()
                            {
                                AreaVeiculo = new Dominio.Entidades.Embarcador.Logistica.AreaVeiculo() { Codigo = codigoAreaVeiculo },
                                Carga = cargaJanelaCarregamento.Carga
                            };

                            repositorioCargaAreaVeiculo.Inserir(cargaAreaVeiculo);
                        }

                        unitOfWork.CommitChanges();

                        retornos.Add(ValueTuple.Create(numeroCarga, Localization.Resources.Cargas.Carga.AreasDeVeiculosAtualizadasComSucesso, true));
                    }
                    catch (ControllerException excecao)
                    {
                        unitOfWork.Rollback();
                        retornos.Add(ValueTuple.Create(numeroCarga, excecao.Message, false));
                    }
                    catch (Exception excecao)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecao);
                        retornos.Add(ValueTuple.Create(numeroCarga, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoSalvarAsAreasDeVeiculos, false));
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoSalvarAsAreasDeVeiculos);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new JsonpResult((
                from retorno in retornos
                select new
                {
                    retorno.NumeroCarga,
                    retorno.MensagemRetorno,
                    retorno.Sucesso,
                    DT_RowColor = retorno.Sucesso ? "#99ff99" : "#ffb3b3"
                }
            ).ToList());
        }

        public async Task<IActionResult> SalvarLocalCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                if (cargaJanelaCarregamento.PreCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarPreCargaVinculadaJanelaDeCarregamento);

                int codigoLocalCarregamento = Request.GetIntParam("LocalCarregamento");
                Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioAreaVeiculoPosicao = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao localCarregamento = repositorioAreaVeiculoPosicao.BuscarPorCodigo(codigoLocalCarregamento);

                if (localCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarLocalDeCarregamento);

                cargaJanelaCarregamento.PreCarga.LocalCarregamento = localCarregamento;

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

                repositorioPreCarga.Atualizar(cargaJanelaCarregamento.PreCarga);

                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoSalvarLocalDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarTipoTransportadorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<(int CodigoCarga, string NumeroCarga, string MensagemRetorno, int CodigoErro)> retornos = new List<(int CodigoCarga, string NumeroPreCarga, string MensagemRetorno, int CodigoErro)>();

            try
            {
                List<int> codigosJanelasCarregamento = Request.GetListParam<int>("Codigos");
                string observacaoTransportador = Request.GetStringParam("ObservacaoTransportador");
                int codigoTransportador = Request.GetIntParam("Transportador");
                TipoTransportadorCentroCarregamento tipoTransportador = Request.GetEnumParam<TipoTransportadorCentroCarregamento>("TipoTransportador");
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa transportador = (codigoTransportador > 0) ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) : null;

                if ((tipoTransportador == TipoTransportadorCentroCarregamento.TransportadorExclusivo) && (transportador == null))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.SelecioneUmTransportadorOuUmTipoDeTransportadorValido);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                HashSet<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamentoAtualizadas = new HashSet<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracaLeilaoManual = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverLeilaoManual);

                foreach (int codigoJanelaCarregamento in codigosJanelasCarregamento)
                {
                    int codigoCarga = 0;
                    string numeroCarga = string.Empty;

                    try
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoJanelaCarregamento);
                        codigoCarga = cargaJanelaCarregamentoReferencia.CargaBase?.Codigo ?? 0;
                        numeroCarga = cargaJanelaCarregamentoReferencia.CargaBase?.Numero ?? string.Empty;

                        if (!IsCargaJanelaCarregamentoEditavel(cargaJanelaCarregamentoReferencia, operadorLogistica) || !IsPermitirLiberarParaTransportador(cargaJanelaCarregamentoReferencia))
                            throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelAlterarNaAtualSituacao);

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !cargaJanelaCarregamentoReferencia.Carga.ExigeNotaFiscalParaCalcularFrete)
                        {
                            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);

                            if (!servicoCargaLocaisPrestacao.ValidarPassagensMDFe(cargaJanelaCarregamentoReferencia.Carga, unitOfWork))
                            {
                                unitOfWork.Rollback();
                                retornos.Add(ValueTuple.Create(codigoCarga, numeroCarga, Localization.Resources.Cargas.Carga.AntesDeLiberarCargaParaOsTransportadoresNecessarioConfigurarPercursoValidoParaGerarOsMDFEs, 3));
                                continue;
                            }
                        }

                        if ((cargaJanelaCarregamentoReferencia.CentroCarregamento?.LiberarCargaManualmenteParaTransportadores ?? false) && (cargaJanelaCarregamentoReferencia.CentroCarregamento?.PermitirInformarAreaVeiculoJanelaCarregamento ?? false))
                        {
                            Repositorio.Embarcador.Cargas.CargaAreaVeiculo repositorioCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unitOfWork);

                            if (!repositorioCargaAreaVeiculo.ExistePorCarga(cargaJanelaCarregamentoReferencia.Carga.Codigo))
                                throw new ControllerException(Localization.Resources.Cargas.Carga.AsAreasDeVeiculosDevemSerInformadasAntesDeLiberarCargaParaOsTransportadores);
                        }

                        PermiteLiberarPorTipoTransportador(cargaJanelaCarregamentoReferencia, tipoTransportador);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoComCargasAgrupadas(cargaJanelaCarregamentoReferencia);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                        {
                            InformarTipoTransportadorCarga(cargaJanelaCarregamento, tipoTransportador, transportador, observacaoTransportador, configuracaoEmbarcador, configuracaoJanelaCarregamento, unitOfWork);
                            cargasJanelaCarregamentoAtualizadas.Add(cargaJanelaCarregamento);
                        }

                        if (existeTipoIntegracaLeilaoManual != null)
                            Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(cargaJanelaCarregamentoReferencia.Carga, existeTipoIntegracaLeilaoManual, unitOfWork, false, false);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoReferencia, null, string.Format(Localization.Resources.Cargas.Carga.InformouTipoDeTransportador, $"({tipoTransportador.ObterDescricao()})"), unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamentoReferencia, null, string.Format(Localization.Resources.Cargas.Carga.LiberouCarga, numeroCarga), unitOfWork);
                        unitOfWork.CommitChanges();

                        retornos.Add(ValueTuple.Create(codigoCarga, numeroCarga, Localization.Resources.Cargas.Carga.TipoDeTransportadorSelecionadoComSucesso, 0));
                    }
                    catch (BaseException excecao)
                    {
                        unitOfWork.Rollback();
                        retornos.Add(ValueTuple.Create(codigoCarga, numeroCarga, excecao.Message, 1));
                    }
                    catch (Exception excecao)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecao);
                        retornos.Add(ValueTuple.Create(codigoCarga, numeroCarga, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoInformarTipoDeTransportador, 2));
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamentoAtualizadas)
                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoInformarTipoDeTransportador);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return new JsonpResult((
                from retorno in retornos
                select new
                {
                    retorno.CodigoCarga,
                    retorno.NumeroCarga,
                    retorno.MensagemRetorno,
                    retorno.CodigoErro,
                    DT_RowColor = retorno.CodigoErro == 0 ? "#99ff99" : "#ffb3b3"
                }
            ).ToList());
        }

        public async Task<IActionResult> SalvarTipoTransportadorCargaPorDataCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                if (!configuracaoEmbarcador.PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoPermitidoInformarTipoDoTransportadorPorDataDeCarregamento);

                int codigoTransportador = Request.GetIntParam("Transportador");
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa transportador = (codigoTransportador > 0) ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) : null;
                TipoTransportadorCentroCarregamento tipoTransportador = Request.GetEnumParam<TipoTransportadorCentroCarregamento>("TipoTransportador");

                if ((tipoTransportador == TipoTransportadorCentroCarregamento.TransportadorExclusivo) && (transportador == null))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.SelecioneUmTransportadorOuUmTipoDeTransportadorValido);

                DateTime dataCarregamento = Request.GetDateTimeParam("DataCarregamento");
                string observacaoTransportador = Request.GetStringParam("ObservacaoTransportador");
                Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarCargasLiberarParaTransportadores(dataCarregamento);
                HashSet<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamentoAtualizadas = new HashSet<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamento)
                {
                    if (!cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete && !servicoCargaLocaisPrestacao.ValidarPassagensMDFe(cargaJanelaCarregamento.Carga, unitOfWork))
                        continue;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> carregamentosJanelas = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoComCargasAgrupadas(cargaJanelaCarregamento);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento carregamentoJanela in carregamentosJanelas)
                    {
                        InformarTipoTransportadorCarga(carregamentoJanela, tipoTransportador, transportador, observacaoTransportador, configuracaoEmbarcador, configuracaoJanelaCarregamento, unitOfWork);
                        cargasJanelaCarregamentoAtualizadas.Add(carregamentoJanela);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, null, string.Format(Localization.Resources.Cargas.Carga.InformouTipoDeTransportador, $"({tipoTransportador.ObterDescricao()})"), unitOfWork);
                }

                unitOfWork.CommitChanges();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in cargasJanelaCarregamentoAtualizadas)
                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

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
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoInformarTipoDeTransportador);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacaoGuarita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = codigo > 0 ? repositorioCargaJanelaCarregamentoGuarita.BuscarPorCarga(codigo) : null;

                if (guarita == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.RegistroNaoFoiEncontrado);

                Servicos.Embarcador.GestaoPatio.Guarita servicoGuarita = new Servicos.Embarcador.GestaoPatio.Guarita(unitOfWork, Auditado);

                unitOfWork.Start();

                servicoGuarita.SalvarObservacao(guarita, observacao);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoSalvarObservacaoDaGuarita);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> InformarCargaAtualizada()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento != null && cargaJanelaCarregamento.CentroCarregamento != null)
                    new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoInformarAtualizacaoDaCarga);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> DispararNotificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamento = Request.GetIntParam("JanelaCargaAtualizada");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorio.BuscarPorCodigo(codigoJanelaCarregamento);

                if (cargaJanelaCarregamento != null)
                    new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoDispararNotificacaoDeCargaAtualizada);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BloquearCargaCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.JanelaDeCarregamentoNaoEncontrada);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, Auditado);

                servicoCargaJanelaCarregamentoCotacao.ValidarPermissaoBloquearParaCotacao(cargaJanelaCarregamento);

                if (configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores)
                {
                    servicoCargaJanelaCarregamento.AlterarSituacao(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores);
                    repositorioCargaJanelaCarregamentoTransportador.DeletarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);
                }

                servicoCargaJanelaCarregamentoCotacao.BloquearParaCotacao(cargaJanelaCarregamento, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBloquearCargaParaCotacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BloquearCargaFilaCarregamento()
        {
            var unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamento = Request.GetIntParam("Codigo");

                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoFilaCarregamento(unidadeDeTrabalho, Auditado).BloquearCargaFilaCarregamentoPorJanelaCarregamento(codigoJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBloquearCargaParaFilaDeCarregamentoManualmente);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DescartarCargaCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCargaJanelaCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.JanelaDeCarregamentoNaoEncontrada);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                if (!configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores || cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NãoPossivelDescartarCotacaoCarga);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, Auditado);

                servicoCargaJanelaCarregamentoCotacao.DescartarCotacao(cargaJanelaCarregamento, TipoServicoMultisoftware);

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
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoDescartarCotacaoCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarCargaCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaCarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.JanelaDeCarregamentoNaoEncontrada);

                unitOfWork.Start();

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, Auditado);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, ConfiguracaoEmbarcador, null);

                servicoCargaJanelaCarregamentoCotacao.ValidarPermissaoLiberarParaCotacao(cargaJanelaCarregamento);
                servicoCargaJanelaCarregamentoCotacao.LiberarParaCotacaoManualmente(cargaJanelaCarregamento, TipoServicoMultisoftware);
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(cargaJanelaCarregamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoLiberarCargaParaCotacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarCargaFilaCarregamento()
        {
            var unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaCarregamento = Request.GetIntParam("Codigo");

                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoFilaCarregamento(unidadeDeTrabalho, Auditado).LiberarCargaFilaCarregamentoPorJanelaCarregamento(codigoJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoLiberarCargaParaFilaDeCarregamentoManualmente);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DesagendarCargaFilaCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = servicoCargaJanelaCarregamento.DesagendarCarga(carga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada ? Localization.Resources.Cargas.Carga.CargaDesagendadaViaJanelaDeCarregamento : Localization.Resources.Cargas.Carga.DesagendamentoDaCargaSolicitadoViaJanelaDeCarregamento), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada ? Localization.Resources.Cargas.Carga.DesagendadaDaFilaDeCarregamentoComSucesso : Localization.Resources.Cargas.Carga.DesagendamentoSolicitadoComSucessoCargaFicaraAguardandoCancelamentoSerFinalizado);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaDesagendarCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoAlteracaoHorario()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>()
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Cargas.Carga.NumeroDaCarga, Propriedade = "CodigoCargaEmbarcador", Tamanho = 80 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Cargas.Carga.Filial, Propriedade = "CodigoFilialEmbarcador", Tamanho = 80 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Cargas.Carga.DataCarregamento, Propriedade = "DataCarregamento", Tamanho = 80 }
            };

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarAlteracaoHorario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
                {
                    PermitirHorarioCarregamentoComLimiteAtingido = true
                };
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, Auditado, configuracaoDisponibilidadeCarregamento);
                Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(Request.GetStringParam("Dados"));

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
                {
                    Importados = 0,
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>(),
                    Total = linhas.Count
                };

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCodigoCargaEmbarcador = (from obj in linha.Colunas where obj.NomeCampo == "CodigoCargaEmbarcador" select obj).FirstOrDefault();
                        string codigoCargaEmbarcador = (string)colunaCodigoCargaEmbarcador.Valor;

                        if (string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
                            throw new ControllerException(Localization.Resources.Cargas.Carga.NumeroDaCargaNaoFoiInformado);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCodigoFilialEmbarcador = (from obj in linha.Colunas where obj.NomeCampo == "CodigoFilialEmbarcador" select obj).FirstOrDefault();
                        string codigoFilialEmbarcador = (string)colunaCodigoFilialEmbarcador.Valor;

                        if (string.IsNullOrWhiteSpace(codigoFilialEmbarcador))
                            throw new ControllerException(Localization.Resources.Cargas.Carga.FilialNaoFoiInformada);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDataCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "DataCarregamento" select obj).FirstOrDefault();
                        DateTime? dataCarregamento = ((string)colunaDataCarregamento.Valor).ToNullableDateTime();

                        if (!dataCarregamento.HasValue)
                            throw new ControllerException(Localization.Resources.Cargas.Carga.DataDeCarregamentoNaoEstaNoFormatoCorreto);

                        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigoEmbarcadorDeCargaEFilial(codigoCargaEmbarcador.Trim(), codigoFilialEmbarcador.Trim());

                        if (cargaJanelaCarregamento == null)
                            throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarJanelaDeCarregamento);

                        cargaJanelaCarregamento.Initialize();
                        cargaJanelaCarregamento.HorarioEncaixado = false;
                        servicoDisponibilidadeCarregamento.ValidarPermissaoAlterarHorarioCarregamento(cargaJanelaCarregamento);
                        servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(cargaJanelaCarregamento, dataCarregamento.Value, TipoServicoMultisoftware);
                        Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), Localization.Resources.Cargas.Carga.DataDeCarregamentoAlterada + (cargaJanelaCarregamento.Excedente ? "" : Localization.Resources.Cargas.Carga.Para + cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")), unitOfWork);

                        unitOfWork.CommitChanges();

                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = 1, mensagemFalha = "", processou = true });
                        retornoImportacao.Importados++;

                        servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
                    }
                    catch (BaseException excecaoPorLinha)
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = 1, mensagemFalha = excecaoPorLinha.Message });
                    }
                    catch (Exception excecaoPorLinha)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecaoPorLinha);
                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = 1, mensagemFalha = Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoProcessarLinha });
                    }
                }

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EncaminharTransportadorRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.RegistroNaoEncontrado);

                if (!IsPermitirLiberarParaShare(cargaJanelaCarregamento))
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.AtualSituacaoDaJanelaNaoPermiteEncaminharTransportadorRota);

                cargaJanelaCarregamento.DataLiberacaoShare = DateTime.Now;
                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmErroAoEncaminharParaTransportadorRota);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDataPrevisaoChegada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.RegistroNaoEncontrado);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);

                DateTime dataPrevisaoChegada = Request.GetDateTimeParam("DataPrevisaoChegada");

                if (dataPrevisaoChegada == DateTime.MinValue)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.DataObrigatoria);

                servicoCargaJanelaCarregamento.AtualizarDataPrevisaoChegada(cargaJanelaCarregamento, dataPrevisaoChegada);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmErroAoSalvarDataDePrevisaoDeChegada);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotalizadoresLegenda()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, TipoJanelaCarregamento.Calendario);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centro = repCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento);

                if (centro == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CentroDeCarregamentoObrigatorio);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = ObterJanelaCarregamentosPorFiltros(filtrosPesquisa, unitOfWork);

                var cargasTotaisJanelaCarregamentoAtual = ObterDadosCargaJanelaCarregamento(cargasJanelaCarregamento, unitOfWork, filtrosPesquisa.DataCarregamento);

                int agAceiteTransportador = 0; int agAprovacaoComercial = 0; int agConfirmacaoTransportador = 0; int agEncosta = 0; int agLiberacaoParaTransportadores = 0; int liberarAutomaticamenteFaturamento = 0;
                int prontaParaCarregamento = 0; int reprovacaoComercial = 0; int semTransportador = 0; int semValorFrete = 0; int descarregamento = 0; int faturada = 0; int fob = 0;

                foreach (var cargaJanelaCarregamentoAtual in cargasTotaisJanelaCarregamentoAtual)
                {
                    if (cargaJanelaCarregamentoAtual.Tipo == TipoCargaJanelaCarregamento.Descarregamento)
                        descarregamento += 1;
                    else if (cargaJanelaCarregamentoAtual.Carga.TipoCondicaoPagamento == TipoCondicaoPagamento.FOB)
                        fob += 1;
                    else if (((SituacaoCarga)cargaJanelaCarregamentoAtual.Carga.SituacaoCarga).IsSituacaoCargaFaturada())
                        faturada += 1;
                    else
                    {
                        switch (cargaJanelaCarregamentoAtual.Situacao)
                        {
                            case SituacaoCargaJanelaCarregamento.AgAceiteTransportador:
                                agAceiteTransportador += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.AgAprovacaoComercial:
                                agAprovacaoComercial += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador:
                                agConfirmacaoTransportador += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.AgEncosta:
                                agEncosta += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores:
                                agLiberacaoParaTransportadores += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.LiberarAutomaticamenteFaturamento:
                                liberarAutomaticamenteFaturamento += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.ProntaParaCarregamento:
                                prontaParaCarregamento += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.ReprovacaoComercial:
                                reprovacaoComercial += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.SemTransportador:
                                semTransportador += 1;
                                break;

                            case SituacaoCargaJanelaCarregamento.SemValorFrete:
                                semValorFrete += 1;
                                break;
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Logistica.JanelaCarregamentoTotalizador janelaCarregamentoTotalizador = new Dominio.ObjetosDeValor.Embarcador.Logistica.JanelaCarregamentoTotalizador()
                {
                    AguardandoAceiteTransportador = agAceiteTransportador,
                    AguardandoConfirmacaoTransportador = agConfirmacaoTransportador,
                    AguardandoEncosta = agEncosta,
                    AguardandoLiberacaoTransportadores = agLiberacaoParaTransportadores,
                    Descarregamento = descarregamento,
                    Faturada = faturada,
                    FOB = fob,
                    ProntaCarregamento = prontaParaCarregamento,
                    SemTransportador = semTransportador,
                    SemValorFrete = semValorFrete
                };


                return new JsonpResult(janelaCarregamentoTotalizador ?? new Dominio.ObjetosDeValor.Embarcador.Logistica.JanelaCarregamentoTotalizador());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os totalizadores da Janela de Carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReverterSituacaoNoShow()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                if (!(this.Usuario.UsuarioAdministrador && permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirReverterCargaNoShow)))
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCarga);

                if (cargaJanelaCarregamento.Carga.NaoComparecido == TipoNaoComparecimento.NaoCompareceu && cargaJanelaCarregamento.NaoComparecido == TipoNaoComparecimento.NaoCompareceu)
                {
                    cargaJanelaCarregamento.Carga.NaoComparecido = TipoNaoComparecimento.Compareceu;
                    cargaJanelaCarregamento.NaoComparecido = TipoNaoComparecimento.Compareceu;

                    repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
                    repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                }
                else
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CargaNaoEstaComoNoShow);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Cargas.Carga.CargaRevertidaComSucesso);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreUmaFalhaAoReverterCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarMotivoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoMotivoAtraso = Request.GetIntParam("MotivoAtraso");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento repositorioMotivoAtrasoCarregamento = new Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento motivoAtraso = repositorioMotivoAtrasoCarregamento.BuscarPorCodigo(codigoMotivoAtraso, false);

                if (cargaJanelaCarregamento == null || motivoAtraso == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.RegistroNaoEncontrado);

                cargaJanelaCarregamento.MotivoAtrasoCarregamento = motivoAtraso;

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Motivo de atraso alterado com sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Não foi possivel alterar motivo de atraso");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarRetirada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(carga.TipoOperacao?.Codigo ?? 0);

                unitOfWork.Start();

                if ((tipoOperacao?.SelecionarRetiradaProduto ?? false) && carga.SituacaoCarga == SituacaoCarga.AgNFe)
                {
                    servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, Auditado);
                    carga.ProcessandoDocumentosFiscais = true;
                }

                repositorioCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Cargas.Carga.RetiradaConfirmada);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmErroAoConfirmarRetirada);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarCargaJanelaCarregamentoTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                string motivoRejeicaoCarga = Request.GetStringParam("MotivoRejeicaoCarga");
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga.Empresa == null)
                    return new JsonpResult(false, true, "Não há Transportador na carga para realizar a Rejeição.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(codigoCarga, carga.Empresa, retornarCargasOriginais: true);

                unitOfWork.Start();

                servicoCargaJanelaCarregamentoTransportador.RejeitarCarga(codigoCarga, motivoRejeicaoCarga, cargasJanelaCarregamentoTransportador, Cliente, Usuario, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, Localization.Resources.Cargas.Carga.CargaRejeitadaComsucesso);
            }
            catch (ServicoException servicoExcecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(servicoExcecao);
                return new JsonpResult(false, true, servicoExcecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoRejeitarCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisaInformacoesCargas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaInformacoesCargas(unidadeDeTrabalho);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", "Janela de Carregamento." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> RetornarParaNovaLiberacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.RegistroNaoEncontrado);

                new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork, TipoServicoMultisoftware).CancelarCargaLiberadaParaTransportadores(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoExcecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(servicoExcecao);
                return new JsonpResult(false, true, servicoExcecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmErroAoEncaminharParaTransportadorRota);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Grid ObterGridPesquisaInformacoesCargas(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho, TipoJanelaCarregamento.Tabela);

            filtrosPesquisa.CodigoVeiculo = Request.GetIntParam("Veiculo");
            filtrosPesquisa.CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
            filtrosPesquisa.NumeroPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador");
            filtrosPesquisa.CodigoDestino = Request.GetIntParam("Destino");

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            bool integracaoBrasilRisk = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao) != null;
            bool integracaoMarfrig = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig) != null;
            bool integracaoOpenTech = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech) != null;

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
            //grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.PesoLiquido, "Origem", 8.8m, Models.Grid.Align.left, false, false);


            grid.AdicionarCabecalho(propriedade: "Editavel", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CodigoCarga", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CodigoFilial", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CodigoModeloVeicular", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CodigoTipoCarga", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CodigoTransportador", visivel: false);
            grid.AdicionarCabecalho(propriedade: "ObservacaoGuarita", visivel: false);
            grid.AdicionarCabecalho(propriedade: "Interessados", visivel: false);
            grid.AdicionarCabecalho(propriedade: "Visualizacoes", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermitirInformarAreaVeiculo", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermitirInformarLocalCarregamento", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermitirInformarObservacaoFluxoPatio", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermitirInformarObservacaoGuarita", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermitirLiberarFilaCarregamentoManualmente", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermitirBloquearFilaCarregamentoManualmente", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CargaLiberadaCotacao", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CargaLiberadaCotacaoAutomaticamente", visivel: false);
            grid.AdicionarCabecalho(propriedade: "Situacao", visivel: false);
            grid.AdicionarCabecalho(propriedade: "SituacaoCarga", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PreCarga", visivel: false);
            grid.AdicionarCabecalho(propriedade: "ExigeNotaFiscalParaCalcularFrete", visivel: false);
            grid.AdicionarCabecalho(propriedade: "CargaDePreCargaFechada", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PermiteLiberarParaShare", visivel: false);
            grid.AdicionarCabecalho(propriedade: "Tipo", visivel: false);
            grid.AdicionarCabecalho(propriedade: "PossuiHistoricoCotacao", visivel: false);
            grid.AdicionarCabecalho(propriedade: "ExpirouTempoLimiteParaEscolhaAutomatica", visivel: false);
            grid.AdicionarCabecalho(propriedade: "ProcessoCotacaoFinalizada", visivel: false);

            if ((centroCarregamento?.TipoOrdenacaoJanelaCarregamento ?? TipoOrdenacaoJanelaCarregamento.InicioCarregamento) == TipoOrdenacaoJanelaCarregamento.Prioridade)
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Prioridade, propriedade: "Prioridade", tamanho: 6.4m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.ObservacaoFluxoPatio, propriedade: "ObservacaoFluxoPatio", tamanho: 6.4m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Hora, propriedade: "HoraCarregamento", tamanho: 4.4m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.DescricaoCarga, propriedade: "NumeroCarga", tamanho: 6.3m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

            if (integracaoMarfrig)
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.OrdemDeEmbarque, propriedade: "OrdemEmbarque", tamanho: 8.3m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: true);

            if (centroCarregamento?.JanelaCarregamentoExibirSituacaoPatio ?? false)
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.SituacaoPatio, propriedade: "SituacaoPatio", tamanho: 6.3m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.CNPJDoTransportador, propriedade: "CnpjTransportador", tamanho: 6.3m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Transportador, propriedade: "Transportador", tamanho: 8.5m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Veiculo, propriedade: "Veiculo", tamanho: 6m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Rastreador, propriedade: "Rastreador", tamanho: 6m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: false);

            if (integracaoBrasilRisk)
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.StatusGR, propriedade: "StatusGR", tamanho: 6m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);

            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDeCarga, propriedade: "TipoCarga", tamanho: 7.6m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);

            if (configuracaoEmbarcador.InformarTipoCondicaoPagamentoMontagemCarga)
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDeFrete, propriedade: "TipoCondicaoPagamento", tamanho: 7.6m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);

            //DataPrevisaoChegadaOrigem

            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDeOperacao, propriedade: "TipoOperacao", tamanho: 9.3m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.ModeloVeicular, propriedade: "DescricaoModeloVeicular", tamanho: 8.8m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Cliente, propriedade: "Cliente", tamanho: 8.5m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Rota, propriedade: "Rota", tamanho: 5m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: !configuracaoEmbarcador.NaoExibirRotaJanelaCarregamento);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.InicioDescarga, propriedade: "HoraInicioDescarga", tamanho: 7m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: !centroCarregamento.UtilizarNumeroReduzidoDeColunas);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.FimDescarga, propriedade: "HoraFimDescarga", tamanho: 7m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.LocalCarregamento, propriedade: "LocalCarregamento", tamanho: 7m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: (!configuracaoEmbarcador.NaoExibirLocalCarregamentoJanelaCarregamento && !centroCarregamento.UtilizarNumeroReduzidoDeColunas));
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.NumeroDoPedido, propriedade: "NumeroPedido", tamanho: 8.8m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: configuracaoEmbarcador.ExibirNumeroPedidoJanelaCarregamentoEDescarregamento);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.PrevisaoDeChegadaNaPlanta, propriedade: "DataPrevisaoChegadaOrigem", tamanho: 8.8m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: configuracaoEmbarcador.ExibirNumeroPedidoJanelaCarregamentoEDescarregamento);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.LimiteDeSaida, propriedade: "HoraLimiteSaida", tamanho: 4.4m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: centroCarregamento.UtilizarNumeroReduzidoDeColunas);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TesteFrio, propriedade: "Licenca", tamanho: 4.4m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: integracaoMarfrig);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Doca, propriedade: "NumeroDoca", tamanho: 5m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: centroCarregamento.UtilizarNumeroReduzidoDeColunas);

            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Armador, propriedade: "Armador", tamanho: 5m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: false);

            if (integracaoMarfrig)
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.SituacaoDaOrdemDeEmbarque, propriedade: "SituacaoCargaOrdemEmbarqueIntegracao", tamanho: 5m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: true);

            if (centroCarregamento?.ExibirDadosAvancadosJanelaCarregamento ?? false)
            {
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.CodigoIntegracaoDestinatarios, propriedade: "CodigoIntegracaoDestinatario", tamanho: 7m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Quantidade, propriedade: "Quantidade", tamanho: 7m, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Estado, propriedade: "Estado", tamanho: 7m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Cidade, propriedade: "Cidade", tamanho: 7m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.PrevisaoDeSaida, propriedade: "PrevisaoSaida", tamanho: 7m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.PrevisaoDeEntrega, propriedade: "PrevisaoEntrega", tamanho: 7m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.CPFMotorista, propriedade: "CpfMotorista", tamanho: 7m, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false, visible: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Motorista, propriedade: "Motorista", tamanho: 7m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
            }

            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.DisponibilidadeDoVeiculo, propriedade: "DisponibilidadeVeiculo", tamanho: 8.8m, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: false);
            grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.ObservacaoAoTransportador, propriedade: "ObservacaoTransportador", tamanho: 8.8m, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.MotivoDeAtraso, "MotivoAtrasoCarregamento", 8.8m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destino, "Destino", 8.8m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.AreaVeiculo, "AreaVeiculos", 8.8m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroOrdem, "NumeroOrdem", 8.8m, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.StatusAE, "StatusAE", 8.8m, Models.Grid.Align.left, false, integracaoOpenTech);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Peso, "Peso", 8.8m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.PesoLiquido, "PesoLiquido", 8.8m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDoAceite, "OrigemDoAceite", 8.8m, Models.Grid.Align.left, false, false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unidadeDeTrabalho, "JanelaCarregamento/PesquisaInformacoesCargas", "grid-tabela-carregamento");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> carregamentos = repositorioCargaJanelaCarregamento.Buscar(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            foreach (var cargaJanelaCarregamento in carregamentos)
            {
                if (cargaJanelaCarregamento.Carga.Carregamento != null)
                {
                    if (!cargasJanelaCarregamento.Any(obj => obj.Carga.Carregamento != null && (obj.Carga.Carregamento.Codigo == cargaJanelaCarregamento.Carga.Carregamento.Codigo)))
                        cargasJanelaCarregamento.Add(cargaJanelaCarregamento);
                }
                else
                    cargasJanelaCarregamento.Add(cargaJanelaCarregamento);
            }

            if (!filtrosPesquisa.SituacaoFaturada && (configuracaoEmbarcador?.UtilizarPreCargaJanelaCarregamento ?? false))
                cargasJanelaCarregamento.AddRange(repositorioCargaJanelaCarregamento.BuscarPreCarga(filtrosPesquisa));

            grid.AdicionaRows(ObterDadosCargaJanelaCarregamentoLista(centroCarregamento, cargasJanelaCarregamento, filtrosPesquisa.DataCarregamento, configuracaoEmbarcador, unidadeDeTrabalho));
            grid.setarQuantidadeTotal(cargasJanelaCarregamento.Count);

            return grid;
        }

        private void AtualizarOperadorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if ((carga == null) || (carga.Operador != null))
                return;

            new Servicos.Embarcador.Carga.CargaOperador(unitOfWork).Atualizar(carga, Usuario, TipoServicoMultisoftware);
            new Repositorio.Embarcador.Cargas.Carga(unitOfWork).Atualizar(carga);
        }

        private decimal CalculoProdutividadeUtilizada(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargaJanelaCarregamentos, List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade> centroCarregamentoProdutividades, Repositorio.UnitOfWork unitOfWork, ref string produtividadeUtilizadaTexto)
        {
            decimal produtividadeUtilizada = 0;
            produtividadeUtilizadaTexto = string.Format("0 {0}s / 0%", Localization.Resources.Cargas.Carga.Hora);

            if (centroCarregamentoProdutividades.Count == 0 || centroCarregamento.HorasTrabalho == 0)
                return produtividadeUtilizada;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCargas((from obj in cargaJanelaCarregamentos select obj.Carga.Codigo).Distinct().ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanela in cargaJanelaCarregamentos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargasPedidos
                                                                                      where obj.Carga.Codigo == cargaJanela.Carga.Codigo
                                                                                      select obj).ToList();

                //Agora vamos localizar os destinatários
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    //Agora precisamos localizar o cadastro da produtividade...
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade produtividade = null;
                    if (cargaPedido.Pedido.Destinatario?.GrupoPessoas != null)
                        produtividade = (from obj in centroCarregamentoProdutividades
                                         where obj.GrupoPessoas != null &&
                                                obj.GrupoPessoas.Codigo == cargaPedido.Pedido.Destinatario.GrupoPessoas.Codigo &&
                                                obj.TipoOperacao.Codigo == cargaJanela.Carga.TipoOperacao.Codigo &&
                                                (obj.Transportador?.Codigo ?? 0) == (cargaPedido.Carga.Empresa?.Codigo ?? 0)
                                         select obj).FirstOrDefault();

                    if (produtividade == null && cargaPedido.Pedido.Destinatario?.GrupoPessoas != null)
                        produtividade = (from obj in centroCarregamentoProdutividades
                                         where obj.GrupoPessoas != null &&
                                                obj.GrupoPessoas.Codigo == cargaPedido.Pedido.Destinatario.GrupoPessoas.Codigo &&
                                                obj.TipoOperacao.Codigo == cargaJanela.Carga.TipoOperacao.Codigo &&
                                                obj.Transportador == null
                                         select obj).FirstOrDefault();

                    if (produtividade == null)
                        produtividade = (from obj in centroCarregamentoProdutividades
                                         where obj.TipoOperacao.Codigo == cargaJanela.Carga.TipoOperacao.Codigo &&
                                               obj.GrupoPessoas == null &&
                                               (obj.Transportador?.Codigo ?? 0) == (cargaPedido.Carga.Empresa?.Codigo ?? 0)
                                         select obj).FirstOrDefault();

                    if (produtividade == null)
                        produtividade = (from obj in centroCarregamentoProdutividades
                                         where obj.TipoOperacao.Codigo == cargaJanela.Carga.TipoOperacao.Codigo &&
                                               obj.GrupoPessoas == null &&
                                               obj.Transportador == null
                                         select obj).FirstOrDefault();

                    if (produtividade == null)
                        continue;

                    int validos = 0;
                    if (produtividade.Separacao > 0) validos++;
                    if (produtividade.Picking > 0) validos++;
                    if (produtividade.Carregamento > 0) validos++;
                    if (validos == 0)
                        continue;

                    //decimal produtividadeHora = (produtividade.Separacao + produtividade.Picking + produtividade.Carregamento) / validos;

                    //decimal produtividadePedido = cargaPedido.Peso / produtividadeHora;
                    //if (centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
                    //    produtividadePedido = cargaPedido.QtVolumes / produtividadeHora;

                    decimal produtividadePedido = (produtividade.Separacao == 0 ? 0 : (cargaPedido.Peso / (decimal)produtividade.Separacao)) + (produtividade.Picking == 0 ? 0 : (cargaPedido.Peso / (decimal)produtividade.Picking)) + (produtividade.Carregamento == 0 ? 0 : (cargaPedido.Peso / (decimal)produtividade.Carregamento));
                    if (centroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
                        produtividadePedido = (produtividade.Separacao == 0 ? 0 : (cargaPedido.QtVolumes / (decimal)produtividade.Separacao)) + (produtividade.Picking == 0 ? 0 : (cargaPedido.QtVolumes / (decimal)produtividade.Picking)) + (produtividade.Carregamento == 0 ? 0 : (cargaPedido.QtVolumes / (decimal)produtividade.Carregamento));

                    produtividadeUtilizada += produtividadePedido;
                }
            }

            produtividadeUtilizadaTexto = string.Format("{0} {1}s / {2}%", produtividadeUtilizada.ToString("f2"), Localization.Resources.Cargas.Carga.Hora, (int)Math.Ceiling((produtividadeUtilizada / centroCarregamento.HorasTrabalho) * 100));

            return produtividadeUtilizada;
        }

        private void InformarTipoTransportadorCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TipoTransportadorCentroCarregamento tipoTransportador, Dominio.Entidades.Empresa transportador, string observacaoTransportador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (cargaJanelaCarregamento.DatasAgendadasDivergentes)
                throw new ControllerException(Localization.Resources.Cargas.Carga.OsPedidosPossuemDatasDeAgendamentoDivergentes);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unidadeDeTrabalho, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unidadeDeTrabalho, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unidadeDeTrabalho, configuracaoEmbarcador);

            if (configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores && (cargaJanelaCarregamento.Carga.Empresa != null) && (cargaJanelaCarregamento.Carga.Empresa.Codigo != transportador?.Codigo))
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento.Carga, $"Removido o transportador {cargaJanelaCarregamento.Carga.Empresa.Descricao} ao liberar para os transportadores", unidadeDeTrabalho);
                if (cargaJanelaCarregamento.TransportadorOriginal == null)
                {
                    cargaJanelaCarregamento.TransportadorOriginal = cargaJanelaCarregamento.Carga.Empresa;
                    repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                }
                cargaJanelaCarregamento.Carga.Empresa = null;
                repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            }

            if (configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros)
                servicoCargaJanelaCarregamentoTransportadorTerceiro.DisponibilizarParaTransportadoresTerceiros(cargaJanelaCarregamento, tipoTransportador, TipoServicoMultisoftware);
            else
                servicoCargaJanelaCarregamentoTransportador.DisponibilizarParaTransportadores(cargaJanelaCarregamento, tipoTransportador, transportador, TipoServicoMultisoftware);

            if (configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores)
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unidadeDeTrabalho, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unidadeDeTrabalho, configuracaoEmbarcador, null);

                servicoCargaJanelaCarregamentoCotacao.LiberarParaCotacaoManualmente(cargaJanelaCarregamento, TipoServicoMultisoftware);
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(cargaJanelaCarregamento, servicoCargaJanelaCarregamentoTransportador.ObterTransportadores(cargaJanelaCarregamento));
            }
            else if (transportador != null)
            {
                cargaJanelaCarregamento.Carga.Empresa = transportador;
                repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            }

            cargaJanelaCarregamento.ObservacaoTransportador = observacaoTransportador;
            cargaJanelaCarregamento.ObservacaoTransportadorInformadaManualmente = true;

            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento.Carga, TipoServicoMultisoftware);
            AtualizarOperadorCarga(cargaJanelaCarregamento.Carga, unidadeDeTrabalho);
        }

        private bool IsCargaJanelaCarregamentoEditavel(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador)
        {
            if (operador == null)
                return false;

            if (!operador.SupervisorLogistica)
            {
                if (cargaJanelaCarregamento.Carga.Operador?.Codigo != operador.Usuario.Codigo)
                    return false;
            }

            if (cargaJanelaCarregamento.Carga.Filial != null)
            {
                if (operador.Filiais.Count > 0)
                {
                    if (!operador.Filiais.Any(o => o.Filial.Codigo == cargaJanelaCarregamento.Carga.Filial.Codigo))
                        return false;
                }
            }

            if (operador.PossuiFiltroTipoOperacao)
            {
                if (
                    (cargaJanelaCarregamento.Carga.TipoOperacao != null) && !operador.TipoOperacoes.ToList().Contains(cargaJanelaCarregamento.Carga.TipoOperacao) ||
                    (cargaJanelaCarregamento.Carga.TipoOperacao == null) && !operador.VisualizaCargasSemTipoOperacao
                )
                    return false;
            }

            if ((cargaJanelaCarregamento.Carga.TipoDeCarga != null) && (operador.TiposCarga.Count > 0))
            {
                if (
                    !operador.TiposCarga.Any(o => (
                        (o.TipoDeCarga.Codigo == cargaJanelaCarregamento.Carga.TipoDeCarga.Codigo) &&
                        (
                            (o.ModelosVeiculares.Count <= 0) ||
                            (cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null) ||
                            o.ModelosVeiculares.Any(modelo => modelo.ModeloVeicularCarga.Codigo == cargaJanelaCarregamento.Carga.ModeloVeicularCarga.Codigo)
                        )
                    )
                ))
                    return false;
            }

            if (cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas && cargaJanelaCarregamento.Carga.SituacaoCarga.IsSituacaoCargaFaturada())
                return false;

            return true;
        }

        private bool IsPermitirEditarAreasVeiculos(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga == null)
                return false;

            if (cargaJanelaCarregamento.Tipo != TipoCargaJanelaCarregamento.Carregamento)
                return false;

            return (
                (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) ||
                (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador) ||
                (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgAceiteTransportador) ||
                (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.ProntaParaCarregamento)
            );
        }

        private bool IsPermitirLiberarParaShare(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if ((cargaJanelaCarregamento.Carga?.TipoCondicaoPagamento != null) && (cargaJanelaCarregamento.Carga.TipoCondicaoPagamento.Value == TipoCondicaoPagamento.FOB) && !(cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores ?? false))
                return false;

            if ((cargaJanelaCarregamento.Tipo != TipoCargaJanelaCarregamento.Carregamento) || (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores))
                return false;

            if (cargaJanelaCarregamento.DataLiberacaoShare.HasValue)
                return true;

            return (
                (cargaJanelaCarregamento.CentroCarregamento?.LiberarCargaAutomaticamenteParaTransportadoras ?? false) &&
                (cargaJanelaCarregamento.CentroCarregamento?.TipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota) &&
                (cargaJanelaCarregamento.Carga?.Rota != null)
            );
        }

        private bool IsPermitirLiberarParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            return (
                (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) &&
                (cargaJanelaCarregamento.Carga != null) &&
                ((cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.Nova) || cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete)
            );
        }

        private void PermiteLiberarPorTipoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TipoTransportadorCentroCarregamento tipoTransportador)
        {
            if (tipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota && cargaJanelaCarregamento.CargaBase.Rota == null)
                throw new ControllerException("Obrigatório que a carga possua uma Rota para disponibilizar por prioridade de rota.");
        }

        private void MandarCargaExcedentesCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool naoComparecimento, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain servicoIntegracaoSaintGobain = new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoPrioridade servicoCargaJanelaCarregamentoPrioridade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoPrioridade(unitOfWork);

                if (cargaJanelaCarregamento.Carga != null && (cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Cancelada || cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPermitidoMoverCargaParaExcedentePoisElaEstaCancelada);

                unitOfWork.Start();

                cargaJanelaCarregamento.Excedente = true;
                cargaJanelaCarregamento.NaoComparecido = naoComparecimento ? TipoNaoComparecimento.NaoCompareceu : TipoNaoComparecimento.Compareceu;
                cargaJanelaCarregamento.Carga.DataCarregamentoCarga = null;

                servicoCargaJanelaCarregamentoPrioridade.RemoverPrioridade(cargaJanelaCarregamento);

                repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, null, Localization.Resources.Cargas.Carga.MudouParaCargaExcedente, unitOfWork);
                servicoIntegracaoSaintGobain.ReenviarIntegrarCarregamento(cargaJanelaCarregamento);

                var guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                if (guarita != null)
                {
                    guarita.HorarioEntradaDefinido = false;
                    repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita);
                }

                AtualizarOperadorCarga(cargaJanelaCarregamento.Carga, unitOfWork);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterCargasPorSituacao(ref int totalRegistro, bool? excedente, bool? emReserva, bool apenasCargaNaoEmitidas, bool apenasCargasPendentes, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa = ObterFiltrosPesquisaCargaJanelaCarregamento(unitOfWork, excedente, emReserva, apenasCargaNaoEmitidas, apenasCargasPendentes);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                InicioRegistros = Request.GetIntParam("Inicio"),
                LimiteRegistros = Request.GetIntParam("Limite")
            };

            bool? ordenacaoAscDesc = Request.GetNullableBoolParam("OrdenacaoAscDesc");

            if (ordenacaoAscDesc.HasValue)
            {
                string direcaoOrdenar = ordenacaoAscDesc.Value ? "asc" : "desc";
                parametrosConsulta.PropriedadeOrdenar = $"Carga.CodigoCargaEmbarcador {direcaoOrdenar}, PreCarga.NumeroPreCarga {direcaoOrdenar}";
            }

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            totalRegistro = repositorioCargaJanelaCarregamento.ContarConsultaCargaJanelaCarregamento(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> carregamentos = (totalRegistro > 0) ? repositorioCargaJanelaCarregamento.ConsultarCargaJanelaCarregamento(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasPendentes = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            for (int i = 0; i < carregamentos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = carregamentos[i];

                if (cargaJanelaCarregamento.Carga?.Carregamento != null)
                {
                    if (!cargasPendentes.Any(obj => obj.Carga.Carregamento != null && (obj.Carga.Carregamento.Codigo == cargaJanelaCarregamento.Carga.Carregamento.Codigo)))
                        cargasPendentes.Add(cargaJanelaCarregamento);
                    else
                        totalRegistro--;
                }
                else
                    cargasPendentes.Add(cargaJanelaCarregamento);
            }

            return cargasPendentes;
        }

        private dynamic ObterCores(string corHexadecimal)
        {
            if (string.IsNullOrWhiteSpace(corHexadecimal))
                return null;

            string corBordaHexadecimal = Utilidades.Cores.ObterCorPorPencentual(corHexadecimal, percentual: 70);
            string corFonteHexadecimal = Utilidades.Cores.ObterCorPorPencentual(corHexadecimal, percentual: 40);

            return new
            {
                Borda = corBordaHexadecimal,
                Fonte = corFonteHexadecimal,
                Fundo = corHexadecimal
            };
        }

        private dynamic ObterCoresPorSituacaoAdicional(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoAdicional = ObterSituacaoAdicional(cargaJanelaCarregamento, carga, periodosCarregamento, situacoesAdicionais, unitOfWork);

            return ObterCores(situacaoAdicional?.Cor);
        }

        private dynamic ObterDadosCargaJanelaCarregamento(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargas, Repositorio.UnitOfWork unidadeTrabalho)
        {
            return ObterDadosCargaJanelaCarregamento(cargas, unidadeTrabalho, dataAtualJanelaCarregamento: null);
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> ObterPeriodosCarregamentoPorCentro(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime? data, Repositorio.UnitOfWork unitOfWork)
        {
            if ((centroCarregamento == null) || !data.HasValue)
                return new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento;
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoDia = data.HasValue ? servicoCargaJanelaCarregamentoDisponibilidade.ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, data.Value.Date) : null;

            if (excecaoDia == null)
                periodosCarregamento = repPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(centroCarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(data.Value));
            else
                periodosCarregamento = repPeriodoCarregamento.BuscarPorExcecao(excecaoDia.Codigo);

            return periodosCarregamento;
        }

        private dynamic ObterPeriodosCarregamentoCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            if (centroCarregamento == null)
                return new List<dynamic>();

            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade repositorioCentroCarregamentoProdutividade = new Repositorio.Embarcador.Logistica.CentroCarregamentoProdutividade(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, ConfiguracaoEmbarcador);

            DateTime dataCarregamento = filtrosPesquisa.DataCarregamento ?? DateTime.Today;
            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradaCentros = null;
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodos = null;

            if (centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                DateTime dataPeriodoInicial = dataCarregamento.Date;
                DateTime dataPeriodoFinal = dataCarregamento.Date.AddDays(1).AddSeconds(-1);
                List<(double? Destinatario, int Carga)> cargasEDestinatarios = repositorioCargaJanelaCarregamento.BuscarCargasEDestinatarioPorIncidenciaDeHorario(0, centroCarregamento.Filial.Codigo, centroCarregamento.Codigo, dataPeriodoInicial, dataPeriodoFinal);

                paradaCentros = servicoDisponibilidadeCarregamento.ObterParadaCentroCarregamento(centroCarregamento, dataCarregamento);
                cargasPeriodos = cargasJanelaCarregamento.Select(obj => new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo
                {
                    Codigo = obj.Codigo,
                    TipoCarga = (obj.Carga.TipoDeCarga != null ? obj.Carga.TipoDeCarga.Codigo : 0),
                    TipoOperacao = (obj.Carga.TipoOperacao != null ? obj.Carga.TipoOperacao.Codigo : 0),
                    Transportador = (obj.Carga.Empresa != null ? obj.Carga.Empresa.Codigo : 0),
                    ModeloVeicularCarga = (obj.Carga.ModeloVeicularCarga != null ? obj.Carga.ModeloVeicularCarga.Codigo : 0),
                    Destinatario = cargasEDestinatarios.Where(o => o.Carga == obj.Carga?.Codigo).FirstOrDefault().Destinatario ?? 0,
                    Encaixe = obj.HorarioEncaixado,
                    TipoOperacaoEncaixe = obj.TipoOperacaoEncaixe?.Codigo ?? 0,
                    DataInicio = obj.InicioCarregamento,
                    DataFim = obj.TerminoCarregamento
                }).ToList();
            }

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> periodosCarregamentoTipoOperacoes = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>();
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = null;
            List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades = null;
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoDia = servicoDisponibilidadeCarregamento.ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, dataCarregamento.Date);
            bool permitirRetornarTipoOperacaoLivre = filtrosPesquisa.CodigoTipoOperacao == 0;

            if (excecaoDia == null)
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(centroCarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(dataCarregamento));
            else
            {
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorExcecao(excecaoDia.Codigo);

                if (centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao)
                {
                    Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repositorioPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(unitOfWork);
                    Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorioExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(unitOfWork);

                    periodosCarregamentoTipoOperacoes = repositorioPeriodoCarregamentoTipoOperacaoSimultaneo.BuscarPorExcecaoETipoOperacao(excecaoDia.Codigo, filtrosPesquisa.CodigoTipoOperacao);
                    exclusividades = repositorioExclusividadeCarregamento.BuscarExclusividadePorPeriodo(centroCarregamento.Codigo, dataCarregamento, DiaSemanaHelper.ObterDiaSemana(dataCarregamento));
                }
            }

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade> centroCarregamentoProdutividades = repositorioCentroCarregamentoProdutividade.BuscarPorCentroDeCarregamento(centroCarregamento.Codigo);

            return (
                from periodo in periodosCarregamento
                select new
                {
                    periodo.HoraInicio,
                    periodo.HoraTermino,
                    periodo.ToleranciaExcessoTempo,
                    periodo.CapacidadeCarregamentoSimultaneo,
                    periodo.CapacidadeCarregamentoVolume,
                    TipoOperacaoSimultaneo = centroCarregamento.ExibirVisualizacaoDosTiposDeOperacao ? ObterListaDisponbilidadePorTipoOperacao(dataCarregamento, periodo, cargasPeriodos, periodosCarregamentoTipoOperacoes, paradaCentros, exclusividades, permitirRetornarTipoOperacaoLivre, unitOfWork) : null,
                    PossuiProdutividade = centroCarregamentoProdutividades.Count > 0,
                    HorasProdutividade = centroCarregamento.HorasTrabalho,
                    ProdutividadeUtilizada = 0
                }
            ).ToList();
        }

        private dynamic ObterPeriodosComBloqueiosCargaJanelaCarregamento(CentroCarregamento centro, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            if (filtrosPesquisa.TipoJanelaCarregamento != TipoJanelaCarregamento.Calendario || !filtrosPesquisa.DataCarregamento.HasValue)
                return new List<dynamic>();

            DiaSemana dia = (DiaSemana)(filtrosPesquisa.DataCarregamento.Value.DayOfWeek + 1);

            Repositorio.Embarcador.Logistica.MotivoParadaCentro repositorioMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> listaMotivos = repositorioMotivoParadaCentro.BuscarPorDataECentroCarregamento(centro.Codigo, filtrosPesquisa.DataCarregamento.Value);

            if (listaMotivos.Count == 0)
                return new List<dynamic>();

            List<PeriodoCarregamento> periodos = centro.PeriodosCarregamento.Where(obj => obj.Dia == dia && listaMotivos.Min(x => x.DataInicio.TimeOfDay) >= obj.HoraInicio && listaMotivos.Max(x => x.DataFim.TimeOfDay) <= obj.HoraTermino).ToList();

            var retornoT = (from o in listaMotivos
                            let periodo = (from m in periodos where m.HoraInicio <= o.DataInicio.TimeOfDay && m.HoraTermino >= o.DataFim.TimeOfDay select m).FirstOrDefault()
                            let numeroCargasBloqueadas = (periodo?.CapacidadeCarregamentoSimultaneo ?? 0) > o.QuantidadeParada ? o.QuantidadeParada : (periodo?.CapacidadeCarregamentoSimultaneo ?? 0)
                            select new
                            {
                                Codigo = o.Codigo,
                                DataInicio = filtrosPesquisa.DataCarregamento.Value.Date.Add(o.DataInicio.TimeOfDay).ToString("dd/MM/yyyy HH:mm"),
                                DataTermino = filtrosPesquisa.DataCarregamento.Value.Date.Add(o.DataFim.TimeOfDay).ToString("dd/MM/yyyy HH:mm"),
                                Duracao = o.DataFim.TimeOfDay - o.DataInicio.TimeOfDay,
                                NumeroCargasBloqueadas = numeroCargasBloqueadas,
                                NumeroCargasPossiveis = (periodo?.CapacidadeCarregamentoSimultaneo ?? 0) - numeroCargasBloqueadas > 0 ? (periodo?.CapacidadeCarregamentoSimultaneo ?? 0) - numeroCargasBloqueadas : 0
                            }).ToList();

            List<dynamic> novoRetorno = new List<dynamic>();

            foreach (var elemento in retornoT)
            {
                novoRetorno.Add(elemento);

                for (int i = 1; i < elemento.NumeroCargasBloqueadas; i++)
                    novoRetorno.Add(elemento);
            }

            return novoRetorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.VisualizacaoGradeTipoOperacao> ObterListaDisponbilidadePorTipoOperacao(DateTime dataBase, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasDia, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> tipoOperacoes, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades, bool permitirRetornarTipoOperacaoLivre, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unidadeTrabalho, ConfiguracaoEmbarcador);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.VisualizacaoGradeTipoOperacao> disponbilidade = servicoDisponibilidadeCarregamento.ObterListaDisponbilidadePorTipoOperacaoVisualizacaoGrade(dataBase, periodo, cargasDia, tipoOperacoes, paradasCentro, exclusividades, permitirRetornarTipoOperacaoLivre);

            return disponbilidade
                .OrderByDescending(o => o.TipoOperacao)
                .ToList();
        }

        private dynamic ObterDadosCargaJanelaCarregamento(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargas, Repositorio.UnitOfWork unidadeTrabalho, DateTime? dataAtualJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao repositorioJanelaCarregamentoHistoricoCotacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorioJanelaCarregamentoSituacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeTrabalho);
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeTrabalho);

            List<int> codigosCarga = (from o in cargas where o.Carga != null select o.Carga.Codigo).ToList();
            List<int> codigosCargaJanelaCarregamento = (from o in cargas select o.Codigo).ToList();
            List<int> codigosFilial = (from o in cargas where o.CentroCarregamento?.Filial != null select o.CentroCarregamento.Filial.Codigo).ToList();
            List<int> codigosFiliaisComSequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterCodigosFiliaisComSequenciaGestaoPatio(TipoFluxoGestaoPatio.Origem);
            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = (from o in cargas select o.CentroCarregamento).ToList();
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = (centrosCarregamento.Count == 1) ? centrosCarregamento.FirstOrDefault() : null;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosSumarizados = repCargaPedido.ObterDadosResumidosPorCargas(codigosCarga);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosInteressados = repCargaJanelaCarregamentoTransportador.BuscarTotalInteressados(codigosCargaJanelaCarregamento);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosVisualizacoes = repCargaJanelaCarregamentoTransportador.BuscarTotalVisualizacoes(codigosCargaJanelaCarregamento);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCargas(codigosCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais = repositorioJanelaCarregamentoSituacao.BuscarTodos();
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoPorCentro(centroCarregamento, dataAtualJanelaCarregamento, unidadeTrabalho);
            List<int> codigosCargaJanelaCarregamentoComHistorico = repositorioJanelaCarregamentoHistoricoCotacao.BuscarCodigosCargaJanelaCarregamentoComHistorico(codigosCargaJanelaCarregamento);

            var retorno = (
                from carga in cargas
                select ObterDadosCargaJanelaCarregamento(
                    carga,
                    configuracaoEmbarcador,
                    unidadeTrabalho,
                    cargaPedidosSumarizados,
                    cargaMotoristas,
                    cargaJanelaCarregamentosInteressados,
                    periodosCarregamento,
                    situacoesAdicionais,
                    codigosFiliaisComSequenciaGestaoPatio.Contains(carga.CentroCarregamento?.Filial?.Codigo ?? 0),
                    operador,
                    dataAtualJanelaCarregamento,
                    possuiHistoricoCotacao: codigosCargaJanelaCarregamentoComHistorico.Any(o => o == carga.Codigo),
                    cargaJanelaCarregamentosVisualizacoes

                )
            ).ToList();

            return retorno;
        }

        private object ObterDadosCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosSumarizados, List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosInteressados, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais, bool permitirInformarObservacaoFluxoPatio, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador, DateTime? dataAtualJanelaCarregamento, bool possuiHistoricoCotacao, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosVisualizacoes)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

            if (carga == null)
                return ObterDadosPreCargaJanelaCarregamento(cargaJanelaCarregamento, permitirInformarObservacaoFluxoPatio);

            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaExibirDados = cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador == null ? carga : cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador.Carga;
            IEnumerable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> pedidos = (from o in cargaPedidosSumarizados where o.CodigoCarga == carga.Codigo select o);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristas = (from o in cargaMotoristas where o.Carga.Codigo == cargaExibirDados.Codigo select o).ToList();
            IEnumerable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> interessadosCarga = (from o in cargaJanelaCarregamentosInteressados where o.Codigo == cargaJanelaCarregamento.Codigo select o);
            IEnumerable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> visualizacoesCarga = (from o in cargaJanelaCarregamentosVisualizacoes where o.Codigo == cargaJanelaCarregamento.Codigo select o);

            string classificacaoPessoaCor = (from o in pedidos where o.Destinatario != null && !string.IsNullOrWhiteSpace(o.Destinatario.ClassificacaoPessoaCor) select o.Destinatario.ClassificacaoPessoaCor).FirstOrDefault() ?? "";
            DateTime? dataPrevisaoEntrega = carga.DataPrevisaoTerminoCarga ?? (from o in pedidos select o.DataPrevisaoEntrega).FirstOrDefault();
            DateTime? dataBaseCalculoLimiteCarregamento = (from o in pedidos select o.DataPrevisaoEntrega).OrderBy(o => o).FirstOrDefault();
            string clientes = cargaExibirDados.DadosSumarizados?.DestinatariosReais ?? "";
            bool permitirLiberarFilaCarregamentoManualmente = false;
            bool permitirBloquearFilaCarregamentoManualmente = false;
            string ordem = string.Join(", ", (from o in pedidos where !string.IsNullOrWhiteSpace(o.Ordem) select o.Ordem));
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = repositorioGuarita.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork).BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork).BuscarPorCarga(carga.Codigo);

            if (configuracaoEmbarcador.UtilizarFilaCarregamento && cargaJanelaCarregamento.IsSituacaoPermiteVincularFilaCarregamento())
            {
                bool cargaPermiteVincularFilaCarregamentoAutomaticamente = cargaJanelaCarregamento.IsCargaDadosPermiteVincularFilaCarregamentoAutomaticamente();

                permitirLiberarFilaCarregamentoManualmente = !cargaPermiteVincularFilaCarregamentoAutomaticamente;
                permitirBloquearFilaCarregamentoManualmente = cargaPermiteVincularFilaCarregamentoAutomaticamente;
            }

            bool existeIntegracaoKlios = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Klios);
            bool possuiVeiculoMotorista = cargaExibirDados.Veiculo != null && (cargaExibirDados.Motoristas.Count > 0);
            bool aguardandoAceiteOuConfirmacao = cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador;
            TimeSpan? tempoDataColeta = (agendamento != null && agendamento.DataColeta.HasValue && aguardandoAceiteOuConfirmacao) ? agendamento.DataColeta.Value - DateTime.Now : (TimeSpan?)null;
            string enderecoCliente = (from o in pedidos where o.Destinatario != null select o.Destinatario.EnderecoCompleto).FirstOrDefault() ?? "";
            dynamic cores = ObterCoresPorSituacaoAdicional(cargaJanelaCarregamento, cargaExibirDados, periodosCarregamento, situacoesAdicionais, unitOfWork);

            List<int> pedidosComNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarListaPedidosComNotaPorPedidos((from o in pedidos select o.CodigoPedido).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> ceps = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork).BuscarPorCargaPedidos((from o in pedidos select o.Codigo).ToList());

            DateTime? dataBaseCalculoLimiteCarregamentoSemNF = ceps.Where(o => o.CargaEntrega.DataAgendamento.HasValue
                                                                            && (!o.CargaPedido.Pedido?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.AgendamentoExigeNotaFiscal ?? false)
                                                                            && !pedidosComNota.Contains(o.CargaPedido.Pedido.Codigo)).OrderBy(o => o.CargaEntrega.DataAgendamento).Select(o => o.CargaEntrega?.DataAgendamento).FirstOrDefault();

            if (dataBaseCalculoLimiteCarregamentoSemNF.HasValue)
                dataBaseCalculoLimiteCarregamento = dataBaseCalculoLimiteCarregamentoSemNF;

            return new
            {
                cargaJanelaCarregamento.Codigo,
                Editavel = IsCargaJanelaCarregamentoEditavel(cargaJanelaCarregamento, operador),
                Tipo = cargaJanelaCarregamento.Tipo,
                Cores = cores,
                CamposVisiveis = string.IsNullOrWhiteSpace(cargaJanelaCarregamento.CentroCarregamento?.CamposVisiveisJanela) ? "1;2;3;4;5;6;7;8;9;10;11;12;13;15" : cargaJanelaCarregamento.CentroCarregamento.CamposVisiveisJanela,
                Carga = new
                {
                    carga.Codigo,
                    Numero = carga?.CodigoCargaEmbarcador ?? string.Empty,
                    DataCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                    cargaExibirDados.SituacaoCarga,
                    PrevisaoEntrega = dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    cargaExibirDados.TipoCondicaoPagamento,
                    cargaExibirDados.ExigeNotaFiscalParaCalcularFrete,
                    CargaPerigosa = agendamento != null ? (agendamento.CargaPerigosa ? "Sim" : "Não") : string.Empty,
                    PesoTotal = carga.DadosSumarizados?.PesoTotal.ToString("n0") ?? string.Empty,
                    DataCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm"),
                    DataLimiteCarregamento = (cargaJanelaCarregamento?.CentroCarregamento?.ValidarSeDataDeCarregamentoAtendeAgendamentoDoPedido ?? false) ? dataBaseCalculoLimiteCarregamento?.AddMinutes(-CalcularTempoLimiteCarregamentoEmMinutos(cargaJanelaCarregamento, unitOfWork)).ToString("dd/MM/yyyy HH:mm") ?? "" : "",
                },
                Rota = new
                {
                    Codigo = cargaExibirDados.Rota?.Codigo ?? 0,
                    Descricao = cargaExibirDados.Rota?.Descricao ?? ""
                },
                InicioCarregamento = dataAtualJanelaCarregamento.HasValue && cargaJanelaCarregamento.InicioCarregamento.Date < dataAtualJanelaCarregamento.Value.Date ? $"{cargaJanelaCarregamento.TerminoCarregamento.ToString("dd/MM/yyyy")} 00:00" : cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                TerminoCarregamento = cargaJanelaCarregamento.TerminoCarregamento.ToString("dd/MM/yyyy HH:mm"),
                DataColeta = tempoDataColeta.HasValue && tempoDataColeta.Value.TotalSeconds > 0 ? $"{(int)tempoDataColeta.Value.TotalHours}h:{tempoDataColeta.Value.Minutes}m" : "",
                cargaJanelaCarregamento.Situacao,
                SituacaoCotacao = cargaJanelaCarregamento.SituacaoCotacao != SituacaoCargaJanelaCarregamentoCotacao.NaoDefinida ? cargaJanelaCarregamento.SituacaoCotacao.ObterDescricao() : string.Empty,
                cargaJanelaCarregamento.DatasAgendadasDivergentes,
                cargaJanelaCarregamento.TempoCarregamento,
                cargaJanelaCarregamento.Excedente,
                cargaJanelaCarregamento.NaoComparecido,
                cargaJanelaCarregamento.CarregamentoReservado,
                PossuiHistoricoCotacao = possuiHistoricoCotacao,
                ChegadaDenegada = cargaJanelaCarregamentoGuarita?.ChegadaDenegada ?? false,
                ObservacaoLocalEntrega = !string.IsNullOrWhiteSpace(cargaExibirDados.ObservacaoLocalEntrega) ? cargaExibirDados.ObservacaoLocalEntrega : "",
                TipoCondicaoPagamento = cargaExibirDados.TipoCondicaoPagamento.HasValue ? cargaExibirDados.TipoCondicaoPagamento.Value.ObterDescricao() : "",
                CentroCarregamento = new
                {
                    Codigo = cargaJanelaCarregamento.CentroCarregamento != null ? cargaJanelaCarregamento.CentroCarregamento.Codigo : 0,
                    Descricao = cargaJanelaCarregamento.CentroCarregamento != null ? cargaJanelaCarregamento.CentroCarregamento.Descricao : ""
                },
                DiasAtrazo = (int)Math.Ceiling((cargaJanelaCarregamento.InicioCarregamento.Date - cargaJanelaCarregamento.DataCarregamentoProgramada.Date).TotalDays),
                ObservacaoFluxoPatio = !string.IsNullOrWhiteSpace(cargaJanelaCarregamento.ObservacaoFluxoPatio) ? cargaJanelaCarregamento.ObservacaoFluxoPatio : "",
                ObservacaoGuarita = !string.IsNullOrWhiteSpace(cargaJanelaCarregamentoGuarita?.Observacao) ? cargaJanelaCarregamentoGuarita.Observacao : "",
                ObservacaoTransportador = !string.IsNullOrWhiteSpace(cargaJanelaCarregamento.ObservacaoTransportador) ? cargaJanelaCarregamento.ObservacaoTransportador : "",
                PermitirInformarObservacaoFluxoPatio = permitirInformarObservacaoFluxoPatio,
                PermitirInformarObservacaoGuarita = cargaJanelaCarregamentoGuarita != null,
                ModeloVeiculo = new
                {
                    Codigo = cargaExibirDados.ModeloVeicularCarga?.Codigo ?? 0,
                    Descricao = cargaExibirDados.ModeloVeicularCarga?.Descricao ?? string.Empty
                },
                Veiculo = new
                {
                    Codigo = cargaExibirDados.Veiculo?.Codigo ?? 0,
                    Descricao = cargaExibirDados.Veiculo?.Placa ?? string.Empty
                },
                Transportador = new
                {
                    Codigo = cargaExibirDados.Empresa?.Codigo ?? 0,
                    Descricao = cargaExibirDados.DadosSumarizados?.PortalRetiraEmpresa ?? cargaExibirDados.Empresa?.RazaoSocial ?? string.Empty
                },
                TipoCarga = new
                {
                    Codigo = cargaExibirDados.TipoDeCarga?.Codigo ?? 0,
                    Descricao = cargaExibirDados.TipoDeCarga?.Descricao ?? string.Empty,
                    ExigeVeiculoRastreado = cargaExibirDados.TipoDeCarga?.ExigeVeiculoRastreado ?? false
                },
                TipoOperacao = new
                {
                    Codigo = cargaExibirDados?.TipoOperacao?.Codigo ?? 0,
                    Descricao = cargaExibirDados?.TipoOperacao?.Descricao ?? "",
                    Cores = ObterCores(cargaExibirDados?.TipoOperacao?.CorJanelaCarregamento)
                },
                DataProximaSituacao = cargaJanelaCarregamento.DataProximaSituacao.ToString("dd/MM/yyyy HH:mm"),
                DataDisponibilizacaoTransportadores = cargaJanelaCarregamento.DataDisponibilizacaoTransportadores.HasValue ? cargaJanelaCarregamento.DataDisponibilizacaoTransportadores.Value.ToString("dd/MM/yyyy HH:mm") : "",
                Ordem = string.IsNullOrWhiteSpace(ordem) ? "" : ordem,
                Origem = cargaExibirDados.DadosSumarizados?.Origens ?? string.Empty,
                Destino = cargaExibirDados.DadosSumarizados?.Destinos ?? string.Empty,
                DestinosCarga = (
                    from o in pedidos
                    where o.Destino != null
                    select new
                    {
                        o.Destino.Codigo,
                        o.Destino.Descricao
                    }
                ).ToList(),
                Remetente = cargaExibirDados.DadosSumarizados?.Remetentes ?? string.Empty,
                Destinatario = cargaExibirDados.DadosSumarizados?.DestinatariosReais ?? string.Empty,
                DestinatariosCarga = (
                    from o in pedidos
                    where o.Destinatario != null
                    select new
                    {
                        Codigo = (long)o.Destinatario.Codigo,
                        Descricao = o.Destinatario.Nome
                    }
                ).ToList(),
                Motoristas = (
                    from o in motoristas
                    select new
                    {
                        o.Motorista.Codigo,
                        Descricao = o.Motorista.Nome
                    }
                ).ToList(),
                Interessados = (from o in interessadosCarga select o.NumeroInteressados).Sum(),
                ClassificacaoPessoaCor = classificacaoPessoaCor,
                Cliente = clientes,
                PermitirLiberarFilaCarregamentoManualmente = permitirLiberarFilaCarregamentoManualmente,
                PermitirBloquearFilaCarregamentoManualmente = permitirBloquearFilaCarregamentoManualmente,
                cargaJanelaCarregamento.CargaLiberadaCotacao,
                cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente,
                cargaExibirDados.CargaDePreCargaFechada,
                cargaExibirDados.CargaDeComplemento,
                PreCarga = false,
                cargaJanelaCarregamento.HorarioEncaixado,
                NaoPermitirExibirTagsPadroes = cargaJanelaCarregamento.CentroCarregamento?.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas ? true : false,
                CargaGeradaPeloMetodoGerarCarregamento = carga.CargaGeradaPeloMetodoGerarCarregamento,
                HorarioDesencaixado = cargaJanelaCarregamento.HorarioDesencaixado,
                QuantidadeEntregas = cargaExibirDados.DadosSumarizados?.NumeroEntregas.ToString("n0") ?? "",
                ValorTarget = cargaJanelaCarregamento?.Carga?.CustoAtualIntegracaoLeilao.ToString("n2") ?? "",
                ValorFrete = cargaExibirDados.ValorFreteAPagar.ToString("n2"),
                RecomendacaoGR = (possuiVeiculoMotorista && existeIntegracaoKlios) ? cargaJanelaCarregamento.RecomendacaoGR?.ObterDescricao() ?? string.Empty : string.Empty,
                EnderecoCliente = enderecoCliente,
                PermiteConfirmarRetirada = ((configuracaoEmbarcador.Pais == TipoPais.Exterior) && (cargaExibirDados.TipoOperacao?.SelecionarRetiradaProduto ?? false) && !(cargaExibirDados.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira ?? false) && (cargaExibirDados.SituacaoCarga == SituacaoCarga.AgNFe) && !cargaExibirDados.ProcessandoDocumentosFiscais),
                PossuiJanelaDestino = cargaExibirDados?.FilialDestino != null && (cargaJanelaCarregamento.CentroCarregamento?.Filial?.Codigo == cargaExibirDados.FilialDestino.Codigo) && (cargaJanelaCarregamento?.CentroCarregamento?.GerarJanelaCarregamentoDestino ?? false),
                ExpirouTempoLimiteParaEscolhaAutomatica = cargaJanelaCarregamento.ProcessoCotacaoFinalizada,
                ProcessoCotacaoFinalizada = cargaJanelaCarregamento.ProcessoCotacaoFinalizada,
                NumeroAgendamento = cargaJanelaCarregamento?.Carga?.NumeroAgendamento ?? string.Empty,
                CargaPerigosa = cargaJanelaCarregamento?.Carga?.CargaPerigosaIntegracaoLeilao ?? false,
                DataTerminoCotacao = (configuracaoJanelaCarregamento?.LiberarCargaParaCotacaoAoLiberarParaTransportadores ?? false) && ((cargaJanelaCarregamento?.CargaLiberadaCotacao ?? false) || (cargaJanelaCarregamento?.ProcessoCotacaoFinalizada ?? false)) ? (cargaJanelaCarregamento.DataTerminoCotacao.HasValue ? cargaJanelaCarregamento.DataTerminoCotacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty) : string.Empty,
                SituacaoCotacaoCarga = (configuracaoJanelaCarregamento?.LiberarCargaParaCotacaoAoLiberarParaTransportadores ?? false) && ((cargaJanelaCarregamento?.CargaLiberadaCotacao ?? false) || (cargaJanelaCarregamento?.ProcessoCotacaoFinalizada ?? false)) ? ObterSituacaoCotacaoCarga(cargaJanelaCarregamento, interessadosCarga.Count(), unitOfWork) : string.Empty,
                Visualizacoes = (from o in visualizacoesCarga select o.NumeroVisualizacoes).Sum(),
            };
        }

        private dynamic ObterDadosCargaJanelaCarregamentoLista(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargas, DateTime? dataCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao repositorioJanelaCarregamentoHistoricoCotacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao repositorioJanelaCarregamentoSituacao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoSituacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unidadeTrabalho);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaAreaVeiculo repositorioCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unidadeTrabalho);

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unidadeTrabalho);

            List<int> codigosCarga = (from o in cargas where o.Carga != null select o.Carga.Codigo).ToList();
            List<int> codigosCargaJanelaCarregamento = (from o in cargas select o.Codigo).ToList();
            List<int> codigosFilial = (from o in cargas where o.CentroCarregamento?.Filial != null select o.CentroCarregamento.Filial.Codigo).ToList();
            List<int> codigosVeiculoCarga = (from o in cargas where o.Carga != null && o.Carga.Veiculo != null select o.Carga.Veiculo.Codigo).Distinct().ToList();
            List<int> codigosFiliaisComSequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterCodigosFiliaisComSequenciaGestaoPatio(TipoFluxoGestaoPatio.Origem);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosSumarizados = repositorioCargaPedido.ObterDadosResumidosPorCargas((from obj in cargas where obj.Carga != null select obj.Carga.CargaAgrupamento?.Codigo ?? obj.Carga.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio = new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargasJanelaCarregamentoGuarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCargasJanelaCarregamento(codigosCargaJanelaCarregamento);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais = repositorioJanelaCarregamentoSituacao.BuscarTodos();
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoPorCentro(centroCarregamento, dataCarregamento, unidadeTrabalho);
            List<int> codigosCargaJanelaCarregamentoComHistorico = repositorioJanelaCarregamentoHistoricoCotacao.BuscarCodigosCargaJanelaCarregamentoComHistorico(codigosCargaJanelaCarregamento);

            if (centroCarregamento?.JanelaCarregamentoExibirSituacaoPatio ?? false)
                fluxosGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCargas(codigosCarga);

            List<Dominio.Entidades.Embarcador.Cargas.CargaLicenca> cargasLicenca = repositorioCargaLicenca.BuscarPorCargas(codigosCarga);
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao> cargaOrdensEmbarqueIntegracao = repositorioCargaOrdemEmbarqueIntegracao.BuscarPorCargas(codigosCarga);
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = repositorioOrdemEmbarque.BuscarAtivasPorCargas(codigosCarga);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosInteressados = repositorioCargaJanelaCarregamentoTransportador.BuscarTotalInteressados((from obj in cargas select obj.Codigo).ToList());
            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosVisualizacoes = repositorioCargaJanelaCarregamentoTransportador.BuscarTotalVisualizacoes((from obj in cargas select obj.Codigo).ToList());

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
            TipoOrdenacaoJanelaCarregamento tipoOrdenacaoJanelaCarregamento = cargas.FirstOrDefault()?.CentroCarregamento?.TipoOrdenacaoJanelaCarregamento ?? TipoOrdenacaoJanelaCarregamento.InicioCarregamento;
            List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador)> dadosPosicaoVeiculos = repositorioPosicaoAtual.BuscarDadosPosicaoPorVeiculos(codigosVeiculoCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> listaAreaVeiculo = repositorioCargaAreaVeiculo.BuscarPorCargas(codigosCarga);

            List<int> idsJanelaCarregamentos = cargas.Select(x => x.Codigo).ToList();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite> origensDoAceite = repositorioCargaJanelaCarregamento.BuscarOrigensDoAceite(idsJanelaCarregamentos);

            if (tipoOrdenacaoJanelaCarregamento == TipoOrdenacaoJanelaCarregamento.Prioridade)
                return (from carga in cargas orderby carga.Prioridade select ObterDadosCargaJanelaCarregamentoLista(carga, configuracaoEmbarcador, unidadeTrabalho, cargaJanelaCarregamentosInteressados, cargaPedidosSumarizados, fluxosGestaoPatio, cargaOrdensEmbarqueIntegracao, ordensEmbarque, cargasLicenca, dadosPosicaoVeiculos, codigosFiliaisComSequenciaGestaoPatio.Contains(carga.CentroCarregamento?.Filial?.Codigo ?? 0), operador, cargasJanelaCarregamentoGuarita, situacoesAdicionais, periodosCarregamento, codigosCargaJanelaCarregamentoComHistorico, listaAreaVeiculo, origensDoAceite, cargaJanelaCarregamentosVisualizacoes)).ToList();

            return (from carga in cargas orderby carga.InicioCarregamento select ObterDadosCargaJanelaCarregamentoLista(carga, configuracaoEmbarcador, unidadeTrabalho, cargaJanelaCarregamentosInteressados, cargaPedidosSumarizados, fluxosGestaoPatio, cargaOrdensEmbarqueIntegracao, ordensEmbarque, cargasLicenca, dadosPosicaoVeiculos, codigosFiliaisComSequenciaGestaoPatio.Contains(carga.CentroCarregamento?.Filial?.Codigo ?? 0), operador, cargasJanelaCarregamentoGuarita, situacoesAdicionais, periodosCarregamento, codigosCargaJanelaCarregamentoComHistorico, listaAreaVeiculo, origensDoAceite, cargaJanelaCarregamentosVisualizacoes)).ToList();
        }

        private object ObterDadosCargaJanelaCarregamentoLista(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosInteressados, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidosSumarizados, List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio, List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao> cargaOrdensEmbarqueIntegracao, List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque, List<Dominio.Entidades.Embarcador.Cargas.CargaLicenca> cargasLicenca, List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador)> dadosPosicaoVeiculos, bool permitirInformarObservacaoFluxoPatio, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> cargasJanelaCarregamentoGuarita, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento, List<int> codigosCargaJanelaCarregamentoComHistorico, List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> listaAreaVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite> origensContratoTransportes, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> cargaJanelaCarregamentosVisualizacoes)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

            if (carga == null)
                return ObterDadosPreCargaJanelaCarregamentoLista(cargaJanelaCarregamento, permitirInformarObservacaoFluxoPatio, unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaExibirDados = cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador == null ? carga : cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador.Carga;

            IEnumerable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> visualizacoesCarga = (from o in cargaJanelaCarregamentosVisualizacoes where o.Codigo == cargaJanelaCarregamento.Codigo select o);
            int visualizacoes = (from o in visualizacoesCarga select o.NumeroVisualizacoes).Sum();

            IEnumerable<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamento> interessadosCarga = (from o in cargaJanelaCarregamentosInteressados where o.Codigo == cargaJanelaCarregamento.Codigo select o);
            int interessados = (from o in interessadosCarga select o.NumeroInteressados).Sum();

            bool possuiHistoricoCotacao = codigosCargaJanelaCarregamentoComHistorico.Any(o => o == cargaJanelaCarregamento.Codigo);
            bool editavel = IsCargaJanelaCarregamentoEditavel(cargaJanelaCarregamento, operador);
            bool permitirLiberarFilaCarregamentoManualmente = false;
            bool permitirBloquearFilaCarregamentoManualmente = false;
            bool permitirLiberarParaShare = IsPermitirLiberarParaShare(cargaJanelaCarregamento);
            string corLinha = "";
            string corFonte = "#666";

            if (configuracaoEmbarcador.UtilizarFilaCarregamento && cargaJanelaCarregamento.IsSituacaoPermiteVincularFilaCarregamento())
            {
                bool cargaPermiteVincularFilaCarregamentoAutomaticamente = cargaJanelaCarregamento.IsCargaDadosPermiteVincularFilaCarregamentoAutomaticamente();

                permitirLiberarFilaCarregamentoManualmente = !cargaPermiteVincularFilaCarregamentoAutomaticamente;
                permitirBloquearFilaCarregamentoManualmente = cargaPermiteVincularFilaCarregamentoAutomaticamente;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoAdicional = ObterSituacaoAdicional(cargaJanelaCarregamento, cargaExibirDados, periodosCarregamento, situacoesAdicionais, unitOfWork);

            if (situacaoAdicional != null)
            {
                corLinha = situacaoAdicional.Cor;
                corFonte = Utilidades.Cores.ObterCorPorPencentual(situacaoAdicional.Cor, 40m);
            }
            else if (!editavel)
                corLinha = "#afb8bf";
            else if (cargaJanelaCarregamento.Tipo == TipoCargaJanelaCarregamento.Descarregamento)
                corLinha = "#7eaacd";
            else if (cargaExibirDados.TipoCondicaoPagamento == TipoCondicaoPagamento.FOB)
                corLinha = "#4dffff";
            else if (cargaExibirDados.SituacaoCarga.IsSituacaoCargaFaturada())
            {
                corLinha = "#016f65";
                corFonte = "#ffffff";
            }
            else
            {
                corLinha = cargaJanelaCarregamento.Situacao.ObterCorLinha();
                corFonte = permitirLiberarFilaCarregamentoManualmente ? "#cc0000" : cargaJanelaCarregamento.Situacao.ObterCorFonte();
            }

            if ((cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador) && (interessados > 0))
                corFonte = "#3333ff";

            ICollection<Dominio.Entidades.Cliente> clientesDestinatarios = cargaExibirDados.DadosSumarizados?.ClientesDestinatarios ?? new List<Dominio.Entidades.Cliente>();
            string clientes = string.Join(", ", (from clienteDestinatario in clientesDestinatarios select clienteDestinatario.Descricao));
            Dominio.Entidades.Cliente remetente = cargaExibirDados.DadosSumarizados?.ClientesRemetentes?.FirstOrDefault();

            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Dominio.Entidades.Cliente destinatario = cargaExibirDados.DadosSumarizados?.ClientesDestinatarios?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;

            //Selecionar o cliente com menor janela de descaregamento
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga descargaPedido = repClienteDescarga.BuscarPorOrigemEDestino(cargaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0d, cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0d);
                if (descargaPedido != null)
                {
                    if (clienteDescarga == null)
                    {
                        clienteDescarga = descargaPedido;
                        destinatario = cargaPedido.Pedido.Destinatario;
                    }
                    else if (int.Parse(descargaPedido.HoraInicioDescarga.Substring(0, 2)) < int.Parse(clienteDescarga.HoraInicioDescarga.Substring(0, 2))) //seleciona o menor horário de inicio descarregamento
                    {
                        clienteDescarga = descargaPedido;
                        destinatario = cargaPedido.Pedido.Destinatario;
                    }
                }
            }

            dadosPosicaoVeiculos.Any(o => o.CodigoVeiculo == (carga.Veiculo?.Codigo ?? 0) && o.Rastreador);
            (int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador) dadosPosicaoVeiculo = (from o in dadosPosicaoVeiculos where o.CodigoVeiculo == (carga.Veiculo?.Codigo ?? 0) select o).FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao = (from o in cargaOrdensEmbarqueIntegracao where o.Carga.Codigo == carga.Codigo || o.Carga.CargaAgrupamento?.Codigo == carga.Codigo select o).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarquePorCarga = (from o in ordensEmbarque where o.Carga.Codigo == carga.Codigo || o.Carga.CargaAgrupamento?.Codigo == carga.Codigo select o).ToList();
            Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca = (from o in cargasLicenca where o.Carga.Codigo == carga.Codigo select o).FirstOrDefault();
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = null;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = (from o in cargasJanelaCarregamentoGuarita where o.Carga?.Codigo == carga.Codigo select o).FirstOrDefault();

            if (cargaJanelaCarregamento.CentroCarregamento?.JanelaCarregamentoExibirSituacaoPatio ?? false)
                fluxoGestaoPatio = (from o in fluxosGestaoPatio where o.Carga.Codigo == carga.Codigo select o).FirstOrDefault();

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech);

            string OrigemDoAceite = ObterOrigemDoAceite(origensContratoTransportes, cargaJanelaCarregamento);
            string ObservacaoTransportador = servicoCargaJanelaCarregamento.VerificarEGerarObservacaoFinal(cargaJanelaCarregamento.ObservacaoTransportador, cargaJanelaCarregamento.Carga.ObservacaoTransportador);

            return new
            {
                OrigemDoAceite,
                cargaJanelaCarregamento.Codigo,
                cargaJanelaCarregamento.Prioridade,
                Editavel = editavel,
                Tipo = cargaJanelaCarregamento.Tipo,
                CodigoCarga = carga.Codigo,
                CodigoFilial = cargaJanelaCarregamento.CentroCarregamento.Filial.Codigo,
                CodigoTipoCarga = cargaExibirDados.TipoDeCarga?.Codigo ?? 0,
                CodigoModeloVeicular = cargaExibirDados.ModeloVeicularCarga?.Codigo ?? 0,
                PermiteLiberarParaShare = permitirLiberarParaShare,
                Cliente = clientes,
                DescricaoModeloVeicular = cargaExibirDados.ModeloVeicularCarga?.Descricao,
                NumeroCarga = servicoCarga.ObterNumeroCarga(cargaExibirDados, configuracaoEmbarcador),
                HoraCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("HH:mm"),
                HoraFimDescarga = ObterHoraFimDescarga(remetente, destinatario, unitOfWork),
                HoraInicioDescarga = ObterHoraInicioDescarga(remetente, destinatario, unitOfWork),
                LocalCarregamento = "",
                cargaJanelaCarregamento.ObservacaoFluxoPatio,
                ObservacaoTransportador,
                ObservacaoGuarita = cargaJanelaCarregamentoGuarita?.Observacao ?? "",
                PermitirInformarAreaVeiculo = cargaJanelaCarregamento.CentroCarregamento?.PermitirInformarAreaVeiculoJanelaCarregamento ?? false,
                PermitirInformarObservacaoFluxoPatio = permitirInformarObservacaoFluxoPatio,
                PermitirInformarObservacaoGuarita = cargaJanelaCarregamentoGuarita != null,
                PermitirInformarLocalCarregamento = false,
                PermitirLiberarFilaCarregamentoManualmente = permitirLiberarFilaCarregamentoManualmente,
                PermitirBloquearFilaCarregamentoManualmente = permitirBloquearFilaCarregamentoManualmente,
                PossuiHistoricoCotacao = possuiHistoricoCotacao,
                cargaJanelaCarregamento.CargaLiberadaCotacao,
                cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente,
                Rota = cargaExibirDados.Rota?.Descricao,
                cargaJanelaCarregamento.Situacao,
                cargaExibirDados.SituacaoCarga,
                SituacaoPatio = fluxoGestaoPatio?.DescricaoSituacao ?? string.Empty,
                PreCarga = false,
                TipoCarga = cargaExibirDados.TipoDeCarga?.Descricao,
                TipoOperacao = cargaExibirDados.TipoOperacao?.Descricao,
                CodigoTransportador = cargaExibirDados.Empresa?.Codigo ?? 0,
                CnpjTransportador = cargaExibirDados.Empresa?.CNPJ_Formatado,
                Transportador = cargaExibirDados.Empresa?.RazaoSocial ?? "",
                Veiculo = cargaExibirDados.RetornarPlacas,
                Rastreador = dadosPosicaoVeiculos.Any(o => o.CodigoVeiculo == (carga.Veiculo?.Codigo ?? 0) && o.Rastreador) ? "Sim" : "Não",
                StatusGR = cargaExibirDados.Veiculo != null ? cargaExibirDados.ProblemaIntegracaoGrMotoristaVeiculo ? "Não OK" : "OK" : "",
                TipoCondicaoPagamento = cargaExibirDados.TipoCondicaoPagamento.HasValue ? cargaExibirDados.TipoCondicaoPagamento.Value.ObterDescricao() : "",
                cargaExibirDados.ExigeNotaFiscalParaCalcularFrete,
                Interessados = interessados,
                NumeroPedido = string.Join(", ", (from o in cargaPedidosSumarizados where o.CodigoCarga == (cargaExibirDados.CargaAgrupamento?.Codigo ?? cargaExibirDados.Codigo) select o.NumeroPedidoEmbarcador).ToList()),
                DataPrevisaoChegadaOrigem = cargaExibirDados.DataPrevisaoChegadaOrigem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DisponibilidadeVeiculo = cargaJanelaCarregamento.DataPrevisaoChegada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                cargaExibirDados.CargaDePreCargaFechada,
                NumeroDoca = !string.IsNullOrWhiteSpace(cargaExibirDados.NumeroDocaEncosta) ? cargaExibirDados?.NumeroDocaEncosta : cargaExibirDados?.NumeroDoca ?? string.Empty,
                Armador = string.Join(", ", (from o in cargaPedidosSumarizados where o.CodigoCarga == (cargaExibirDados.CargaAgrupamento?.Codigo ?? cargaExibirDados.Codigo) select o.Armador?.Descricao ?? "").ToList()),
                HoraLimiteSaida = cargaExibirDados.Rota?.HoraLimiteSaidaCD?.ToString(@"hh\:mm") ?? string.Empty,
                Licenca = cargaLicenca?.Situacao.ObterDescricao() ?? string.Empty,
                SituacaoCargaOrdemEmbarqueIntegracao = cargaOrdemEmbarqueIntegracao?.SituacaoIntegracao.ObterDescricao() ?? "",
                OrdemEmbarque = string.Join(", ", (from o in ordensEmbarquePorCarga select o.Numero)),
                CodigoIntegracaoDestinatario = cargaExibirDados.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? "",
                Quantidade = cargaExibirDados.DadosSumarizados?.VolumesTotal.ToString() ?? "",
                Cidade = cargaExibirDados.DadosSumarizados?.Destinos ?? "",
                Estado = cargaExibirDados.DadosSumarizados?.UFDestinos ?? "",
                PrevisaoSaida = cargaExibirDados.DadosSumarizados?.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm") ?? "",
                PrevisaoEntrega = cargaExibirDados.DadosSumarizados?.DataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Motorista = string.Join(", ", cargaExibirDados.Motoristas?.Select(obj => obj.Nome)),
                CpfMotorista = string.Join(", ", cargaExibirDados.Motoristas?.Select(obj => obj.CPF_CNPJ_Formatado)),
                MotivoAtrasoCarregamento = cargaJanelaCarregamento?.MotivoAtrasoCarregamento?.Descricao ?? string.Empty,
                Destino = cargaExibirDados.DadosSumarizados?.Destinos ?? string.Empty,
                AreaVeiculos = listaAreaVeiculo?.Count > 0 ? string.Join(", ", listaAreaVeiculo.Where(a => a.Carga.Codigo == carga.Codigo).Select(o => o.AreaVeiculo.Descricao).ToList()) : string.Empty,
                StatusAE = cargaCargaIntegracao != null ? (cargaCargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado ? "Ok" : (cargaCargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao ? "Erro" : string.Empty)) : "N/A",
                PermiteConfirmarRetirada = ((configuracaoEmbarcador.Pais == TipoPais.Exterior) && (cargaExibirDados.TipoOperacao?.SelecionarRetiradaProduto ?? false) && !(cargaExibirDados.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira ?? false) && (cargaExibirDados.SituacaoCarga == SituacaoCarga.AgNFe) && !cargaExibirDados.ProcessandoDocumentosFiscais),
                NumeroOrdem = cargaExibirDados.DadosSumarizados?.NumeroOrdem ?? string.Empty,
                Peso = cargaExibirDados.DadosSumarizados?.PesoTotal ?? 0m,
                PesoLiquido = cargaExibirDados.DadosSumarizados?.PesoLiquidoTotal ?? 0m,
                ExpirouTempoLimiteParaEscolhaAutomatica = DateTime.Now > cargaJanelaCarregamento.DataTerminoCotacao,
                ProcessoCotacaoFinalizada = cargaJanelaCarregamento.ProcessoCotacaoFinalizada,
                Visualizacoes = visualizacoes,
                DT_RowColor = corLinha,
                DT_FontColor = corFonte,
            };
        }

        private string ObterOrigemDoAceite(List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite> origensContratoTransportes, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (origensContratoTransportes.Where(x => x.CodigoCarga == cargaJanelaCarregamento.Carga.Codigo && x.Origem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceite.Fila).Count() > 0)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceiteHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceite.Fila);

            if (origensContratoTransportes.Where(x => x.CodigoJanela == cargaJanelaCarregamento.Codigo && x.Origem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceite.OfertaCarga).Count() > 0)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceiteHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceite.OfertaCarga);

            if (origensContratoTransportes.Where(x => x.CodigoJanela == cargaJanelaCarregamento.Codigo && x.Origem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceite.OfertaPrePlano).Count() > 0)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceiteHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemDoAceite.OfertaPrePlano);

            return "";

            /*             
    se tivermos fila então origem vem de fila 
        SISTEMA 00
            select C.CAR_CODIGO,0 CJC_CODIGO,FLV_TIPO from T_FILA_CARREGAMENTO_VEICULO F
            INNER JOIN T_CARGA C ON C.CAR_CODIGO = F.CAR_CODIGO
            INNER JOIN T_CARGA_JANELA_CARREGAMENTO J ON J.CAR_CODIGO = C.CAR_CODIGO
            WHERE  J.CJC_CODIGO IN (212743,212744,212746,212748,212749,212750)
    senão 
        SISTEMA 01 select * from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR se o tipo for rota ou rota grupo então é um chere de CARGA eles reconhecem como pre carga kkkk outro tipo sera por transportador indica manual na maioria das vezes porem terei que pesquisar no outro Sistema -> 

            select CAR_CODIGO,J.CJC_CODIGO,JCT_TIPO TIPO from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR JT
            INNER JOIN T_CARGA_JANELA_CARREGAMENTO J ON JT.CJC_CODIGO = J.CJC_CODIGO
            WHERE JT.CJC_CODIGO IN (212743,212744,212746,212748,212749,212750)
            AND JCT_TIPO IN (1,2)			

        SITEMA 01 

        SISTEMA 02 - "select * from T_PRE_CARGA_OFERTA	select * from T_PRE_CARGA_OFERTA_TRANSPORTADOR" se o tipo for rota ou rota grupo então foi shere de PP do contrario foi manual 

            select C.CAR_CODIGO,J.CJC_CODIGO,POT_TIPO TIPO  from T_CARGA_JANELA_CARREGAMENTO J
            INNER JOIN T_CARGA C ON J.CAR_CODIGO = C.CAR_CODIGO
            INNER JOIN T_PRE_CARGA PC ON PC.CAR_CODIGO = C.CAR_CODIGO 
            INNER JOIN T_PRE_CARGA_OFERTA PCO ON PC.PCA_CODIGO = PCO.PCA_CODIGO 
            INNER JOIN T_PRE_CARGA_OFERTA_TRANSPORTADOR T ON T.PCO_CODIGO = PCO.PCO_CODIGO
            WHERE J.CJC_CODIGO IN (212743,212744,212746,212748,212749,212750)
            AND POT_TIPO IN (1,2)


        PorTipoTransportadorCarga = 0,
        PorPrioridadeRotaGrupo = 1,
        PorPrioridadeRota = 2,
        PorMenorValorFreteTabelaCalculado = 3,
        PorGrupoTransportador = 4
             */
        }

        private object ObterDadosPreCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool permitirInformarObservacaoFluxoPatio)
        {
            List<Dominio.Entidades.Localidade> destinos = (from obj in cargaJanelaCarregamento.PreCarga.Destinatarios select obj.Localidade).Distinct().ToList();
            string strDestinos = "";

            if (destinos.Count > 0)
                strDestinos = string.Join(", ", (from obj in destinos select obj.DescricaoCidadeEstado).ToList());

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoa = (from obj in cargaJanelaCarregamento.PreCarga.Destinatarios where obj.GrupoPessoas != null select obj.GrupoPessoas).Distinct().ToList();
            List<Dominio.Entidades.Cliente> clientes = (from obj in cargaJanelaCarregamento.PreCarga.Destinatarios where obj.GrupoPessoas == null select obj).Distinct().ToList();
            string classificacaoPessoaCor = (from o in grupoPessoa where o.Classificacao != null && !string.IsNullOrWhiteSpace(o.Classificacao.Cor) select o.Classificacao.Cor).FirstOrDefault() ?? "";
            string destinatarios = string.Join(", ", (from obj in grupoPessoa select obj.Descricao).ToList());
            destinatarios += string.Join(", ", (from obj in clientes select "(" + obj.CPF_CNPJ_Formatado + ") " + obj.Nome).ToList());
            string listaClientes = string.Join(", ", (from cliente in cargaJanelaCarregamento.PreCarga.Destinatarios select cliente.Descricao));
            string enderecoCliente = cargaJanelaCarregamento.PreCarga.Destinatarios?.FirstOrDefault()?.EnderecoCompleto ?? "";
            dynamic cores = null;

            return new
            {
                cargaJanelaCarregamento.Codigo,
                Editavel = false,
                Tipo = cargaJanelaCarregamento.Tipo,
                Cores = cores,
                CamposVisiveis = string.IsNullOrWhiteSpace(cargaJanelaCarregamento.CentroCarregamento?.CamposVisiveisJanela) ? "1;2;3;4;5;6;7;8;9;10;11;12;13;14" : cargaJanelaCarregamento.CentroCarregamento.CamposVisiveisJanela,
                Carga = new
                {
                    Codigo = 0,
                    Numero = cargaJanelaCarregamento.PreCarga.NumeroPreCarga,
                    DataCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                    SituacaoCarga = SituacaoCarga.Nova,
                    PrevisaoEntrega = "",
                    TipoCondicaoPagamento = (TipoCondicaoPagamento?)null,
                    ExigeNotaFiscalParaCalcularFrete = false,
                    CargaPerigosa = "",
                    PesoTotal = "",
                    DataCarga = ""
                },
                Rota = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                InicioCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                TerminoCarregamento = cargaJanelaCarregamento.TerminoCarregamento.ToString("dd/MM/yyyy HH:mm"),
                DataColeta = "",
                cargaJanelaCarregamento.Situacao,
                SituacaoCotacao = cargaJanelaCarregamento.SituacaoCotacao != SituacaoCargaJanelaCarregamentoCotacao.NaoDefinida ? cargaJanelaCarregamento.SituacaoCotacao.ObterDescricao() : string.Empty,
                cargaJanelaCarregamento.DatasAgendadasDivergentes,
                cargaJanelaCarregamento.TempoCarregamento,
                cargaJanelaCarregamento.Excedente,
                cargaJanelaCarregamento.NaoComparecido,
                cargaJanelaCarregamento.CarregamentoReservado,
                PossuiHistoricoCotacao = false,
                ChegadaDenegada = "",
                ObservacaoLocalEntrega = "",
                TipoCondicaoPagamento = "",
                CentroCarregamento = new
                {
                    Codigo = cargaJanelaCarregamento.CentroCarregamento?.Codigo ?? 0,
                    Descricao = cargaJanelaCarregamento.CentroCarregamento?.Descricao ?? ""
                },
                DiasAtrazo = (int)Math.Ceiling((cargaJanelaCarregamento.InicioCarregamento.Date - cargaJanelaCarregamento.DataCarregamentoProgramada.Date).TotalDays),
                ObservacaoFluxoPatio = !string.IsNullOrWhiteSpace(cargaJanelaCarregamento.ObservacaoFluxoPatio) ? cargaJanelaCarregamento.ObservacaoFluxoPatio : "",
                ObservacaoGuarita = "",
                ObservacaoTransportador = !string.IsNullOrWhiteSpace(cargaJanelaCarregamento.ObservacaoTransportador) ? cargaJanelaCarregamento.ObservacaoTransportador : "",
                PermitirInformarObservacaoFluxoPatio = permitirInformarObservacaoFluxoPatio,
                PermitirInformarObservacaoGuarita = false,
                ModeloVeiculo = new
                {
                    Codigo = cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga?.Codigo ?? 0,
                    Descricao = cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga?.Descricao ?? string.Empty
                },
                Veiculo = new
                {
                    Codigo = cargaJanelaCarregamento.PreCarga.Veiculo?.Codigo ?? 0,
                    Descricao = cargaJanelaCarregamento.PreCarga.Veiculo?.Placa ?? string.Empty
                },
                Transportador = new
                {
                    Codigo = cargaJanelaCarregamento.PreCarga.Empresa?.Codigo ?? 0,
                    Descricao = cargaJanelaCarregamento.PreCarga.Empresa?.RazaoSocial ?? ""
                },
                TipoCarga = new
                {
                    Codigo = cargaJanelaCarregamento.PreCarga.TipoDeCarga?.Codigo ?? 0,
                    Descricao = cargaJanelaCarregamento.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                    ExigeVeiculoRastreado = cargaJanelaCarregamento.PreCarga.TipoDeCarga?.ExigeVeiculoRastreado ?? false
                },
                TipoOperacao = new
                {
                    Codigo = cargaJanelaCarregamento.PreCarga?.TipoOperacao?.Codigo ?? 0,
                    Descricao = cargaJanelaCarregamento.PreCarga?.TipoOperacao?.Descricao ?? "",
                    Cores = ObterCores(cargaJanelaCarregamento.PreCarga?.TipoOperacao?.CorJanelaCarregamento)
                },
                DataProximaSituacao = cargaJanelaCarregamento.DataProximaSituacao.ToString("dd/MM/yyyy HH:mm"),
                DataDisponibilizacaoTransportadores = "",
                Ordem = "",
                Origem = cargaJanelaCarregamento.PreCarga.Filial.Localidade.DescricaoCidadeEstado ?? string.Empty,
                Destino = strDestinos,
                DestinosCarga = new List<dynamic>(),
                Remetente = cargaJanelaCarregamento.PreCarga.Filial.Descricao ?? string.Empty,
                Destinatario = destinatarios,
                DestinatariosCarga = new List<dynamic>(),
                Motoristas = (
                    from o in cargaJanelaCarregamento.PreCarga.Motoristas
                    select new
                    {
                        o.Codigo,
                        Descricao = o.Nome
                    }
                ).ToList(),
                Interessados = 0,
                ClassificacaoPessoaCor = classificacaoPessoaCor,
                Cliente = listaClientes,
                PermitirLiberarFilaCarregamentoManualmente = false,
                PermitirBloquearFilaCarregamentoManualmente = false,
                cargaJanelaCarregamento.CargaLiberadaCotacao,
                cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente,
                CargaDePreCargaFechada = false,
                CargaDeComplemento = false,
                PreCarga = true,
                cargaJanelaCarregamento.HorarioEncaixado,
                NaoPermitirExibirTagsPadroes = cargaJanelaCarregamento.CentroCarregamento?.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas ? true : false,
                CargaGeradaPeloMetodoGerarCarregamento = false,
                HorarioDesencaixado = cargaJanelaCarregamento.HorarioDesencaixado,
                QuantidadeEntregas = "",
                ValorTarget = "",
                ValorFrete = "",
                EnderecoCliente = enderecoCliente,
                PermiteConfirmarRetirada = false,
                ExpirouTempoLimiteParaEscolhaAutomatica = cargaJanelaCarregamento.ProcessoCotacaoFinalizada,
                ProcessoCotacaoFinalizada = cargaJanelaCarregamento.ProcessoCotacaoFinalizada,
                Visualizacoes = 0,
            };
        }

        private object ObterDadosPreCargaJanelaCarregamentoLista(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, bool permitirInformarObservacaoFluxoPatio, Repositorio.UnitOfWork unidadeTrabalho)
        {
            string clientes = string.Join(", ", (from cliente in cargaJanelaCarregamento.PreCarga.Destinatarios select cliente.Descricao));
            Dominio.Entidades.Cliente destinatario = cargaJanelaCarregamento.PreCarga.Destinatarios.FirstOrDefault();

            return new
            {
                cargaJanelaCarregamento.Codigo,
                cargaJanelaCarregamento.Prioridade,
                Editavel = false,
                Tipo = cargaJanelaCarregamento.Tipo,
                CodigoCarga = cargaJanelaCarregamento.PreCarga.Codigo,
                CodigoFilial = cargaJanelaCarregamento.CentroCarregamento.Filial.Codigo,
                CodigoModeloVeicular = cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoTipoCarga = cargaJanelaCarregamento.PreCarga.TipoDeCarga?.Codigo ?? 0,
                Cliente = clientes,
                DescricaoModeloVeicular = cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga?.Descricao,
                NumeroCarga = cargaJanelaCarregamento.PreCarga.NumeroPreCarga,
                HoraCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("HH:mm"),
                HoraFimDescarga = ObterHoraFimDescarga(null, destinatario, unidadeTrabalho),
                HoraInicioDescarga = ObterHoraInicioDescarga(null, destinatario, unidadeTrabalho),
                LocalCarregamento = cargaJanelaCarregamento.PreCarga?.LocalCarregamento?.DescricaoAcao ?? "",
                cargaJanelaCarregamento.ObservacaoFluxoPatio,
                cargaJanelaCarregamento.ObservacaoTransportador,
                ObservacaoGuarita = "",
                PermitirInformarAreaVeiculo = false,
                PermitirInformarObservacaoFluxoPatio = permitirInformarObservacaoFluxoPatio,
                PermitirInformarObservacaoGuarita = false,
                PermitirInformarLocalCarregamento = cargaJanelaCarregamento.PreCarga?.LocalCarregamento == null,
                PermitirLiberarFilaCarregamentoManualmente = false,
                PermitirBloquearFilaCarregamentoManualmente = false,
                PossuiHistoricoCotacao = false,
                cargaJanelaCarregamento.CargaLiberadaCotacao,
                cargaJanelaCarregamento.CargaLiberadaCotacaoAutomaticamente,
                Rota = cargaJanelaCarregamento.PreCarga.Rota?.Descricao,
                cargaJanelaCarregamento.Situacao,
                SituacaoCarga = SituacaoCarga.Nova,
                SituacaoPatio = "",
                PreCarga = true,
                TipoCarga = cargaJanelaCarregamento.PreCarga.TipoDeCarga?.Descricao,
                TipoOperacao = cargaJanelaCarregamento.PreCarga.TipoOperacao?.Descricao,
                CodigoTransportador = cargaJanelaCarregamento.PreCarga.Empresa?.Codigo ?? 0,
                CnpjTransportador = cargaJanelaCarregamento.PreCarga.Empresa?.CNPJ_Formatado,
                Transportador = cargaJanelaCarregamento.PreCarga.Empresa?.RazaoSocial ?? "",
                Veiculo = cargaJanelaCarregamento.CargaBase.RetornarPlacas,
                Rastreador = "",
                StatusGR = "",
                TipoCondicaoPagamento = "",
                ExigeNotaFiscalParaCalcularFrete = false,
                Interessados = 0,
                NumeroPedido = "",
                DataPrevisaoChegadaOrigem = "",
                DisponibilidadeVeiculo = cargaJanelaCarregamento.DataPrevisaoChegada?.ToString("dd/MM/yyyy HH:mm"),
                CargaDePreCargaFechada = false,
                NumeroDoca = "",
                HoraLimiteSaida = cargaJanelaCarregamento.PreCarga.Rota?.HoraInicioCarregamento?.ToString(@"hh\:mm") ?? string.Empty,
                Armador = cargaJanelaCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.ArmadorImportacao ?? string.Empty,
                Licenca = "",
                SituacaoCargaOrdemEmbarqueIntegracao = "",
                OrdemEmbarque = "",
                PermiteConfirmarRetirada = false,
                NumeroOrdem = "",
                Peso = "",
                PesoLiquido = "",
                Visualizacoes = 0,
                DT_RowColor = "#afb8bf"
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, TipoJanelaCarregamento tipoJanelaCarregamento)
        {
            List<string> listaSituacao = JsonConvert.DeserializeObject<List<string>>(Request.Params("Situacao"));
            List<SituacaoCargaJanelaCarregamento> situacoes = (from situacao in listaSituacao where situacao.ToNullableEnum<SituacaoCargaJanelaCarregamento>().HasValue select situacao.ToEnum<SituacaoCargaJanelaCarregamento>()).ToList();
            bool situacaoFaturada = (from situacao in listaSituacao where situacao == "99" select true).FirstOrDefault();
            bool situacaoNaoFaturada = (from situacao in listaSituacao where situacao == "98" select true).FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                DataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                Situacoes = situacoes,
                SituacaoFaturada = situacaoFaturada,
                SituacaoNaoFaturada = situacaoNaoFaturada,
                TipoJanelaCarregamento = tipoJanelaCarregamento,
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                ExibirCargaQueNaoEstaoEmInicioViagem = Request.GetBoolParam("ExibirCargaQueNaoEstaoEmInicioViagem"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroExp = Request.GetStringParam("NumeroExp"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                SituacaoCargaJanelaCarregamento = Request.GetListEnumParam<SituacaoCargaJanelaCarregamento>("SituacaoCargaJanelaCarregamento"),
                SituacaoCotacao = Request.GetListEnumParam<SituacaoCargaJanelaCarregamentoCotacao>("SituacaoCotacao"),
                SituacaoLeilao = Request.GetListEnumParam<SituacaoCotacaoPesquisa>("SituacaoLeilao"),
                RecomendacaoGR = Request.GetNullableEnumParam<RecomendacaoGR>("StatusRecomendacaoGR"),
                UFOrigem = Request.GetStringParam("UFOrigem"),
                UFDestino = Request.GetStringParam("UFDestino"),
            };

            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
            if (operador != null && operador.SupervisorLogistica) filtrosPesquisa.CodigoOperador = Request.GetIntParam("Operador");

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento ObterFiltrosPesquisaCargaJanelaCarregamento(Repositorio.UnitOfWork unitOfWork, bool? excedente, bool? emReserva, bool apenasCargaNaoEmitidas, bool apenasCargasPendentes)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            List<string> listaSituacao = Request.GetListParam<string>("Situacao");

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento()
            {
                ApenasCargasNaoEmitidas = apenasCargaNaoEmitidas,
                ApenasCargasPendentes = apenasCargasPendentes,
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoPaisDestino = Request.GetIntParam("PaisDestino"),
                CodigoRota = Request.GetIntParam("Rota"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                EmReserva = emReserva,
                Excedente = excedente,
                NumeroPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                Ordem = Request.GetStringParam("Ordem"),
                PortoSaida = Request.GetStringParam("PortoSaida"),
                TipoEmbarque = Request.GetStringParam("TipoEmbarque"),
                UFOrigem = Request.GetStringParam("UFOrigem"),
                UFDestino = Request.GetStringParam("UFDestino"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                ExibirCargaQueNaoEstaoEmInicioViagem = Request.GetBoolParam("ExibirCargaQueNaoEstaoEmInicioViagem"),
                RecomendacaoGR = Request.GetNullableEnumParam<RecomendacaoGR>("StatusRecomendacaoGR"),
                SituacaoCotacao = Request.GetNullableListParam<SituacaoCargaJanelaCarregamentoCotacao>("SituacaoCotacao"),
                SituacaoLeilao = Request.GetNullableListParam<SituacaoCotacaoPesquisa>("SituacaoLeilao")
            };

            filtrosPesquisa.Operador = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
            if (filtrosPesquisa.Operador != null && filtrosPesquisa.Operador.SupervisorLogistica) filtrosPesquisa.CodigoOperador = Request.GetIntParam("Operador");
            filtrosPesquisa.RetornarPreCargas = (!emReserva.HasValue || !emReserva.Value) ? (configuracaoEmbarcador?.UtilizarPreCargaJanelaCarregamento ?? false) : true;

            if (filtrosPesquisa.UFDestino.Contains("0")) filtrosPesquisa.UFDestino = string.Empty;
            if (filtrosPesquisa.UFOrigem.Contains("0")) filtrosPesquisa.UFOrigem = string.Empty;

            if (apenasCargasPendentes || (excedente.HasValue && excedente.Value))
                filtrosPesquisa.RetornarPreCargas = false;

            filtrosPesquisa.SituacaoFaturada = (from situacao in listaSituacao where situacao == "99" select true).FirstOrDefault();
            filtrosPesquisa.SituacaoNaoFaturada = (from situacao in listaSituacao where situacao == "98" select true).FirstOrDefault();
            filtrosPesquisa.Situacoes = (from situacao in listaSituacao where situacao.ToNullableEnum<SituacaoCargaJanelaCarregamento>().HasValue select situacao.ToEnum<SituacaoCargaJanelaCarregamento>()).ToList();

            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            filtrosPesquisa.CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosCentroCarregamento = codigoCentroCarregamento == 0 ? ObterListaCodigoCentroCarregamentoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoCentroCarregamento };
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };


            return filtrosPesquisa;
        }

        private string ObterHoraFimDescarga(Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (cliente == null)
                return "";

            Repositorio.Embarcador.Pessoas.ClienteDescarga repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;
            if (clienteOrigem != null)
                clienteDescarga = repositorioClienteDescarga.BuscarPorOrigemEDestino(clienteOrigem.CPF_CNPJ, cliente.CPF_CNPJ);
            else
                clienteDescarga = repositorioClienteDescarga.BuscarPorPessoa(cliente.CPF_CNPJ);

            return clienteDescarga?.HoraLimiteDescarga ?? "";
        }

        private string ObterHoraInicioDescarga(Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (cliente == null)
                return "";

            Repositorio.Embarcador.Pessoas.ClienteDescarga repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;
            if (clienteOrigem != null)
                clienteDescarga = repositorioClienteDescarga.BuscarPorOrigemEDestino(clienteOrigem.CPF_CNPJ, cliente.CPF_CNPJ);
            else
                clienteDescarga = repositorioClienteDescarga.BuscarPorPessoa(cliente.CPF_CNPJ);

            return clienteDescarga?.HoraInicioDescarga ?? "";
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterJanelaCarregamentosPorFiltros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> carregamentos = repCargaJanelaCarregamento.Buscar(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            foreach (var cargaJanelaCarregamento in carregamentos)
            {
                if (cargaJanelaCarregamento.Carga.Carregamento == null)
                    cargasJanelaCarregamento.Add(cargaJanelaCarregamento);
                else if (cargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos != LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                {
                    if (!cargasJanelaCarregamento.Any(obj => obj.Carga.Carregamento != null && (obj.Carga.Carregamento.Codigo == cargaJanelaCarregamento.Carga.Carregamento.Codigo)))
                        cargasJanelaCarregamento.Add(cargaJanelaCarregamento);
                }
                else
                {
                    if (!cargasJanelaCarregamento.Any(obj => obj.Carga.Carregamento != null && (obj.Carga.Carregamento.Codigo == cargaJanelaCarregamento.Carga.Carregamento.Codigo) && (obj.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador)))
                        cargasJanelaCarregamento.Add(cargaJanelaCarregamento);
                }
            }

            if (!filtrosPesquisa.SituacaoFaturada && (ConfiguracaoEmbarcador?.UtilizarPreCargaJanelaCarregamento ?? false))
                cargasJanelaCarregamento.AddRange(repCargaJanelaCarregamento.BuscarPreCarga(filtrosPesquisa));

            return cargasJanelaCarregamento;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao ObterSituacaoAdicional(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> situacoesAdicionais, Repositorio.UnitOfWork unitOfWork)
        {
            if (situacoesAdicionais.Count == 0)
                return null;

            if ((cargaJanelaCarregamento.InicioCarregamento.Date >= DateTime.Now.Date) && (periodosCarregamento.Count > 0))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoForaPeriodo = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaCarregamentoAdicional.ForaPeriodoCarregamento).FirstOrDefault();

                if (situacaoForaPeriodo != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento = (
                        from o in periodosCarregamento
                        where (cargaJanelaCarregamento.InicioCarregamento.TimeOfDay >= o.HoraInicio && cargaJanelaCarregamento.InicioCarregamento.TimeOfDay <= o.HoraTermino)
                        select o
                    ).FirstOrDefault();

                    if (periodoCarregamento == null)
                        return situacaoForaPeriodo;
                }
            }

            if (carga.SituacaoCarga.IsSituacaoCargaFaturada())
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoDocumentosEmitidos = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaCarregamentoAdicional.DocumentosEmitidos).FirstOrDefault();
                return situacaoDocumentosEmitidos;
            }

            if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete || (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.ProcessandoDocumentosFiscais))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoDocumentosEmitidos = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaCarregamentoAdicional.NotaFiscalEmitida).FirstOrDefault();
                return situacaoDocumentosEmitidos;
            }

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);
            bool etapaChegadaVeiculoPendente = (fluxoGestaoPatio != null) && fluxoGestaoPatio.Etapas.Any(o => o.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.ChegadaVeiculo) && !fluxoGestaoPatio.DataChegadaVeiculo.HasValue;

            if (etapaChegadaVeiculoPendente)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

                if (DateTime.Now > cargaJanelaCarregamento.InicioCarregamento)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoAtrasoChegadaVeiculo = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaCarregamentoAdicional.AtrasoChegadaVeiculo).FirstOrDefault();
                    return situacaoAtrasoChegadaVeiculo;
                }
            }

            if (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.ProntaParaCarregamento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoDadosTransporteInformados = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaCarregamentoAdicional.DadosTransporteInformados).FirstOrDefault();
                return situacaoDadosTransporteInformados;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao situacaoSemDadosTransporte = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaCarregamentoAdicional.SemDadosTransporte).FirstOrDefault();
            return situacaoSemDadosTransporte;
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento BuscarCentroCarregamentoPorParametro(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

            int codigoCarga = Request.GetIntParam("Carga");
            int codigoFilial = Request.GetIntParam("Filial");

            if (codigoCarga > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                return cargaJanelaCarregamento.CentroCarregamento;
            }

            return repositorioCentroCarregamento.BuscarPorFilial(codigoFilial);
        }

        private Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento PreencherConfiguracaoDisponibilidadeCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento;
            int codigoCarga = Request.GetIntParam("Carga");

            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

                configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento
                {
                    CodigoCarga = carga.Codigo,
                    CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                    CodigoTransportador = carga.Empresa?.Codigo ?? 0,
                    CpfCnpjCliente = carga.Pedidos.FirstOrDefault()?.Pedido?.Destinatario?.CPF_CNPJ ?? 0,
                    CodigoModeloVeicularCarga = carga.ModeloVeicularCarga?.Codigo ?? 0
                };
            }
            else
            {
                configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
                {
                    CpfCnpjCliente = Request.GetIntParam("Cliente"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                };
            }

            return configuracaoDisponibilidadeCarregamento;
        }

        private double CalcularTempoLimiteCarregamentoEmMinutos(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            TimeSpan? dataLimite = null;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork).BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo).FirstOrDefault();

            if (cargaEntrega != null)
            {

                if (cargaEntrega.Carga.DataInicioViagem.HasValue)
                    dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataInicioViagem;
                else
                {
                    switch (configuracaoTMS.DataBaseCalculoPrevisaoControleEntrega)
                    {
                        case DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataCriacaoCarga;
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaEntrega.Carga.DataPrevisaoTerminoCarga;
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - (cargaEntrega.Carga.DataCarregamentoCarga.HasValue ? cargaEntrega.Carga.DataCarregamentoCarga : cargaEntrega.Carga.DataInicioViagemPrevista);
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - (cargaEntrega.Carga.DataInicioViagemPrevista.HasValue ? cargaEntrega.Carga.DataInicioViagemPrevista : cargaEntrega.Carga.DataPrevisaoTerminoCarga ?? cargaEntrega.Carga.DataCriacaoCarga);
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                            dataLimite = (cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada : cargaEntrega.DataPrevista) - cargaJanelaCarregamento?.InicioCarregamento;
                            break;
                    }
                }
            }

            return dataLimite.HasValue ? dataLimite.Value.TotalMinutes : 0;
        }

        private string ObterSituacaoCotacaoCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, int interessados, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaJanelaCarregamento.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao)
                return Localization.Resources.Cargas.Carga.LeilaoAguardandoAprovacaoDoVencedor;

            if (cargaJanelaCarregamento.ProcessoCotacaoFinalizada)
                if (interessados == 0 && cargaJanelaCarregamento.Carga.Empresa == null)
                    return Localization.Resources.Cargas.Carga.LeilaoSemLances;
                else
                    return Localization.Resources.Cargas.Carga.LeilaoFinalizado;

            if (cargaJanelaCarregamento.CargaLiberadaCotacao)
                return Localization.Resources.Cargas.Carga.LeilaoAbertoParaLances;

            return "";
        }

        private void SalvarDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo);

            if (carga.Veiculo == null)
                carga.Veiculo = veiculo;

            if (cargaMotoristas.Count == 0)
                servicoCargaMotorista.AtualizarMotorista(carga, motorista);

            repCarga.Atualizar(carga);
        }
        #endregion
    }
}
