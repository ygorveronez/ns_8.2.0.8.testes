using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoDisponibilidade
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento _configuracaoDisponibilidadeCarregamento;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private bool _definicaoHorarioCarregamentoAteLimiteTentativas;
        private readonly int _tabelaCodigoOperacaoLivre = 0;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, configuracaoDisponibilidadeCarregamento: null, auditado: null) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, auditado: null, configuracaoDisponibilidadeCarregamento: null) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador: null, auditado, configuracaoDisponibilidadeCarregamento: null) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento) : this(unitOfWork, configuracaoEmbarcador: null, auditado: null, configuracaoDisponibilidadeCarregamento) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador, auditado, configuracaoDisponibilidadeCarregamento: null) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento) : this(unitOfWork, configuracaoEmbarcador: null, auditado, configuracaoDisponibilidadeCarregamento) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento) : this(unitOfWork, configuracaoEmbarcador, auditado: null, configuracaoDisponibilidadeCarregamento) { }

        public CargaJanelaCarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento)
        {
            _auditado = auditado;
            _configuracaoDisponibilidadeCarregamento = configuracaoDisponibilidadeCarregamento ?? new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento();
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AtualizarDatasRelacionadas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((cargaJanelaCarregamento.Carga != null) && (cargaJanelaCarregamento.Tipo == TipoCargaJanelaCarregamento.Carregamento))
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                cargaJanelaCarregamento.Carga.DataCarregamentoCarga = cargaJanelaCarregamento.InicioCarregamento;
                cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;

                if (configuracaoEmbarcador.DataBaseCalculoPrevisaoControleEntrega == DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista)
                    Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(cargaJanelaCarregamento.Carga, configuracaoEmbarcador, _unitOfWork, cargaJanelaCarregamento.TerminoCarregamento, tipoServicoMultisoftware);
                else if (configuracaoEmbarcador.DataBaseCalculoPrevisaoControleEntrega == DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela)
                    Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(cargaJanelaCarregamento.Carga, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);

                repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);

                new Logistica.CargaJanelaDescarregamento(_unitOfWork, configuracaoEmbarcador, _auditado).Atualizar(cargaJanelaCarregamento.Carga, cargaJanelaCarregamento);

                if (cargaJanelaCarregamento.Carga.Pedidos?.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaJanelaCarregamento.Carga.Pedidos)
                    {
                        cargaPedido.Pedido.DataCarregamentoPedido = cargaJanelaCarregamento.InicioCarregamento;

                        if (configuracaoJanelaCarregamento.AtualizarDataInicialColetaAoAlterarHorarioCarregamento)
                            cargaPedido.Pedido.DataInicialColeta = cargaJanelaCarregamento.InicioCarregamento;

                        repositorioPedido.Atualizar(cargaPedido.Pedido);
                    }
                }

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                if (guarita != null)
                {
                    guarita.HorarioEntradaDefinido = true;
                    guarita.DataProgramadaParaChegada = cargaJanelaCarregamento.InicioCarregamento;

                    repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita);
                }
            }

            new GestaoPatio.FluxoGestaoPatio(_unitOfWork).AtualizarDataPrevisaoInicioEtapas(cargaJanelaCarregamento);
            new CargaJanelaCarregamentoTransportador(_unitOfWork).AtualizarDataLiberacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        private void CalcularHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (EncontrarDataCarregamentoAgendada(cargaJanelaCarregamento))
            {
                cargaJanelaCarregamento.Carga.DataCarregamentoCarga = cargaJanelaCarregamento.InicioCarregamento;
                return;
            }

            Dominio.Entidades.RotaFrete rotaFrete = ObterRotaFrete(cargaJanelaCarregamento);
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = ObterClienteDescarga(cargaJanelaCarregamento);

            if (clienteDescarga == null)
            {
                cargaJanelaCarregamento.Excedente = true;
                return;
            }

            DateTime dataHoraInicioDescarga = new DateTime(cargaJanelaCarregamento.DataCarregamentoProgramada.Year, cargaJanelaCarregamento.DataCarregamentoProgramada.Month, cargaJanelaCarregamento.DataCarregamentoProgramada.Day, int.Parse(clienteDescarga.HoraInicioDescarga.Substring(0, 2)), int.Parse(clienteDescarga.HoraInicioDescarga.Substring(3, 2)), 0);
            DateTime dataHoraFimDescarga = new DateTime(dataHoraInicioDescarga.Year, dataHoraInicioDescarga.Month, dataHoraInicioDescarga.Day, int.Parse(clienteDescarga.HoraLimiteDescarga.Substring(0, 2)), int.Parse(clienteDescarga.HoraLimiteDescarga.Substring(3, 2)), 0);

            if (dataHoraFimDescarga < dataHoraInicioDescarga)
                dataHoraFimDescarga = dataHoraFimDescarga.AddDays(1);

            int tempoPercurso = rotaFrete?.ObterTempoViagemEmMinutos() ?? 2;
            DateTime dataHoraCarregamento = dataHoraInicioDescarga > DateTime.MinValue ? (tempoPercurso > 0 ? dataHoraInicioDescarga.AddMinutes(tempoPercurso * -1) : dataHoraInicioDescarga) : cargaJanelaCarregamento.DataCarregamentoProgramada;
            DateTime dataHoraAtual = DateTime.Now;

            dataHoraCarregamento = dataHoraCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento * -1);

            if ((dataHoraCarregamento < dataHoraAtual) && (dataHoraFimDescarga > DateTime.MinValue))
            {
                dataHoraAtual = dataHoraAtual.AddHours(1);

                if (dataHoraAtual.AddMinutes(cargaJanelaCarregamento.TempoCarregamento).AddMinutes(tempoPercurso) <= dataHoraFimDescarga)
                    dataHoraCarregamento = dataHoraAtual;
                else
                {
                    dataHoraInicioDescarga = dataHoraInicioDescarga.AddDays(1);
                    dataHoraFimDescarga = dataHoraFimDescarga.AddDays(1);
                    dataHoraCarregamento = dataHoraCarregamento.AddDays(1);
                }
            }

            int tentativasDias = 0;
            bool encontrouHorario = EncontrarProximoHorarioComRestricao(cargaJanelaCarregamento, rotaFrete, dataHoraCarregamento, tempoPercurso, dataHoraInicioDescarga, dataHoraFimDescarga, ref tentativasDias);

            if (!encontrouHorario)
                cargaJanelaCarregamento.Excedente = true;
            else if ((cargaJanelaCarregamento.InicioCarregamento >= DateTime.MinValue) && (cargaJanelaCarregamento.Carga != null))
                cargaJanelaCarregamento.Carga.DataCarregamentoCarga = cargaJanelaCarregamento.InicioCarregamento;
        }

        private bool EncontrarDataCarregamentoAgendada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga?.Filial == null)
                return false;

            Repositorio.Embarcador.Cargas.CargaDataCarregamento repositorioCargaDataCarregamento = new Repositorio.Embarcador.Cargas.CargaDataCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento cargaDataCarregamento = repositorioCargaDataCarregamento.BuscarDataCarregamentoCargaFilial(cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador, cargaJanelaCarregamento.Carga.Filial);

            if (!(cargaDataCarregamento?.DataCarregamento.HasValue ?? false) || (cargaDataCarregamento.DataCarregamento == DateTime.MinValue))
                return false;

            cargaJanelaCarregamento.InicioCarregamento = cargaDataCarregamento.DataCarregamento.Value;
            cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
            cargaJanelaCarregamento.Excedente = false;

            if (cargaDataCarregamento.DataSaidaCentroCarregamento.HasValue && cargaDataCarregamento.DataSaidaCentroCarregamento > DateTime.MinValue)
                cargaJanelaCarregamento.DataSaida = cargaDataCarregamento.DataSaidaCentroCarregamento.Value;

            if (cargaJanelaCarregamento.InicioCarregamento < DateTime.Now)
                cargaJanelaCarregamento.LimiteInicioCarregamento = cargaJanelaCarregamento.InicioCarregamento;
            else if (cargaJanelaCarregamento.DataSaida.HasValue && cargaJanelaCarregamento.DataSaida >= DateTime.MinValue)
                cargaJanelaCarregamento.LimiteInicioCarregamento = cargaJanelaCarregamento.DataSaida;

            return true;
        }

        private bool EncontrarProximoHorario(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, DateTime dataInicio, DateTime dataTermino, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades)
        {
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario = (
                from carga in cargasPeriodo
                where (
                    (carga.DataInicio >= dataInicio && carga.DataInicio < dataTermino) ||
                    (carga.DataFim > dataInicio && carga.DataFim <= dataTermino) ||
                    (carga.DataInicio <= dataInicio && carga.DataFim >= dataTermino)
                )
                select carga
            ).ToList();

            _configuracaoDisponibilidadeCarregamento.CodigoTipoOperacao = cargaJanelaCarregamento.CargaBase.TipoOperacao?.Codigo ?? 0;

            if (cargaJanelaCarregamento.HorarioEncaixado || IsPossuiCapacidadeCarregamentoSimultaneo(dataInicio, dataTermino, periodo, cargasPeriodoHorario, paradasCentro))
            {
                cargaJanelaCarregamento.InicioCarregamento = dataInicio;
                cargaJanelaCarregamento.TerminoCarregamento = dataTermino;
                cargaJanelaCarregamento.Excedente = false;
                return true;
            }

            if (cargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                return false;

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo primeiraCargaAFinalizarCarregamento = (from cargaPeriodoHorario in cargasPeriodoHorario select cargaPeriodoHorario).OrderBy(obj => obj.DataFim).FirstOrDefault();

            if (primeiraCargaAFinalizarCarregamento != null && primeiraCargaAFinalizarCarregamento.DataInicio < primeiraCargaAFinalizarCarregamento.DataFim)
            {
                dataInicio = (from cargaPeriodoHorario in cargasPeriodoHorario select cargaPeriodoHorario.DataFim).Min();
                cargaJanelaCarregamento.TempoCarregamento = ObterTempoCarregamento(cargaJanelaCarregamento, dataInicio.TimeOfDay);
                dataTermino = dataInicio.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);

                if ((dataTermino.TimeOfDay <= periodo.HoraTermino.Add(TimeSpan.FromMinutes(periodo.ToleranciaExcessoTempo))) && cargaJanelaCarregamento.IsTempoCarregamentoValido())
                    return EncontrarProximoHorario(cargaJanelaCarregamento, cargasPeriodo, periodo, dataInicio, dataTermino, paradasCentro, exclusividades);
            }

            return false;
        }

        private bool EncontrarProximoHorarioComRestricao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.RotaFrete rotaFrete, DateTime dataHoraCarregamento, double tempoPercurso, DateTime dataHoraInicioDescarga, DateTime dataHoraFimDescarga, ref int tentativasDias)
        {
            DateTime dataHoraFimCarregamento = dataHoraCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodos = ObterPeriodosCarregamento(cargaJanelaCarregamento, dataHoraCarregamento);
            bool encontrouHorario = false;

            if (!IsLimiteCarregamentoValido(cargaJanelaCarregamento, dataHoraCarregamento))
                return false;

            if (!IsPossuiCapacidadeCarregamentoDia(cargaJanelaCarregamento, dataHoraCarregamento))
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente || cargaJanelaCarregamento.HorarioEncaixado)
                    throw new ServicoException("A capacidade de carregamento diária foi atingida");

                return false;
            }

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo in periodos)
            {
                bool dataHoraDentroPeriodoCarregamento = (dataHoraCarregamento.TimeOfDay >= periodo.HoraInicio && dataHoraCarregamento.TimeOfDay <= periodo.HoraTermino);

                if (dataHoraDentroPeriodoCarregamento)
                {
                    if (dataHoraFimCarregamento.AddMinutes(tempoPercurso) <= dataHoraFimDescarga)
                    {
                        if (!IsPossuiRestricaoRota(cargaJanelaCarregamento, rotaFrete, dataHoraCarregamento, cargaJanelaCarregamento.TempoCarregamento, tempoPercurso))
                        {
                            if (!IsPossuiCapacidadeCarregamentoPeriodo(cargaJanelaCarregamento, dataHoraCarregamento, periodo))
                            {
                                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente || cargaJanelaCarregamento.HorarioEncaixado)
                                    throw new ServicoException("A capacidade de carregamento do período foi atingida");

                                continue;
                            }

                            IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(cargaJanelaCarregamento, periodo, dataHoraCarregamento);

                            if (EncontrarProximoHorario(cargaJanelaCarregamento, cargasPeriodo, periodo, dataHoraCarregamento, dataHoraFimCarregamento, paradasCentro: null, exclusividades: null))
                            {
                                bool horarioNaoUltrapassaJanelaDescarregamento = (cargaJanelaCarregamento.InicioCarregamento.AddMinutes(tempoPercurso).AddMinutes(cargaJanelaCarregamento.TempoCarregamento) <= dataHoraFimDescarga);

                                encontrouHorario = horarioNaoUltrapassaJanelaDescarregamento;
                            }

                            if (encontrouHorario)
                            {
                                cargaJanelaCarregamento.LimiteInicioCarregamento = dataHoraFimDescarga.AddMinutes(cargaJanelaCarregamento.TempoCarregamento * -1).AddMinutes(tempoPercurso * -1);
                                break;
                            }
                        }
                    }
                }
            }

            if (!encontrouHorario && tentativasDias < 3)
            {
                tentativasDias++;

                bool tempoPercursoUltrapassaHoraDescarga = (dataHoraCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento).AddMinutes(tempoPercurso) > dataHoraFimDescarga);

                if (tempoPercursoUltrapassaHoraDescarga)
                {
                    if (dataHoraInicioDescarga > DateTime.MinValue)
                    {
                        dataHoraInicioDescarga = dataHoraInicioDescarga.AddDays(1);
                        dataHoraFimDescarga = dataHoraFimDescarga.AddDays(1);
                    }

                    dataHoraCarregamento = dataHoraInicioDescarga > DateTime.MinValue ? dataHoraInicioDescarga.AddMinutes(tempoPercurso * -1) : cargaJanelaCarregamento.DataCarregamentoProgramada.AddDays(1);
                    dataHoraCarregamento = dataHoraCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento * -1);
                }
                else
                    dataHoraCarregamento = dataHoraCarregamento.AddMinutes(30);

                encontrouHorario = EncontrarProximoHorarioComRestricao(cargaJanelaCarregamento, rotaFrete, dataHoraCarregamento, tempoPercurso, dataHoraInicioDescarga, dataHoraFimDescarga, ref tentativasDias);
            }

            return encontrouHorario;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> FiltrarExclusividades(List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividadeCarregamentos, DateTime dataInicial, DateTime dataFinal)
        {
            if (exclusividadeCarregamentos == null)
                return new List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento>();

            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataInicial);

            return (
                from o in exclusividadeCarregamentos
                where (
                    (o.DataInicial <= dataInicial || o.DataFinal >= dataFinal) &&
                    (
                        (diaSemana == DiaSemana.Segunda && o.DisponivelSegunda) ||
                        (diaSemana == DiaSemana.Terca && o.DisponivelTerca) ||
                        (diaSemana == DiaSemana.Quarta && o.DisponivelQuarta) ||
                        (diaSemana == DiaSemana.Quinta && o.DisponivelQuinta) ||
                        (diaSemana == DiaSemana.Sexta && o.DisponivelSexta) ||
                        (diaSemana == DiaSemana.Sabado && o.DisponivelSabado) ||
                        (diaSemana == DiaSemana.Domingo && o.DisponivelDomingo)
                    )
                )
                select o
            ).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> FiltrarParadasCentro(List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro, DateTime dataInicial, DateTime dataFinal)
        {
            if (paradasCentro == null)
                return new List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>();

            return (
                from o in paradasCentro
                where o.DataInicio <= dataInicial && o.DataFim >= dataFinal
                select o
            ).ToList();
        }

        private Dictionary<int, GradeCarregamento> GerarGradePorTipoOperacao(int capacidadeCarregamentoSimultaneo, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> tipoOperacaoSimultaneos)
        {
            Dictionary<int, GradeCarregamento> tabelaExclusividade = new Dictionary<int, GradeCarregamento>();

            int quantidadeSimultaneasLivres = capacidadeCarregamentoSimultaneo - tipoOperacaoSimultaneos.Select(o => o.CapacidadeCarregamentoSimultaneo).Sum();

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo tipoOperacaoSimultaneo in tipoOperacaoSimultaneos)
                tabelaExclusividade[tipoOperacaoSimultaneo.TipoOperacao.Codigo] = new GradeCarregamento(tipoOperacaoSimultaneo);

            tabelaExclusividade[_tabelaCodigoOperacaoLivre] = new GradeCarregamento(quantidadeSimultaneasLivres);

            return tabelaExclusividade;
        }

        private void InformarHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento?.Carga?.HorarioCarregamentoInformadoNoPedido ?? false)
                return;

            DateTime diaCarregamentoProgramado = cargaJanelaCarregamento.CarregamentoReservado ? cargaJanelaCarregamento.DataCarregamentoProgramada.Date : cargaJanelaCarregamento.InicioCarregamento;
            DateTime dataCarregamento = cargaJanelaCarregamento.CarregamentoReservado ? cargaJanelaCarregamento.DataCarregamentoProgramada : cargaJanelaCarregamento.InicioCarregamento;
            Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades = repExclusividadeCarregamento.BuscarExclusividadePorPeriodo(cargaJanelaCarregamento.CentroCarregamento.Codigo, dataCarregamento, DiaSemanaHelper.ObterDiaSemana(dataCarregamento));
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodos = ObterPeriodosCarregamento(cargaJanelaCarregamento, dataCarregamento);
            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro = ObterParadaCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento, dataCarregamento);
            bool encontrouHorario = false;

            if (IsDataCarregamentoRetroativaValida(diaCarregamentoProgramado, cargaJanelaCarregamento.CentroCarregamento) || cargaJanelaCarregamento.HorarioEncaixado)
            {
                ValidarToleranciaDataCarregamento(cargaJanelaCarregamento, diaCarregamentoProgramado);

                if (!IsLimiteCarregamentoValido(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento))
                {
                    cargaJanelaCarregamento.Excedente = true;
                    return;
                }

                if (!IsPossuiCapacidadeCarregamentoDia(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento))
                {
                    if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente || cargaJanelaCarregamento.HorarioEncaixado)
                        throw new ServicoException("A capacidade de carregamento diária foi atingida", CodigoExcecao.HorarioCarregamentoIndisponivel);

                    cargaJanelaCarregamento.Excedente = true;
                    return;
                }

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo in periodos)
                {
                    DateTime dataInicio = ObterDataInicial(cargaJanelaCarregamento, periodo);
                    cargaJanelaCarregamento.TempoCarregamento = ObterTempoCarregamento(cargaJanelaCarregamento, dataInicio.TimeOfDay);

                    if (((dataInicio.TimeOfDay >= periodo.HoraTermino) || !cargaJanelaCarregamento.IsTempoCarregamentoValido()) && !cargaJanelaCarregamento.HorarioEncaixado)
                        continue;

                    DateTime? dataTermino = dataInicio.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
                    DateTime dataTerminoPeriodo = dataInicio.Date.Add(periodo.HoraTermino).AddMinutes(periodo.ToleranciaExcessoTempo);

                    if (cargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                    {
                        dataInicio = dataInicio.Date.Add(periodo.HoraInicio);
                        dataTermino = ObterDataTerminoPorVagasOcupadasNaGrade(cargaJanelaCarregamento, periodo, dataInicio);

                        if (!dataTermino.HasValue)
                            break;
                    }

                    IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(cargaJanelaCarregamento, dataInicio, dataTerminoPeriodo);

                    if (!IsPossuiCapacidadeCarregamentoPeriodo(cargaJanelaCarregamento, dataInicio, periodo))
                    {
                        if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente || cargaJanelaCarregamento.HorarioEncaixado)
                            throw new ServicoException("A capacidade de carregamento do período foi atingida", CodigoExcecao.HorarioCarregamentoIndisponivel);

                        continue;
                    }

                    if (EncontrarProximoHorario(cargaJanelaCarregamento, cargasPeriodo, periodo, dataInicio, dataTermino.Value, paradasCentro, exclusividades))
                    {
                        encontrouHorario = true;
                        break;
                    }
                }
            }

            if (!encontrouHorario)
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                    throw new ServicoException($"Não foi possível encontrar um horário de carregamento para as configurações desta {cargaJanelaCarregamento.CargaBase.DescricaoEntidade}", CodigoExcecao.HorarioCarregamentoIndisponivel);

                cargaJanelaCarregamento.Excedente = true;
            }
        }

        private bool IsDataCarregamentoRetroativaValida(DateTime data, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            try
            {
                ValidarDataCarregamentoRetroativa(data, centroCarregamento);
                return true;
            }
            catch (ServicoException)
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                    throw;

                return false;
            }
        }

        private bool IsLimiteCarregamentoValido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dia)
        {
            try
            {
                ValidarLimiteCarregamentoAtingido(cargaJanelaCarregamento, dia);
                return true;
            }
            catch (ServicoException)
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                    throw;

                return false;
            }
        }

        private bool IsPossuiCapacidadeCarregamentoDia(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dia)
        {
            if (_configuracaoDisponibilidadeCarregamento.PermitirCapacidadeCarregamentoExcedida)
                return true;

            int capacidadeCarregamentoDia = ObterCapacidadeCarregamentoPorDia(cargaJanelaCarregamento.CentroCarregamento, dia);

            if (capacidadeCarregamentoDia == 0)
                return true;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            decimal TotalCarregamentoDia = repositorioCargaJanelaCarregamento.BuscarPesoTotalCarregamentoDia(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia);
            decimal valorCarregamento = (cargaJanelaCarregamento.Peso > 0) ? cargaJanelaCarregamento.Peso : cargaJanelaCarregamento.CargaBase.DadosSumarizados?.PesoTotal ?? 0m;

            if (cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento.HasValue && cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
            {
                TotalCarregamentoDia = repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoDia(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia);
                valorCarregamento = (cargaJanelaCarregamento.Volume > 0) ? cargaJanelaCarregamento.Volume : cargaJanelaCarregamento.CargaBase.DadosSumarizados?.VolumesTotal ?? 0m;
            }
            else if (cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.VolumeECubagem) //precisa validar os dois.
            {
                int capacidadeCarregamentoDiaCubagem = ObterCapacidadeCarregamentoPorDia(cargaJanelaCarregamento.CentroCarregamento, dia, true);

                decimal TotalCarregamentoDiaVolume = repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoDia(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia); //BUSCAR VOLUMES
                decimal valorCarregamentoVolume = (cargaJanelaCarregamento.Volume > 0) ? cargaJanelaCarregamento.Volume : cargaJanelaCarregamento.CargaBase.DadosSumarizados?.VolumesTotal ?? 0m;

                decimal TotalCarregamentoDiaCubagem = repositorioCargaJanelaCarregamento.BuscarCubagemTotalCarregamentoDia(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia); //BUSCAR CUBAGEM
                decimal valorCarregamentoCubagem = cargaJanelaCarregamento.CargaBase.DadosSumarizados?.CubagemTotal ?? 0m;


                return ((TotalCarregamentoDiaVolume + valorCarregamentoVolume) <= capacidadeCarregamentoDia) && ((TotalCarregamentoDiaCubagem + valorCarregamentoCubagem) <= capacidadeCarregamentoDiaCubagem);
            }

            return ((TotalCarregamentoDia + valorCarregamento) <= capacidadeCarregamentoDia);
        }

        private bool IsPossuiCapacidadeCarregamentoPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dia, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento, bool naoBloquearCapacidadeExcedida = false)
        {
            if (_configuracaoDisponibilidadeCarregamento.PermitirCapacidadeCarregamentoExcedida)
                return true;

            int capacidadeCarregamentoPeriodo = ObterCapacidadeCarregamentoPorPeriodo(cargaJanelaCarregamento.CentroCarregamento, dia, periodoCarregamento);

            if (capacidadeCarregamentoPeriodo == 0)
                return true;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            decimal pesoTotalCarregamentoPeriodo = repositorioCargaJanelaCarregamento.BuscarPesoTotalCarregamentoPeriodo(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);

            decimal valorCarregamento = (cargaJanelaCarregamento.Peso > 0) ? cargaJanelaCarregamento.Peso : cargaJanelaCarregamento.CargaBase.DadosSumarizados?.PesoTotal ?? 0m;

            if (cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento.HasValue && cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
            {
                pesoTotalCarregamentoPeriodo = repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoPeriodo(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);
                valorCarregamento = (cargaJanelaCarregamento.Volume > 0) ? cargaJanelaCarregamento.Volume : cargaJanelaCarregamento.CargaBase.DadosSumarizados?.VolumesTotal ?? 0m;
            }

            decimal capacidadeCarregamentoDisponivelPeriodo = capacidadeCarregamentoPeriodo - pesoTotalCarregamentoPeriodo;

            if (capacidadeCarregamentoDisponivelPeriodo <= 0m && !naoBloquearCapacidadeExcedida)
                return false;

            if (valorCarregamento <= capacidadeCarregamentoDisponivelPeriodo)
                return true;

            Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorioCapacidadeCarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(_unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = ObterExcecaoCentroCarregamentoPorData(cargaJanelaCarregamento.CentroCarregamento.Codigo, dia);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento;

            if (excecao != null)
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorExcecao(excecao.Codigo);
            else
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(cargaJanelaCarregamento.CentroCarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(dia));

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoAnterioresOuPosteriores = (from o in periodosCarregamento where o.Codigo != periodoCarregamento.Codigo select o).OrderByDescending(o => o.HoraInicio).ToList();
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo> capacidadesCarregamentoDisponiveisPorPeriodo = new List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo>();

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamentoAnteriorOuPosterior in periodosCarregamentoAnterioresOuPosteriores)
            {
                int capacidadeCarregamentoPeriodoAnteriorOuPosterior = ObterCapacidadeCarregamentoPorPeriodo(cargaJanelaCarregamento.CentroCarregamento, dia, periodoCarregamentoAnteriorOuPosterior);
                decimal pesoTotalCarregamentoPeriodoAnteriorOuPosterior = repositorioCargaJanelaCarregamento.BuscarPesoTotalCarregamentoPeriodo(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia, periodoCarregamentoAnteriorOuPosterior.HoraInicio, periodoCarregamentoAnteriorOuPosterior.HoraTermino);

                if (cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento.HasValue && cargaJanelaCarregamento.CentroCarregamento.TipoCapacidadeCarregamento == TipoCapacidadeCarregamento.Volume)
                    pesoTotalCarregamentoPeriodoAnteriorOuPosterior = repositorioCargaJanelaCarregamento.BuscarVolumeTotalCarregamentoPeriodo(cargaJanelaCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, dia, periodoCarregamentoAnteriorOuPosterior.HoraInicio, periodoCarregamentoAnteriorOuPosterior.HoraTermino);

                decimal capacidadeCarregamentoDisponivelPeriodoAnteriorOuPosterior = capacidadeCarregamentoPeriodoAnteriorOuPosterior - pesoTotalCarregamentoPeriodoAnteriorOuPosterior;

                capacidadesCarregamentoDisponiveisPorPeriodo.Add(new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo()
                {
                    PeriodoCarregamento = periodoCarregamentoAnteriorOuPosterior,
                    CapacidadeDisponivel = capacidadeCarregamentoDisponivelPeriodoAnteriorOuPosterior
                });
            }

            decimal capacidadeCarregamentoDisponivelOutrosPeriodos = capacidadesCarregamentoDisponiveisPorPeriodo.Sum(o => o.CapacidadeDisponivel);
            decimal toleranciaPesoCarregamentoUtilizada = 0.0m;

            if (valorCarregamento > (capacidadeCarregamentoDisponivelPeriodo + capacidadeCarregamentoDisponivelOutrosPeriodos))
            {
                decimal toleranciaPesoCarregamento = valorCarregamento * cargaJanelaCarregamento.CentroCarregamento.PercentualToleranciaPesoCarregamento / 100;

                if (valorCarregamento > (capacidadeCarregamentoDisponivelPeriodo + capacidadeCarregamentoDisponivelOutrosPeriodos + toleranciaPesoCarregamento) && !naoBloquearCapacidadeExcedida)
                    return false;

                if (repositorioCapacidadeCarregamentoAdicional.ExisteCapacidadeCarregamentoAutomatica(cargaJanelaCarregamento.CentroCarregamento.Codigo, dia) && !naoBloquearCapacidadeExcedida)
                    return false;

                toleranciaPesoCarregamentoUtilizada = valorCarregamento - (capacidadeCarregamentoDisponivelPeriodo + capacidadeCarregamentoDisponivelOutrosPeriodos);
            }

            decimal maiorCapacidadeCarregamentoDisponivel = capacidadeCarregamentoDisponivelPeriodo;

            foreach (Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo capacidadeDisponivelPorPeriodo in capacidadesCarregamentoDisponiveisPorPeriodo)
            {
                if (capacidadeDisponivelPorPeriodo.CapacidadeDisponivel > valorCarregamento && !naoBloquearCapacidadeExcedida)
                    return false;

                if (capacidadeDisponivelPorPeriodo.CapacidadeDisponivel > maiorCapacidadeCarregamentoDisponivel)
                    maiorCapacidadeCarregamentoDisponivel = capacidadeDisponivelPorPeriodo.CapacidadeDisponivel;
            }

            if (maiorCapacidadeCarregamentoDisponivel != capacidadeCarregamentoDisponivelPeriodo && !naoBloquearCapacidadeExcedida)
                return false;

            Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamentoAumentarCapacidade = periodoCarregamento;

            if (excecao == null)
            {
                Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repositorioPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(_unitOfWork);
                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(_unitOfWork);
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorioExcecaoCapacidadeCarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(_unitOfWork);
                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dia);

                excecao = new Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento();
                excecao.CapacidadeCarregamento = cargaJanelaCarregamento.CentroCarregamento.ObterCapacidadeCarregamento(diaSemana, false);
                excecao.CentroCarregamento = cargaJanelaCarregamento.CentroCarregamento;
                excecao.Data = dia.Date;
                excecao.Descricao = "Capacidade de carregamento dinâmica";
                excecao.TipoAbrangencia = TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia;

                repositorioExcecaoCapacidadeCarregamento.Inserir(excecao);

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoOriginal in periodosCarregamento)
                {
                    Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoDuplicado = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento();

                    periodoDuplicado.CapacidadeCarregamentoVolume = periodoOriginal.CapacidadeCarregamentoVolume;
                    periodoDuplicado.CapacidadeCarregamentoSimultaneo = periodoOriginal.CapacidadeCarregamentoSimultaneo;
                    periodoDuplicado.CentroCarregamento = excecao.CentroCarregamento;
                    periodoDuplicado.Dia = periodoOriginal.Dia;
                    periodoDuplicado.ExcecaoCapacidadeCarregamento = excecao;
                    periodoDuplicado.HoraInicio = periodoOriginal.HoraInicio;
                    periodoDuplicado.HoraTermino = periodoOriginal.HoraTermino;
                    periodoDuplicado.ToleranciaExcessoTempo = periodoOriginal.ToleranciaExcessoTempo;

                    repositorioPeriodoCarregamento.Inserir(periodoDuplicado);

                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo tipoOperacaoSimultaneoOriginal in periodoOriginal.TipoOperacaoSimultaneo)
                    {
                        Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo tipoOperacaoSimultaneoDuplicado = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo();

                        tipoOperacaoSimultaneoDuplicado.CapacidadeCarregamentoSimultaneo = tipoOperacaoSimultaneoOriginal.CapacidadeCarregamentoSimultaneo;
                        tipoOperacaoSimultaneoDuplicado.PeriodoCarregamento = periodoDuplicado;
                        tipoOperacaoSimultaneoDuplicado.TipoOperacao = tipoOperacaoSimultaneoOriginal.TipoOperacao;

                        repositorioPeriodoCarregamentoTipoOperacaoSimultaneo.Inserir(tipoOperacaoSimultaneoDuplicado);
                    }

                    if (periodoOriginal.Codigo == periodoCarregamento.Codigo)
                        periodoCarregamentoAumentarCapacidade = periodoDuplicado;
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo capacidadeDisponivelPorPeriodo = capacidadesCarregamentoDisponiveisPorPeriodo
                            .Where(o => o.PeriodoCarregamento.Codigo == periodoOriginal.Codigo)
                            .First();

                        capacidadeDisponivelPorPeriodo.PeriodoCarregamento = periodoDuplicado;
                    }
                }

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> previsoesCarregamentoDuplicar = (from o in cargaJanelaCarregamento.CentroCarregamento.PrevisoesCarregamento where o.Dia == diaSemana select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamentoOriginal in previsoesCarregamentoDuplicar)
                {
                    Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamentoDuplicada = new Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento();

                    previsaoCarregamentoDuplicada.CentroCarregamento = excecao.CentroCarregamento;
                    previsaoCarregamentoDuplicada.ExcecaoCapacidadeCarregamento = excecao;
                    previsaoCarregamentoDuplicada.QuantidadeCargas = previsaoCarregamentoOriginal.QuantidadeCargas;
                    previsaoCarregamentoDuplicada.QuantidadeCargasExcedentes = previsaoCarregamentoOriginal.QuantidadeCargasExcedentes;
                    previsaoCarregamentoDuplicada.Dia = previsaoCarregamentoOriginal.Dia;
                    previsaoCarregamentoDuplicada.Descricao = previsaoCarregamentoOriginal.Descricao;
                    previsaoCarregamentoDuplicada.Rota = previsaoCarregamentoOriginal.Rota;
                    previsaoCarregamentoDuplicada.ModelosVeiculos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga in previsaoCarregamentoOriginal.ModelosVeiculos)
                        previsaoCarregamentoDuplicada.ModelosVeiculos.Add(modeloVeicularCarga);

                    repositorioPrevisaoCarregamento.Inserir(previsaoCarregamentoDuplicada);
                }
            }

            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo> capacidadesCarregamentoDisponiveisPorPeriodoOrdenada = capacidadesCarregamentoDisponiveisPorPeriodo.OrderBy(o => o.CapacidadeDisponivel).ToList();
            int pesoCarregamentoFaltante = (int)Math.Ceiling(valorCarregamento - capacidadeCarregamentoDisponivelPeriodo - toleranciaPesoCarregamentoUtilizada);

            periodoCarregamentoAumentarCapacidade.CapacidadeCarregamentoVolume += pesoCarregamentoFaltante;

            repositorioPeriodoCarregamento.Atualizar(periodoCarregamentoAumentarCapacidade);

            foreach (Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CapacidadeCarregamentoPeriodo capacidadeDisponivelPorPeriodo in capacidadesCarregamentoDisponiveisPorPeriodoOrdenada)
            {
                int capacidadeCarregamentoDisponivel = (int)Math.Floor(capacidadeDisponivelPorPeriodo.CapacidadeDisponivel);
                int capacidadeCarregamentoDescontar = (capacidadeCarregamentoDisponivel > pesoCarregamentoFaltante) ? pesoCarregamentoFaltante : capacidadeCarregamentoDisponivel;

                pesoCarregamentoFaltante -= capacidadeCarregamentoDescontar;
                capacidadeDisponivelPorPeriodo.PeriodoCarregamento.CapacidadeCarregamentoVolume -= capacidadeCarregamentoDescontar;

                repositorioPeriodoCarregamento.Atualizar(capacidadeDisponivelPorPeriodo.PeriodoCarregamento);

                if (pesoCarregamentoFaltante == 0)
                    break;
            }

            if (toleranciaPesoCarregamentoUtilizada > 0.0m)
            {
                Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional capacidadeCarregamentoAdicional = new Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional()
                {
                    CapacidadeCarregamentoVolume = (int)Math.Ceiling(toleranciaPesoCarregamentoUtilizada),
                    CentroCarregamento = cargaJanelaCarregamento.CentroCarregamento,
                    Data = cargaJanelaCarregamento.InicioCarregamento.Date,
                    Observacao = "Tolerância de carregamento dinâmica",
                    PeriodoInicio = cargaJanelaCarregamento.InicioCarregamento.Date.Add(periodoCarregamento.HoraInicio),
                    PeriodoTermino = cargaJanelaCarregamento.InicioCarregamento.Date.Add(periodoCarregamento.HoraTermino)
                };

                repositorioCapacidadeCarregamentoAdicional.Inserir(capacidadeCarregamentoAdicional);
            }

            return true;
        }

        private bool IsPossuiCapacidadeCarregamentoSimultaneo(DateTime dataInicio, DateTime dataTermino, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoBase, List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividadeCarregamentos = null)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoValidar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>() { periodoBase };

            if (periodoBase.CentroCarregamento?.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
            {
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoPorDia(periodoBase.CentroCarregamento, dataInicio);
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoPosteriores = periodosCarregamento.Where(periodoCarregamento =>
                    periodoCarregamento.HoraInicio > dataInicio.TimeOfDay &&
                    periodoCarregamento.HoraTermino <= dataTermino.TimeOfDay
                ).ToList();

                periodosCarregamentoValidar.AddRange(periodosCarregamentoPosteriores);
            }

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo in periodosCarregamentoValidar)
            {
                DateTime dataInicioPeriodo = dataInicio.Date.Add(periodo.HoraInicio);
                DateTime dataTerminoPeriodo = dataInicio.Date.Add(periodo.HoraTermino);

                List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoFiltradas = cargasPeriodo.Where(carga =>
                    (!(carga.Encaixe ?? false) || ((carga.TipoOperacaoEncaixe ?? 0) > 0)) &&
                    (
                        (carga.DataInicio >= dataInicioPeriodo && carga.DataInicio < dataTerminoPeriodo) ||
                        (carga.DataFim > dataInicioPeriodo && carga.DataFim <= dataTerminoPeriodo) ||
                        (carga.DataInicio <= dataInicioPeriodo && carga.DataFim >= dataTerminoPeriodo)
                    )
                ).ToList();

                List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentroPeriodo = FiltrarParadasCentro(paradasCentro, dataInicioPeriodo, dataTerminoPeriodo);

                if (paradasCentroPeriodo.Any(parada => (parada.TiposOperacao?.Count ?? 0) == 0))
                    return false;

                int quantidadeDocasParadas = paradasCentroPeriodo.Sum(o => o.QuantidadeParada);

                if (cargasPeriodoFiltradas.Count >= (periodo.CapacidadeCarregamentoSimultaneo - quantidadeDocasParadas))
                    return false;

                if ((periodo.TipoOperacaoSimultaneo == null) || (periodo.TipoOperacaoSimultaneo.Count == 0))
                    continue;

                if (!IsPossuiCapacidadeCarregamentoSimultaneoPorTipoOperacao(dataInicioPeriodo, periodo, cargasPeriodoFiltradas, paradasCentroPeriodo, exclusividadeCarregamentos))
                    return false;
            }

            return true;
        }

        private bool IsPossuiCapacidadeCarregamentoSimultaneoPeriodo(DateTime dataInicio, DateTime dataTermino, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento periodo, List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro)
        {
            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentroPeriodo = FiltrarParadasCentro(paradasCentro, dataInicio, dataTermino);

            if (paradasCentroPeriodo.Any(parada => (parada.TiposOperacao?.Count ?? 0) == 0))
                return false;

            int quantidadeDocasParadas = paradasCentroPeriodo.Sum(o => o.QuantidadeParada);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorarioFiltradas = cargasPeriodoHorario.Where(carga => !(carga.Encaixe ?? false) || ((carga.TipoOperacaoEncaixe ?? 0) > 0)).ToList();

            return !(cargasPeriodoHorarioFiltradas.Count >= (periodo.CapacidadeCarregamentoSimultaneo - quantidadeDocasParadas));
        }

        private bool IsPossuiCapacidadeCarregamentoSimultaneoPorTipoOperacao(DateTime dataBase, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro = null, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividadeCarregamentos = null)
        {
            try
            {
                Dictionary<int, GradeCarregamento> tabelaCarregamento = PreencheGradeCarregamento(dataBase, periodo, cargasPeriodoHorario, paradasCentro, exclusividadeCarregamentos, preencherGradesTotalmenteExclusivas: false);
                bool tabelaPossuiDocaParaTipoOperacao = tabelaCarregamento.ContainsKey(_configuracaoDisponibilidadeCarregamento.CodigoTipoOperacao);
                bool tabelaPossuiVagaParaTipoOperacao = tabelaPossuiDocaParaTipoOperacao && tabelaCarregamento[_configuracaoDisponibilidadeCarregamento.CodigoTipoOperacao].PossuiDisponbilidadeTipoOperacao(
                    _configuracaoDisponibilidadeCarregamento.CodigoTransportador,
                    _configuracaoDisponibilidadeCarregamento.CpfCnpjCliente,
                    _configuracaoDisponibilidadeCarregamento.CodigoModeloVeicularCarga
                );
                bool tabelaPossuiVagaLivre = tabelaCarregamento.ContainsKey(_tabelaCodigoOperacaoLivre) && tabelaCarregamento[_tabelaCodigoOperacaoLivre].Quantidade > 0;

                return (tabelaPossuiVagaParaTipoOperacao || tabelaPossuiVagaLivre);
            }
            catch (ServicoException excecao)
            {
                if (excecao.ErrorCode != CodigoExcecao.ExclusividadeCarregamento)
                    throw;

                return false;
            }
        }

        private bool IsPossuiRestricaoRota(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.RotaFrete rotaFrete, DateTime dataCarregamento, int tempoCarregamento, double tempoPercurso)
        {
            if (rotaFrete == null)
                return false;

            Repositorio.RotaFreteRestricao repositorioRotaFreteRestricao = new Repositorio.RotaFreteRestricao(_unitOfWork);
            List<Dominio.Entidades.RotaFreteRestricao> restricoesRotaFrete = repositorioRotaFreteRestricao.BuscarPorRotaFrete(rotaFrete.Codigo);

            if ((restricoesRotaFrete == null) || (restricoesRotaFrete.Count() == 0))
                return false;

            DateTime dataInicioViagem = dataCarregamento.AddMinutes(tempoCarregamento);
            DateTime dataFimViagem = dataInicioViagem.AddMinutes(tempoPercurso);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDiaSemana(dataInicioViagem);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = cargaJanelaCarregamento.Carga != null ? cargaJanelaCarregamento.Carga.ModeloVeicularCarga : cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga;
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = cargaJanelaCarregamento.Carga != null ? cargaJanelaCarregamento.Carga.TipoDeCarga : cargaJanelaCarregamento.PreCarga.TipoDeCarga;

            return (
                from restricao in restricoesRotaFrete
                where (
                    (
                        diaSemana == DiaSemana.Domingo ? restricao.Domingo = true :
                        diaSemana == DiaSemana.Segunda ? restricao.Segunda = true :
                        diaSemana == DiaSemana.Terca ? restricao.Terca = true :
                        diaSemana == DiaSemana.Quarta ? restricao.Quarta = true :
                        diaSemana == DiaSemana.Quinta ? restricao.Quinta = true :
                        diaSemana == DiaSemana.Sexta ? restricao.Sexta = true :
                        diaSemana == DiaSemana.Sabado ? restricao.Sabado = true : restricao.Segunda = true
                    ) &&
                    (restricao.TipoDeCarga == null || restricao.TipoDeCarga == tipoCarga) &&
                    (restricao.ModeloVeicularCarga == null || restricao.ModeloVeicularCarga == modeloVeicularCarga) &&
                    (dataInicioViagem.TimeOfDay >= restricao.HoraInicio && dataInicioViagem.TimeOfDay < restricao.HoraTermino || dataFimViagem.TimeOfDay >= restricao.HoraInicio && dataFimViagem.TimeOfDay < restricao.HoraTermino)
                )
                select restricao
            ).Count() > 0;
        }

        private void NotificarAlteracaoHorario(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            if (!_configuracaoDisponibilidadeCarregamento.NotificarAlteracaoHorarioCarregamento || configuracaoJanelaCarregamento.NaoEnviarEmailAlteracaoDataCarregamento)
                return;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);
            string mensagem = string.Format(Localization.Resources.Cargas.Carga.AlteradoHorarioCarregamentoCargaPara, $"({carga?.CodigoCargaEmbarcador ?? " "})", $"({carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? " "})");

            if (carga.DadosSumarizados != null)
                mensagem += ObterInformacoesDadosSumarizados(carga.DadosSumarizados);

            mensagem += ObterInformacoesPedidos(carga.Pedidos);

            servicoCarga.NotificarAlteracaoAoOperador(carga, mensagem, _unitOfWork, tipoServicoMultisoftware);

            if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores)
                servicoCarga.NotificarAlteracaoAoTransportador(carga, mensagem, _unitOfWork, tipoServicoMultisoftware);
        }

        private string ObterInformacoesDadosSumarizados(Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados cargaDadosSumarizados)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(cargaDadosSumarizados.Remetentes != null ? "Remetente: " + cargaDadosSumarizados.Remetentes : "");
            sb.AppendLine();
            sb.Append(cargaDadosSumarizados.Origens != null ? "Origem: " + cargaDadosSumarizados.Origens : "");
            sb.AppendLine();
            sb.Append(cargaDadosSumarizados.Destinos != null ? "Destino: " + cargaDadosSumarizados.Destinos : "");

            return sb.ToString();
        }

        private string ObterInformacoesPedidos(ICollection<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido)
        {
            StringBuilder sb = new StringBuilder();

            if (listaCargaPedido == null)
                return "";

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
            {
                if (cargaPedido == null)
                    continue;

                string texto = "Pedido: " + cargaPedido.Pedido.NumeroPedidoEmbarcador;
                if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.Ordem))
                    texto += " / Ordem: " + cargaPedido.Pedido.Ordem;
                sb.AppendLine();
                sb.Append(texto);
            }

            return sb.Length > 0 ? sb.ToString() : string.Empty;
        }

        private int ObterCapacidadeCarregamentoPorDia(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia, bool porCubagem = false)
        {
            if (!centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso || (centroCarregamento.TipoCapacidadeCarregamentoPorPeso != TipoCapacidadeCarregamentoPorPeso.DiaSemana))
                return 0;

            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dia);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, dia);
            Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorioCapacidadeCarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(_unitOfWork);
            int capacidadeCarregamento = excecao?.CapacidadeCarregamentoVolume ?? centroCarregamento.ObterCapacidadeCarregamento(diaSemana, porCubagem);
            int capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamento(centroCarregamento.Codigo, dia);

            return capacidadeCarregamento + capacidadeCarregamentoAdicional;
        }

        private int ObterCapacidadeCarregamentoPorPeriodo(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento)
        {
            if (!centroCarregamento.UtilizarCapacidadeCarregamentoPorPeso || (centroCarregamento.TipoCapacidadeCarregamentoPorPeso != TipoCapacidadeCarregamentoPorPeso.PeriodoCarregamento))
                return 0;

            Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional repositorioCapacidadeCarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeCarregamentoAdicional(_unitOfWork);
            int capacidadeCarregamentoVolume = periodoCarregamento.CapacidadeCarregamentoVolume;
            int capacidadeCarregamentoAdicional = repositorioCapacidadeCarregamentoAdicional.BuscarCapacidadeCarregamentoPorPeriodo(centroCarregamento.Codigo, dia, periodoCarregamento.HoraInicio, periodoCarregamento.HoraTermino);

            return capacidadeCarregamentoVolume + capacidadeCarregamentoAdicional;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> ObterCargasConflitantesNoPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime novoHorarioInicio, DateTime novoHorarioTermino)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            int codigoFilial = cargaJanelaCarregamento.CargaBase.Filial?.Codigo ?? 0;
            int codigoCentroCarregamento = cargaJanelaCarregamento.CentroCarregamento.Codigo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasConflitantes = repCargaJanelaCarregamento.BuscarPorIncidenciaDeHorario(cargaJanelaCarregamento.Codigo, codigoFilial, codigoCentroCarregamento, novoHorarioInicio, novoHorarioTermino);
            var cargasEDestinatarios = repCargaJanelaCarregamento.BuscarCargasEDestinatarioPorIncidenciaDeHorario(cargaJanelaCarregamento.Codigo, codigoFilial, codigoCentroCarregamento, novoHorarioInicio, novoHorarioTermino);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> objCargasConflitantes = new List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo>();

            for (var i = 0; i < cargasConflitantes.Count; i++)
            {
                for (var j = 0; j < cargasConflitantes.Count; j++)
                {
                    if (cargasConflitantes[i].Codigo == cargasConflitantes[j].Codigo && objCargasConflitantes.Any(o => o.Codigo == cargasConflitantes[j].Codigo))
                        continue;

                    if (
                        (
                            (cargasConflitantes[i].InicioCarregamento >= cargasConflitantes[j].InicioCarregamento && cargasConflitantes[i].InicioCarregamento < cargasConflitantes[j].TerminoCarregamento) ||
                            (cargasConflitantes[i].TerminoCarregamento > cargasConflitantes[j].InicioCarregamento && cargasConflitantes[i].TerminoCarregamento <= cargasConflitantes[j].TerminoCarregamento) ||
                            (cargasConflitantes[i].InicioCarregamento <= cargasConflitantes[j].InicioCarregamento && cargasConflitantes[i].TerminoCarregamento >= cargasConflitantes[j].TerminoCarregamento)
                        ) && !objCargasConflitantes.Any(v => v.Codigo == cargasConflitantes[i].Codigo)
                    )
                    {
                        objCargasConflitantes.Add(new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo()
                        {
                            Codigo = cargasConflitantes[i].Codigo,
                            TipoCarga = cargasConflitantes[i].CargaBase?.TipoDeCarga?.Codigo ?? 0,
                            TipoOperacao = cargasConflitantes[i].CargaBase?.TipoOperacao?.Codigo ?? 0,
                            Transportador = cargasConflitantes[i].CargaBase?.Empresa?.Codigo ?? 0,
                            ModeloVeicularCarga = cargasConflitantes[i].CargaBase?.ModeloVeicularCarga?.Codigo ?? 0,
                            Destinatario = cargasEDestinatarios.Where(o => o.Carga == cargasConflitantes[i].Carga?.Codigo).FirstOrDefault().Destinatario ?? 0,
                            Encaixe = cargasConflitantes[i].HorarioEncaixado,
                            TipoOperacaoEncaixe = cargasConflitantes[i].TipoOperacaoEncaixe?.Codigo ?? 0,
                            DataInicio = cargasConflitantes[i].InicioCarregamento,
                            DataFim = cargasConflitantes[i].TerminoCarregamento,
                        });
                    }
                }
            }

            //if (totalCargasConflitantes == 0 && cargasConflitantes.Count > 0)
            //    totalCargasConflitantes = 1;

            return objCargasConflitantes.Distinct().ToList();
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> ObterCargasPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, DateTime dataHoraCarregamento)
        {
            DateTime dataInicial = dataHoraCarregamento.Date.Add(periodo.HoraInicio);
            DateTime dataFim = dataHoraCarregamento.Date.Add(periodo.HoraTermino).AddMinutes(periodo.ToleranciaExcessoTempo);

            return ObterCargasPeriodo(cargaJanelaCarregamento, dataInicial, dataFim);
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> ObterCargasPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataInicial, DateTime dataFim)
        {
            return ObterCargasPeriodo(cargaJanelaCarregamento, cargaJanelaCarregamento.CentroCarregamento, dataInicial, dataFim);
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> ObterCargasPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dataInicial, DateTime dataFim)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            return repositorioCargaJanelaCarregamento.BuscarCargaPeriodoPorIncidenciaDeHorario(cargaJanelaCarregamento?.Codigo ?? 0, centroCarregamento?.Codigo ?? 0, dataInicial, dataFim);
        }

        private Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga ObterClienteDescarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Pessoas.ClienteDescarga repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPedidos(cargaJanelaCarregamento);
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescargaPedido = repositorioClienteDescarga.BuscarPorOrigemEDestino(pedido.Remetente.CPF_CNPJ, pedido.Destinatario?.CPF_CNPJ ?? 0);

                if (clienteDescargaPedido != null)
                {
                    if (clienteDescarga == null)
                        clienteDescarga = clienteDescargaPedido;
                    else
                    {
                        bool horarioInicioDescarregamentoMenor = (!string.IsNullOrWhiteSpace(clienteDescargaPedido.HoraInicioDescarga) && int.Parse(clienteDescargaPedido.HoraInicioDescarga.Substring(0, 2)) < int.Parse(clienteDescarga.HoraInicioDescarga.Substring(0, 2)));

                        if (horarioInicioDescarregamentoMenor)
                            clienteDescarga = clienteDescargaPedido;
                    }
                }
            }

            return clienteDescarga;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private DateTime ObterDataInicial(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            DateTime dataCarregamento = cargaJanelaCarregamento.CarregamentoReservado ? cargaJanelaCarregamento.DataCarregamentoProgramada.Date : cargaJanelaCarregamento.InicioCarregamento;

            if (dataCarregamento.TimeOfDay <= periodo.HoraInicio)
                return dataCarregamento.Date.Add(periodo.HoraInicio);
            else
                return dataCarregamento;
        }

        private DateTime? ObterDataTerminoPorVagasOcupadasNaGrade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoBase, DateTime dataInicio)
        {
            cargaJanelaCarregamento.QuantidadeAdicionalVagasOcupadas = ObterQuantidadeVagasOcuparGradeNaCarregamento(cargaJanelaCarregamento, dataInicio) - 1;

            if (cargaJanelaCarregamento.QuantidadeAdicionalVagasOcupadas == 0)
                return dataInicio.Date.Add(periodoBase.HoraTermino);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodos = ObterPeriodosCarregamentoPorDia(cargaJanelaCarregamento.CentroCarregamento, dataInicio);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosPosteriores = periodos.Where(periodoCarregamento => periodoCarregamento.HoraInicio > dataInicio.TimeOfDay).ToList();

            if (periodosPosteriores.Count < cargaJanelaCarregamento.QuantidadeAdicionalVagasOcupadas)
                return null;

            Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoTerminarCarregamento = periodosPosteriores[cargaJanelaCarregamento.QuantidadeAdicionalVagasOcupadas - 1];

            return dataInicio.Date.Add(periodoTerminarCarregamento.HoraTermino);
        }

        private int[] ObterGrupoPessoas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            int[] grupoPessoa = null;

            if ((cargaJanelaCarregamento.PreCarga != null) && (cargaJanelaCarregamento.Carga == null))
            {
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(cargaJanelaCarregamento.PreCarga.Codigo);

                grupoPessoa = (from destinatario in preCarga.Destinatarios where destinatario.GrupoPessoas != null select destinatario.GrupoPessoas.Codigo).Distinct().ToArray();
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = repCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                grupoPessoa = (from pedido in pedidos where pedido.Pedido.Destinatario != null && pedido.Pedido.Destinatario.GrupoPessoas != null select pedido.Pedido.Destinatario.GrupoPessoas.Codigo).Distinct().ToArray();
            }

            return grupoPessoa;
        }

        private TimeSpan? ObterHoraMinimaPeriodoDataAtual(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            DateTime dataBase = DateTime.Now;
            int tempoTolerancia = ObterTempoTolerancia(centroCarregamento);
            TimeSpan horaInicial = dataBase.TimeOfDay.Add(TimeSpan.FromMinutes(tempoTolerancia));

            return horaInicial.Days > 0 ? (TimeSpan?)null : horaInicial;
        }

        private TimeSpan? ObterHoraMinimaPeriodoDataPosterior(DateTime dia, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            DateTime dataAtual = DateTime.Today;
            TimeSpan horaAtual = DateTime.Now.TimeOfDay;
            bool diaSeguinteDataAtual = dia.AddDays(-1) == dataAtual;

            if (!diaSeguinteDataAtual)
                return new TimeSpan();

            int minutosTolerancia = ObterTempoTolerancia(centroCarregamento);

            if (minutosTolerancia == 0)
                return new TimeSpan();

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoDataAtual = ObterPeriodosCarregamentoCentroCarregamentoOuExcecao(centroCarregamento, dataAtual);

            if (periodosCarregamentoDataAtual.Count == 0)
                return new TimeSpan();

            TimeSpan horaTerminoExpedienteDataAtual = periodosCarregamentoDataAtual.LastOrDefault().HoraTermino;
            TimeSpan horaMinimaAteFimExpedienteDataAtual = horaTerminoExpedienteDataAtual.Subtract(TimeSpan.FromMinutes(minutosTolerancia));

            if (horaAtual <= horaMinimaAteFimExpedienteDataAtual)
                return new TimeSpan();

            if (horaAtual < horaTerminoExpedienteDataAtual)
                minutosTolerancia -= (int)horaTerminoExpedienteDataAtual.Subtract(horaAtual).TotalMinutes;

            TimeSpan horaInicioExpediente = periodosCarregamento.FirstOrDefault().HoraInicio;
            TimeSpan horaMinimaInicioExpediente = horaInicioExpediente.Add(TimeSpan.FromMinutes(minutosTolerancia));

            if (horaMinimaInicioExpediente.Days == 0)
                return horaMinimaInicioExpediente;

            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> ObterIntervalosPorPeriodos(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoDentroDaTolerancia(centroCarregamento, dia);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> intervalosDisponiveis = new List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento>();
            int intervaloSelecaoHorario = centroCarregamento.IntervaloSelecaoHorarioCarregamentoTransportador;

            TimeSpan filtroHoraMinima = dia.Date == DateTime.Today ? DateTime.Now.TimeOfDay : new TimeSpan();

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo in periodosCarregamento)
            {
                double minutosPeriodo = (periodo.HoraTermino - periodo.HoraInicio).TotalMinutes;
                int quantidadeIntervalosNoPeriodo = (int)Math.Ceiling(minutosPeriodo / intervaloSelecaoHorario);

                TimeSpan horaInicial = periodo.HoraInicio;

                for (int i = 0; i < quantidadeIntervalosNoPeriodo; i++)
                {
                    bool isUltimoIntervalo = (i + 1) == quantidadeIntervalosNoPeriodo;

                    TimeSpan tempoIntervalo = TimeSpan.FromMinutes(intervaloSelecaoHorario - 1);
                    if (isUltimoIntervalo)
                        tempoIntervalo = TimeSpan.FromMinutes(minutosPeriodo - intervaloSelecaoHorario * (quantidadeIntervalosNoPeriodo - 1));

                    var intervalo = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento
                    {
                        Periodo = periodo.Codigo,
                        Index = i,
                        CapacidadeCarregamentoSimultaneo = periodo.CapacidadeCarregamentoSimultaneo,
                        HoraInicio = horaInicial,
                        HoraTermino = horaInicial.Add(tempoIntervalo),
                    };

                    horaInicial = intervalo.HoraTermino.Add(TimeSpan.FromMinutes(1));

                    if (intervalo.HoraInicio >= filtroHoraMinima)
                        intervalosDisponiveis.Add(intervalo);
                }
            }

            return intervalosDisponiveis;
        }

        private int ObterNumeroCargas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.RotaFrete rotaFrete, DateTime dia, Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamentoDia, bool somenteReservadas)
        {
            List<int> modelos = (from obj in previsaoCarregamentoDia.ModelosVeiculos select obj.Codigo).ToList();
            int codigoPreCarga = 0;

            if (cargaJanelaCarregamento.Carga != null && cargaJanelaCarregamento.PreCarga != null)
                codigoPreCarga = cargaJanelaCarregamento.PreCarga.Codigo;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            return repCargaJanelaCarregamento.ContarCargasPorRota(cargaJanelaCarregamento.CentroCarregamento.Codigo, rotaFrete.Codigo, dia, modelos.ToArray(), codigoPreCarga, somenteReservadas, cargaJanelaCarregamento.Codigo);
        }

        private int ObterNumeroCargasReservadaPorDia(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamentoDia, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repositorioReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(_unitOfWork);
            int[] grupoPessoa = ObterGrupoPessoas(cargaJanelaCarregamento);
            int numeroCargasReservada = repositorioReservaCargaGrupoPessoa.ContarReservasPorDia(cargaJanelaCarregamento.CentroCarregamento.Codigo, previsaoCarregamentoDia.Codigo, dia);
            int numeroCargasReservadaGrupo = repositorioReservaCargaGrupoPessoa.ContarReservasPorGrupos(cargaJanelaCarregamento.CentroCarregamento.Codigo, previsaoCarregamentoDia.Codigo, dia, grupoPessoa);

            return (numeroCargasReservada - numeroCargasReservadaGrupo);
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidos(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                return (from cargaPedido in cargasPedidos select cargaPedido.Pedido).ToList();
            }

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            return repositorioPedido.BuscarPorPreCarga(cargaJanelaCarregamento.PreCarga.Codigo);
        }

        private Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento ObterPeriodoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime novoHorarioInicio, DateTime novoHorarioTermino)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoPorDia(cargaJanelaCarregamento.CentroCarregamento, novoHorarioInicio);

            if (cargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                novoHorarioTermino = novoHorarioInicio;

            Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento = (
                from o in periodosCarregamento
                where (
                    o.HoraInicio <= novoHorarioInicio.TimeOfDay &&
                    o.HoraTermino.Add(TimeSpan.FromMinutes(o.ToleranciaPorLimiteCarregamentos)) >= novoHorarioInicio.TimeOfDay &&
                    o.HoraTermino.Add(TimeSpan.FromMinutes(o.ToleranciaPorLimiteCarregamentos)) >= novoHorarioTermino.TimeOfDay
                )
                select o
            ).FirstOrDefault();

            if (periodoCarregamento == null)
                throw new ServicoException("O horário informado não está configurado para este centro de carregamento.");

            return periodoCarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> ObterPeriodosCarregamentoPorDia(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime data)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, data);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento;

            if (excecao != null)
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorExcecao(excecao.Codigo);
            else
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(centroCarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(data));

            return periodosCarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> ObterPeriodosCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dia)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoPorDia(cargaJanelaCarregamento.CentroCarregamento, dia);

            if (!_definicaoHorarioCarregamentoAteLimiteTentativas && (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente || cargaJanelaCarregamento.HorarioEncaixado))
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento = (from o in periodosCarregamento where o.HoraInicio <= dia.TimeOfDay && o.HoraTermino >= dia.TimeOfDay select o).FirstOrDefault();

                if (periodoCarregamento == null)
                    throw new ServicoException($"Não existe período de carregamento para as configurações desta {cargaJanelaCarregamento.CargaBase.DescricaoEntidade}", CodigoExcecao.HorarioCarregamentoIndisponivel);

                return new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>() { periodoCarregamento };
            }

            return periodosCarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> ObterPeriodosCarregamentoCentroCarregamentoOuExcecao(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = ObterExcecaoCentroCarregamentoPorData(centroCarregamento.Codigo, dia);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            if (excecao != null)
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorExcecao(excecao.Codigo);
            else if (!centroCarregamento.EscolherHorarioCarregamentoPorLista)
                periodosCarregamento = repositorioPeriodoCarregamento.BuscarPorCentroCarregamentoEDia(centroCarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(dia));

            return periodosCarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> ObterPeriodosCarregamentoDentroDaTolerancia(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime data)
        {
            DateTime dia = data.Date;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoCentroCarregamentoOuExcecao(centroCarregamento, dia);

            if (configuracaoEmbarcador.PermitirAlterarCargaHorarioCarregamentoInferiorAtual)
                return periodosCarregamento;

            if (dia < DateTime.Today || periodosCarregamento.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            TimeSpan? horaMinimaPeriodoDisponivel = null;

            if (dia == DateTime.Today)
                horaMinimaPeriodoDisponivel = ObterHoraMinimaPeriodoDataAtual(centroCarregamento);
            else
                horaMinimaPeriodoDisponivel = ObterHoraMinimaPeriodoDataPosterior(dia, periodosCarregamento, centroCarregamento);

            if (!horaMinimaPeriodoDisponivel.HasValue)
                return new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            /**
             * Ultragaz utiliza intervalos e tem um periodo de carregamento
             * que vai de manha até noite, por isso, o filtro tem que ser
             * por hora termino
             */
            bool isCentroCarregamentoUtilizaIntervalo = centroCarregamento.IntervaloSelecaoHorarioCarregamentoTransportador > 0;

            if (isCentroCarregamentoUtilizaIntervalo)
                return (from periodo in periodosCarregamento where periodo.HoraTermino >= horaMinimaPeriodoDisponivel.Value select periodo).ToList();

            return (from periodo in periodosCarregamento where periodo.HoraInicio >= horaMinimaPeriodoDisponivel.Value select periodo).ToList();
        }

        private Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento ObterPrevisaoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime novoHorarioInicio)
        {
            int codigoRota = cargaJanelaCarregamento.CargaBase.Rota?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = cargaJanelaCarregamento.CargaBase.ModeloVeicularCarga;
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = ObterExcecaoCentroCarregamentoPorData(cargaJanelaCarregamento.CentroCarregamento.Codigo, novoHorarioInicio);
            Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamento = null;

            if (excecao != null && excecao.PrevisoesCarregamento != null)
            {
                previsaoCarregamento = (
                    from o in excecao.PrevisoesCarregamento
                    where (
                        o.ModelosVeiculos.Contains(modeloVeicularCarga) &&
                        o.Rota.Codigo == codigoRota
                    )
                    select o
                ).FirstOrDefault();
            }
            else
            {
                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(novoHorarioInicio);

                previsaoCarregamento = (
                    from o in cargaJanelaCarregamento.CentroCarregamento.PrevisoesCarregamento
                    where (
                        o.Dia == diaSemana &&
                        o.ModelosVeiculos.Contains(modeloVeicularCarga) &&
                        o.Rota.Codigo == codigoRota
                    )
                    select o
                ).FirstOrDefault();
            }

            return previsaoCarregamento;
        }

        private Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento ObterPrevisaoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.RotaFrete rotaFrete, DateTime dia, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular)
        {
            if (rotaFrete != null)
            {
                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = ObterExcecaoCentroCarregamentoPorData(cargaJanelaCarregamento.CentroCarregamento.Codigo, dia);

                if (excecao != null)
                    return repositorioPrevisaoCarregamento.BuscarPorExcecao(excecao.Codigo, rotaFrete.Codigo, modeloVeicular.Codigo);

                return repositorioPrevisaoCarregamento.BuscarPorCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento.Codigo, rotaFrete.Codigo, modeloVeicular.Codigo, DiaSemanaHelper.ObterDiaSemana(dia));
            }

            return null;
        }

        private DateTime? ObterProximaDataComPeriodoCarregamento(int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            DateTime dataCarregamentoProximoDia = dataCarregamento.AddDays(1);
            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataCarregamento);
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorioExcecaoCapacidadeCarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(_unitOfWork);
            List<DiaSemana> diasSemanaComPeriodosCarregamento = repositorioPeriodoCarregamento.BuscarDiasSemanaComPeriodosCarregamento(codigoCentroCarregamento);
            List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento> capacidadesCarregamentoPorExcecao = repositorioExcecaoCapacidadeCarregamento.BuscarTodosPorCentroCarregamentoEPeriodo(codigoCentroCarregamento, dataCarregamentoProximoDia);
            DateTime? proximaDataCarregamentoPorExcecaoEDia = repositorioExcecaoCapacidadeCarregamento.BuscarProximaDataPorCentroCarregamentoEDia(codigoCentroCarregamento, dataCarregamento);
            DateTime? proximaDataCarregamentoPorPeriodo = null;
            List<DateTime> proximasDataCarregamentoPorExcecao = new List<DateTime>();

            if (diasSemanaComPeriodosCarregamento.Count > 0)
            {
                DiaSemana proximoDiaSemana = diasSemanaComPeriodosCarregamento.Where(o => o > diaSemana).Select(o => (DiaSemana?)o).FirstOrDefault() ?? diasSemanaComPeriodosCarregamento.FirstOrDefault();
                int diasAdicionar = (proximoDiaSemana > diaSemana) ? (int)proximoDiaSemana - (int)diaSemana : (int)proximoDiaSemana + 7 - (int)diaSemana;

                proximaDataCarregamentoPorPeriodo = dataCarregamento.AddDays(diasAdicionar);
            }

            if (proximaDataCarregamentoPorExcecaoEDia.HasValue)
                proximasDataCarregamentoPorExcecao.Add(proximaDataCarregamentoPorExcecaoEDia.Value);

            foreach (Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento capacidadeCarregamento in capacidadesCarregamentoPorExcecao)
            {
                List<DiaSemana> diasSemanaComPeriodosCarregamentoPorExcecao = new List<DiaSemana>();

                if (capacidadeCarregamento.DisponivelDomingo)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Domingo);

                if (capacidadeCarregamento.DisponivelSegunda)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Segunda);

                if (capacidadeCarregamento.DisponivelTerca)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Terca);

                if (capacidadeCarregamento.DisponivelQuarta)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Quarta);

                if (capacidadeCarregamento.DisponivelQuinta)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Quinta);

                if (capacidadeCarregamento.DisponivelSexta)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Sexta);

                if (capacidadeCarregamento.DisponivelSabado)
                    diasSemanaComPeriodosCarregamentoPorExcecao.Add(DiaSemana.Sabado);

                if (diasSemanaComPeriodosCarregamentoPorExcecao.Count > 0)
                {
                    DiaSemana proximoDiaSemana = diasSemanaComPeriodosCarregamentoPorExcecao.Where(o => o > diaSemana).Select(o => (DiaSemana?)o).FirstOrDefault() ?? diasSemanaComPeriodosCarregamentoPorExcecao.FirstOrDefault();
                    int diasAdicionar = (proximoDiaSemana > diaSemana) ? (int)proximoDiaSemana - (int)diaSemana : (int)proximoDiaSemana + 7 - (int)diaSemana;
                    DateTime proximaDataCarregamento = dataCarregamento.AddDays(diasAdicionar);

                    if (!capacidadeCarregamento.DataFinal.HasValue || (proximaDataCarregamento <= capacidadeCarregamento.DataFinal.Value.Date))
                        proximasDataCarregamentoPorExcecao.Add(proximaDataCarregamento);
                }
            }

            DateTime? proximaDataCarregamentoPorExcecao = proximasDataCarregamentoPorExcecao.Min(o => (DateTime?)o);

            return DateTimeExtensions.Min(proximaDataCarregamentoPorPeriodo, proximaDataCarregamentoPorExcecao)?.Date;
        }

        private int ObterQuantidadeVagasOcuparGradeNaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataInicio)
        {
            CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new CargaJanelaCarregamentoConsulta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.TempoCarregamento configuracaoTempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterConfiguracaoTempoCarregamento(cargaJanelaCarregamento, dataInicio.TimeOfDay);
            int quantidadeVagasOcuparGradeNaCarregamento = configuracaoTempoCarregamento?.QuantidadeVagasOcuparGradeNaCarregamento ?? 0;

            return (quantidadeVagasOcuparGradeNaCarregamento > 0) ? quantidadeVagasOcuparGradeNaCarregamento : 1;
        }

        private Dominio.Entidades.RotaFrete ObterRotaFrete(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga != null)
            {
                if (cargaJanelaCarregamento.Carga.Rota == null)
                    Carga.RotaFrete.SetarRotaFreteCargaPorPedidos(cargaJanelaCarregamento.Carga, _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                return cargaJanelaCarregamento.Carga.Rota;
            }

            return cargaJanelaCarregamento.PreCarga.Rota;
        }

        private int ObterTempoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TimeSpan horarioInicioCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.UtilizarTempoCarregamentoPorPeriodo)
                return cargaJanelaCarregamento.TempoCarregamento;

            return new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador).ObterTempoCarregamento(cargaJanelaCarregamento, horarioInicioCarregamento);
        }

        private int ObterTempoTolerancia(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            bool cargaEstaFechada = false;
            if (_configuracaoDisponibilidadeCarregamento.CodigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                cargaEstaFechada = repCarga.TodosProdutosDaCargaSaoPalletsFechado(_configuracaoDisponibilidadeCarregamento.CodigoCarga);
            }

            return cargaEstaFechada ? centroCarregamento.TempoToleranciaCargaFechada : centroCarregamento.TempoToleranciaChegadaAtraso;
        }

        private Dictionary<int, GradeCarregamento> PreencheGradeCarregamento(DateTime dataBase, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades, bool preencherGradesTotalmenteExclusivas)
        {
            DateTime dataInicial = dataBase.Date.Add(periodo.HoraInicio);
            DateTime dataFinal = dataBase.Date.Add(periodo.HoraTermino);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> tipoOperacaoSimultaneos = periodo.TipoOperacaoSimultaneo.ToList();
            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentroPeriodo = FiltrarParadasCentro(paradasCentro, dataInicial, dataFinal);
            Dictionary<int, GradeCarregamento> tabelaCarregamento = GerarGradePorTipoOperacao(periodo.CapacidadeCarregamentoSimultaneo, tipoOperacaoSimultaneos);

            PreencheGradeCarregamentoComParadas(tabelaCarregamento, paradasCentroPeriodo);
            PreencheGradeCarregamentoComExclusividades(tabelaCarregamento, exclusividades, dataInicial, dataFinal, preencherGradesTotalmenteExclusivas);
            PreencheGradeCarregamentoComCargasAlocadas(tabelaCarregamento, tipoOperacaoSimultaneos, cargasPeriodoHorario);

            return tabelaCarregamento;
        }

        private void PreencheGradeCarregamentoComCargasAlocadas(Dictionary<int, GradeCarregamento> tabelaExclusividade, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> tipoOperacaoSimultaneos, IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario)
        {
            List<int> codigosTipoOperacaoConfiguradosNoPeriodo = tipoOperacaoSimultaneos.Select(o => o.TipoOperacao.Codigo).ToList();

            foreach (Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo cargaPeriodo in cargasPeriodoHorario)
            {
                if ((cargaPeriodo.Encaixe ?? false) && ((cargaPeriodo.TipoOperacaoEncaixe ?? 0) == 0))
                    continue;

                int chaveVagaExclusivaOuLivre = _tabelaCodigoOperacaoLivre;

                if (codigosTipoOperacaoConfiguradosNoPeriodo.Contains(cargaPeriodo.ObterTipoOperacao) && tabelaExclusividade[cargaPeriodo.ObterTipoOperacao].PossuiDisponbilidadeTipoOperacao(cargaPeriodo))
                    chaveVagaExclusivaOuLivre = cargaPeriodo.ObterTipoOperacao;

                tabelaExclusividade[chaveVagaExclusivaOuLivre].Ocupadas += 1;
            }
        }

        private void PreencheGradeCarregamentoComExclusividades(Dictionary<int, GradeCarregamento> tabelaExclusividade, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividadeCarregamentos, DateTime dataInicial, DateTime dataFinal, bool preencherGradesTotalmenteExclusivas)
        {
            List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividadeCarregamentosPeriodo = FiltrarExclusividades(exclusividadeCarregamentos, dataInicial, dataFinal);

            foreach (Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividadeCarregamento in exclusividadeCarregamentosPeriodo)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoComExclusividade = exclusividadeCarregamento.PeriodosCarregamento.Where(obj => obj.HoraInicio <= dataInicial.TimeOfDay && obj.HoraTermino >= dataFinal.TimeOfDay).FirstOrDefault();

                if (periodoComExclusividade == null)
                    continue;

                bool regraSeAplicaATransportador = exclusividadeCarregamento.Transportador == null || exclusividadeCarregamento.Transportador.Codigo == _configuracaoDisponibilidadeCarregamento.CodigoTransportador;
                bool regraSeAplicaACliente = exclusividadeCarregamento.Cliente == null || exclusividadeCarregamento.Cliente.CPF_CNPJ == _configuracaoDisponibilidadeCarregamento.CpfCnpjCliente;
                bool regraSeAplicaAoModeloVeicularCarga = exclusividadeCarregamento.ModeloVeicularCarga == null || exclusividadeCarregamento.ModeloVeicularCarga.Codigo == _configuracaoDisponibilidadeCarregamento.CodigoModeloVeicularCarga;

                if (regraSeAplicaATransportador && regraSeAplicaACliente && regraSeAplicaAoModeloVeicularCarga)
                    continue;

                // Se não há configuração, o periodo é dedicado totalmente ao transportador/cliente
                if (periodoComExclusividade.TipoOperacaoSimultaneo.Count == 0)
                {
                    if (!preencherGradesTotalmenteExclusivas)
                        throw new ServicoException($"O Centro de Carregamento {exclusividadeCarregamento.CentroCarregamento.Descricao} está exclusivo para {exclusividadeCarregamento.DescricaoExclusividade} para o periodo {periodoComExclusividade.DescricaoPeriodo}", CodigoExcecao.ExclusividadeCarregamento);

                    List<int> tiposOperacoesTabela = tabelaExclusividade.Keys.ToList();
                    foreach (int tipoOperacaoTabela in tiposOperacoesTabela)
                    {
                        tabelaExclusividade[tipoOperacaoTabela].AddExclusividade(exclusividadeCarregamento, tabelaExclusividade[tipoOperacaoTabela].Disponiveis);
                    }

                    continue;
                }

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo tipoOperacao in periodoComExclusividade.TipoOperacaoSimultaneo)
                {
                    if (!tabelaExclusividade.ContainsKey(tipoOperacao.TipoOperacao.Codigo)) continue;

                    tabelaExclusividade[tipoOperacao.TipoOperacao.Codigo].AddExclusividade(exclusividadeCarregamento, tipoOperacao.CapacidadeCarregamentoSimultaneo);
                }
            }
        }

        private void PreencheGradeCarregamentoComParadas(Dictionary<int, GradeCarregamento> tabelaExclusividade, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentroPeriodo)
        {
            if (paradasCentroPeriodo == null)
                return;

            foreach (Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro motivoParadaCentro in paradasCentroPeriodo)
            {
                if (motivoParadaCentro.TiposOperacao.Count == 0)
                {
                    tabelaExclusividade.Clear();
                    return;
                }

                foreach (Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao tipoOperacaoParada in motivoParadaCentro.TiposOperacao)
                {
                    if (!tabelaExclusividade.ContainsKey(tipoOperacaoParada.TipoOperacao.Codigo))
                        tabelaExclusividade[tipoOperacaoParada.TipoOperacao.Codigo] = new GradeCarregamento(tipoOperacaoParada);
                    else
                        tabelaExclusividade[tipoOperacaoParada.TipoOperacao.Codigo].Disponiveis -= tipoOperacaoParada.Quantidade;
                }
            }
        }

        private DateTime ReprogramarDatasComTolerancia(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime programacaoOriginal, DateTime dataAntiga, DateTime novaData)
        {
            DiaSemana diaSemanaAntigo = DiaSemanaHelper.ObterDiaSemana(dataAntiga);
            DiaSemana diaSemanaNovo = DiaSemanaHelper.ObterDiaSemana(novaData);

            int toleranciaDiaAnterior = centroCarregamento.ObterToleranciaAtraso(diaSemanaAntigo);
            int toleranciaNovoDia = centroCarregamento.ObterToleranciaAtraso(diaSemanaNovo);

            int toleranciaMudanca = toleranciaNovoDia - toleranciaDiaAnterior;

            return programacaoOriginal.AddDays(toleranciaMudanca);
        }

        private void ValidarDataCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime data)
        {
            ValidarDataCarregamentoRetroativa(data, cargaJanelaCarregamento.CentroCarregamento);
            ValidarToleranciaDataCarregamento(cargaJanelaCarregamento, data);
        }

        private void ValidarNumeroAlteracoesTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, DateTime novoHorarioInicio)
        {
            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento == null)
                return;

            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos != LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                return;

            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.LimiteAlteracoesHorarioTransportador == 0)
                return;

            int alteracoesPeloEmbarcador = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.QuantidadeAlteracoesManuaisHorarioCarregamento;

            if ((cargaJanelaCarregamentoTransportador.NumeroAlteracoesHorarioRealizadas + alteracoesPeloEmbarcador) >= cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento.LimiteAlteracoesHorarioTransportador)
                throw new ServicoException("Você excedeu o número máximo de alterações de horário. Entre em contato com a filial caso necessário.");
        }

        private void ValidarDataCarregamentoRetroativa(DateTime data, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            DateTime dataLimite = DateTime.Now.AddMinutes(-centroCarregamento?.ToleranciaDataRetroativa ?? 0);

            dataLimite = dataLimite.AddSeconds(-dataLimite.Second);

            if ((data <= dataLimite) && !(configuracaoEmbarcador.PermitirAlterarCargaHorarioCarregamentoInferiorAtual || _configuracaoDisponibilidadeCarregamento.PermitirHorarioCarregamentoInferiorAoAtual))
                throw new ServicoException("Não é possível alterar a carga para uma hora menor que a hora corrente.", CodigoExcecao.HorarioCarregamentoInferiorAtual);
        }

        private void ValidarLimiteCarregamentoAtingido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dia)
        {
            if (_configuracaoDisponibilidadeCarregamento.PermitirHorarioCarregamentoComLimiteAtingido)
                return;

            if (cargaJanelaCarregamento.CargaBase.TipoDeCarga == null)
                return;

            if ((cargaJanelaCarregamento.CentroCarregamento.LimitesCarregamento?.Count ?? 0) == 0)
                return;

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento limiteCarregamento = (
                from o in cargaJanelaCarregamento.CentroCarregamento.LimitesCarregamento
                where o.Dia == DiaSemanaHelper.ObterDiaSemana(dia) && o.TipoCarga.Codigo == cargaJanelaCarregamento.CargaBase.TipoDeCarga.Codigo
                select o
            ).FirstOrDefault();

            if (limiteCarregamento == null)
                return;

            DateTime dataLimite = dia.Date.AddDays(-limiteCarregamento.DiasAntecedencia).Add(limiteCarregamento.HoraLimite);
            bool dataLimiteCarregamentoAtingida = DateTime.Now > dataLimite;

            if (!dataLimiteCarregamentoAtingida)
                return;

            throw new ServicoException($"Data limite ({dataLimite.ToString("dd/MM/yyyy HH:mm")}) para a janela de {dia.ToString("dd/MM/yyyy")} ultrapassada.", CodigoExcecao.HorarioLimiteCarregamentoAtingido);
        }

        private void ValidarToleranciaDataCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime data)
        {
            if (cargaJanelaCarregamento?.CentroCarregamento?.LimiteCarregamentos != LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                return;

            if (cargaJanelaCarregamento.HorarioEncaixado)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            DateTime dataComTolerancia = DateTime.Now.AddMinutes(ObterTempoTolerancia(cargaJanelaCarregamento.CentroCarregamento));

            if ((data <= dataComTolerancia) && !(configuracaoEmbarcador.PermitirAlterarCargaHorarioCarregamentoInferiorAtual || _configuracaoDisponibilidadeCarregamento.PermitirHorarioCarregamentoInferiorAoAtual))
                throw new ServicoException("Não é possível alterar a carga para uma hora menor que a tolerada.", CodigoExcecao.HorarioCarregamentoInferiorAoTolerado);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AlterarHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime novoHorarioInicio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool naoBloquearCapacidadeExcedida = false)
        {
            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && cargaJanelaCarregamento.Carga?.Empresa != null)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(cargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamento.Carga.Empresa.Codigo);

                if (cargaJanelaCarregamentoTransportador != null)
                {
                    ValidarNumeroAlteracoesTransportador(cargaJanelaCarregamentoTransportador, novoHorarioInicio);
                    cargaJanelaCarregamentoTransportador.NumeroAlteracoesHorarioRealizadas += 1;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                }
            }

            if (cargaJanelaCarregamento.Carga != null)
            {
                if ((cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Cancelada) || (cargaJanelaCarregamento.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ServicoException("Não é permitido alterar o horário do carregamento pois a carga está cancelada.");

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(cargaJanelaCarregamento.Carga, tipoServicoMultisoftware) && (cargaJanelaCarregamento.Carga.SituacaoCarga != SituacaoCarga.AgNFe) && _configuracaoEmbarcador.Pais != TipoPais.Exterior)
                    throw new ServicoException("A carga já está pronta para carregamento, não sendo possível realizar a alteração do horário.");
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            ValidarDataCarregamento(cargaJanelaCarregamento, novoHorarioInicio);

            cargaJanelaCarregamento.TempoCarregamento = new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador).ObterTempoCarregamento(cargaJanelaCarregamento, novoHorarioInicio.TimeOfDay);

            if (!cargaJanelaCarregamento.IsTempoCarregamentoValido())
                throw new ServicoException("Não existe uma configuração de tempo de carregamento criada para as configurações desta carga, por favor, crie a configuração do tempo de carregamento e tente novamente.");

            ValidarLimiteCarregamentoAtingido(cargaJanelaCarregamento, novoHorarioInicio);

            if (!IsPossuiCapacidadeCarregamentoDia(cargaJanelaCarregamento, novoHorarioInicio))
                throw new ServicoException("A capacidade de carregamento diária foi atingida.");

            DateTime? novoHorarioTermino = novoHorarioInicio.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
            Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento = ObterPeriodoCarregamento(cargaJanelaCarregamento, novoHorarioInicio, novoHorarioTermino.Value);

            if (cargaJanelaCarregamento.CentroCarregamento.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
            {
                novoHorarioInicio = novoHorarioInicio.Date.Add(periodoCarregamento.HoraInicio);
                novoHorarioTermino = ObterDataTerminoPorVagasOcupadasNaGrade(cargaJanelaCarregamento, periodoCarregamento, novoHorarioInicio);

                if (!novoHorarioTermino.HasValue)
                    throw new ServicoException("Não existe período de carregamento disponível para o horário informado.");

                ValidarDataCarregamento(cargaJanelaCarregamento, novoHorarioInicio);
            }

            bool adicionarPrioridade = cargaJanelaCarregamento.Excedente;
            DateTime horarioInicioAntigo = cargaJanelaCarregamento.InicioCarregamento;
            int codigoFilial = cargaJanelaCarregamento.CargaBase.Filial?.Codigo ?? 0;
            int codigoRota = cargaJanelaCarregamento.CargaBase.Rota?.Codigo ?? 0;
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repositorioExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades = repositorioExclusividadeCarregamento.BuscarExclusividadePorPeriodo(cargaJanelaCarregamento.CentroCarregamento.Codigo, novoHorarioInicio, DiaSemanaHelper.ObterDiaSemana(novoHorarioInicio));
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasConflitantesNoPeriodo = ObterCargasConflitantesNoPeriodo(cargaJanelaCarregamento, novoHorarioInicio, novoHorarioTermino.Value);

            if (!IsPossuiCapacidadeCarregamentoPeriodo(cargaJanelaCarregamento, novoHorarioInicio, periodoCarregamento, naoBloquearCapacidadeExcedida))
            {
                if (!naoBloquearCapacidadeExcedida)
                    throw new ServicoException(Localization.Resources.Cargas.Carga.CapacidadeCarregamentoPeriodoAtingida);//   "A capacidade de carregamento do período foi atingida.");
            }

            Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamento = ObterPrevisaoCarregamento(cargaJanelaCarregamento, novoHorarioInicio);
            bool restringeRota = false;

            if (previsaoCarregamento == null && codigoRota > 0)
            {
                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(_unitOfWork);
                restringeRota = repositorioPrevisaoCarregamento.VerificarSePossuiRestricaoPorCentroCarregamentoNaRota(cargaJanelaCarregamento.CentroCarregamento.Codigo, codigoRota, cargaJanelaCarregamento.CargaBase.ModeloVeicularCarga.Codigo);
            }

            if (previsaoCarregamento == null)
            {
                if (restringeRota)
                    throw new ServicoException("Não há previsão de carregamento configurada para o conjunto: modelo de veículo, rota e dia da semana.");
            }
            else if ((previsaoCarregamento.QuantidadeCargas + previsaoCarregamento.QuantidadeCargasExcedentes) <= repositorioCargaJanelaCarregamento.ContarCargasDiarias(cargaJanelaCarregamento.Codigo, codigoFilial, cargaJanelaCarregamento.CentroCarregamento.Codigo, codigoRota, previsaoCarregamento.ModelosVeiculos.Select(o => o.Codigo).ToArray(), novoHorarioInicio))
                throw new ServicoException("A capacidade de cargas diárias para este modelo de veículo e rota já atingiu o limite.");

            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro = ObterParadaCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento, novoHorarioInicio, novoHorarioTermino.Value);
            _configuracaoDisponibilidadeCarregamento.CodigoTipoOperacao = cargaJanelaCarregamento.CargaBase.TipoOperacao?.Codigo ?? 0;

            if (!cargaJanelaCarregamento.HorarioEncaixado && !IsPossuiCapacidadeCarregamentoSimultaneo(novoHorarioInicio, novoHorarioTermino.Value, periodoCarregamento, cargasConflitantesNoPeriodo, paradasCentro, exclusividades))
                throw new ServicoException("A capacidade de carregamento simultâneo para este período já atingiu o limite.");

            cargaJanelaCarregamento.InicioCarregamento = novoHorarioInicio;
            cargaJanelaCarregamento.TerminoCarregamento = novoHorarioTermino.Value;
            cargaJanelaCarregamento.Excedente = false;
            cargaJanelaCarregamento.DataCarregamentoProgramada = ReprogramarDatasComTolerancia(cargaJanelaCarregamento.CentroCarregamento, cargaJanelaCarregamento.DataCarregamentoProgramada, horarioInicioAntigo, novoHorarioInicio);
            cargaJanelaCarregamento.DataProximaSituacao = new PrazoSituacaoCarga(_unitOfWork).ObterDataProximaSituacao(cargaJanelaCarregamento);

            AtualizarDatasRelacionadas(cargaJanelaCarregamento, tipoServicoMultisoftware);
            NotificarAlteracaoHorario(cargaJanelaCarregamento, tipoServicoMultisoftware);

            if (adicionarPrioridade)
                new CargaJanelaCarregamentoPrioridade(_unitOfWork).AdicionarPrioridade(cargaJanelaCarregamento);

            repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
        }

        public void AtualizarDefinicaoHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Excedente || cargaJanelaCarregamento.CentroCarregamento == null)
                return;

            if (!IsPossuiCapacidadeCarregamentoDia(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento))
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                    throw new ServicoException("A capacidade de carregamento diária foi atingida");

                cargaJanelaCarregamento.Excedente = true;
            }

            Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo = ObterPeriodoCarregamento(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento, cargaJanelaCarregamento.InicioCarregamento);

            if ((periodo != null) && !IsPossuiCapacidadeCarregamentoPeriodo(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento, periodo))
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                    throw new ServicoException("A capacidade de carregamento do período foi atingida");

                cargaJanelaCarregamento.Excedente = true;
            }
        }

        public void DefinirHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            DateTime dataCarregamento = cargaJanelaCarregamento.CarregamentoReservado ? cargaJanelaCarregamento.DataCarregamentoProgramada : cargaJanelaCarregamento.InicioCarregamento;

            if (!IsPossuiDisponibilidadeCarregamentoDia(cargaJanelaCarregamento, dataCarregamento, cargaJanelaCarregamento.CargaBase.ModeloVeicularCarga, cargaJanelaCarregamento.CargaBase.Rota))
            {
                cargaJanelaCarregamento.Excedente = true;
                return;
            }

            if (IsCalcularHorarioCarregamento())
                CalcularHorarioCarregamento(cargaJanelaCarregamento); //Configuração para testes do GPA Fila
            else
                InformarHorarioCarregamento(cargaJanelaCarregamento);
        }

        public void DefinirHorarioCarregamentoAteLimiteTentativas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            bool encontrouHorario = false;
            DateTime dataInicioCarregamentoAnterior = cargaJanelaCarregamento.InicioCarregamento;

            for (int i = 0; i < _configuracaoDisponibilidadeCarregamento.DiasLimiteParaDefinicaoHorarioCarregamento; i++)
            {
                DateTime? proximaDataCarregamento = ObterProximaDataComPeriodoCarregamento(cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.InicioCarregamento.Date);

                if (!proximaDataCarregamento.HasValue)
                    break;

                cargaJanelaCarregamento.InicioCarregamento = proximaDataCarregamento.Value;
                cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);

                try
                {
                    _definicaoHorarioCarregamentoAteLimiteTentativas = true;

                    DefinirHorarioCarregamento(cargaJanelaCarregamento);

                    if (!cargaJanelaCarregamento.Excedente)
                    {
                        encontrouHorario = true;
                        break;
                    }
                }
                catch (ServicoException excecao)
                {
                    if (
                        (excecao.ErrorCode != CodigoExcecao.HorarioCarregamentoIndisponivel) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioCarregamentoInferiorAtual) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioCarregamentoInferiorAoTolerado) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioLimiteCarregamentoAtingido)
                    )
                        throw;
                }
                finally
                {
                    _definicaoHorarioCarregamentoAteLimiteTentativas = false;
                }
            }

            if (!encontrouHorario)
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                {
                    DateTime dataInicialDefinicaoHorario = dataInicioCarregamentoAnterior.Date;
                    DateTime dataUltimaTentativaDefinicaoHorario = cargaJanelaCarregamento.InicioCarregamento.Date;

                    if (dataUltimaTentativaDefinicaoHorario > dataInicialDefinicaoHorario)
                        throw new ServicoException($"Não foi possível encontrar um horário no período de {dataInicialDefinicaoHorario.ToString("dd/MM/yyyy")} a {dataUltimaTentativaDefinicaoHorario.ToString("dd/MM/yyyy")} do centro de carregamento {cargaJanelaCarregamento.CentroCarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioCarregamentoIndisponivel);

                    throw new ServicoException($"Não foi possível encontrar um horário no dia {dataInicialDefinicaoHorario.ToString("dd/MM/yyyy")} do centro de carregamento {cargaJanelaCarregamento.CentroCarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioCarregamentoIndisponivel);
                }

                cargaJanelaCarregamento.InicioCarregamento = dataInicioCarregamentoAnterior;
                cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
                cargaJanelaCarregamento.Excedente = true;
            }
        }

        public void DefinirHorarioCarregamentoPorPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo)
        {
            DefinirHorarioCarregamentoPorPeriodo(cargaJanelaCarregamento, periodo, cargaJanelaCarregamento.InicioCarregamento, tipoServicoMultisoftware: null);
        }

        public void DefinirHorarioCarregamentoPorPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, DateTime dataCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            DateTime diaCarregamento = dataCarregamento.Date;
            DateTime dataInicioPeriodoCarregamento = diaCarregamento.Add(periodo.HoraInicio);

            ValidarDataCarregamento(cargaJanelaCarregamento, dataInicioPeriodoCarregamento);

            DateTime inicioCarregamentoAnterior = cargaJanelaCarregamento.InicioCarregamento;
            bool adicionarPrioridade = cargaJanelaCarregamento.Excedente;

            cargaJanelaCarregamento.TempoCarregamento = new CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador).ObterTempoCarregamento(cargaJanelaCarregamento, dataInicioPeriodoCarregamento.TimeOfDay);

            if (!cargaJanelaCarregamento.IsTempoCarregamentoValido())
                throw new ServicoException("Não existe uma configuração de tempo de carregamento criada para as configurações desta carga, por favor, crie a configuração do tempo de carregamento e tente novamente.");

            DateTime dataTerminoCarregamento = dataInicioPeriodoCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
            DateTime dataTerminoPeriodoCarregamento = diaCarregamento.Add(periodo.HoraTermino).AddMinutes(periodo.ToleranciaExcessoTempo);
            IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(cargaJanelaCarregamento, dataInicioPeriodoCarregamento, dataTerminoPeriodoCarregamento);

            ValidarLimiteCarregamentoAtingido(cargaJanelaCarregamento, dataInicioPeriodoCarregamento);

            if (!IsPossuiCapacidadeCarregamentoDia(cargaJanelaCarregamento, dataInicioPeriodoCarregamento))
                throw new ServicoException("A capacidade de carregamento diária foi atingida");

            if (!IsPossuiCapacidadeCarregamentoPeriodo(cargaJanelaCarregamento, dataInicioPeriodoCarregamento, periodo))
                throw new ServicoException("A capacidade de carregamento do período foi atingida");

            if (EncontrarProximoHorario(cargaJanelaCarregamento, cargasPeriodo, periodo, dataInicioPeriodoCarregamento, dataTerminoCarregamento, paradasCentro: null, exclusividades: null))
            {
                cargaJanelaCarregamento.DataCarregamentoProgramada = ReprogramarDatasComTolerancia(cargaJanelaCarregamento.CentroCarregamento, cargaJanelaCarregamento.DataCarregamentoProgramada, inicioCarregamentoAnterior, cargaJanelaCarregamento.InicioCarregamento);
                cargaJanelaCarregamento.DataProximaSituacao = new PrazoSituacaoCarga(_unitOfWork).ObterDataProximaSituacao(cargaJanelaCarregamento);

                AtualizarDatasRelacionadas(cargaJanelaCarregamento, tipoServicoMultisoftware.HasValue ? tipoServicoMultisoftware.Value : AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                if (tipoServicoMultisoftware.HasValue)
                    NotificarAlteracaoHorario(cargaJanelaCarregamento, tipoServicoMultisoftware.Value);

                if (adicionarPrioridade)
                    new Logistica.CargaJanelaCarregamentoPrioridade(_unitOfWork).AdicionarPrioridade(cargaJanelaCarregamento);

                return;
            }

            throw new ServicoException($"Não foi possível encontrar um horário de carregamento para as configurações desta {cargaJanelaCarregamento.CargaBase.DescricaoEntidade}");
        }

        public bool IsCalcularHorarioCarregamento()
        {
            return Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().CalcularHorarioDoCarregamento.HasValue && Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().CalcularHorarioDoCarregamento.Value == true;
        }

        public bool IsDataRetroativaDentroDaToleranciaValida(DateTime data, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            DateTime dataLimite = DateTime.Now.AddMinutes(-centroCarregamento?.ToleranciaDataRetroativa ?? 0);
            dataLimite = dataLimite.AddSeconds(-dataLimite.Second);

            if (data <= dataLimite)
                throw new ServicoException("Não é possível alterar a carga para uma hora menor que a hora corrente.");

            return true;
        }

        public bool IsPossuiDisponibilidadeCarregamentoDia(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dia, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular, Dominio.Entidades.RotaFrete rotaFrete)
        {
            if (modeloVeicular == null)
            {
                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente && cargaJanelaCarregamento.Carga.Redespacho == null)
                    throw new ServicoException($"Não foi possível encontrar o modelo veicular desta {cargaJanelaCarregamento.CargaBase.DescricaoEntidade}");

                return false;
            }

            if (!IsPossuiRestricaoRota(cargaJanelaCarregamento, rotaFrete))
                return true;

            Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamentoDia = ObterPrevisaoCarregamento(cargaJanelaCarregamento, rotaFrete, dia, modeloVeicular);

            if (previsaoCarregamentoDia != null)
            {
                int numeroCargas = ObterNumeroCargas(cargaJanelaCarregamento, rotaFrete, dia, previsaoCarregamentoDia, _configuracaoDisponibilidadeCarregamento.ValidarSomenteEmResevas);

                if (previsaoCarregamentoDia.QuantidadeCargas > numeroCargas)
                {
                    int numeroCargasReservada = ObterNumeroCargasReservadaPorDia(cargaJanelaCarregamento, previsaoCarregamentoDia, dia);

                    if (previsaoCarregamentoDia.QuantidadeCargas > (numeroCargas + numeroCargasReservada))
                        return true;
                }

                if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                    throw new ServicoException($"Não existe disponibilidade prevista de carregamento para as configurações desta {cargaJanelaCarregamento.CargaBase.DescricaoEntidade}", CodigoExcecao.PrevisaoCarregamentoIndisponivel);
            }
            else if (_configuracaoDisponibilidadeCarregamento.BloquearJanelaCarregamentoExcedente)
                throw new ServicoException($"Não existe previsão de carregamento para as configurações desta {cargaJanelaCarregamento.CargaBase.DescricaoEntidade}", CodigoExcecao.PrevisaoCarregamentoIndisponivel);

            return false;
        }

        public bool IsPossuiRestricaoRota(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.RotaFrete rotaFrete)
        {
            if (rotaFrete == null)
                return false;

            Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(_unitOfWork);

            return repositorioPrevisaoCarregamento.VerificarSePossuiRestricaoPorCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento.Codigo);
        }

        public List<GradeCarregamento> ObterEncaixesCarregamentosDisponiveis(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoCentroCarregamentoOuExcecao(centroCarregamento, dia);
            Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo = periodosCarregamento.Where(o => o.HoraInicio == dia.TimeOfDay).FirstOrDefault();

            if (periodo == null)
                return new List<GradeCarregamento>();

            DateTime dataInicio = dia.Date.Add(periodo.HoraInicio);
            DateTime dataTermino = dia.Date.Add(periodo.HoraTermino);

            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro = ObterParadaCentroCarregamento(centroCarregamento, dataInicio, dataTermino);
            IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(null, centroCarregamento, dataInicio, dataTermino);
            List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades = repExclusividadeCarregamento.BuscarExclusividadePorPeriodo(centroCarregamento.Codigo, dia, DiaSemanaHelper.ObterDiaSemana(dia));

            Dictionary<int, GradeCarregamento> tabelaCarregamento = new Dictionary<int, GradeCarregamento>();

            try
            {
                tabelaCarregamento = PreencheGradeCarregamento(dia.Date, periodo, cargasPeriodo, paradasCentro, exclusividades, preencherGradesTotalmenteExclusivas: false);
            }
            catch (ServicoException)
            {
                // Periodo possui exclusividade total
                return new List<GradeCarregamento>();
            }

            List<int> keys = tabelaCarregamento.Keys.ToList();

            return (from codigo in keys
                    where
                        codigo != _tabelaCodigoOperacaoLivre
                        && tabelaCarregamento[codigo].Quantidade > 0
                    select tabelaCarregamento[codigo]
                    ).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento ObterExcecaoCentroCarregamentoPorData(int codigoCentroCarregamento, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorioExcecaoCapacidadeCarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoDia = repositorioExcecaoCapacidadeCarregamento.BuscarPorCentroCarregamentoEDia(codigoCentroCarregamento, dia);

            if (excecaoDia == null)
                excecaoDia = repositorioExcecaoCapacidadeCarregamento.BuscarPorCentroCarregamentoEPeriodo(codigoCentroCarregamento, dia);

            return excecaoDia;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> ObterIntervalosCarregamentosDisponiveis(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia)
        {
            DateTime dataInicial = dia.Date;
            DateTime dataFinal = dia.Date.Add(new TimeSpan(23, 59, 0));

            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> periodosCarregamento = ObterIntervalosPorPeriodos(centroCarregamento, dia);
            IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(null, centroCarregamento, dataInicial, dataFinal);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> intervalosDisponiveis = new List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento>();
            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradaCentros = ObterParadaCentroCarregamento(centroCarregamento, dia);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento> listaFiltrada = new List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento>();

            foreach (Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento periodo in periodosCarregamento)
            {
                DateTime dataInicio = dia.Date.Add(periodo.HoraInicio);
                DateTime dataTermino = dia.Date.Add(periodo.HoraTermino);

                List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario = (
                    from carga in cargasPeriodo
                    where (
                        (carga.DataInicio >= dataInicio && carga.DataInicio < dataTermino) ||
                        (carga.DataFim > dataInicio && carga.DataFim <= dataTermino) ||
                        (carga.DataInicio <= dataInicio && carga.DataFim >= dataTermino)
                    )
                    select carga
                ).ToList();

                if (!IsPossuiCapacidadeCarregamentoSimultaneoPeriodo(dataInicio, dataTermino, periodo, cargasPeriodoHorario, paradaCentros))
                    continue;

                listaFiltrada.Add(new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.PeriodoCarregamento
                {
                    Index = periodo.Index,
                    Periodo = periodo.Periodo,
                    HoraInicio = periodo.HoraInicio,
                    HoraTermino = periodo.HoraTermino,
                });
            }

            return listaFiltrada;
        }

        public List<VisualizacaoGradeTipoOperacao> ObterListaDisponbilidadePorTipoOperacaoVisualizacaoGrade(DateTime dataBase, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo, List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasDoDia, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> tipoOperacoes, List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro, List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades, bool permitirRetornarTipoOperacaoLivre)
        {
            List<VisualizacaoGradeTipoOperacao> disponibilidade = new List<VisualizacaoGradeTipoOperacao>();
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = cargasDoDia
                .Where(carga =>
                    (periodo.HoraInicio <= carga.DataInicio.TimeOfDay && periodo.HoraTermino >= carga.DataInicio.TimeOfDay) ||
                    (periodo.HoraInicio <= carga.DataFim.TimeOfDay && periodo.HoraTermino >= carga.DataFim.TimeOfDay) ||
                    (periodo.HoraInicio >= carga.DataInicio.TimeOfDay && periodo.HoraTermino <= carga.DataFim.TimeOfDay)
                 ).ToList();

            Dictionary<int, GradeCarregamento> tabelaCarregamento = PreencheGradeCarregamento(dataBase, periodo, cargasPeriodo, paradasCentro, exclusividades, preencherGradesTotalmenteExclusivas: true);
            List<int> tiposOperacoesTabela = tabelaCarregamento.Keys.ToList();

            foreach (int tipoOperacaoTabela in tiposOperacoesTabela)
            {
                GradeCarregamento gradeCarregamento = tabelaCarregamento[tipoOperacaoTabela];
                int totalDisponivel = gradeCarregamento.Disponiveis - gradeCarregamento.Ocupadas;

                if (totalDisponivel <= 0)
                    continue;

                if (tipoOperacaoTabela == _tabelaCodigoOperacaoLivre)
                {
                    if (!permitirRetornarTipoOperacaoLivre)
                        continue;
                }
                else if (!tipoOperacoes.Any(o => o.TipoOperacao.Codigo == gradeCarregamento.CodigoTipoOperacao))
                    continue;

                disponibilidade.Add(new VisualizacaoGradeTipoOperacao()
                {
                    Periodo = periodo.Codigo,
                    TipoOperacao = gradeCarregamento.CodigoTipoOperacao,
                    Descricao = (tipoOperacaoTabela == _tabelaCodigoOperacaoLivre ? Localization.Resources.Cargas.Carga.Livre : gradeCarregamento.TipoOperacao),
                    Total = totalDisponivel,
                });
            }

            return disponibilidade;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> ObterParadaCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dataInicio, DateTime dataFinal)
        {
            Repositorio.Embarcador.Logistica.MotivoParadaCentro repositorioMotivoParadaCentro = new Repositorio.Embarcador.Logistica.MotivoParadaCentro(_unitOfWork);
            return repositorioMotivoParadaCentro.BuscarPorPeriodoECentroCarregamento(centroCarregamento.Codigo, dataInicio, dataFinal);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> ObterParadaCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime data)
        {
            return ObterParadaCentroCarregamento(centroCarregamento, data.Date, data.Date.Add(new TimeSpan(23, 59, 0)));
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> ObterPeriodosCarregamentosDisponiveis(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.ExclusividadeCarregamento repExclusividadeCarregamento = new Repositorio.Embarcador.Logistica.ExclusividadeCarregamento(_unitOfWork);

            DateTime dataInicial = dia.Date;
            DateTime dataFinal = dia.Date.Add(new TimeSpan(23, 59, 0));

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = ObterPeriodosCarregamentoDentroDaTolerancia(centroCarregamento, dia);
            List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> paradasCentro = ObterParadaCentroCarregamento(centroCarregamento, dia);
            IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(null, centroCarregamento, dataInicial, dataFinal);
            List<Dominio.Entidades.Embarcador.Logistica.ExclusividadeCarregamento> exclusividades = repExclusividadeCarregamento.BuscarExclusividadePorPeriodo(centroCarregamento.Codigo, dia, DiaSemanaHelper.ObterDiaSemana(dia));

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> listaFiltrada = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();
            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo in periodosCarregamento)
            {
                DateTime dataInicio = dia.Date.Add(periodo.HoraInicio);
                DateTime dataTermino = dia.Date.Add(periodo.HoraTermino);

                List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario = (
                    from carga in cargasPeriodo
                    where (
                        (carga.DataInicio >= dataInicio && carga.DataInicio < dataTermino) ||
                        (carga.DataFim > dataInicio && carga.DataFim <= dataTermino) ||
                        (carga.DataInicio <= dataInicio && carga.DataFim >= dataTermino)
                    )
                    select carga
                ).ToList();

                if (!IsPossuiCapacidadeCarregamentoSimultaneo(dataInicio, dataTermino, periodo, cargasPeriodoHorario, paradasCentro, exclusividades))
                    continue;

                listaFiltrada.Add(periodo);
            }

            return listaFiltrada;
        }

        public void ValidarPermissaoAlterarHorarioCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Carga == null)
                return;

            Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

            if (servicoCarga.RecebeuNumeroCargaEmbarcador(cargaJanelaCarregamento.Carga, _unitOfWork))
                throw new ServicoException("A carga já recebeu o número de carga do Embarcador e não permite essa alteração.");

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios integracaoKlios = new Repositorio.Embarcador.Configuracoes.IntegracaoKlios(_unitOfWork).Buscar();

            if(new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork).ExistePorTipo(TipoIntegracao.Klios) && (integracaoKlios?.PossuiIntegracao ?? false))
            {
                if ((cargaJanelaCarregamento.Carga?.Filial?.GerarIntegracaoKlios ?? false) && (cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoIntegracao?.GerarIntegracaoKlios ?? false)
                    && !servicoCarga.IsDadosTransporteinformados(cargaJanelaCarregamento.Carga, _unitOfWork))
                    throw new ServicoException($"Obrigatório informar os Dados de Transporte para essa carga.");

                if (cargaJanelaCarregamento.RecomendacaoGR != null && cargaJanelaCarregamento.RecomendacaoGR != RecomendacaoGR.Recomendado)
                    throw new ServicoException($"A carga possui pendência com a GR, Motivo: {cargaJanelaCarregamento.RecomendacaoGR?.ObterDescricao() ?? ""}.");
            }
        }

        #endregion Métodos Públicos
    }
}
