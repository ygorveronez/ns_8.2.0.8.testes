using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CentroDescarregamento")]
    public class CentroDescarregamentoController : BaseController
    {
        #region Construtores

        public CentroDescarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Enum.TryParse(Request.Params("Ativo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo);

                int.TryParse(Request.Params("TipoCarga"), out int codigoTipoCarga);
                double.TryParse(Request.Params("Destinatario"), out double codigoDestinatario);

                bool.TryParse(Request.Params("SomenteCentrosOperadorLogistica"), out bool somenteCentrosOperadorLogistica);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoCapacidadeDescarregamentoPorPeso", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CentrosDescarregamento.Descricao, "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CentrosDescarregamento.Destinatario, "Destinatario", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CentrosDescarregamento.CanalEntrega, "CanalEntrega", 15, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.CentrosDescarregamento.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Destinatario")
                    propOrdena += ".Descricao";

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);

                /*if (somenteCentrosOperadorLogistica && !(operadorLogistica?.CentrosDescarregamento.Any() ?? true))
                    somenteCentrosOperadorLogistica = false;*/

                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> listaCentroDescarregamento = repCentroDescarregamento.Consultar(descricao, codigoDestinatario, codigoTipoCarga, ativo, somenteCentrosOperadorLogistica, operadorLogistica?.Codigo ?? 0, propOrdena, grid.dirOrdena, grid.inicio, grid.limite > 0 ? grid.limite : 1);
                grid.setarQuantidadeTotal(repCentroDescarregamento.ContarConsulta(descricao, codigoDestinatario, codigoTipoCarga, ativo, somenteCentrosOperadorLogistica, operadorLogistica?.Codigo ?? 0));

                var lista = (from p in listaCentroDescarregamento
                             select new
                             {
                                 p.Codigo,
                                 p.TipoCapacidadeDescarregamentoPorPeso,
                                 p.Descricao,
                                 Destinatario = p.Destinatario?.Descricao ?? string.Empty,
                                 CanalEntrega = p.CanalEntrega?.Descricao ?? string.Empty,
                                 p.DescricaoAtivo,
                             }).ToList();

                grid.AdicionaRows(lista);

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPeriodoDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                DateTime? dataDescarregamento = Request.GetNullableDateTimeParam("Data");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = null;

                if ((centroDescarregamento == null) || !dataDescarregamento.HasValue)
                    periodosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();
                else
                {
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoDia = (
                        from o in centroDescarregamento.ExcecoesCapacidadeDescarregamento
                        where o.DataInicial <= dataDescarregamento.Value.Date && o.DataFinal >= dataDescarregamento.Value.Date
                        select o
                    ).FirstOrDefault();

                    if (excecaoDia != null)
                        periodosDescarregamento = (from o in excecaoDia.PeriodosDescarregamento orderby o.HoraInicio select o).ToList();
                    else
                    {
                        DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataDescarregamento.Value);
                        periodosDescarregamento = (from o in centroDescarregamento.PeriodosDescarregamento where o.Dia == diaSemana select o).ToList();
                    }
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("HoraInicio", false);
                grid.AdicionarCabecalho("HoraTermino", false);
                grid.AdicionarCabecalho("DataDescarregamento", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroDescarregamento.DescarSimultaneo, "CapacidadeDescarregamentoSimultaneo", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroDescarregamento.Tolerancia, "ToleranciaExcessoTempo", 15, Models.Grid.Align.center, false);

                if (centroDescarregamento?.TipoCapacidadeDescarregamentoPorPeso == TipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroDescarregamento.CapacidadeTotal, "CapacidadeDescarregamento", 18, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho(Localization.Resources.Logistica.CentroDescarregamento.CapacidadeDisponivel, "CapacidadeDisponivel", 18, Models.Grid.Align.center, false);
                }

                var periodosDescarregamentoRetornar = (
                    from periodoDescarregamento in periodosDescarregamento
                    orderby periodoDescarregamento.HoraInicio
                    select ObterPeriodoDescarregamento(periodoDescarregamento, centroDescarregamento, dataDescarregamento.Value, unitOfWork)
                ).ToList();

                grid.AdicionaRows(periodosDescarregamentoRetornar);
                grid.setarQuantidadeTotal(periodosDescarregamento.Count());

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPeriodoDescarregamentoSugerido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
                DateTime? dataDescarregamento = Request.GetNullableDateTimeParam("Data");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatario(cpfCnpjDestinatario);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento;

                if ((centroDescarregamento == null) || !dataDescarregamento.HasValue)
                    periodosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();
                else
                {
                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoDia = (
                        from o in centroDescarregamento.ExcecoesCapacidadeDescarregamento
                        where o.DataInicial <= dataDescarregamento.Value.Date && o.DataFinal >= dataDescarregamento.Value.Date
                        select o
                    ).FirstOrDefault();

                    if (excecaoDia != null)
                        periodosDescarregamento = (from o in excecaoDia.PeriodosDescarregamento orderby o.HoraInicio select o).ToList();
                    else
                    {
                        DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataDescarregamento.Value);
                        periodosDescarregamento = (from o in centroDescarregamento.PeriodosDescarregamento where o.Dia == diaSemana orderby o.HoraInicio select o).ToList();
                    }
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("DataDescarregamento", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, false);

                List<dynamic> periodosDescarregamentoRetornar = new List<dynamic>();

                if (periodosDescarregamento.Count > 0)
                {
                    int tempoSugestaoHorario = (centroDescarregamento.TempoPadraoSugestaoHorario > 0) ? centroDescarregamento.TempoPadraoSugestaoHorario : 30;

                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento in periodosDescarregamento)
                    {
                        TimeSpan horaInicio = periodoDescarregamento.HoraInicio;
                        DateTime dataInicioPeriodo = dataDescarregamento.Value.Date.Add(periodoDescarregamento.HoraInicio);
                        DateTime dataTerminoPeriodo = dataDescarregamento.Value.Date.Add(periodoDescarregamento.HoraTermino);
                        List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Codigo).ToList();
                        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                        List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = repositorioCargaJanelaDescarregamento.BuscarCargaPeriodoPorIncidenciaDeHorario(codigoCargaDesconsiderar: 0, centroDescarregamento.Codigo, dataInicioPeriodo, dataTerminoPeriodo, codigosCanaisVenda);

                        while (true)
                        {
                            TimeSpan horaTermino = horaInicio.Add(TimeSpan.FromMinutes(tempoSugestaoHorario));

                            if (horaTermino > periodoDescarregamento.HoraTermino)
                                horaTermino = periodoDescarregamento.HoraTermino;

                            DateTime dataInicioPeriodoSugerido = dataDescarregamento.Value.Date.Add(horaInicio);
                            DateTime dataTerminoPeriodoSugerido = dataDescarregamento.Value.Date.Add(horaTermino);

                            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario = (
                                from o in cargasPeriodo
                                where (o.DataInicio >= dataInicioPeriodoSugerido && o.DataInicio < dataTerminoPeriodoSugerido) || (o.DataFim > dataInicioPeriodoSugerido && o.DataFim <= dataTerminoPeriodoSugerido)
                                select o
                            ).ToList();

                            if ((dataInicioPeriodoSugerido > DateTime.Now) && (cargasPeriodoHorario.Count < periodoDescarregamento.CapacidadeDescarregamentoSimultaneo))
                                periodosDescarregamentoRetornar.Add(new
                                {
                                    Codigo = horaTermino.Ticks,
                                    DataDescarregamento = dataDescarregamento.Value.Date.Add(horaInicio).ToDateTimeString(),
                                    Descricao = horaInicio.ToTimeString()
                                });

                            if (horaTermino == periodoDescarregamento.HoraTermino)
                                break;

                            horaInicio = horaTermino;
                        }
                    }
                }

                grid.AdicionaRows(periodosDescarregamentoRetornar);
                grid.setarQuantidadeTotal(periodosDescarregamentoRetornar.Count());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("Ativo"), out bool ativo);
                bool.TryParse(Request.Params("LiberarCargaAutomaticamenteParaTransportadoras"), out bool liberarCargaAutomaticamenteParaTransportadoras);

                double.TryParse(Request.Params("Destinatario"), out double codigoDestinatario);
                int.TryParse(Request.Params("NumeroDocas"), out int numeroDocas);
                int.TryParse(Request.Params("CanalEntrega"), out int codigoCanalEntrega);

                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroDescarregamento tipoTransportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroDescarregamento.Todos;
                Enum.TryParse(Request.Params("TipoTransportadorCentroDescarregamento"), out tipoTransportador);

                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unidadeDeTrabalho);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);

                int codigoFilial = Request.GetIntParam("Filial");

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = new Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento
                {
                    Ativo = ativo,
                    Descricao = descricao,
                    Destinatario = repCliente.BuscarPorCPFCNPJ(codigoDestinatario),
                    CanalEntrega = repCanalEntrega.BuscarPorCodigo(codigoCanalEntrega),
                    Filial = codigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null,
                    NumeroDocas = numeroDocas,
                    Observacao = observacao,
                    TipoTransportador = tipoTransportador,
                    LiberarCargaAutomaticamenteParaTransportadoras = liberarCargaAutomaticamenteParaTransportadoras,
                    CapacidadeDescarregamentoSegunda = Request.GetIntParam("CapacidadeDescarregamentoSegunda"),
                    CapacidadeDescarregamentoTerca = Request.GetIntParam("CapacidadeDescarregamentoTerca"),
                    CapacidadeDescarregamentoQuarta = Request.GetIntParam("CapacidadeDescarregamentoQuarta"),
                    CapacidadeDescarregamentoQuinta = Request.GetIntParam("CapacidadeDescarregamentoQuinta"),
                    CapacidadeDescarregamentoSexta = Request.GetIntParam("CapacidadeDescarregamentoSexta"),
                    CapacidadeDescarregamentoSabado = Request.GetIntParam("CapacidadeDescarregamentoSabado"),
                    CapacidadeDescarregamentoDomingo = Request.GetIntParam("CapacidadeDescarregamentoDomingo"),
                    UtilizarCapacidadeDescarregamentoPorPeso = Request.GetBoolParam("UtilizarCapacidadeDescarregamentoPorPeso"),
                    UtilizarCapacidadeDescarregamentoPesoLiquido = Request.GetBoolParam("CapacidadeDescarregamentoPesoLiquido"),
                    TipoCapacidadeDescarregamentoPorPeso = Request.GetNullableEnumParam<TipoCapacidadeDescarregamentoPorPeso>("TipoCapacidadeDescarregamentoPorPeso"),
                    BloquearJanelaDescarregamentoExcedente = Request.GetBoolParam("BloquearJanelaDescarregamentoExcedente"),
                    AprovarAutomaticamenteDescargaComHorarioDisponivel = Request.GetBoolParam("AprovarAutomaticamenteDescargaComHorarioDisponivel"),
                    PermitirGeracaoJanelaParaCargaRedespacho = Request.GetBoolParam("PermitirGeracaoJanelaParaCargaRedespacho"),
                    CapacidadeDescaregamentoPorDia = Request.GetBoolParam("CapacidadeDescaregamentoPorDia"),
                    TempoPadraoSugestaoHorario = Request.GetIntParam("TempoPadraoSugestaoHorario"),
                    PermitirBuscarAteFimDaJanela = Request.GetBoolParam("PermitirBuscarAteFimDaJanela"),
                    ExibirJanelaDescargaPorPedido = Request.GetBoolParam("ExibirJanelaDescargaPorPedido"),
                    LimitePadrao = Request.GetIntParam("LimitePadrao"),
                    PercentualToleranciaPesoDescarregamento = Request.GetDecimalParam("PercentualToleranciaPesoDescarregamento"),
                    BuscarSenhaViaIntegracao = Request.GetBoolParam("BuscarSenhaViaIntegracao"),
                    DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada = Request.GetBoolParam("DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada"),
                    UsarLayoutAgendamentoPorCaixaItem = Request.GetBoolParam("UsarLayoutAgendamentoPorCaixaItem"),
                    PermitirGerarDescargaArmazemExterno = Request.GetBoolParam("PermitirGerarDescargaArmazemExterno"),
                    ExigeAprovacaoCargaParaDescarregamento = Request.GetBoolParam("ExigeAprovacaoCargaParaDescarregamento"),
                    GerarFluxoPatioAposConfirmacaoAgendamento = Request.GetBoolParam("GerarFluxoPatioAposConfirmacaoAgendamento"),
                };

                repCentroDescarregamento.Inserir(centroDescarregamento, Auditado);

                int dia = Request.GetIntParam("Dia");
                int mes = Request.GetIntParam("Mes");

                SetarTransportadores(ref centroDescarregamento);
                SetarTiposDeCarga(ref centroDescarregamento);
                SetarVeiculosPermitidos(ref centroDescarregamento);
                SalvarTemposDescarregamento(centroDescarregamento, null, unidadeDeTrabalho);
                SalvarLimitesAgendamento(centroDescarregamento, null, unidadeDeTrabalho);
                SalvarQuantidadePorTipoDeCarga(centroDescarregamento, unidadeDeTrabalho);
                SalvarEmails(centroDescarregamento, null, unidadeDeTrabalho);
                SalvarPeriodosDescarregamento(centroDescarregamento, null, unidadeDeTrabalho, dia, mes);
                SalvarPrevisoesDescarregamento(centroDescarregamento, null, unidadeDeTrabalho);
                SalvarLimiteDescarregamento(centroDescarregamento, null, unidadeDeTrabalho);
                SalvarHorariosAprovacaoAutomatica(centroDescarregamento, unidadeDeTrabalho);

                repCentroDescarregamento.Atualizar(centroDescarregamento);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("Ativo"), out bool ativo);
                bool.TryParse(Request.Params("LiberarCargaAutomaticamenteParaTransportadoras"), out bool liberarCargaAutomaticamenteParaTransportadoras);

                double.TryParse(Request.Params("Destinatario"), out double codigoDestinatario);
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("NumeroDocas"), out int numeroDocas);
                int.TryParse(Request.Params("CanalEntrega"), out int codigoCanalEntrega);

                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");
                int codigoFilial = Request.GetIntParam("Filial");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroDescarregamento tipoTransportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTransportadorCentroDescarregamento.Todos;
                Enum.TryParse(Request.Params("TipoTransportadorCentroDescarregamento"), out tipoTransportador);

                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);
                Repositorio.Cliente repClientes = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigo, true);

                if (centroDescarregamento == null)
                    return new JsonpResult(true, false, Localization.Resources.Logistica.CentroDescarregamento.CentroCarregamentoNaoEncontrado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                centroDescarregamento.Ativo = ativo;
                centroDescarregamento.Descricao = descricao;
                centroDescarregamento.Destinatario = repClientes.BuscarPorCPFCNPJ(codigoDestinatario);
                centroDescarregamento.CanalEntrega = repCanalEntrega.BuscarPorCodigo(codigoCanalEntrega);
                centroDescarregamento.Observacao = observacao;
                centroDescarregamento.NumeroDocas = numeroDocas;
                centroDescarregamento.Filial = codigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null;
                centroDescarregamento.TipoTransportador = tipoTransportador;
                centroDescarregamento.LiberarCargaAutomaticamenteParaTransportadoras = liberarCargaAutomaticamenteParaTransportadoras;
                centroDescarregamento.CapacidadeDescarregamentoSegunda = Request.GetIntParam("CapacidadeDescarregamentoSegunda");
                centroDescarregamento.CapacidadeDescarregamentoTerca = Request.GetIntParam("CapacidadeDescarregamentoTerca");
                centroDescarregamento.CapacidadeDescarregamentoQuarta = Request.GetIntParam("CapacidadeDescarregamentoQuarta");
                centroDescarregamento.CapacidadeDescarregamentoQuinta = Request.GetIntParam("CapacidadeDescarregamentoQuinta");
                centroDescarregamento.CapacidadeDescarregamentoSexta = Request.GetIntParam("CapacidadeDescarregamentoSexta");
                centroDescarregamento.CapacidadeDescarregamentoSabado = Request.GetIntParam("CapacidadeDescarregamentoSabado");
                centroDescarregamento.CapacidadeDescarregamentoDomingo = Request.GetIntParam("CapacidadeDescarregamentoDomingo");
                centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso = Request.GetBoolParam("UtilizarCapacidadeDescarregamentoPorPeso");
                centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso = Request.GetNullableEnumParam<TipoCapacidadeDescarregamentoPorPeso>("TipoCapacidadeDescarregamentoPorPeso");
                centroDescarregamento.BloquearJanelaDescarregamentoExcedente = Request.GetBoolParam("BloquearJanelaDescarregamentoExcedente");
                centroDescarregamento.AprovarAutomaticamenteDescargaComHorarioDisponivel = Request.GetBoolParam("AprovarAutomaticamenteDescargaComHorarioDisponivel");
                centroDescarregamento.PermitirGeracaoJanelaParaCargaRedespacho = Request.GetBoolParam("PermitirGeracaoJanelaParaCargaRedespacho");
                centroDescarregamento.CapacidadeDescaregamentoPorDia = Request.GetBoolParam("CapacidadeDescaregamentoPorDia");
                centroDescarregamento.TempoPadraoSugestaoHorario = Request.GetIntParam("TempoPadraoSugestaoHorario");
                centroDescarregamento.PermitirBuscarAteFimDaJanela = Request.GetBoolParam("PermitirBuscarAteFimDaJanela");
                centroDescarregamento.TempoPadraoDeEntrega = Request.GetIntParam("TempoPadraoDeEntrega");
                centroDescarregamento.ExibirJanelaDescargaPorPedido = Request.GetBoolParam("ExibirJanelaDescargaPorPedido");
                centroDescarregamento.LimitePadrao = Request.GetIntParam("LimitePadrao");
                centroDescarregamento.PercentualToleranciaPesoDescarregamento = Request.GetDecimalParam("PercentualToleranciaPesoDescarregamento");
                centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido = Request.GetBoolParam("CapacidadeDescarregamentoPesoLiquido");
                centroDescarregamento.BuscarSenhaViaIntegracao = Request.GetBoolParam("BuscarSenhaViaIntegracao");
                centroDescarregamento.DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada = Request.GetBoolParam("DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada");
                centroDescarregamento.UsarLayoutAgendamentoPorCaixaItem = Request.GetBoolParam("UsarLayoutAgendamentoPorCaixaItem");
                centroDescarregamento.PermitirGerarDescargaArmazemExterno = Request.GetBoolParam("PermitirGerarDescargaArmazemExterno");
                centroDescarregamento.ExigeAprovacaoCargaParaDescarregamento = Request.GetBoolParam("ExigeAprovacaoCargaParaDescarregamento");
                centroDescarregamento.GerarFluxoPatioAposConfirmacaoAgendamento = Request.GetBoolParam("GerarFluxoPatioAposConfirmacaoAgendamento");

                SetarTransportadores(ref centroDescarregamento);
                SetarTiposDeCarga(ref centroDescarregamento);
                SetarVeiculosPermitidos(ref centroDescarregamento);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repCentroDescarregamento.Atualizar(centroDescarregamento, Auditado);

                int dia = Request.GetIntParam("Dia");
                int mes = Request.GetIntParam("Mes");

                SalvarTemposDescarregamento(centroDescarregamento, historico, unidadeDeTrabalho);
                SalvarLimitesAgendamento(centroDescarregamento, historico, unidadeDeTrabalho);
                SalvarQuantidadePorTipoDeCarga(centroDescarregamento, unidadeDeTrabalho);
                SalvarEmails(centroDescarregamento, historico, unidadeDeTrabalho);
                SalvarPeriodosDescarregamento(centroDescarregamento, historico, unidadeDeTrabalho, dia, mes);
                SalvarPrevisoesDescarregamento(centroDescarregamento, historico, unidadeDeTrabalho);
                SalvarLimiteDescarregamento(centroDescarregamento, historico, unidadeDeTrabalho);
                SalvarHorariosAprovacaoAutomatica(centroDescarregamento, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");
                DateTime dataAtual = DateTime.Now;
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigo);

                if (centroDescarregamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.CentroDescarregamento.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadeTipoCargaTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento repositorioQuantidadePorTipoDeCargaDescarregamento = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento repositorioHoraroAprovacaoAutomatica = new Repositorio.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PrevisaoDescarregamento repositorioPrevisaoDescarregamento = new Repositorio.Embarcador.Logistica.PrevisaoDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento repositorioLimiteDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorioPeriodoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda repositorioPeriodoCanalVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> quantidadePorTipoCargaTipoCarga = repositorioQuantidadeTipoCargaTipoCarga.BuscarPorCentroDescarregamento(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> quantidadePorTipoCarga = repositorioQuantidadePorTipoDeCargaDescarregamento.BuscarPorCentroDescarregamento(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento> horariosAprovacaoAutomatica = repositorioHoraroAprovacaoAutomatica.BuscarPorCentroDescarregamento(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> gruposPessoa = repositorioPeriodoGrupoPessoa.BuscarPorCentroDescarregamento(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> canaisVenda = repositorioPeriodoCanalVenda.BuscarPorCentroDescarregamento(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> grupoProdutos = repositorioGrupoProduto.BuscarPorCentroDescarregamento(codigo);

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> listaPeriodoDescarregamento = !(centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false)
                    ? centroDescarregamento.PeriodosDescarregamento.ToList() : repositorioPeriodoDescarregamento.BuscarPorDiaMes(codigo, dataAtual.Day, dataAtual.Month);

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> listaPrevisaoDescarregamento = !(centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false)
                    ? centroDescarregamento.PrevisoesDescarregamento.ToList() : repositorioPrevisaoDescarregamento.BuscarPorDiaMes(codigo, dataAtual.Day, dataAtual.Month);

                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento> listaLimiteDescarregamento = !(centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false)
                    ? centroDescarregamento.LimitesDescarregamento.ToList() : repositorioLimiteDescarregamento.BuscarPorDiaMes(codigo, dataAtual.Day, dataAtual.Month);

                var retorno = new
                {
                    Codigo = duplicar ? 0 : centroDescarregamento.Codigo,
                    centroDescarregamento.Descricao,
                    centroDescarregamento.Ativo,
                    centroDescarregamento.Observacao,
                    centroDescarregamento.NumeroDocas,
                    Filial = new { Codigo = centroDescarregamento.Filial?.Codigo ?? 0, Descricao = centroDescarregamento.Filial?.Descricao ?? "" },
                    TipoTransportadorCentroDescarregamento = centroDescarregamento.TipoTransportador,
                    centroDescarregamento.LiberarCargaAutomaticamenteParaTransportadoras,
                    CapacidadeDescarregamentoSegunda = centroDescarregamento.CapacidadeDescarregamentoSegunda > 0 ? centroDescarregamento.CapacidadeDescarregamentoSegunda.ToString("n0") : "",
                    CapacidadeDescarregamentoTerca = centroDescarregamento.CapacidadeDescarregamentoTerca > 0 ? centroDescarregamento.CapacidadeDescarregamentoTerca.ToString("n0") : "",
                    CapacidadeDescarregamentoQuarta = centroDescarregamento.CapacidadeDescarregamentoQuarta > 0 ? centroDescarregamento.CapacidadeDescarregamentoQuarta.ToString("n0") : "",
                    CapacidadeDescarregamentoQuinta = centroDescarregamento.CapacidadeDescarregamentoQuinta > 0 ? centroDescarregamento.CapacidadeDescarregamentoQuinta.ToString("n0") : "",
                    CapacidadeDescarregamentoSexta = centroDescarregamento.CapacidadeDescarregamentoSexta > 0 ? centroDescarregamento.CapacidadeDescarregamentoSexta.ToString("n0") : "",
                    CapacidadeDescarregamentoSabado = centroDescarregamento.CapacidadeDescarregamentoSabado > 0 ? centroDescarregamento.CapacidadeDescarregamentoSabado.ToString("n0") : "",
                    CapacidadeDescarregamentoDomingo = centroDescarregamento.CapacidadeDescarregamentoDomingo > 0 ? centroDescarregamento.CapacidadeDescarregamentoDomingo.ToString("n0") : "",
                    centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso,
                    centroDescarregamento.AprovarAutomaticamenteDescargaComHorarioDisponivel,
                    centroDescarregamento.PermitirGeracaoJanelaParaCargaRedespacho,
                    centroDescarregamento.CapacidadeDescaregamentoPorDia,
                    TempoPadraoSugestaoHorario = centroDescarregamento.TempoPadraoSugestaoHorario > 0 ? centroDescarregamento.TempoPadraoSugestaoHorario.ToString("n0") : "",
                    centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso,
                    centroDescarregamento.BloquearJanelaDescarregamentoExcedente,
                    centroDescarregamento.ExibirJanelaDescargaPorPedido,
                    centroDescarregamento.PermitirBuscarAteFimDaJanela,
                    centroDescarregamento.TempoPadraoDeEntrega,
                    centroDescarregamento.PercentualToleranciaPesoDescarregamento,
                    centroDescarregamento.BuscarSenhaViaIntegracao,
                    centroDescarregamento.DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada,
                    centroDescarregamento.UsarLayoutAgendamentoPorCaixaItem,
                    centroDescarregamento.PermitirGerarDescargaArmazemExterno,
                    centroDescarregamento.ExigeAprovacaoCargaParaDescarregamento,
                    centroDescarregamento.GerarFluxoPatioAposConfirmacaoAgendamento,
                    CapacidadeDescarregamentoPesoLiquido = centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido,
                    Destinatario = new { Codigo = centroDescarregamento.Destinatario?.Codigo ?? 0, Descricao = centroDescarregamento.Destinatario?.Descricao ?? string.Empty },
                    CanalEntrega = new { Codigo = centroDescarregamento.CanalEntrega?.Codigo ?? 0, Descricao = centroDescarregamento.CanalEntrega?.Descricao ?? string.Empty },
                    DiaMes = new
                    {
                        Mes = dataAtual.Month,
                        Dia = dataAtual.Day,
                    },
                    LimiteAgendamentoPadrao = new
                    {
                        centroDescarregamento.LimitePadrao
                    },
                    TiposCarga = (
                        from obj in centroDescarregamento.TiposCarga
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao
                        }
                    ).ToList(),
                    VeiculosPermitidos = (
                        from obj in centroDescarregamento.VeiculosPermitidos
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao
                        }
                    ).ToList(),
                    Transportadores = (
                        from obj in centroDescarregamento.Transportadores
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.RazaoSocial
                        }
                    ).ToList(),
                    TemposDescarregamento = (
                        from obj in centroDescarregamento.TemposDescarregamento
                        select new
                        {
                            Codigo = duplicar ? 0 : obj.Codigo,
                            obj.Tempo,
                            obj.TipoTempo,
                            SkuDe = obj.SkuDe?.ToString() ?? string.Empty,
                            SkuAte = obj.SkuAte?.ToString() ?? string.Empty,
                            TipoCarga = new
                            {
                                Codigo = obj.TipoCarga.Codigo,
                                Descricao = obj.TipoCarga.Descricao
                            },
                            ModeloVeiculo = new
                            {
                                Codigo = obj.ModeloVeicular.Codigo,
                                Descricao = obj.ModeloVeicular.Descricao
                            },
                        }
                    ).ToList(),
                    QuantidadePorTipoDeCarga = (
                        from obj in quantidadePorTipoCarga
                        select new
                        {
                            Codigo = duplicar ? 0 : obj.Codigo,
                            obj.Volumes,
                            Tolerancia = obj?.Tolerancia ?? 0,
                            ToleranciaCancelamentoAgendaConfirmada = obj?.ToleranciaCancelamentoAgendaConfirmada ?? 0,
                            ToleranciaCancelamentoAgendaNaoConfirmada = obj?.ToleranciaCancelamentoAgendaNaoConfirmada ?? 0,
                            CodigoDia = obj.Dia,
                            Dia = obj.Dia.ObterDescricao(),
                            DescricaoTipoCarga = string.Join(", ", (from elemento in quantidadePorTipoCargaTipoCarga where elemento.QuantidadePorTipoDeCargaDescarregamento.Codigo == obj.Codigo select elemento.TipoCarga.Descricao).ToList()),
                            TiposCarga = (
                                from elemento in quantidadePorTipoCargaTipoCarga
                                where elemento.QuantidadePorTipoDeCargaDescarregamento.Codigo == obj.Codigo
                                select new
                                {
                                    elemento.TipoCarga.Codigo,
                                    elemento.TipoCarga.Descricao
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    LimiteAgendamento = (
                        from obj in centroDescarregamento.LimitesAgendamento
                        select new
                        {
                            Codigo = duplicar ? 0 : obj.Codigo,
                            NovoLimite = obj.Limite,
                            PermiteUltrapassarLimiteVolume = obj.PermiteUltrapassarLimiteVolume,
                            PermiteUltrapassarLimiteVolumeDescricao = obj.PermiteUltrapassarLimiteVolume ? "Sim" : "Não",
                            GrupoPessoa = new
                            {
                                obj.GrupoPessoa.Codigo,
                                obj.GrupoPessoa.Descricao
                            }
                        }
                    ).ToList(),
                    Emails = (
                        from obj in centroDescarregamento.Emails
                        select new
                        {
                            Codigo = duplicar ? 0 : obj.Codigo,
                            obj.Email
                        }
                    ).ToList(),
                    PeriodosDescarregamento = (
                        from obj in listaPeriodoDescarregamento
                        select new
                        {
                            Codigo = duplicar ? 0 : obj.Codigo,
                            DiaSemana = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? 0 : obj.Dia,
                            Dia = obj.DiaDoMes,
                            Mes = obj.Mes,
                            HoraInicio = string.Format("{0:00}:{1:00}", obj.HoraInicio.Hours, obj.HoraInicio.Minutes),
                            HoraTermino = string.Format("{0:00}:{1:00}", obj.HoraTermino.Hours, obj.HoraTermino.Minutes),
                            SkuDe = obj.SkuDe?.ToString() ?? string.Empty,
                            SkuAte = obj.SkuAte?.ToString() ?? string.Empty,
                            obj.ToleranciaExcessoTempo,
                            obj.CapacidadeDescarregamentoSimultaneo,
                            obj.CapacidadeDescarregamentoSimultaneoAdicional,
                            CapacidadeDescarregamento = (obj.CapacidadeDescarregamento > 0) ? obj.CapacidadeDescarregamento.ToString("n0") : "",
                            Remetentes = (from remetente in obj.Remetentes select new { remetente?.Remetente?.Codigo, remetente?.Remetente?.Descricao }).ToList(),
                            TiposCarga = (from tipoCarga in obj.TiposDeCarga select new { tipoCarga?.TipoDeCarga?.Codigo, tipoCarga?.TipoDeCarga?.Descricao }).ToList(),
                            GruposPessoas = (from grupoPessoa in gruposPessoa where grupoPessoa.PeriodoDescarregamento.Codigo == obj.Codigo select new { grupoPessoa.GrupoPessoas.Codigo, grupoPessoa.GrupoPessoas.Descricao }).ToList(),
                            CanaisVenda = (from canalVenda in canaisVenda where canalVenda.PeriodoDescarregamento.Codigo == obj.Codigo select new { canalVenda.CanalVenda.Codigo, canalVenda.CanalVenda.Descricao }).ToList(),
                            GrupoProduto = (from grupoProduto in grupoProdutos where grupoProduto.PeriodoDescarregamento.Codigo == obj.Codigo select new { grupoProduto.GrupoProduto.Codigo, grupoProduto.GrupoProduto.Descricao }).ToList()
                        }
                    ).ToList(),
                    PrevisoesDescarregamento = (
                    listaPrevisaoDescarregamento != null && listaPrevisaoDescarregamento.Count > 0 && listaPrevisaoDescarregamento[0] != null ?
                        (from obj in listaPrevisaoDescarregamento
                         select new
                         {
                             Codigo = duplicar ? 0 : obj.Codigo,
                             Descricao = obj.Descricao ?? string.Empty,
                             DiaSemana = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? 0 : obj.Dia,
                             Dia = obj.DiaDoMes,
                             Mes = obj.Mes,
                             Rota = new
                             {
                                 obj.Rota.Codigo,
                                 obj.Rota.Descricao
                             },
                             ModelosVeiculos = (
                                 from modelo in obj.ModelosVeiculos
                                 select new
                                 {
                                     Codigo = modelo.Codigo,
                                     Descricao = modelo.Descricao
                                 }
                             ).ToList(),
                             obj.QuantidadeCargas,
                             obj.QuantidadeCargasExcedentes
                         }
                    ).ToList() : null),
                    LimitesDescarregamento = (
                    listaLimiteDescarregamento != null && listaLimiteDescarregamento.Count > 0 && listaLimiteDescarregamento[0] != null ?
                        (from o in listaLimiteDescarregamento
                         select new
                         {
                             Codigo = duplicar ? 0 : o.Codigo,
                             DiaSemana = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? 0 : o.Dia,
                             Dia = o.DiaDoMes,
                             Mes = o.Mes,
                             o.HorasAntecedencia,
                             TipoCarga = new
                             {
                                 o.TipoCarga.Codigo,
                                 o.TipoCarga.Descricao
                             }
                         }
                    ).ToList() : null),
                    HorariosAprovacaoAutomatica = (
                        from obj in horariosAprovacaoAutomatica
                        select new
                        {
                            Codigo = duplicar ? 0 : obj.Codigo,
                            DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                            DataFinal = obj.DataFinal.ToString("dd/MM/yyyy")
                        }
                    ).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCapacidadePorDiaMes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int dia = Request.GetIntParam("Dia");
                int mes = Request.GetIntParam("Mes");

                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PrevisaoDescarregamento repPrevisaoDescarregamento = new Repositorio.Embarcador.Logistica.PrevisaoDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodosDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento repLimiteDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorioPeriodoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda repositorioPeriodoCanalVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> listaPrevisaoDescarregamento = repPrevisaoDescarregamento.BuscarPorDiaMes(codigo, dia, mes);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> listaPeriodoDescarregamento = repPeriodosDescarregamento.BuscarPorDiaMes(codigo, dia, mes);
                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento> listaLimiteDescarregamento = repLimiteDescarregamento.BuscarPorDiaMes(codigo, dia, mes);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> gruposPessoa = repositorioPeriodoGrupoPessoa.BuscarPorDiaMes(codigo, dia, mes);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> canaisVenda = repositorioPeriodoCanalVenda.BuscarPorCentroDescarregamento(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> grupoProdutos = repositorioGrupoProduto.BuscarPorCentroDescarregamento(codigo);

                var retorno = new
                {
                    CapacidadeDescarregamentoPesoLiquido = centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido,
                    TiposCarga = (
                        from obj in centroDescarregamento.TiposCarga
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    ).ToList(),
                    PeriodosDescarregamento = (
                        from obj in listaPeriodoDescarregamento
                        select new
                        {
                            obj.Codigo,
                            DiaSemana = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? 0 : obj.Dia,
                            HoraInicio = string.Format("{0:00}:{1:00}", obj.HoraInicio.Hours, obj.HoraInicio.Minutes),
                            HoraTermino = string.Format("{0:00}:{1:00}", obj.HoraTermino.Hours, obj.HoraTermino.Minutes),
                            SkuDe = obj.SkuDe?.ToString() ?? string.Empty,
                            SkuAte = obj.SkuAte?.ToString() ?? string.Empty,
                            obj.ToleranciaExcessoTempo,
                            obj.CapacidadeDescarregamentoSimultaneo,
                            obj.CapacidadeDescarregamentoSimultaneoAdicional,
                            CapacidadeDescarregamento = (obj.CapacidadeDescarregamento > 0) ? obj.CapacidadeDescarregamento.ToString("n0") : "",
                            Remetentes = (from remetente in obj.Remetentes select new { remetente?.Remetente?.Codigo, remetente?.Remetente?.Descricao }).ToList(),
                            TiposCarga = (from tipoCarga in obj.TiposDeCarga select new { tipoCarga?.TipoDeCarga?.Codigo, tipoCarga?.TipoDeCarga?.Descricao }).ToList(),
                            GruposPessoas = (from grupoPessoa in gruposPessoa where grupoPessoa?.PeriodoDescarregamento?.Codigo == obj.Codigo select new { grupoPessoa?.GrupoPessoas?.Codigo, grupoPessoa?.GrupoPessoas?.Descricao }).ToList(),
                            CanaisVenda = (from canalVenda in canaisVenda where canalVenda.PeriodoDescarregamento.Codigo == obj.Codigo select new { canalVenda.CanalVenda.Codigo, canalVenda.CanalVenda.Descricao }).ToList(),
                            GrupoProduto = (from grupoProduto in grupoProdutos where grupoProduto?.PeriodoDescarregamento?.Codigo == obj.Codigo select new { grupoProduto?.GrupoProduto?.Codigo, grupoProduto?.GrupoProduto?.Descricao }).ToList()

                        }
                    ).ToList(),
                    PrevisoesDescarregamento = (
                        from obj in listaPrevisaoDescarregamento
                        select new
                        {
                            obj.Codigo,
                            Descricao = obj.Descricao ?? string.Empty,
                            DiaSemana = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? 0 : obj.Dia,
                            Rota = new
                            {
                                obj.Rota.Codigo,
                                obj.Rota.Descricao
                            },
                            ModelosVeiculos = (
                                from modelo in obj.ModelosVeiculos
                                select new
                                {
                                    modelo.Codigo,
                                    modelo.Descricao
                                }
                            ).ToList(),
                            obj.QuantidadeCargas,
                            obj.QuantidadeCargasExcedentes
                        }
                    ).ToList(),
                    LimitesDescarregamento = (
                        from o in listaLimiteDescarregamento
                        select new
                        {
                            o.Codigo,
                            DiaSemana = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? 0 : o.Dia,
                            o.HorasAntecedencia,
                            TipoCarga = new
                            {
                                o.TipoCarga.Codigo,
                                o.TipoCarga.Descricao
                            }
                        }
                    ).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Logistica.TempoDescarregamento repTempoDescarregamento = new Repositorio.Embarcador.Logistica.TempoDescarregamento(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigo);

                centroDescarregamento.TemposDescarregamento.Clear();
                centroDescarregamento.Emails.Clear();
                centroDescarregamento.PeriodosDescarregamento.Clear();
                centroDescarregamento.PrevisoesDescarregamento.Clear();
                centroDescarregamento.LimitesDescarregamento.Clear();
                centroDescarregamento.TiposCarga.Clear();
                centroDescarregamento.Transportadores.Clear();
                centroDescarregamento.ExcecoesCapacidadeDescarregamento.Clear();

                repCentroDescarregamento.Deletar(centroDescarregamento, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Logistica.CentroDescarregamento.NaoPossívelExcluirRegistroMesmoPossuiVinculoRecursosSistemautilizalo);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);

            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfigurarImportacaoCentroDescarregamento();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                string dados = Request.Params("Dados");
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = Servicos.Embarcador.Logistica.CentroDescarregamento.ImportarCentroCarregamento(dados, this.Usuario, configuracoes, operadorLogistica, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao, unitOfWork);
                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarCapacidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfigurarImportacaoCapacidade();
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                int.TryParse((string)parametro.Codigo, out int codigo);
                string dados = Request.Params("Dados");
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = Servicos.Embarcador.Logistica.CentroDescarregamento.ImportarCapacidadeDescarregamento(dados, this.Usuario, configuracoes, operadorLogistica, TipoServicoMultisoftware, Auditado, codigo, unitOfWork);
                retornoImportacao.Retorno = true;
                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfigurarImportacaoCentroDescarregamento();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfiguracaoImportacaoCapacidade()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfigurarImportacaoCapacidade();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> SalvarCapacidadeCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCentroDescarregamento = Request.GetIntParam("CodigoCentroDescarregamento");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento, auditavel: true);

                if (centroDescarregamento == null)
                    throw new ControllerException(Localization.Resources.Logistica.CentroDescarregamento.UmCentroDescarregamentPrecisaCadastradaSalvarDia);

                if (!centroDescarregamento.CapacidadeDescaregamentoPorDia)
                    throw new ControllerException(Localization.Resources.Logistica.CentroDescarregamento.OCentroDescarregamentoPrecisaConfigurado);

                int dia = Request.GetIntParam("Dia");
                int mes = Request.GetIntParam("Mes");
                bool copiarPeriodo = Request.GetBoolParam("CopiarPeriodo");
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorioCentroDescarregamento.Atualizar(centroDescarregamento, Auditado);

                SalvarPeriodosDescarregamento(centroDescarregamento, historico, unitOfWork, dia, mes, copiarPeriodo);
                SalvarPrevisoesDescarregamento(centroDescarregamento, historico, unitOfWork, dia, mes, copiarPeriodo);
                SalvarLimiteDescarregamento(centroDescarregamento, historico, unitOfWork, dia, mes, copiarPeriodo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CopiarCapacidadeCarregamentoDiaParaMes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCentroDescarregamento = Request.GetIntParam("CodigoCentroDescarregamento");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento, auditavel: true);

                if (centroDescarregamento == null)
                    throw new ControllerException(Localization.Resources.Logistica.CentroDescarregamento.UmCentroDescarregamentPrecisaCadastradaSalvarDia);

                if (!centroDescarregamento.CapacidadeDescaregamentoPorDia)
                    throw new ControllerException(Localization.Resources.Logistica.CentroDescarregamento.OCentroDescarregamentoPrecisaConfigurado);

                int dia = Request.GetIntParam("Dia");
                int mes = Request.GetIntParam("Mes");
                int ano = Request.GetIntParam("Ano");
                int diasNoMes = System.DateTime.DaysInMonth(ano, mes);
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorioCentroDescarregamento.Atualizar(centroDescarregamento, Auditado);

                for (int diaCopiar = 1; diaCopiar <= diasNoMes; diaCopiar++)
                {
                    if (diaCopiar == dia)
                        continue;

                    SalvarPeriodosDescarregamento(centroDescarregamento, historico, unitOfWork, diaCopiar, mes, copia: true);
                    SalvarPrevisoesDescarregamento(centroDescarregamento, historico, unitOfWork, diaCopiar, mes, copia: true);
                    SalvarLimiteDescarregamento(centroDescarregamento, historico, unitOfWork, diaCopiar, mes, copia: true);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfigurarImportacaoCentroDescarregamento()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Gerais.Geral.CodRemetente, Propriedade = "CodigoRemetente", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Gerais.Geral.CodDestinatario, Propriedade = "CodigoDestinatario", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Gerais.Geral.TipoCarga, Propriedade = "TipoCarga", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Logistica.CentroDescarregamento.Domingo, Propriedade = "Dom", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Logistica.CentroDescarregamento.SegundaFeira, Propriedade = "Seg", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Logistica.CentroDescarregamento.TercaFeira, Propriedade = "Ter", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Logistica.CentroDescarregamento.QuartaFeira, Propriedade = "Qua", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Logistica.CentroDescarregamento.QuintaFeira, Propriedade = "Qui", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Logistica.CentroDescarregamento.SextaFeira, Propriedade = "Sex", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Logistica.CentroDescarregamento.Sabado, Propriedade = "Sab", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Logistica.CentroDescarregamento.HoraInicioUm, Propriedade = "HoraInicio1", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Logistica.CentroDescarregamento.HoraFimUm, Propriedade = "HoraFim1", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Logistica.CentroDescarregamento.HoraInicioDois, Propriedade = "HoraInicio2", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Logistica.CentroDescarregamento.HoraFimDois, Propriedade = "HoraFim2", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Logistica.CentroDescarregamento.HoraInicioTres, Propriedade = "HoraInicio3", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Logistica.CentroDescarregamento.HoraFimTres, Propriedade = "HoraFim3", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoUm, Propriedade = "ModeloVeiculo1", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao =Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoDois, Propriedade = "ModeloVeiculo2", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoTres, Propriedade = "ModeloVeiculo3", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoQuatro, Propriedade = "ModeloVeiculo4", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoCinco, Propriedade = "ModeloVeiculo5", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoSeis, Propriedade = "ModeloVeiculo6", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoSete, Propriedade = "ModeloVeiculo7", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoOito, Propriedade = "ModeloVeiculo8", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoNove, Propriedade = "ModeloVeiculo9", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = Localization.Resources.Logistica.CentroDescarregamento.ModeloVeiculoDez, Propriedade = "ModeloVeiculo10", Tamanho = 150 },
            };

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfigurarImportacaoCapacidade()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = string.Concat ("*",Localization.Resources.Gerais.Geral.DiaMes, ":"), Propriedade = "DiaDoMes", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = string.Concat ("*",Localization.Resources.Gerais.Geral.Mes, ":") , Propriedade = "Mes", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = string.Concat ("*",Localization.Resources.Gerais.Geral.InicioJanela, ":"), Propriedade = "InicioJanela", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = string.Concat ("*",Localization.Resources.Gerais.Geral.FimJanela, ":"), Propriedade = "FimJanela", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = string.Concat (Localization.Resources.Gerais.Geral.TipoCarga, ":"), Propriedade = "TipoCarga", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = string.Concat (Localization.Resources.Gerais.Geral.RemetenteCodigoIntegracao, ":"), Propriedade = "RemetenteCodigoIntegracao", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = string.Concat (Localization.Resources.Gerais.Geral.GrupoPessoasCodigoIntegracao, ":"), Propriedade = "GrupoPessoasCodigoIntegracao", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = string.Concat (Localization.Resources.Gerais.Geral.GrupoProdutosCodigoIntegracao, ":"), Propriedade = "GrupoProdutoCodigoIntegracao", Tamanho = 150 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = string.Concat ("*",Localization.Resources.Gerais.Geral.DescarregamentosSimultaneos, ":"), Propriedade = "DescarregamentoSimultaneo", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = string.Concat (Localization.Resources.Gerais.Geral.QuantidadeItensDe, ":"), Propriedade = "QuantidadeItensDe", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = string.Concat (Localization.Resources.Gerais.Geral.QuantidadeItensAte, ":"), Propriedade = "QuantidadeItensAte", Tamanho = 200 },
            };

            return configuracoes;
        }

        private dynamic ObterPeriodoDescarregamento(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime dataDescarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            int capacidadeDescarregamentoTotal = 0;
            int capacidadeDisponivel = 0;

            if (centroDescarregamento?.TipoCapacidadeDescarregamentoPorPeso == TipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
                List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Codigo).ToList();

                int capacidadeDescarregamento = periodoDescarregamento.CapacidadeDescarregamento;
                int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamentoPorPeriodo(centroDescarregamento.Codigo, dataDescarregamento, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, codigosCanaisVenda);
                int capacidadeUtilizada = (int)repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoPeriodo(0, centroDescarregamento.Codigo, dataDescarregamento, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido, codigosCanaisVenda);

                capacidadeDescarregamentoTotal = capacidadeDescarregamento + capacidadeDescarregamentoAdicional;
                capacidadeDisponivel = capacidadeDescarregamentoTotal - capacidadeUtilizada;
            }

            return new
            {
                periodoDescarregamento.Codigo,
                HoraInicio = periodoDescarregamento.HoraInicio.ToString(@"hh\:mm"),
                HoraTermino = periodoDescarregamento.HoraTermino.ToString(@"hh\:mm"),
                DataDescarregamento = dataDescarregamento.ToString("dd/MM/yyyy"),
                Descricao = periodoDescarregamento.DescricaoPeriodo,
                periodoDescarregamento.CapacidadeDescarregamentoSimultaneo,
                periodoDescarregamento.ToleranciaExcessoTempo,
                CapacidadeDescarregamento = (capacidadeDescarregamentoTotal > 0) ? capacidadeDescarregamentoTotal.ToString("n0") : "",
                CapacidadeDisponivel = (capacidadeDescarregamentoTotal > 0) ? capacidadeDisponivel.ToString("n0") : "",
            };
        }

        private void SetarTiposDeCarga(ref Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCarga"));

            centroDescarregamento.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            foreach (var tipoCarga in tiposCarga)
                centroDescarregamento.TiposCarga.Add(new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)tipoCarga.Codigo });
        }

        private void SetarVeiculosPermitidos(ref Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            dynamic veiculosPermitidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("VeiculosPermitidos"));

            centroDescarregamento.VeiculosPermitidos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            foreach (var veiculoPermitido in veiculosPermitidos)
                centroDescarregamento.VeiculosPermitidos.Add(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)veiculoPermitido.Codigo });
        }

        private void SetarTransportadores(ref Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            dynamic transportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transportadores"));

            centroDescarregamento.Transportadores = new List<Dominio.Entidades.Empresa>();

            foreach (var transportador in transportadores)
                centroDescarregamento.Transportadores.Add(new Dominio.Entidades.Empresa() { Codigo = (int)transportador.Codigo });
        }

        private void SalvarQuantidadePorTipoDeCarga(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento repositorioQuantidadePorTipoDeCargaDescarregamento = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadePorTipoDeCargaDescarregamentoTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);

            dynamic quantidadesPorTipoCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("QuantidadePorTipoDeCarga"));

            if (centroDescarregamento.QuantidadesPorTipoDeCargaDescarregamento != null && centroDescarregamento.QuantidadesPorTipoDeCargaDescarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var quantidadePorTipoCarga in quantidadesPorTipoCarga)
                    if (quantidadePorTipoCarga.Codigo != null)
                        codigos.Add((int)quantidadePorTipoCarga.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> quantidadesTipoCargaDeletar = repositorioQuantidadePorTipoDeCargaDescarregamento.BuscarPorCentroDescarregamentoCodigosDesconsiderar(centroDescarregamento.Codigo, codigos);
                List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> quantidadesTipoCargaTiposCargaDeletar = repositorioQuantidadePorTipoDeCargaDescarregamentoTipoCarga.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo);

                for (var i = 0; i < quantidadesTipoCargaTiposCargaDeletar.Count; i++)
                    repositorioQuantidadePorTipoDeCargaDescarregamentoTipoCarga.Deletar(quantidadesTipoCargaTiposCargaDeletar[i]);

                for (var i = 0; i < quantidadesTipoCargaDeletar.Count; i++)
                    repositorioQuantidadePorTipoDeCargaDescarregamento.Deletar(quantidadesTipoCargaDeletar[i]);
            }
            else
                centroDescarregamento.QuantidadesPorTipoDeCargaDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>();

            foreach (var quantidadePorTipoCarga in quantidadesPorTipoCarga)
            {
                Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento quantidadePorTipoDeCargaDescarregamento = quantidadePorTipoCarga.Codigo != null ? repositorioQuantidadePorTipoDeCargaDescarregamento.BuscarPorCodigo((int)quantidadePorTipoCarga.Codigo) : null;

                if (quantidadePorTipoDeCargaDescarregamento == null)
                    quantidadePorTipoDeCargaDescarregamento = new Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento();
                else
                    quantidadePorTipoDeCargaDescarregamento.Initialize();

                quantidadePorTipoDeCargaDescarregamento.CentroDescarregamento = centroDescarregamento;
                quantidadePorTipoDeCargaDescarregamento.Tolerancia = null;
                quantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaConfirmada = null;
                quantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaNaoConfirmada = null;
                if (int.TryParse((string)quantidadePorTipoCarga.Volumes, out int volumes))
                    quantidadePorTipoDeCargaDescarregamento.Volumes = volumes;
                if (Enum.TryParse((string)quantidadePorTipoCarga.Dia, out DiaSemana dia))
                    quantidadePorTipoDeCargaDescarregamento.Dia = dia;
                if (int.TryParse((string)quantidadePorTipoCarga.Tolerancia, out int tolerancia))
                    quantidadePorTipoDeCargaDescarregamento.Tolerancia = tolerancia;
                if (int.TryParse((string)quantidadePorTipoCarga.ToleranciaCancelamentoAgendaConfirmada, out int toleranciaAgendaConfirmada))
                    quantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaConfirmada = toleranciaAgendaConfirmada;
                if (int.TryParse((string)quantidadePorTipoCarga.ToleranciaCancelamentoAgendaNaoConfirmada, out int toleranciaAgendaNaoConfirmada))
                    quantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaNaoConfirmada = toleranciaAgendaNaoConfirmada;

                List<int> codigosTipoCarga = new List<int>();

                foreach (var tipoCarga in quantidadePorTipoCarga.TiposCarga)
                {
                    if (tipoCarga.Codigo != null)
                        codigosTipoCarga.Add((int)tipoCarga.Codigo);
                }

                if (quantidadePorTipoDeCargaDescarregamento.Codigo > 0)
                    repositorioQuantidadePorTipoDeCargaDescarregamento.Atualizar(quantidadePorTipoDeCargaDescarregamento);
                else
                    repositorioQuantidadePorTipoDeCargaDescarregamento.Inserir(quantidadePorTipoDeCargaDescarregamento);

                codigosTipoCarga = codigosTipoCarga.Distinct().ToList();

                foreach (int tipoCarga in codigosTipoCarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(tipoCarga);
                    var quantidadeTipoCargaTipoCarga = new Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga()
                    {
                        QuantidadePorTipoDeCargaDescarregamento = quantidadePorTipoDeCargaDescarregamento,
                        TipoCarga = tipoDeCarga
                    };

                    repositorioQuantidadePorTipoDeCargaDescarregamentoTipoCarga.Inserir(quantidadeTipoCargaTipoCarga);
                }

            }
        }

        private void SalvarTemposDescarregamento(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.TempoDescarregamento repTempoDescarregamento = new Repositorio.Embarcador.Logistica.TempoDescarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);

            dynamic temposDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TemposDescarregamento"));

            if (centroDescarregamento.TemposDescarregamento != null && centroDescarregamento.TemposDescarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var tempoDescarregamento in temposDescarregamento)
                    if (tempoDescarregamento.Codigo != null)
                        codigos.Add((int)tempoDescarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento> temposDescarregamentoDeletar = (from obj in centroDescarregamento.TemposDescarregamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < temposDescarregamentoDeletar.Count; i++)
                    repTempoDescarregamento.Deletar(temposDescarregamentoDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
            {
                centroDescarregamento.TemposDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento>();
            }

            foreach (var tempoDescarregamento in temposDescarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento tempo = tempoDescarregamento.Codigo != null ? repTempoDescarregamento.BuscarPorCodigo((int)tempoDescarregamento.Codigo) : null;

                if (tempo == null)
                    tempo = new Dominio.Entidades.Embarcador.Logistica.TempoDescarregamento();
                else
                    tempo.Initialize();

                tempo.CentroDescarregamento = centroDescarregamento;
                tempo.ModeloVeicular = repModeloVeicular.BuscarPorCodigo((int)tempoDescarregamento.ModeloVeiculo.Codigo);
                tempo.TipoCarga = repTipoCarga.BuscarPorCodigo((int)tempoDescarregamento.TipoCarga.Codigo);
                tempo.Tempo = (int)tempoDescarregamento.Tempo;
                tempo.TipoTempo = (TempoDescarregamentoTipoTempo)tempoDescarregamento.TipoTempo;
                tempo.SkuDe = null;
                tempo.SkuAte = null;
                if (int.TryParse((string)tempoDescarregamento.SkuDe, out int skuDe))
                    tempo.SkuDe = skuDe;
                if (int.TryParse((string)tempoDescarregamento.SkuAte, out int skuAte))
                    tempo.SkuAte = skuAte;

                if (tempo.Codigo > 0)
                    repTempoDescarregamento.Atualizar(tempo, historico != null ? Auditado : null, historico);
                else
                    repTempoDescarregamento.Inserir(tempo, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarLimitesAgendamento(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.LimiteAgendamento repositorioLimiteAgendamento = new Repositorio.Embarcador.Logistica.LimiteAgendamento(unidadeDeTrabalho);

            dynamic limitesAgendamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LimiteAgendamento"));

            if (centroDescarregamento.LimitesAgendamento != null && centroDescarregamento.LimitesAgendamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var limiteAgendamento in limitesAgendamento)
                    if (limiteAgendamento.Codigo != null)
                        codigos.Add((int)limiteAgendamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento> limitesAgendamentoDeletar = (from obj in centroDescarregamento.LimitesAgendamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < limitesAgendamentoDeletar.Count; i++)
                    repositorioLimiteAgendamento.Deletar(limitesAgendamentoDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
            {
                centroDescarregamento.LimitesAgendamento = new List<Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento>();
            }

            foreach (var limiteAgendamento in limitesAgendamento)
            {
                Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento limite = limiteAgendamento.Codigo != null ? repositorioLimiteAgendamento.BuscarPorCodigo((int)limiteAgendamento.Codigo) : null;

                if (limite == null)
                    limite = new Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento();
                else
                    limite.Initialize();

                limite.CentroDescarregamento = centroDescarregamento;
                limite.GrupoPessoa = repositorioGrupoPessoas.BuscarPorCodigo(((string)limiteAgendamento.GrupoPessoa.Codigo).ToInt());
                limite.Limite = ((string)limiteAgendamento.NovoLimite).ToInt();
                limite.PermiteUltrapassarLimiteVolume = ((string)limiteAgendamento.PermiteUltrapassarLimiteVolume).ToBool();

                if (limite.Codigo > 0)
                    repositorioLimiteAgendamento.Atualizar(limite, historico != null ? Auditado : null, historico);
                else
                    repositorioLimiteAgendamento.Inserir(limite, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarLimiteDescarregamento(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho, int dia = 0, int mes = 0, bool copia = false)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento repositorioLimiteDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            dynamic limitesDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LimitesDescarregamento"));

            if (centroDescarregamento.LimitesDescarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var limiteDescarregamento in limitesDescarregamento)
                {
                    int? codigo = ((string)limiteDescarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento> limitesDescarregamentoDeletar = centroDescarregamento.CapacidadeDescaregamentoPorDia ? repositorioLimiteDescarregamento.BuscarPorDiaMes(centroDescarregamento.Codigo, dia, mes) : (from o in centroDescarregamento.LimitesDescarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento limiteDescarregamento in limitesDescarregamentoDeletar)
                    repositorioLimiteDescarregamento.Deletar(limiteDescarregamento, historico != null ? Auditado : null, historico);
            }
            else
                centroDescarregamento.LimitesDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento>();

            foreach (var limiteDescarregamento in limitesDescarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento limiteDescarregamentoSalvar;
                int? codigo = ((string)limiteDescarregamento.Codigo).ToNullableInt();

                if (codigo.HasValue && !centroDescarregamento.CapacidadeDescaregamentoPorDia && !copia)
                    limiteDescarregamentoSalvar = repositorioLimiteDescarregamento.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException(Localization.Resources.Logistica.CentroDescarregamento.LimiteDescarregamentoNaoEncontrado);
                else
                    limiteDescarregamentoSalvar = new Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento();

                limiteDescarregamentoSalvar.CentroDescarregamento = centroDescarregamento;
                limiteDescarregamentoSalvar.Dia = ((string)limiteDescarregamento.DiaSemana).ToEnum<DiaSemana>();
                limiteDescarregamentoSalvar.DiaDoMes = dia;
                limiteDescarregamentoSalvar.Mes = mes;
                limiteDescarregamentoSalvar.HorasAntecedencia = ((string)limiteDescarregamento.HorasAntecedencia).ToInt();
                limiteDescarregamentoSalvar.TipoCarga = repositorioTipoCarga.BuscarPorCodigo(((string)limiteDescarregamento.TipoCarga.Codigo).ToInt()) ?? throw new ControllerException(Localization.Resources.Logistica.CentroDescarregamento.TipoCargaLimiteCarregamentoNaoEncontrado);

                if (limiteDescarregamentoSalvar.Codigo > 0 && !copia)
                    repositorioLimiteDescarregamento.Atualizar(limiteDescarregamentoSalvar, historico != null ? Auditado : null, historico);
                else
                    repositorioLimiteDescarregamento.Inserir(limiteDescarregamentoSalvar, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarEmails(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamentoEmail repEmail = new Repositorio.Embarcador.Logistica.CentroDescarregamentoEmail(unidadeDeTrabalho);

            dynamic emails = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Emails"));

            if (centroDescarregamento.Emails != null && centroDescarregamento.Emails.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var email in emails)
                    if (email.Codigo != null)
                        codigos.Add((int)email.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail> emailsDeletar = (from obj in centroDescarregamento.Emails where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < emailsDeletar.Count; i++)
                    repEmail.Deletar(emailsDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
            {
                centroDescarregamento.Emails = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail>();
            }

            foreach (var email in emails)
            {
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail emailAdd = email.Codigo != null ? repEmail.BuscarPorCodigo((int)email.Codigo) : null;

                if (emailAdd == null)
                    emailAdd = new Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoEmail();
                else
                    emailAdd.Initialize();

                emailAdd.CentroDescarregamento = centroDescarregamento;
                emailAdd.Email = (string)email.Email;

                if (emailAdd.Codigo > 0)
                    repEmail.Atualizar(emailAdd, historico != null ? Auditado : null, historico);
                else
                    repEmail.Inserir(emailAdd, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarPeriodoDescarregamentoRemetentes(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic PeriodoRemetentes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repPeriodoDescarregamentoRemetente = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> remetentesExcluir = repPeriodoDescarregamentoRemetente.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);
            foreach (var remetenteExluir in remetentesExcluir)
            {
                repPeriodoDescarregamentoRemetente.Deletar(remetenteExluir);
            }

            dynamic remetentes = PeriodoRemetentes ?? new List<dynamic>();

            if (remetentes.Count == 0)
                return;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);


            foreach (var remetente in remetentes)
            {
                var cliente = repCliente.BuscarPorCPFCNPJ((double)remetente.Codigo);

                if (cliente != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente periodoRemetente = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente()
                    {
                        Remetente = cliente,
                        PeriodoDescarregamento = periodo
                    };


                    repPeriodoDescarregamentoRemetente.Inserir(periodoRemetente);
                }
            }
        }

        private void SalvarPeriodoDescarregamentoTiposCarga(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic peridoTiposCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repPeriodoDescarregamentoTipoCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> tiposCargaExluir = repPeriodoDescarregamentoTipoCarga.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);
            foreach (var tipoCargaExcluir in tiposCargaExluir)
            {
                repPeriodoDescarregamentoTipoCarga.Deletar(tipoCargaExcluir);
            }

            dynamic tiposCarga = peridoTiposCarga ?? new List<dynamic>();

            if (tiposCarga.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);


            foreach (var tipoCarga in tiposCarga)
            {
                var tipoDeCarga = repTipoCarga.BuscarPorCodigo((int)tipoCarga.Codigo);

                if (tipoDeCarga != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga periodoTipoCarga = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga()
                    {
                        TipoDeCarga = tipoDeCarga,
                        PeriodoDescarregamento = periodo
                    };

                    repPeriodoDescarregamentoTipoCarga.Inserir(periodoTipoCarga);
                }
            }
        }

        private void SalvarPeriodoDescarregamentoGruposPessoas(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoGruposPessoas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorioPeriodoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> gruposPessoaExcluir = repositorioPeriodoGrupoPessoa.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);
            foreach (var grupoPessoaExcluir in gruposPessoaExcluir)
                repositorioPeriodoGrupoPessoa.Deletar(grupoPessoaExcluir);

            dynamic gruposPessoa = periodoGruposPessoas ?? new List<dynamic>();

            if (gruposPessoa.Count == 0)
                return;

            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);

            foreach (var grupoPessoa in gruposPessoa)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas entidadeGrupoPessoa = repositorioGrupoPessoas.BuscarPorCodigo((int)grupoPessoa.Codigo);

                if (entidadeGrupoPessoa == null)
                    continue;

                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa periodoGrupoPessoa = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa()
                {
                    GrupoPessoas = entidadeGrupoPessoa,
                    PeriodoDescarregamento = periodo
                };

                repositorioPeriodoGrupoPessoa.Inserir(periodoGrupoPessoa);
            }
        }

        private void SalvarPeriodoDescarregamentoCanaisVenda(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoCanaisVenda, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda repositorioPeriodoCanalVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> canaisVendaCadastrados = repositorioPeriodoCanalVenda.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda canalVendaExcluir in canaisVendaCadastrados)
                repositorioPeriodoCanalVenda.Deletar(canalVendaExcluir);

            dynamic canaisVenda = periodoCanaisVenda ?? new List<dynamic>();

            if (canaisVenda.Count == 0)
                return;

            foreach (var canalVenda in canaisVenda)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda periodoCanalVendaSalvar = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda()
                {
                    CanalVenda = new Dominio.Entidades.Embarcador.Pedidos.CanalVenda() { Codigo = ((string)canalVenda.Codigo).ToInt() },
                    PeriodoDescarregamento = periodo
                };

                repositorioPeriodoCanalVenda.Inserir(periodoCanalVendaSalvar);
            }
        }

        private void SalvarPeriodoDescarregamentoGrupoProdutos(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, dynamic periodoGrupoProdutos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioPeriodoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> grupoProdutosCadastrados = repositorioPeriodoGrupoProduto.BuscarPorPeriodoDescarregamento(periodo?.Codigo ?? 0);

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto grupoProdutoExcluir in grupoProdutosCadastrados)
                repositorioPeriodoGrupoProduto.Deletar(grupoProdutoExcluir);

            dynamic grupoProdutos = periodoGrupoProdutos ?? new List<dynamic>();

            if (grupoProdutos.Count == 0)
                return;

            foreach (var grupoProduto in grupoProdutos)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto periodoGrupoProdutoSalvar = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto()
                {
                    GrupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto() { Codigo = ((string)grupoProduto.Codigo).ToInt() },
                    PeriodoDescarregamento = periodo
                };

                repositorioPeriodoGrupoProduto.Inserir(periodoGrupoProdutoSalvar);
            }
        }

        private void SalvarPeriodosDescarregamento(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho, int dia = 0, int mes = 0, bool copia = false)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repositorioPeriodoDescarregamentoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repositorioPeriodoDescarregamentoRemetente = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repositorioPeriodoDescarregamentoTipoDeCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repositorioPeriodoDescarregamentoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            dynamic periodosDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PeriodosDescarregamento"));

            if (centroDescarregamento.PeriodosDescarregamento != null && centroDescarregamento.PeriodosDescarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var periodoDescarregamento in periodosDescarregamento)
                    if (periodoDescarregamento.Codigo != null)
                        codigos.Add((int)periodoDescarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamentoDeletar = centroDescarregamento.CapacidadeDescaregamentoPorDia ? repPeriodoDescarregamento.BuscarPorDiaMes(centroDescarregamento.Codigo, dia, mes) : (from obj in centroDescarregamento.PeriodosDescarregamento where !codigos.Contains(obj.Codigo) select obj).ToList();
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> periodosGruposPessoasDeletar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>();
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> periodosRemetentesDeletar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente>();
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> periodosTiposDeCargaDeletar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>();
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> periodosGrupoProdutosDeletar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>();

                if (periodosDescarregamentoDeletar.Count > 0)
                {
                    List<int> codigosPeriodosDescarregamento = periodosDescarregamentoDeletar.Select(obj => obj.Codigo).ToList();

                    for (int i = 0; i < codigosPeriodosDescarregamento.Count; i += 2000)
                    {
                        List<int> codigosLote = codigosPeriodosDescarregamento.Skip(i).Take(2000).ToList();

                        periodosGruposPessoasDeletar.AddRange(repositorioPeriodoDescarregamentoGrupoPessoa.BuscarPorPeriodosDescarregamento(codigosLote));
                        periodosRemetentesDeletar.AddRange(repositorioPeriodoDescarregamentoRemetente.BuscarPorPeriodosDescarregamento(codigosLote));
                        periodosTiposDeCargaDeletar.AddRange(repositorioPeriodoDescarregamentoTipoDeCarga.BuscarPorPeriodosDescarregamento(codigosLote));
                        periodosGrupoProdutosDeletar.AddRange(repositorioPeriodoDescarregamentoGrupoProduto.BuscarPorPeriodosDescarregamento(codigosLote));
                    }
                }

                for (var i = 0; i < periodosGruposPessoasDeletar.Count; i++)
                    repositorioPeriodoDescarregamentoGrupoPessoa.Deletar(periodosGruposPessoasDeletar[i], historico != null ? Auditado : null, historico);

                for (var i = 0; i < periodosRemetentesDeletar.Count; i++)
                    repositorioPeriodoDescarregamentoRemetente.Deletar(periodosRemetentesDeletar[i], historico != null ? Auditado : null, historico);

                for (var i = 0; i < periodosTiposDeCargaDeletar.Count; i++)
                    repositorioPeriodoDescarregamentoTipoDeCarga.Deletar(periodosTiposDeCargaDeletar[i], historico != null ? Auditado : null, historico);

                for (var i = 0; i < periodosGrupoProdutosDeletar.Count; i++)
                    repositorioPeriodoDescarregamentoGrupoProduto.Deletar(periodosGrupoProdutosDeletar[i], historico != null ? Auditado : null, historico);

                unidadeDeTrabalho.Flush();

                for (var i = 0; i < periodosDescarregamentoDeletar.Count; i++)
                    repPeriodoDescarregamento.Deletar(periodosDescarregamentoDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
                centroDescarregamento.PeriodosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();

            foreach (var periodoDescarregamento in periodosDescarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = periodoDescarregamento.Codigo != null ? repPeriodoDescarregamento.BuscarPorCodigo((int)periodoDescarregamento.Codigo) : null;

                if (periodo == null || copia)
                    periodo = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento();
                else
                    periodo.Initialize();

                periodo.CentroDescarregamento = centroDescarregamento;
                periodo.CapacidadeDescarregamento = ((string)periodoDescarregamento?.CapacidadeDescarregamento).ToInt();
                periodo.CapacidadeDescarregamentoSimultaneo = ((string)periodoDescarregamento?.CapacidadeDescarregamentoSimultaneo).ToInt();
                periodo.CapacidadeDescarregamentoSimultaneoAdicional = ((string)periodoDescarregamento?.CapacidadeDescarregamentoSimultaneoAdicional).ToInt();
                periodo.Dia = (centroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false) ? DiaSemana.Domingo : (DiaSemana)periodoDescarregamento.DiaSemana;
                periodo.DiaDoMes = dia;
                periodo.Mes = mes;
                periodo.HoraInicio = TimeSpan.ParseExact((string)periodoDescarregamento.HoraInicio, "g", null, System.Globalization.TimeSpanStyles.None);
                periodo.HoraTermino = TimeSpan.ParseExact((string)periodoDescarregamento.HoraTermino, "g", null, System.Globalization.TimeSpanStyles.None);
                periodo.ToleranciaExcessoTempo = ((string)periodoDescarregamento.ToleranciaExcessoTempo).ToInt();
                periodo.SkuDe = null;
                periodo.SkuAte = null;

                if (int.TryParse((string)periodoDescarregamento.SkuDe, out int skuDe))
                    periodo.SkuDe = skuDe;
                if (int.TryParse((string)periodoDescarregamento.SkuAte, out int skuAte))
                    periodo.SkuAte = skuAte;

                if (periodo.Codigo > 0 && !copia)
                    repPeriodoDescarregamento.Atualizar(periodo, historico != null ? Auditado : null, historico);
                else
                    repPeriodoDescarregamento.Inserir(periodo, historico != null ? Auditado : null, historico);

                SalvarPeriodoDescarregamentoRemetentes(periodo, periodoDescarregamento.Remetentes, unidadeDeTrabalho);
                SalvarPeriodoDescarregamentoTiposCarga(periodo, periodoDescarregamento.TiposCarga, unidadeDeTrabalho);
                SalvarPeriodoDescarregamentoGruposPessoas(periodo, periodoDescarregamento.GruposPessoas, unidadeDeTrabalho);
                SalvarPeriodoDescarregamentoCanaisVenda(periodo, periodoDescarregamento.CanaisVenda, unidadeDeTrabalho);
                SalvarPeriodoDescarregamentoGrupoProdutos(periodo, periodoDescarregamento.GrupoProduto, unidadeDeTrabalho);
            }
        }

        private void SalvarPrevisoesDescarregamento(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho, int dia = 0, int mes = 0, bool copia = false)
        {
            Repositorio.Embarcador.Logistica.PrevisaoDescarregamento repPrevisaoDescarregamento = new Repositorio.Embarcador.Logistica.PrevisaoDescarregamento(unidadeDeTrabalho);
            dynamic previsoesDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PrevisoesDescarregamento"));

            if (centroDescarregamento.PrevisoesDescarregamento != null && centroDescarregamento.PrevisoesDescarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var previsaoDescarregamento in previsoesDescarregamento)
                    if (previsaoDescarregamento.Codigo != null)
                        codigos.Add((int)previsaoDescarregamento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento> previsoesDescarregamentoDeletar = centroDescarregamento.CapacidadeDescaregamentoPorDia ? repPrevisaoDescarregamento.BuscarPorDiaMes(centroDescarregamento.Codigo, dia, mes) : (from obj in centroDescarregamento.PrevisoesDescarregamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < previsoesDescarregamentoDeletar.Count; i++)
                    repPrevisaoDescarregamento.Deletar(previsoesDescarregamentoDeletar[i], historico != null ? Auditado : null, historico);
            }
            else
            {
                centroDescarregamento.PrevisoesDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento>();
            }

            foreach (var previsaoDescarregamento in previsoesDescarregamento)
            {
                Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento previsao = previsaoDescarregamento.Codigo != null ? repPrevisaoDescarregamento.BuscarPorCodigo((int)previsaoDescarregamento.Codigo) : null;

                if (previsao == null || copia)
                    previsao = new Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento();
                else
                    previsao.Initialize();

                previsao.CentroDescarregamento = centroDescarregamento;
                previsao.Rota = new Dominio.Entidades.RotaFrete() { Codigo = (int)previsaoDescarregamento.Rota.Codigo };
                previsao.QuantidadeCargas = (int)previsaoDescarregamento.QuantidadeCargas;
                previsao.QuantidadeCargasExcedentes = (int)previsaoDescarregamento.QuantidadeCargasExcedentes;
                previsao.Dia = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)previsaoDescarregamento.DiaSemana;
                previsao.DiaDoMes = dia;
                previsao.Mes = mes;
                previsao.Descricao = (string)previsaoDescarregamento.Descricao;

                previsao.ModelosVeiculos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                var modelosVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(previsaoDescarregamento.ModelosVeiculos.ToString());

                foreach (var modeloVeiculo in modelosVeiculos)
                    previsao.ModelosVeiculos.Add(new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modeloVeiculo.Codigo });

                if (previsao.Codigo > 0 && !copia)
                    repPrevisaoDescarregamento.Atualizar(previsao, historico != null ? Auditado : null, historico);
                else
                    repPrevisaoDescarregamento.Inserir(previsao, historico != null ? Auditado : null, historico);
            }
        }

        private void SalvarHorariosAprovacaoAutomatica(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic dynHorariosAprovacaoAutomatica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("HorariosAprovacaoAutomatica"));

            Repositorio.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento repositorioHoraroAprovacaoAutomatica = new Repositorio.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento> horariosAprovacaoAutomaticaDeletar = repositorioHoraroAprovacaoAutomatica.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento horarioAprovacaoAutomaticaDeletar in horariosAprovacaoAutomaticaDeletar)
                repositorioHoraroAprovacaoAutomatica.Deletar(horarioAprovacaoAutomaticaDeletar);

            foreach (dynamic horarioAprovacaoAutomatica in dynHorariosAprovacaoAutomatica)
            {
                Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento horarioAdicionar = new Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento()
                {
                    CentroDescarregamento = centroDescarregamento,
                    DataInicial = ((string)horarioAprovacaoAutomatica.DataInicial).ToDateTime(),
                    DataFinal = ((string)horarioAprovacaoAutomatica.DataFinal).ToDateTime()
                };

                repositorioHoraroAprovacaoAutomatica.Inserir(horarioAdicionar);
            }
        }

        #endregion
    }
}
