using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaDescarregamentoDisponibilidade
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento _configuracaoDisponibilidadeDescarregamento;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaJanelaDescarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, configuracaoDisponibilidadeDescarregamento: null) { }

        public CargaJanelaDescarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, configuracaoDisponibilidadeDescarregamento: null) { }

        public CargaJanelaDescarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento configuracaoDisponibilidadeDescarregamento) : this(unitOfWork, configuracaoEmbarcador: null, configuracaoDisponibilidadeDescarregamento: configuracaoDisponibilidadeDescarregamento) { }

        public CargaJanelaDescarregamentoDisponibilidade(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento configuracaoDisponibilidadeDescarregamento)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
            _configuracaoDisponibilidadeDescarregamento = configuracaoDisponibilidadeDescarregamento ?? new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento();
        }

        #endregion

        #region Métodos Privados

        private void AtualizarDatasRelacionadas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = repositorioAgendamentoEntrega.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

            cargaJanelaDescarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
            repositorioCarga.Atualizar(cargaJanelaDescarregamento.Carga);

            if (agendamentoColeta != null)
            {
                agendamentoColeta.DataEntrega = cargaJanelaDescarregamento.InicioDescarregamento;
                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
            }

            if (agendamentoEntrega != null)
            {
                agendamentoEntrega.DataAgendamento = cargaJanelaDescarregamento.InicioDescarregamento;
                repositorioAgendamentoEntrega.Atualizar(agendamentoEntrega);
            }
        }

        private bool EncontrarProximoHorario(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento, DateTime dataInicio, DateTime dataTermino)
        {
            if (IsLimiteDescarregamentoAtingido(cargaJanelaDescarregamento, dataInicio))
                return false;

            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodoHorario = (
                from o in cargasPeriodo
                where (o.DataInicio >= dataInicio && o.DataInicio < dataTermino) || (o.DataFim > dataInicio && o.DataFim <= dataTermino)
                select o
            ).ToList();

            if (cargasPeriodoHorario.Count < periodoDescarregamento.CapacidadeDescarregamentoSimultaneoTotal)
            {
                cargaJanelaDescarregamento.InicioDescarregamento = dataInicio;
                cargaJanelaDescarregamento.TerminoDescarregamento = dataTermino;
                cargaJanelaDescarregamento.Excedente = false;
                cargaJanelaDescarregamento.PeriodoDescarregamentoExclusivo = IsPeriodoDescarregamentoExclusivo(periodoDescarregamento);

                return true;
            }

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo primeiraCargaAFinalizarCarregamento = (from cargaPeriodoHorario in cargasPeriodoHorario select cargaPeriodoHorario).OrderBy(obj => obj.DataFim).FirstOrDefault();

            if (!_configuracaoDisponibilidadeDescarregamento.NaoPermitirBuscarOutroPeriodo && primeiraCargaAFinalizarCarregamento != null && primeiraCargaAFinalizarCarregamento.DataInicio < primeiraCargaAFinalizarCarregamento.DataFim)
            {
                DateTime dataTerminoPeriodo = dataInicio.Date.Add(periodoDescarregamento.HoraTermino).Add(TimeSpan.FromMinutes(periodoDescarregamento.ToleranciaExcessoTempo));

                dataInicio = (from cargaPeriodoHorario in cargasPeriodoHorario select cargaPeriodoHorario.DataFim).Min();
                dataTermino = dataInicio.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);

                if (dataTermino <= dataTerminoPeriodo)
                    return EncontrarProximoHorario(cargaJanelaDescarregamento, cargasPeriodo, periodoDescarregamento, dataInicio, dataTermino);
            }

            return false;
        }

        private bool IsBloquearJanelaDescarregamentoExcedente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            return (cargaJanelaDescarregamento.CentroDescarregamento.BloquearJanelaDescarregamentoExcedente || cargaJanelaDescarregamento.CentroDescarregamento.PermitirBuscarAteFimDaJanela);
        }

        private bool IsDataDescarregamentoRetroativaValida(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime data)
        {
            try
            {
                ValidarDataDescarregamentoRetroativa(data);
                return true;
            }
            catch (ServicoException)
            {
                if (IsBloquearJanelaDescarregamentoExcedente(cargaJanelaDescarregamento))
                    throw;

                return false;
            }
        }

        private bool IsLimiteDescarregamentoAtingido(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dia)
        {
            if (_configuracaoDisponibilidadeDescarregamento.PermitirHorarioDescarregamentoComLimiteAtingido)
                return false;

            if (cargaJanelaDescarregamento.Carga.TipoDeCarga == null)
                return false;

            if ((cargaJanelaDescarregamento.CentroDescarregamento.LimitesDescarregamento?.Count ?? 0) == 0)
                return false;

            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento limiteDescarregamento = (
                from o in cargaJanelaDescarregamento.CentroDescarregamento.LimitesDescarregamento
                where o.Dia == DiaSemanaHelper.ObterDiaSemana(dia) && o.TipoCarga.Codigo == cargaJanelaDescarregamento.Carga.TipoDeCarga.Codigo
                select o
            ).FirstOrDefault();

            if (limiteDescarregamento == null)
                return false;

            DateTime dataLimite = dia.AddHours(-limiteDescarregamento.HorasAntecedencia);
            bool dataLimiteDescarregamentoAtingida = DateTime.Now > dataLimite;

            if (IsBloquearJanelaDescarregamentoExcedente(cargaJanelaDescarregamento) && dataLimiteDescarregamentoAtingida)
                throw new ServicoException($"Data limite ({dataLimite.ToString("dd/MM/yyyy HH:mm")}) para a janela de {dia.ToString("dd/MM/yyyy")} ultrapassada", errorCode: CodigoExcecao.HorarioLimiteDescarregamentoAtingido);

            return dataLimiteDescarregamentoAtingida;
        }

        private bool IsPeriodoDescarregamentoExclusivo(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            if (!configuracaoJanelaCarregamento.UtilizarPeriodoDescarregamentoExclusivo)
                return false;

            return (periodoDescarregamento.GruposPessoas?.Count > 0) || (periodoDescarregamento.Remetentes?.Count > 0);
        }

        private bool IsPossuiCapacidadeDescarregamentoPorDia(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dia)
        {
            int capacidadeDescarregamentoDia = ObterCapacidadeDescarregamentoDia(cargaJanelaDescarregamento.CentroDescarregamento, dia);

            if (capacidadeDescarregamentoDia == 0)
                return true;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(_unitOfWork);
            decimal pesoTotalDescarregamentoDia = repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoDia(cargaJanelaDescarregamento.Carga.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dia, cargaJanelaDescarregamento.CentroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido);
            decimal pesoDescarregamento = ObterPesoDescarregamento(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);
            decimal capacidadeDescarregamentoDisponivelDia = (capacidadeDescarregamentoDia - pesoTotalDescarregamentoDia);
            decimal toleranciaPesoDescarregamentoUtilizada = 0.0m;

            if (pesoDescarregamento > capacidadeDescarregamentoDisponivelDia)
            {
                decimal toleranciaPesoDescarregamento = pesoDescarregamento * cargaJanelaDescarregamento.CentroDescarregamento.PercentualToleranciaPesoDescarregamento / 100;

                if (pesoDescarregamento > (capacidadeDescarregamentoDisponivelDia + toleranciaPesoDescarregamento))
                    return false;

                toleranciaPesoDescarregamentoUtilizada = pesoDescarregamento - capacidadeDescarregamentoDisponivelDia;
            }

            if (toleranciaPesoDescarregamentoUtilizada > 0.0m)
            {
                Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamentoAutomaticaPorDia(cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dia);

                if (capacidadeDescarregamentoAdicional == null)
                {
                    capacidadeDescarregamentoAdicional = new Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional()
                    {
                        CapacidadeDescarregamento = (int)Math.Ceiling(toleranciaPesoDescarregamentoUtilizada),
                        CentroDescarregamento = cargaJanelaDescarregamento.CentroDescarregamento,
                        Data = dia.Date,
                        Observacao = "Tolerância de descarregamento dinâmica"
                    };

                    repositorioCapacidadeDescarregamentoAdicional.Inserir(capacidadeDescarregamentoAdicional);
                }
                else
                {
                    capacidadeDescarregamentoAdicional.CapacidadeDescarregamento += (int)Math.Ceiling(toleranciaPesoDescarregamentoUtilizada);

                    repositorioCapacidadeDescarregamentoAdicional.Atualizar(capacidadeDescarregamentoAdicional);
                }
            }

            return true;
        }

        private bool IsPossuiCapacidadeDescarregamentoPorPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dia, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento)
        {
            int capacidadeDescarregamentoPeriodo = ObterCapacidadeDescarregamentoPeriodo(cargaJanelaDescarregamento.CentroDescarregamento, dia, periodoDescarregamento);

            if (capacidadeDescarregamentoPeriodo == 0)
                return true;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(_unitOfWork);
            List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Codigo).ToList();
            decimal pesoTotalDescarregamentoPeriodo = repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoPeriodo(cargaJanelaDescarregamento.Carga.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dia, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, cargaJanelaDescarregamento.CentroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido, codigosCanaisVenda);
            decimal pesoDescarregamento = ObterPesoDescarregamento(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);
            decimal capacidadeDescarregamentoDisponivelPeriodo = (capacidadeDescarregamentoPeriodo - pesoTotalDescarregamentoPeriodo);
            decimal toleranciaPesoDescarregamentoUtilizada = 0.0m;

            if (pesoDescarregamento > capacidadeDescarregamentoDisponivelPeriodo)
            {
                decimal toleranciaPesoDescarregamento = pesoDescarregamento * cargaJanelaDescarregamento.CentroDescarregamento.PercentualToleranciaPesoDescarregamento / 100;

                if (pesoDescarregamento > (capacidadeDescarregamentoDisponivelPeriodo + toleranciaPesoDescarregamento))
                    return false;

                toleranciaPesoDescarregamentoUtilizada = pesoDescarregamento - capacidadeDescarregamentoDisponivelPeriodo;
            }

            if (toleranciaPesoDescarregamentoUtilizada > 0.0m)
            {
                Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamentoAutomaticaPorPeriodo(cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dia, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, codigosCanaisVenda);

                if (capacidadeDescarregamentoAdicional == null)
                {
                    capacidadeDescarregamentoAdicional = new Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional()
                    {
                        CapacidadeDescarregamento = (int)Math.Ceiling(toleranciaPesoDescarregamentoUtilizada),
                        CentroDescarregamento = cargaJanelaDescarregamento.CentroDescarregamento,
                        Data = dia.Date,
                        Observacao = "Tolerância de descarregamento dinâmica",
                        PeriodoInicio = dia.Date.Add(periodoDescarregamento.HoraInicio),
                        PeriodoTermino = dia.Date.Add(periodoDescarregamento.HoraTermino),
                        CanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda).ToList()
                    };

                    repositorioCapacidadeDescarregamentoAdicional.Inserir(capacidadeDescarregamentoAdicional);
                }
                else
                {
                    capacidadeDescarregamentoAdicional.CapacidadeDescarregamento += (int)Math.Ceiling(toleranciaPesoDescarregamentoUtilizada);

                    repositorioCapacidadeDescarregamentoAdicional.Atualizar(capacidadeDescarregamentoAdicional);
                }
            }

            return true;
        }

        private bool IsPossuiLimiteRaizCnpjNoDia(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, DateTime dataProgramada, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            if ((agendamentoColeta == null) || (agendamentoColeta.Remetente == null) || (agendamentoColeta.Remetente.GrupoPessoas == null))
                return true;

            Repositorio.Embarcador.Logistica.LimiteAgendamento repositorioLimiteAgendamento = new Repositorio.Embarcador.Logistica.LimiteAgendamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento limiteAgendamento = repositorioLimiteAgendamento.BuscarPorGrupoPessoaCentroDescarregamento(agendamentoColeta.Remetente.GrupoPessoas.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Codigo);
            int quantidadeJanelasAtuais = repositorioCargaJanelaDescarregamento.BuscarQuantidadePorDiaRaizCnpj(dataProgramada, agendamentoColeta.Remetente.GrupoPessoas.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Codigo, cargaJanelaDescarregamento.Carga.Codigo);

            if (limiteAgendamento != null)
                return !(quantidadeJanelasAtuais >= limiteAgendamento.Limite);

            if (cargaJanelaDescarregamento.CentroDescarregamento.LimitePadrao > 0)
                return !(quantidadeJanelasAtuais >= cargaJanelaDescarregamento.CentroDescarregamento.LimitePadrao);

            return true;
        }

        private bool IsPossuiCapacidadeSKUDescarregamentoPorDia(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento)
        {
            int quantidadeSKU = ObterSku(carga, centroDescarregamento.Destinatario);

            return IsQuantidadeSkuDentroFaixaPorPeriodo(quantidadeSKU, periodoDescarregamento);
        }

        private bool IsQuantidadeSkuDentroFaixaPorPeriodo(int quantidadeSKU, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento)
        {
            if (periodoDescarregamento.SkuDe.HasValue && periodoDescarregamento.SkuDe.Value > quantidadeSKU)
                return false;

            if (periodoDescarregamento.SkuAte.HasValue && periodoDescarregamento.SkuAte.Value < quantidadeSKU)
                return false;

            return true;
        }

        private int ObterCapacidadeDescarregamentoDia(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime dia)
        {
            if (!centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso || (centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso != TipoCapacidadeDescarregamentoPorPeso.DiaSemana))
                return 0;

            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dia);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoDia = (from o in centroDescarregamento.ExcecoesCapacidadeDescarregamento where o.DataInicial <= dia.Date && o.DataFinal >= dia.Date select o).FirstOrDefault();
            Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(_unitOfWork);
            int capacidadeDescarregamento = excecaoDia?.CapacidadeDescarregamento ?? centroDescarregamento.ObterCapacidadeDescarregamento(diaSemana);
            int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamento(centroDescarregamento.Codigo, dia);

            return capacidadeDescarregamento + capacidadeDescarregamentoAdicional;
        }

        private int ObterCapacidadeDescarregamentoPeriodo(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime dia, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento)
        {
            if (!centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso || (centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso != TipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento))
                return 0;

            Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(_unitOfWork);
            List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Codigo).ToList();
            int capacidadeDescarregamento = periodoDescarregamento.CapacidadeDescarregamento;
            int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamentoPorPeriodo(centroDescarregamento.Codigo, dia, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, codigosCanaisVenda);

            return capacidadeDescarregamento + capacidadeDescarregamentoAdicional;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> ObterCargasPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento, DateTime dataInicial, DateTime dataLimite, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Codigo).ToList();
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = repositorioCargaJanelaDescarregamento.BuscarCargaPeriodoPorIncidenciaDeHorario(cargaJanelaDescarregamento.Carga.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dataInicial, dataLimite, codigosCanaisVenda);

            if (periodoDescarregamento.TiposDeCarga.Count > 0)
                cargasPeriodo = cargasPeriodo.Where(carga => !carga.TipoCarga.HasValue || periodoDescarregamento.TiposDeCarga.Any(tipoCarga => tipoCarga.TipoDeCarga.Codigo == carga.TipoCarga)).ToList();

            cargasPeriodo = cargasPeriodo.Where(carga => IsQuantidadeSkuDentroFaixaPorPeriodo(carga.QuantidadeSku ?? 0, periodoDescarregamento)).ToList();

            if (agendamentoColeta != null)
            {
                if (periodoDescarregamento.Remetentes?.Count > 0)
                    cargasPeriodo = cargasPeriodo.Where(carga => !carga.CPFCNPJRemetente.HasValue || periodoDescarregamento.Remetentes.Any(remetente => remetente.Remetente.CPF_CNPJ == carga.CPFCNPJRemetente)).ToList();

                if (periodoDescarregamento.GruposPessoas?.Count > 0)
                    cargasPeriodo = cargasPeriodo.Where(carga => !carga.CodigoGrupoPessoaRemetente.HasValue || periodoDescarregamento.GruposPessoas.Any(grupoPessoa => grupoPessoa.GrupoPessoas.Codigo == carga.CodigoGrupoPessoaRemetente)).ToList();

                if (periodoDescarregamento.GruposProdutos?.Count > 0)
                    cargasPeriodo = cargasPeriodo.Where(carga => carga.CodigoGrupoProdutoDominante == 0 || periodoDescarregamento.GruposProdutos.Any(grupoProduto => grupoProduto.GrupoProduto.Codigo == carga.CodigoGrupoProdutoDominante)).ToList();
            }

            return cargasPeriodo;
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

        private DateTime ObterDataInicioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            DateTime dataInicioDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento;

            if (!configuracaoJanelaCarregamento.SugerirDataEntregaAgendamentoColeta && (dataInicioDescarregamento.Date == DateTime.Now.Date))
                dataInicioDescarregamento = DateTime.Now.AddMinutes(2);

            if (dataInicioDescarregamento.TimeOfDay <= periodoDescarregamento.HoraInicio)
                return dataInicioDescarregamento.Date.Add(periodoDescarregamento.HoraInicio);

            return dataInicioDescarregamento;
        }

        private string ObterDescritivoFaixaVolume(int? volumeDe, int? volumeAte)
        {
            List<string> descritivoFaixa = new List<string>();

            if (volumeDe.HasValue)
                descritivoFaixa.Add("de " + volumeDe.ToString());

            if (volumeAte.HasValue)
                descritivoFaixa.Add("até " + volumeAte.ToString());

            return string.Join(" ", descritivoFaixa);
        }

        private List<(DateTime Data, List<string> Pedidos)> ObterListaDatasBloqueados(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            List<(DateTime Data, List<string> Pedidos)> datasBloqueadas = new List<(DateTime Data, List<string> Pedidos)>();

            if (agendamentoColeta == null || agendamentoColeta.Remetente == null)
                return datasBloqueadas;

            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoPedidos = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedidos = repositorioAgendamentoPedidos.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaCategorias = agendamentoColetaPedidos.Where(obj => obj.Pedido.ProdutoPrincipal != null).Select(obj => obj.Pedido.ProdutoPrincipal).Distinct().ToList();

            if (listaCategorias.Count == 0)
                return datasBloqueadas;

            Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria repositorioDatasPreferenciaisFornecedorCategoria = new Repositorio.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> listaFornecedorCategoria = new List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria>();
            List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> listaDatas = repositorioDatasPreferenciaisFornecedorCategoria.BuscarPorCPFCategorias(agendamentoColeta.Remetente.CPF_CNPJ, listaCategorias.Select(obj => obj.Codigo).ToList());

            if (listaDatas.Count == 0)
                return datasBloqueadas;

            DateTime? dataLimite = repositorioAgendamentoPedidos.BuscarMenorDataPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

            if (!dataLimite.HasValue)
                return datasBloqueadas;

            dataLimite = dataLimite.Value.AddDays(10);

            for (DateTime data = cargaJanelaDescarregamento.InicioDescarregamento.Date; data <= dataLimite; data = data.AddDays(1))
            {
                if (listaDatas.Any(obj => obj.DataPreferencialDescarga.DiaPreferencial == data.Day))
                {
                    List<Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria> datasPreferenciaisCategoriaFornecedor = listaDatas.Where(obj => obj.DataPreferencialDescarga.DiaPreferencial == data.Day).ToList();

                    foreach (Dominio.Entidades.Embarcador.Logistica.DataPreferencialDescargaFornecedorCategoria dataPreferencialCategoriaFornecedor in datasPreferenciaisCategoriaFornecedor)
                    {
                        for (int i = 0; i < dataPreferencialCategoriaFornecedor.DataPreferencialDescarga.DiasBloqueio; i++)
                        {
                            List<string> pedidos = agendamentoColetaPedidos.Where(obj => obj.Pedido.ProdutoPrincipal.Codigo == dataPreferencialCategoriaFornecedor.Categoria.Codigo).Select(obj => obj.Pedido.NumeroPedidoEmbarcador).ToList();
                            DateTime dataBloqueada = data.Date.AddDays(-dataPreferencialCategoriaFornecedor.DataPreferencialDescarga.DiasBloqueio + i);
                            datasBloqueadas.Add((dataBloqueada, pedidos));
                        }
                    }
                }
            }

            return datasBloqueadas;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> ObterListaQuantidadePorTipoDeCargaDescarregamento(List<int> tiposDeCarga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime data)
        {
            Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadeTipoCargaTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoDia = (from o in centroDescarregamento.ExcecoesCapacidadeDescarregamento where o.DataInicial <= data.Date && o.DataFinal >= data.Date select o).FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> listaRetorno = new List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();

            if (excecaoDia != null)
                listaRetorno = repositorioQuantidadeTipoCargaTipoCarga.BuscarPorExcecaoTiposDeCarga(excecaoDia.Codigo, tiposDeCarga);

            if (listaRetorno.Count == 0)
                listaRetorno = repositorioQuantidadeTipoCargaTipoCarga.BuscarPorTipoCargaDiaCentroDescarregamento(tiposDeCarga, centroDescarregamento.Codigo, data);

            return listaRetorno;
        }

        private Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento ObterPeriodoDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime novoHorarioInicio, DateTime novoHorarioTermino)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = ObterPeriodosDescarregamento(cargaJanelaDescarregamento, novoHorarioInicio.Date, agendamentoColeta);

            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = periodosDescarregamento.Where(o =>
                o.HoraInicio <= novoHorarioInicio.TimeOfDay &&
                o.HoraTermino.Add(TimeSpan.FromMinutes(o.ToleranciaExcessoTempo)) >= novoHorarioInicio.TimeOfDay &&
                o.HoraTermino.Add(TimeSpan.FromMinutes(o.ToleranciaExcessoTempo)) >= novoHorarioTermino.TimeOfDay
            ).FirstOrDefault();

            if (periodoDescarregamento == null)
                throw new ServicoException($"O horário informado não está configurado para o centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            return periodoDescarregamento;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> ObterPeriodosDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dia, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorioExcecaoCapacidadeDescarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao = repositorioExcecaoCapacidadeDescarregamento.BuscarPorCentroDescarregamento(cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dia, dia);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamentoRetornar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento;

            int grupoProdutoDominante = repCarga.BuscarCodigoGrupoProdutoDominantePorCarga(cargaJanelaDescarregamento.Carga.Codigo);

            if (excecao != null)
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorExcecao(excecao.Codigo);
            else if (cargaJanelaDescarregamento.CentroDescarregamento.CapacidadeDescaregamentoPorDia)
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorDiaMes(cargaJanelaDescarregamento.CentroDescarregamento.Codigo, dia.Day, dia.Month);
            else
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCentroDescarregamentoEDia(cargaJanelaDescarregamento.CentroDescarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(dia));

            if (periodosDescarregamento.Count > 0)
            {
                bool filtrarPorCanaisVenda = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(_unitOfWork).ExistePorCentroDescarregamento(cargaJanelaDescarregamento.CentroDescarregamento.Codigo);
                int quantidadeSKU = ObterSku(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento.Destinatario);
                double cpfCnpjRemetente = agendamentoColeta?.Remetente?.CPF_CNPJ ?? 0d;
                int codigoGrupoPessoa = (agendamentoColeta?.Remetente != null) ? new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork).BuscarCodigoPorRaizCNPJ(agendamentoColeta.Remetente.RaizCnpjSemFormato) : 0;
                int codigoCanalVenda = filtrarPorCanaisVenda ? repositorioCargaPedido.BuscarCodigoCanalVendaPorDestinatarioDaCarga(cargaJanelaDescarregamento.Carga.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Destinatario.CPF_CNPJ) : 0;
                List<int> codigosTiposCarga = new List<int>();

                if (agendamentoColeta != null && agendamentoColeta.TipoCarga == null)
                    codigosTiposCarga.AddRange(new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork).BuscarCodigosTipoCargaDosPedidosPorAgendamentoColeta(agendamentoColeta.Codigo));
                else if (cargaJanelaDescarregamento.Carga.TipoDeCarga != null)
                    codigosTiposCarga.Add(cargaJanelaDescarregamento.Carga.TipoDeCarga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento in periodosDescarregamento)
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoasPorPeriodo = periodoDescarregamento.GruposPessoas.Select(o => o.GrupoPessoas).ToList();

                    if ((gruposPessoasPorPeriodo.Count > 0) && ((codigoGrupoPessoa > 0) || configuracaoJanelaCarregamento.UtilizarPeriodoDescarregamentoExclusivo) && !gruposPessoasPorPeriodo.Any(grupoPessoas => grupoPessoas.Codigo == codigoGrupoPessoa))
                        continue;

                    List<Dominio.Entidades.Cliente> remetentesPorPeriodo = periodoDescarregamento.Remetentes.Select(o => o.Remetente).ToList();

                    if ((remetentesPorPeriodo.Count > 0) && ((cpfCnpjRemetente > 0d) || configuracaoJanelaCarregamento.UtilizarPeriodoDescarregamentoExclusivo) && !remetentesPorPeriodo.Any(remetente => remetente.CPF_CNPJ == cpfCnpjRemetente))
                        continue;

                    List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaPorPeriodo = periodoDescarregamento.TiposDeCarga.Select(o => o.TipoDeCarga).ToList();

                    if ((tiposCargaPorPeriodo.Count > 0) && (codigosTiposCarga.Count > 0) && !codigosTiposCarga.All(codigoTipoCarga => tiposCargaPorPeriodo.Any(tipoCarga => tipoCarga.Codigo == codigoTipoCarga)))
                        continue;

                    if (filtrarPorCanaisVenda)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> canaisVendaPorPeriodo = periodoDescarregamento.CanaisVenda.Select(o => o.CanalVenda).ToList();

                        if (((canaisVendaPorPeriodo.Count > 0) || (codigoCanalVenda > 0)) && !canaisVendaPorPeriodo.Any(canalVenda => canalVenda.Codigo == codigoCanalVenda))
                            continue;
                    }

                    List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProdutosPorPeriodo = periodoDescarregamento.GruposProdutos.Select(o => o.GrupoProduto).ToList();

                    if (gruposProdutosPorPeriodo.Count > 0 && grupoProdutoDominante > 0 && !gruposProdutosPorPeriodo.Any(grupoProduto => grupoProduto.Codigo == grupoProdutoDominante))
                        continue;

                    if (!IsQuantidadeSkuDentroFaixaPorPeriodo(quantidadeSKU, periodoDescarregamento))
                        continue;

                    periodosDescarregamentoRetornar.Add(periodoDescarregamento);
                }
            }

            if ((periodosDescarregamentoRetornar.Count == 0) && IsBloquearJanelaDescarregamentoExcedente(cargaJanelaDescarregamento))
                throw new ServicoException($"Não existe período de descarregamento no dia {dia:dd/MM/yyyy} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            if (agendamentoColeta != null)
            {
                if (agendamentoColeta.HoraInicioFaixa.HasValue && agendamentoColeta.HoraInicioFaixa.Value.TotalMinutes > 0)
                    periodosDescarregamentoRetornar = periodosDescarregamentoRetornar.Where(obj => obj.HoraInicio.TotalMinutes >= agendamentoColeta.HoraInicioFaixa.Value.TotalMinutes).ToList();

                if (agendamentoColeta.HoraLimiteFaixa.HasValue && agendamentoColeta.HoraLimiteFaixa.Value.TotalMinutes > 0)
                    periodosDescarregamentoRetornar = periodosDescarregamentoRetornar.Where(obj => obj.HoraTermino.TotalMinutes <= agendamentoColeta.HoraLimiteFaixa.Value.TotalMinutes).ToList();

                if (periodosDescarregamentoRetornar.Count == 0 && IsBloquearJanelaDescarregamentoExcedente(cargaJanelaDescarregamento) && (agendamentoColeta.HoraInicioFaixa.HasValue || agendamentoColeta.HoraLimiteFaixa.HasValue))
                    throw new ServicoException($"Não foram encontrados períodos disponíveis na faixa de horário selecionada para o centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}.", errorCode: CodigoExcecao.PeriodosIndisponiveisFaixaHorarioSelecionada);
            }

            return periodosDescarregamentoRetornar
                .OrderBy(obj => obj.HoraInicio)
                .OrderByDescending(obj => obj.TiposDeCarga?.Count > 0)
                .OrderByDescending(obj => obj.GruposProdutos?.Count > 0)
                .OrderByDescending(obj => obj.Remetentes?.Count > 0)
                .OrderByDescending(obj => obj.GruposPessoas?.Count > 0)
                .ToList();
        }

        private decimal ObterPesoDescarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            return repositorioCargaPedido.BuscarPesoTotalPedidoPorCargaEDestinatario(carga.Codigo, centroDescarregamento.Destinatario.CPF_CNPJ, centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido);
        }

        private void ValidarDataBloqueada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            List<(DateTime Data, List<string> Pedidos)> datasBloqueadas = ObterListaDatasBloqueados(agendamentoColeta, cargaJanelaDescarregamento);
            List<string> pedidosComBloqueio = new List<string>();

            foreach (List<string> pedidosBloqueados in datasBloqueadas.Select(o => o.Pedidos))
                pedidosComBloqueio.AddRange(pedidosBloqueados);

            pedidosComBloqueio = pedidosComBloqueio.Distinct().ToList();

            if (datasBloqueadas.Select(obj => obj.Data).Contains(cargaJanelaDescarregamento.InicioDescarregamento.Date))
                throw new ServicoException($"O(s) pedido(s) {string.Join(", ", pedidosComBloqueio)} possuem datas bloqueadas no período. Entre em contato com o analista da sua categoria e solicite a mudança da data de validade.", errorCode: CodigoExcecao.DataBloqueada);
        }

        private void ValidarDataDescarregamentoRetroativa(DateTime data)
        {
            DateTime dataLimite = DateTime.Now;

            if ((data <= dataLimite) && !_configuracaoDisponibilidadeDescarregamento.PermitirHorarioDescarregamentoInferiorAoAtual)
                throw new ServicoException("A data de descarregamento deve ser maior ou igual a data atual.", CodigoExcecao.HorarioDescarregamentoInferiorAtual);
        }

        private void ValidarTolerancia(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dataInicioDescarregamento, int codigoCentroDescarregamento)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);

            List<int> tiposDeCarga = repositorioAgendamentoColeta.BuscarCodigosTiposDeCarga(cargaJanelaDescarregamento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> listaQuantidadePorTipoDeCargaTipoCarga = ObterListaQuantidadePorTipoDeCargaDescarregamento(tiposDeCarga, cargaJanelaDescarregamento.CentroDescarregamento, dataInicioDescarregamento);

            foreach (Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga quantidadePorTipoDeCargaTipoCarga in listaQuantidadePorTipoDeCargaTipoCarga)
            {
                if (quantidadePorTipoDeCargaTipoCarga.QuantidadePorTipoDeCargaDescarregamento.Tolerancia.HasValue && ((dataInicioDescarregamento - DateTime.Now).TotalHours < quantidadePorTipoDeCargaTipoCarga.QuantidadePorTipoDeCargaDescarregamento.Tolerancia.Value))
                    throw new ServicoException($"O CD {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} não permite agendamento com menos de {quantidadePorTipoDeCargaTipoCarga.QuantidadePorTipoDeCargaDescarregamento.Tolerancia.Value} horas de antecedência para o tipo de carga {quantidadePorTipoDeCargaTipoCarga.TipoCarga.Descricao} na data {dataInicioDescarregamento.ToString("dd/MM/yyyy")}.", errorCode: CodigoExcecao.ToleranciaAlocacaoHorarioDescarregamentoAtingida);
            }
        }

        private void ValidarVeiculosPermitidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            if (carga.ModeloVeicularCarga == null)
                return;

            if ((centroDescarregamento.VeiculosPermitidos == null) || (centroDescarregamento.VeiculosPermitidos.Count == 0))
                return;

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaPermitido = (
                from o in centroDescarregamento.VeiculosPermitidos
                where o.Codigo == carga.ModeloVeicularCarga.Codigo
                select o
            ).FirstOrDefault();

            if (modeloVeicularCargaPermitido == null)
                throw new ServicoException($"O modelo veicular da {carga.DescricaoEntidade} não é permitido no centro de descarregamento {centroDescarregamento.Descricao}.");
        }

        private bool ValidarVeiculosPermitidos(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, int codigoModeloVeicular)
        {
            if ((centroDescarregamento.VeiculosPermitidos == null) || (centroDescarregamento.VeiculosPermitidos.Count == 0))
                return true;

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaPermitido = (
                from o in centroDescarregamento.VeiculosPermitidos
                where o.Codigo == codigoModeloVeicular
                select o
            ).FirstOrDefault();

            return modeloVeicularCargaPermitido != null;
        }

        private void ValidarVolumeDiarioPorTipoCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

            if ((agendamentoColeta?.Remetente?.GrupoPessoas != null) && (cargaJanelaDescarregamento.CentroDescarregamento?.LimitesAgendamento?.Any(l => l.PermiteUltrapassarLimiteVolume && l.GrupoPessoa.Codigo == agendamentoColeta.Remetente.GrupoPessoas.Codigo) ?? false))
                return;

            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            int codigoCentroDescarregamento = cargaJanelaDescarregamento.CentroDescarregamento?.Codigo ?? 0;
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            List<string> tiposCargaNaoValidados = new List<string>();

            if (agendamentoColeta?.TipoCarga != null)
                tiposCarga.Add(agendamentoColeta.TipoCarga);
            else
                tiposCarga = repositorioAgendamentoColeta.BuscarTiposDeCarga(cargaJanelaDescarregamento.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaAgendamentoColetaPedidos = repositorioAgendamentoColetaPedido.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> listaQuantidadePorTipoDeCargaTipoCarga = ObterListaQuantidadePorTipoDeCargaDescarregamento(tiposCarga.Select(o => o.Codigo).ToList(), cargaJanelaDescarregamento.CentroDescarregamento, dia);
            List<(int TipoDeCarga, int QuantidadeVolumesNoDia)> listaVolumesNoDia = new List<(int TipoDeCarga, int QuantidadeVolumesNoDia)>();

            if (agendamentoColeta?.TipoCarga != null)
                listaVolumesNoDia = repositorioJanelaDescarregamento.BuscarPorTipoDeCargaAgendamentoDia(agendamentoColeta.TipoCarga.Codigo, dia, cargaJanelaDescarregamento.Carga.Codigo, codigoCentroDescarregamento);
            else
                listaVolumesNoDia = repositorioJanelaDescarregamento.BuscarPorTipoDeCargaDia(tiposCarga.Select(o => o.Codigo).ToList(), dia, cargaJanelaDescarregamento.Carga.Codigo, codigoCentroDescarregamento);

            int quantidadeVolumesNoDia = 0;
            int volumes = 0;
            int volumesPermitidoNoDia = 0;

            List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> listaQuantidadePorTipoCarga = listaQuantidadePorTipoDeCargaTipoCarga.Select(obj => obj.QuantidadePorTipoDeCargaDescarregamento).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento quantidadeTipoCarga in listaQuantidadePorTipoCarga)
            {
                List<int> codigosTipoCarga = listaQuantidadePorTipoDeCargaTipoCarga.Where(obj => obj.QuantidadePorTipoDeCargaDescarregamento.Codigo == quantidadeTipoCarga.Codigo).Select(obj => obj.TipoCarga.Codigo).ToList();
                codigosTipoCarga = codigosTipoCarga.Distinct().ToList();

                quantidadeVolumesNoDia = (from o in listaVolumesNoDia where codigosTipoCarga.Contains(o.TipoDeCarga) select o.QuantidadeVolumesNoDia).Sum();

                if (agendamentoColeta?.TipoCarga != null)
                    volumes = (from o in listaAgendamentoColetaPedidos select o.VolumesEnviar).Sum();
                else
                    volumes = (from o in listaAgendamentoColetaPedidos where codigosTipoCarga.Contains(o.Pedido.TipoDeCarga.Codigo) select o.VolumesEnviar).Sum();

                quantidadeVolumesNoDia += volumes;

                volumesPermitidoNoDia = quantidadeTipoCarga.Volumes;

                if (quantidadeVolumesNoDia > volumesPermitidoNoDia)
                    tiposCargaNaoValidados.AddRange((from o in tiposCarga where codigosTipoCarga.Contains(o.Codigo) select o.Descricao).ToList());
            }

            if (tiposCargaNaoValidados.Count > 0)
                throw new ServicoException($"O centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} não possuí capacidade de descarregamento para o(s) tipo(s) de carga {string.Join(", ", tiposCargaNaoValidados)} no dia {dia.ToString("dd/MM/yyyy")}.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);
        }

        #endregion

        #region Métodos Públicos

        public void AlterarHorarioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime novoHorarioInicio)
        {
            ValidarVeiculosPermitidos(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);
            ValidarDataDescarregamentoRetroativa(novoHorarioInicio);

            Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Logistica.CargaJanelaDescarregamento(_unitOfWork);

            cargaJanelaDescarregamento.TempoDescarregamento = servicoJanelaDescarregamento.ObterTempoDescarregamento(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            if (cargaJanelaDescarregamento.TempoDescarregamento == 0)
                throw new ServicoException($"Não existe uma configuração de tempo de descarregamento criada para as configurações desta carga no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}.");

            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga?.Codigo ?? 0);

            if (!IsPossuiLimiteRaizCnpjNoDia(agendamentoColeta, novoHorarioInicio, cargaJanelaDescarregamento))
                throw new ServicoException($"O Grupo de Pessoas {agendamentoColeta.Remetente.GrupoPessoas.Descricao} já está com a capacidade máxima ({cargaJanelaDescarregamento.CentroDescarregamento.LimitePadrao}) de janelas alocadas no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para o dia {novoHorarioInicio.ToString("dd/MM/yyyy")}.", errorCode: CodigoExcecao.CapacidadeGrupoPessoaAtigida);

            ValidarTolerancia(cargaJanelaDescarregamento, novoHorarioInicio, cargaJanelaDescarregamento.CentroDescarregamento?.Codigo ?? 0);
            ValidarVolumeDiarioPorTipoCarga(cargaJanelaDescarregamento, novoHorarioInicio);

            DateTime novoHorarioTermino = novoHorarioInicio.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = ObterPeriodoDescarregamento(cargaJanelaDescarregamento, novoHorarioInicio, novoHorarioTermino);

            if (!IsPossuiCapacidadeDescarregamentoPorDia(cargaJanelaDescarregamento, novoHorarioInicio))
                throw new ServicoException($"A capacidade do dia {novoHorarioInicio.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            if (!IsPossuiCapacidadeDescarregamentoPorPeriodo(cargaJanelaDescarregamento, novoHorarioInicio, periodoDescarregamento))
                throw new ServicoException($"A capacidade do dia {novoHorarioInicio.ToString("dd/MM/yyyy")} {periodoDescarregamento.DescricaoPeriodo.ToLower()} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(cargaJanelaDescarregamento, periodoDescarregamento, novoHorarioInicio, novoHorarioTermino, agendamentoColeta);

            if (cargasPeriodo.Count >= periodoDescarregamento.CapacidadeDescarregamentoSimultaneoTotal)
                throw new ServicoException($"A capacidade de descarregamento simultâneo do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para o período já atingiu o limite.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

            cargaJanelaDescarregamento.InicioDescarregamento = novoHorarioInicio;
            cargaJanelaDescarregamento.TerminoDescarregamento = novoHorarioTermino;
            cargaJanelaDescarregamento.Excedente = false;
            cargaJanelaDescarregamento.Carga.AgendaExtra = false;
            cargaJanelaDescarregamento.PeriodoDescarregamentoExclusivo = IsPeriodoDescarregamentoExclusivo(periodoDescarregamento);

            repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);

            AtualizarDatasRelacionadas(cargaJanelaDescarregamento);
        }

        public void AlterarHorarioDescarregamentoSemVerificarDisponibilidade(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime novoHorarioInicio, bool agendaExtra)
        {
            ValidarVeiculosPermitidos(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Logistica.CargaJanelaDescarregamento(_unitOfWork);

            cargaJanelaDescarregamento.TempoDescarregamento = servicoJanelaDescarregamento.ObterTempoDescarregamento(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            if (cargaJanelaDescarregamento.TempoDescarregamento == 0)
                throw new ServicoException($"Não existe uma configuração de tempo de descarregamento criada para as configurações desta carga no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}.");

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

            DateTime novoHorarioTermino = novoHorarioInicio.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);

            cargaJanelaDescarregamento.InicioDescarregamento = novoHorarioInicio;
            cargaJanelaDescarregamento.TerminoDescarregamento = novoHorarioTermino;
            cargaJanelaDescarregamento.Excedente = false;
            cargaJanelaDescarregamento.Carga.AgendaExtra = agendaExtra;
            cargaJanelaDescarregamento.PeriodoDescarregamentoExclusivo = false;

            repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);

            AtualizarDatasRelacionadas(cargaJanelaDescarregamento);
        }

        public void AtualizarDefinicaoHorarioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            ValidarVeiculosPermitidos(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            if (cargaJanelaDescarregamento.Excedente || cargaJanelaDescarregamento.Carga.AgendaExtra)
                return;

            if (!IsPossuiCapacidadeDescarregamentoPorDia(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento))
            {
                if (cargaJanelaDescarregamento.CentroDescarregamento.BloquearJanelaDescarregamentoExcedente)
                    throw new ServicoException($"A capacidade do dia {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.");

                cargaJanelaDescarregamento.Excedente = true;
            }

            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = ObterPeriodoDescarregamento(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento, cargaJanelaDescarregamento.TerminoDescarregamento);

            if ((periodo != null) && !IsPossuiCapacidadeDescarregamentoPorPeriodo(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento, periodo))
            {
                if (cargaJanelaDescarregamento.CentroDescarregamento.BloquearJanelaDescarregamentoExcedente)
                    throw new ServicoException($"A capacidade do dia {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy")} {periodo.DescricaoPeriodo.ToLower()} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.");

                cargaJanelaDescarregamento.Excedente = true;
            }
        }

        public void DefinirHorarioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            ValidarVeiculosPermitidos(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            bool encontrouHorario = false;
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

            if (!IsDataDescarregamentoRetroativaValida(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento.Date.Add(DateTime.MaxValue.TimeOfDay)))
            {
                cargaJanelaDescarregamento.Excedente = true;
                return;
            }

            if (!IsPossuiLimiteRaizCnpjNoDia(agendamentoColeta, cargaJanelaDescarregamento.InicioDescarregamento, cargaJanelaDescarregamento))
                throw new ServicoException($"O Grupo de Pessoas {agendamentoColeta.Remetente.GrupoPessoas.Descricao} já está com a capacidade máxima ({cargaJanelaDescarregamento.CentroDescarregamento.LimitePadrao}) de janelas alocadas no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para o dia {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy")}.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            ValidarTolerancia(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento, cargaJanelaDescarregamento.CentroDescarregamento?.Codigo ?? 0);
            ValidarVolumeDiarioPorTipoCarga(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento.Date);
            ValidarDataBloqueada(cargaJanelaDescarregamento, agendamentoColeta);

            if (!IsPossuiCapacidadeDescarregamentoPorDia(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento))
            {
                if (IsBloquearJanelaDescarregamentoExcedente(cargaJanelaDescarregamento))
                    throw new ServicoException($"A capacidade do dia {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

                cargaJanelaDescarregamento.Excedente = true;
                return;
            }

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = ObterPeriodosDescarregamento(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento, agendamentoColeta);

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo in periodosDescarregamento)
            {
                DateTime dataInicioDescarregamento = ObterDataInicioDescarregamento(cargaJanelaDescarregamento, periodo);
                DateTime dataTerminoPeriodoDescarregamento = dataInicioDescarregamento.Date.Add(periodo.HoraTermino).AddMinutes(periodo.ToleranciaExcessoTempo);
                DateTime dataTerminoDescarregamento = dataInicioDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
                DateTime dataFimPeriodo = new DateTime(dataInicioDescarregamento.Year, dataInicioDescarregamento.Month, dataInicioDescarregamento.Day).Add(periodo.HoraTermino);

                if (dataFimPeriodo.AddMinutes(periodo.ToleranciaExcessoTempo).TimeOfDay < periodo.HoraInicio)
                {
                    dataFimPeriodo = dataFimPeriodo.AddDays(1);
                    dataTerminoPeriodoDescarregamento = dataTerminoPeriodoDescarregamento.AddDays(1);
                }

                if (dataInicioDescarregamento >= dataFimPeriodo)
                    continue;

                if (dataTerminoDescarregamento > dataFimPeriodo.AddMinutes(periodo.ToleranciaExcessoTempo))
                    continue;

                if (!IsPossuiCapacidadeDescarregamentoPorPeriodo(cargaJanelaDescarregamento, dataInicioDescarregamento, periodo))
                    continue;

                List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(cargaJanelaDescarregamento, periodo, dataInicioDescarregamento, dataTerminoPeriodoDescarregamento, agendamentoColeta);

                if (EncontrarProximoHorario(cargaJanelaDescarregamento, cargasPeriodo, periodo, dataInicioDescarregamento, dataTerminoDescarregamento))
                {
                    encontrouHorario = true;
                    break;
                }
            }

            if (!encontrouHorario)
            {
                if (IsBloquearJanelaDescarregamentoExcedente(cargaJanelaDescarregamento))
                    throw new ServicoException($"Não foi possível encontrar um horário no dia {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

                cargaJanelaDescarregamento.Excedente = true;
            }
        }

        public void DefinirHorarioDescarregamentoAteDataLimite(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, DateTime? dataLimiteSugestaoHorario = null)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);
            DateTime? menorDataCarga = repositorioAgendamentoColetaPedido.BuscarMenorDataPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
            DateTime? dataLimite = dataLimiteSugestaoHorario ?? menorDataCarga;
            DateTime? dataDescarregamentoSugerida = null;
            bool encontrouHorario = false;
            bool semPeriodosDisponiveisFaixaHorario = false;
            bool datasBloqueadas = false;
            string mensagemDatasBloqueadas = string.Empty;

            cargaJanelaDescarregamento.InicioDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.Date;

            while (cargaJanelaDescarregamento.InicioDescarregamento.Date <= dataLimite?.Date)
            {
                try
                {
                    DefinirHorarioDescarregamento(cargaJanelaDescarregamento);

                    if (!cargaJanelaDescarregamento.Excedente)
                    {
                        if (!dataLimiteSugestaoHorario.HasValue)
                        {
                            encontrouHorario = true;
                            break;
                        }
                        else
                        {
                            dataDescarregamentoSugerida = cargaJanelaDescarregamento.InicioDescarregamento.Date.AddDays(3);
                            break;
                        }
                    }
                }
                catch (ServicoException excecao)
                {
                    if (
                        (excecao.ErrorCode != CodigoExcecao.HorarioDescarregamentoIndisponivel) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioDescarregamentoInferiorAtual) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioLimiteDescarregamentoAtingido) &&
                        (excecao.ErrorCode != CodigoExcecao.ToleranciaAlocacaoHorarioDescarregamentoAtingida) &&
                        (excecao.ErrorCode != CodigoExcecao.PeriodosIndisponiveisFaixaHorarioSelecionada) &&
                        (excecao.ErrorCode != CodigoExcecao.DataBloqueada)
                    )
                        throw;

                    semPeriodosDisponiveisFaixaHorario = (excecao.ErrorCode == CodigoExcecao.PeriodosIndisponiveisFaixaHorarioSelecionada);
                    datasBloqueadas = (excecao.ErrorCode == CodigoExcecao.DataBloqueada);
                    mensagemDatasBloqueadas = excecao.Message;
                }

                cargaJanelaDescarregamento.InicioDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.AddDays(1).Date;
                cargaJanelaDescarregamento.TerminoDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
            }

            if (!encontrouHorario)
            {
                if (!dataLimiteSugestaoHorario.HasValue)
                {
                    if (semPeriodosDisponiveisFaixaHorario)
                        throw new ServicoException($"Não existe horário de recebimento no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para a faixa de horário selecionada.");

                    if (datasBloqueadas)
                        throw new ServicoException(mensagemDatasBloqueadas);
                }

                if (cargaJanelaDescarregamento.CentroDescarregamento.BloquearJanelaDescarregamentoExcedente)
                {
                    if (dataLimite.HasValue && !dataLimiteSugestaoHorario.HasValue)
                        DefinirHorarioDescarregamentoAteDataLimite(cargaJanelaDescarregamento, dataLimiteSugestaoHorario: dataLimite.Value.AddDays(10));

                    if (dataDescarregamentoSugerida.HasValue)
                        throw new ServicoException($"Data indisponível para Agendamento. Entre em contato com o analista da sua categoria para ajustes. Sugestão de data: {dataDescarregamentoSugerida.Value.ToString("dd/MM/yyyy")}");

                    throw new ServicoException($"Não foi possível encontrar um horário do dia {cargaJanelaDescarregamento.DataDescarregamentoProgramada.ToString("dd/MM/yyyy")} até o dia {menorDataCarga?.ToString("dd/MM/yyyy")} no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);
                }

                cargaJanelaDescarregamento.InicioDescarregamento = cargaJanelaDescarregamento.DataDescarregamentoProgramada;
                cargaJanelaDescarregamento.TerminoDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
                cargaJanelaDescarregamento.Excedente = true;
            }
        }

        public void DefinirHorarioDescarregamentoAteLimiteTentativas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento)
        {
            bool encontrouHorario = false;

            for (int i = 0; i < _configuracaoDisponibilidadeDescarregamento.DiasLimiteParaDefinicaoHorarioDescarregamento; i++)
            {
                DateTime? proximaDataDescarregamento = ObterProximaDataComPeriodoDescarregamento(cargaJanelaDescarregamento.CentroDescarregamento.Codigo, cargaJanelaDescarregamento.InicioDescarregamento.Date);

                if (!proximaDataDescarregamento.HasValue)
                    break;

                cargaJanelaDescarregamento.InicioDescarregamento = proximaDataDescarregamento.Value;
                cargaJanelaDescarregamento.TerminoDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);

                try
                {
                    DefinirHorarioDescarregamento(cargaJanelaDescarregamento);

                    if (!cargaJanelaDescarregamento.Excedente)
                    {
                        encontrouHorario = true;
                        break;
                    }
                }
                catch (ServicoException excecao)
                {
                    if (
                        (excecao.ErrorCode != CodigoExcecao.HorarioDescarregamentoIndisponivel) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioDescarregamentoInferiorAtual) &&
                        (excecao.ErrorCode != CodigoExcecao.HorarioLimiteDescarregamentoAtingido) &&
                        (excecao.ErrorCode != CodigoExcecao.ToleranciaAlocacaoHorarioDescarregamentoAtingida) &&
                        (excecao.ErrorCode != CodigoExcecao.PeriodosIndisponiveisFaixaHorarioSelecionada)
                    )
                        throw;
                }
            }

            if (!encontrouHorario)
            {
                if (cargaJanelaDescarregamento.CentroDescarregamento.BloquearJanelaDescarregamentoExcedente)
                {
                    DateTime dataInicialDefinicaoHorario = cargaJanelaDescarregamento.DataDescarregamentoProgramada.Date;
                    DateTime dataUltimaTentativaDefinicaoHorario = cargaJanelaDescarregamento.InicioDescarregamento.Date;

                    if (dataUltimaTentativaDefinicaoHorario > dataInicialDefinicaoHorario)
                        throw new ServicoException($"Não foi possível encontrar um horário no período de {dataInicialDefinicaoHorario.ToString("dd/MM/yyyy")} a {dataUltimaTentativaDefinicaoHorario.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

                    throw new ServicoException($"Não foi possível encontrar um horário no dia {dataInicialDefinicaoHorario.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);
                }

                cargaJanelaDescarregamento.InicioDescarregamento = cargaJanelaDescarregamento.DataDescarregamentoProgramada;
                cargaJanelaDescarregamento.TerminoDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
                cargaJanelaDescarregamento.Excedente = true;
            }
        }

        public void DefinirHorarioDescarregamentoPorPeriodo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, DateTime dataDescarregamento)
        {
            ValidarVeiculosPermitidos(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            DateTime diaDescarregamento = dataDescarregamento.Date;
            DateTime dataInicioPeriodoDescarregamento = diaDescarregamento.Add(periodo.HoraInicio);

            ValidarDataDescarregamentoRetroativa(dataInicioPeriodoDescarregamento);

            Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Logistica.CargaJanelaDescarregamento(_unitOfWork);

            cargaJanelaDescarregamento.TempoDescarregamento = servicoJanelaDescarregamento.ObterTempoDescarregamento(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento);

            if (cargaJanelaDescarregamento.TempoDescarregamento == 0)
                throw new ServicoException($"Não existe uma configuração de tempo de descarregamento criada para as configurações desta carga no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}.");

            ValidarTolerancia(cargaJanelaDescarregamento, dataInicioPeriodoDescarregamento.Date, cargaJanelaDescarregamento.CentroDescarregamento?.Codigo ?? 0);
            ValidarVolumeDiarioPorTipoCarga(cargaJanelaDescarregamento, dataInicioPeriodoDescarregamento.Date);

            if (!IsPossuiCapacidadeSKUDescarregamentoPorDia(cargaJanelaDescarregamento.Carga, cargaJanelaDescarregamento.CentroDescarregamento, periodo))
                throw new ServicoException($"A quantidade de itens da carga não é compatível com a faixa de itens ({ObterDescritivoFaixaVolume(periodo.SkuDe, periodo.SkuAte)}) para período no centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao}.");

            if (!IsPossuiCapacidadeDescarregamentoPorDia(cargaJanelaDescarregamento, dataInicioPeriodoDescarregamento))
                throw new ServicoException($"A capacidade do dia {dataInicioPeriodoDescarregamento.ToString("dd/MM/yyyy")} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            if (!IsPossuiCapacidadeDescarregamentoPorPeriodo(cargaJanelaDescarregamento, dataInicioPeriodoDescarregamento, periodo))
                throw new ServicoException($"A capacidade do dia {dataInicioPeriodoDescarregamento.ToString("dd/MM/yyyy")} {periodo.DescricaoPeriodo.ToLower()} do centro de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} foi atingida.", errorCode: CodigoExcecao.HorarioDescarregamentoIndisponivel);

            DateTime dataTerminoDescarregamento = dataInicioPeriodoDescarregamento.AddMinutes(cargaJanelaDescarregamento.TempoDescarregamento);
            DateTime dataTerminoPeriodoDescarregamento = diaDescarregamento.Add(periodo.HoraTermino).AddMinutes(periodo.ToleranciaExcessoTempo);
            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = ObterCargasPeriodo(cargaJanelaDescarregamento, periodo, dataInicioPeriodoDescarregamento, dataTerminoPeriodoDescarregamento, agendamentoColeta: null);

            if (!EncontrarProximoHorario(cargaJanelaDescarregamento, cargasPeriodo, periodo, dataInicioPeriodoDescarregamento, dataTerminoDescarregamento))
                throw new ServicoException($"Não foi possível encontrar um horário de descarregamento {cargaJanelaDescarregamento.CentroDescarregamento.Descricao} para as configurações desta carga.");

            AtualizarDatasRelacionadas(cargaJanelaDescarregamento);
        }

        public DateTime? ObterProximaDataComPeriodoDescarregamento(int codigoCentroDescarregamento, DateTime dataDescarregamento)
        {
            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorioExcecaoCapacidadeDescarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(_unitOfWork);
            List<DiaSemana> diasSemanaComPeriodosDescarregamento = repositorioPeriodoDescarregamento.BuscarDiasSemanaComPeriodosDescarregamento(codigoCentroDescarregamento);
            DateTime? proximaDataDescarregamentoPorExcecao = repositorioExcecaoCapacidadeDescarregamento.BuscarProximaDataPorCentroDescarregamento(codigoCentroDescarregamento, dataDescarregamento);
            DateTime? proximaDataDescarregamentoPorPeriodo = null;

            if (diasSemanaComPeriodosDescarregamento.Count > 0)
            {
                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataDescarregamento);
                DiaSemana proximoDiaSemana = diasSemanaComPeriodosDescarregamento.Where(o => o > diaSemana).Select(o => (DiaSemana?)o).FirstOrDefault() ?? diasSemanaComPeriodosDescarregamento.FirstOrDefault();
                int diasAdicionar = (proximoDiaSemana > diaSemana) ? (int)proximoDiaSemana - (int)diaSemana : (int)proximoDiaSemana + 7 - (int)diaSemana;

                proximaDataDescarregamentoPorPeriodo = dataDescarregamento.AddDays(diasAdicionar);
            }

            return DateTimeExtensions.Min(proximaDataDescarregamentoPorPeriodo, proximaDataDescarregamentoPorExcecao)?.Date;
        }

        public int ObterSku(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente destinatarioRecebedor)
        {
            double cpfCnpfjDestinatarioRecebedor = destinatarioRecebedor.CPF_CNPJ;

            if (carga.DadosSumarizados != null && carga.DadosSumarizados.QuantidadeSKU.HasValue)
                return carga.DadosSumarizados.QuantidadeSKU.Value;

            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);

            return repositorioAgendamentoColetaPedido.BuscarSKUPorCargaEDestinatarioRecebedor(carga.Codigo, cpfCnpfjDestinatarioRecebedor);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> ObterPeriodosDescarregamentoDisponiveis(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime dia)
        {
            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorioExcecaoCapacidadeDescarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecao = repositorioExcecaoCapacidadeDescarregamento.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo, dia, dia);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamentoRetornar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();

            if (!ValidarVeiculosPermitidos(centroDescarregamento, _configuracaoDisponibilidadeDescarregamento.CodigoModeloVeicular))
                return periodosDescarregamentoRetornar;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento;

            if (excecao != null)
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorExcecao(excecao.Codigo);
            else if (centroDescarregamento.CapacidadeDescaregamentoPorDia)
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorDiaMes(centroDescarregamento.Codigo, dia.Day, dia.Month);
            else
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCentroDescarregamentoEDia(centroDescarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(dia));

            if (periodosDescarregamento.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento in periodosDescarregamento)
                {
                    List<Dominio.Entidades.Cliente> remetentesPorPeriodo = periodoDescarregamento.Remetentes.Select(o => o.Remetente).ToList();

                    if ((remetentesPorPeriodo.Count > 0) && (_configuracaoDisponibilidadeDescarregamento.CodigoRemetente > 0d) && !remetentesPorPeriodo.Any(remetente => remetente.CPF_CNPJ.Equals(_configuracaoDisponibilidadeDescarregamento.CodigoRemetente)))
                        continue;

                    List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaPorPeriodo = periodoDescarregamento.TiposDeCarga.Select(o => o.TipoDeCarga).ToList();

                    if ((tiposCargaPorPeriodo.Count > 0) && (_configuracaoDisponibilidadeDescarregamento.CodigoTipoCarga > 0) && !tiposCargaPorPeriodo.Any(tipoCarga => tipoCarga.Codigo == _configuracaoDisponibilidadeDescarregamento.CodigoTipoCarga))
                        continue;

                    if (!repositorioCargaJanelaDescarregamento.ExisteCargasPorPeriodo(centroDescarregamento.Codigo, periodoDescarregamento, dia))
                        continue;

                    periodosDescarregamentoRetornar.Add(periodoDescarregamento);
                }
            }

            return periodosDescarregamentoRetornar
                .OrderBy(obj => obj.HoraInicio)
                .OrderByDescending(obj => obj.TiposDeCarga?.Count > 0)
                .OrderByDescending(obj => obj.Remetentes?.Count > 0)
                .OrderByDescending(obj => obj.GruposPessoas?.Count > 0)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
