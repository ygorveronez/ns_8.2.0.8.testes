using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public abstract class AbstractEvento
    {

        #region Atributos protegidos

        protected virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }
        protected virtual Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento MonitoramentoEvento { get; set; }
        protected virtual List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> ListaMonitoramentoEvento { get; set; }
        protected virtual List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> PermanenciaLocais { get; set; }

        protected Repositorio.UnitOfWork unitOfWork;

        #endregion

        #region Métodos púbicos

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta GetTipoAlerta()
        {
            return this.TipoAlerta;
        }

        public int GetTempoEvento()
        {
            return this.ListaMonitoramentoEvento?.Max(x => x?.Gatilho?.TempoEvento ?? 0) ?? 0;
        }

        public int GetTempoEvento2()
        {
            return this.ListaMonitoramentoEvento?.Max(x => x?.Gatilho?.TempoEvento2 ?? 0) ?? 0;
        }

        public List<int> GetListaCodigosLocais()
        {
            return this.ListaMonitoramentoEvento?.Where(e => e.Gatilho?.PontosDeApoio != null).SelectMany(e => e.Gatilho?.PontosDeApoio?.Select(p => p.Codigo).ToList()).ToList() ?? new List<int>();
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public void Processar(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = null, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = null, List<double> codigosClientesAlvo = null, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = null, List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> permanenciaLocais = null)
        {
            this.MonitoramentoEvento = monitoramentoEvento;

            // Confirma quando o evento deste monitoramento deve ser processado
            bool deveProcessar = true;
            switch (this.MonitoramentoEvento.QuandoProcessar)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoReceberPosicao:
                    deveProcessar = true;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento:
                    deveProcessar = monitoramento != null;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento:
                    deveProcessar = monitoramento != null && monitoramento.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem:
                    deveProcessar = monitoramento != null &&
                                    monitoramento.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando &&
                                    monitoramento.Carga != null &&
                                    monitoramento.Carga.DataInicioViagem != null;
                    break;
            }
            if (!deveProcessar) return;

            // Respeita a configuração de restição de horário
            if (this.MonitoramentoEvento.Horario != null && (monitoramentoProcessarEvento.DataVeiculoPosicao.Value.TimeOfDay < this.MonitoramentoEvento.Horario.HoraInicio || monitoramentoProcessarEvento.DataVeiculoPosicao.Value.TimeOfDay > this.MonitoramentoEvento.Horario.HoraFim)) return;

            // Verifica o status de viagem do monitoramento
            if (!VerificaStatusViagemMonitoramento(monitoramentoProcessarEvento, monitoramento)) return;

            // Verifica o status de viagem do monitoramento
            if (!VerificaTipoDeCargaMonitoramento(monitoramentoProcessarEvento, monitoramento)) return;

            // Verifica o status de viagem do monitoramento
            if (!VerificaTipoDeOperacaoMonitoramento(monitoramentoProcessarEvento, monitoramento)) return;

            // Não pode haver um alerta em aberto OU um fechado mas muito recente de acordo com o parâmetro
            if (this.MonitoramentoEvento.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal && this.MonitoramentoEvento.TipoAlerta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PerdaDeSinal && ExisteAlertaAbertoOuFechadoHaPouco(monitoramentoProcessarEvento, monitoramento, alertas)) return;

            // Verificar se processa para pre carga
            if (NaoProcessarParaPreCarga(monitoramentoProcessarEvento, monitoramento)) return;

            //Preenche as permanencias para utilizar nos eventos necessários.
            this.PermanenciaLocais = permanenciaLocais;

            // Execura a regra específica do evento 
            ProcessarEvento(monitoramentoProcessarEvento, monitoramento, posicoesObjetoValor, alertas, entregas, codigosClientesAlvo, cargaJanelaCarregamento, cargaJanelasDescarregamento);
        }

        public bool ExisteAlertaAbertoOuFechadoHaPouco(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.CodigoVeiculo != null && (monitoramento != null && monitoramento.Carga != null))
            {
                // Busca o último alerta para o veículo ou para a carga
                Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor alerta = ObterUltimoAlertaDaLista(alertas, monitoramento.Carga.Codigo, monitoramentoProcessarEvento.CodigoVeiculo);
                if (alerta != null)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> alertasSinal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>()
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PerdaDeSinal,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SensorTemperaturaComProblema,
                    };

                    // Tempo para geração de novo alerta, cadastrado no Gatilho do Evento.
                    int tempoParaGeracaoDeNovoAlerta = this.MonitoramentoEvento.Gatilho.Tempo;

                    // Data base do veículo / do processamento do evento de sinal.
                    DateTime dataBase = monitoramentoProcessarEvento.DataVeiculoPosicao.Value;

                    // Data base do alerta para validação de geração de novo alerta: Data de Criação.
                    DateTime dataAlerta = alerta.Data;

                    if (alertasSinal.Contains(TipoAlerta))
                    {
                        dataBase = DateTime.Now;
                        dataAlerta = alerta.DataCadastro;
                    }

                    // Caso o alerta foi Reprogramado, compara Tempo Reprogramado e a Data de Finalização do Alerta.
                    if (alerta.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado && alerta.AlertaReprogramado && alerta.DataFim.HasValue)
                    {
                        tempoParaGeracaoDeNovoAlerta = alerta.TempoReprogramado;
                        dataAlerta = alerta.DataFim.Value;
                    }

                    // Não deve gerar um novo evento caso o alerta ainda esteja em aberto e se nao é continuo
                    else if (alerta.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto && !(this.MonitoramentoEvento.Gatilho?.EventoContinuo ?? false)) return true;

                    // Deve respeitar o tempo mínimo configurado entre os alertas, mesmo sendo evento contínuo
                    int diferencaEmMinutos = (int)(dataBase - dataAlerta).TotalMinutes;
                    if (diferencaEmMinutos <= tempoParaGeracaoDeNovoAlerta) return true;

                }
            }

            return false;
        }

        #endregion

        #region Métodos abstratos

        public abstract void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento);

        public abstract void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento);

        #endregion

        #region Métodos protegidos

        protected AbstractEvento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta)
        {
            this.unitOfWork = unitOfWork;
            this.TipoAlerta = TipoAlerta;
            this.CarregarListaMonitoramentoEvento();
        }

        /**
         * Verifica se o evento está configurado e ativo
         */
        protected bool EstaAtivo()
        {
            return this.ListaMonitoramentoEvento?.Any(x => x?.Ativo ?? false) ?? false;
        }

        /**
         * Cria um alerta em aberto
         */
        protected void CriarAlertaEmAberto(List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Cargas.Carga Carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, string descricao = null, bool TratativaAutomatica = false, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.CodigoVeiculo != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null)
            {
                if (cargaEntrega == null)
                    cargaEntrega = BuscarProximaEntregaPendente(entregas);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(this.unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(this.unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(this.unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista = repCargaMotorista.BuscarPorCarga(Carga?.Codigo ?? 0).FirstOrDefault();

                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor()
                {
                    TipoAlerta = this.TipoAlerta,
                    MonitoramentoEvento = this.MonitoramentoEvento,
                    Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto,
                    DataCadastro = DateTime.Now,
                    Data = monitoramentoProcessarEvento.DataVeiculoPosicao.Value,
                    Latitude = (decimal)monitoramentoProcessarEvento.LatitudePosicao,
                    Longitude = (decimal)monitoramentoProcessarEvento.LongitudePosicao,
                    Veiculo = (monitoramentoProcessarEvento.CodigoVeiculo != null) ? new Dominio.Entidades.Veiculo { Codigo = monitoramentoProcessarEvento.CodigoVeiculo.Value } : null,
                    Carga = Carga,
                    CargaEntrega = cargaEntrega != null ? repCargaEntrega.BuscarPorCodigo(cargaEntrega.Codigo) : null, //pode ser q a entrega nao exista mais.
                    AlertaDescricao = descricao.Length > 50 ? descricao.Substring(0, 50) : descricao,
                    AlertaTrativaAutomatica = TratativaAutomatica,
                    AlertaPossuiPosicaoRetroativa = false,
                    Motorista = cargaMotorista?.Motorista ?? null,
                };

                repAlerta.Inserir(alerta);

                // Chama o callback que roda depois da criação de um alerta
                ExecutarDepoisDeCriarAlerta(alerta, monitoramentoProcessarEvento);

                //Geração de integração de eventos de alerta.
                GerarIntegracaoEvento(alerta, monitoramentoProcessarEvento, cargaEntrega, repAlerta);

                CriarAlertaAcompanhamentoCarga(alerta);

                alerta.Chamado = CriarAtendimentoCarga(alerta);
                repAlerta.Atualizar(alerta);

                // Adiciona na lista de alertas abertos para posteriores vericações de existência do alerta nesta mesma sessão
                alertas.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor
                {
                    Codigo = alerta.Codigo,
                    CodigoCarga = Carga?.Codigo ?? null,
                    CodigoVeiculo = monitoramentoProcessarEvento.CodigoVeiculo,
                    DataCadastro = alerta.DataCadastro,
                    Data = alerta.Data,
                    DataFim = alerta.DataFim,
                    TipoAlerta = alerta.TipoAlerta,
                    CodigoMonitoramentoEvento = this.MonitoramentoEvento.Codigo,
                    Status = alerta.Status,
                });
            }
        }

        /**
         * Cria um alerta caso não exista um alerta em aberto ou um alerta no antes do período mínimo de recorrência do alerta
         */
        protected void CriarAlertaSeNaoExistir(List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, string descricao = null, bool tratativaAutomatica = false, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaentrega = null)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && (monitoramentoProcessarEvento.CodigoVeiculo != null || carga != null) && monitoramentoProcessarEvento.DataFimMonitoramento == null)
            {
                CriarAlertaEmAberto(alertas, monitoramentoProcessarEvento, carga, entregas, descricao, tratativaAutomatica, cargaentrega);
            }
        }

        /**
         * Busca o alerta para a carga, veículo e tipo
         */
        protected Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor ObterUltimoAlertaDaLista(List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, int? codigoCarga = null, int? codigoVeiculo = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos)
        {
            if (alertas != null && (codigoCarga != null || codigoVeiculo != null))
            {
                int total = alertas.Count;
                for (int i = total - 1; i >= 0; i--)
                {
                    if (
                        alertas[i].TipoAlerta == this.TipoAlerta
                        &&
                        alertas[i].CodigoMonitoramentoEvento == this.MonitoramentoEvento.Codigo
                        &&
                        (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos || alertas[i].Status == status)
                        &&
                        (
                            (codigoCarga != null && codigoVeiculo != null && alertas[i].CodigoCarga != null && alertas[i].CodigoCarga == codigoCarga && alertas[i].CodigoVeiculo != null && alertas[i].CodigoVeiculo == codigoVeiculo)
                            ||
                            (codigoCarga != null && codigoVeiculo == null && alertas[i].CodigoCarga != null && alertas[i].CodigoCarga == codigoCarga)
                            ||
                            (codigoVeiculo != null && codigoCarga == null && alertas[i].CodigoVeiculo != null && alertas[i].CodigoVeiculo == codigoVeiculo)
                        )
                    )
                    {
                        return alertas[i];
                    }
                }
            }
            return null;
        }

        /**
         * Busca os alertas em aberto para o veículo
         */
        protected List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> ObterAlertasEmAberto(Dominio.Entidades.Veiculo Veiculo)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = ObterAlertas(Veiculo: Veiculo, status: Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);
            return alertas;
        }

        /**
         * Busca os alertas em aberto para a carga
         */
        protected List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> ObterAlertasEmAberto(Dominio.Entidades.Embarcador.Cargas.Carga Carga)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = ObterAlertas(Carga: Carga, status: Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);
            return alertas;
        }

        /**
         * Verifica se existe um alerta em aberto para o veículo
         */
        protected bool ExisteAlertaEmAberto(Dominio.Entidades.Veiculo Veiculo)
        {
            if (Veiculo == null) return false;
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = ObterAlertasEmAberto(Veiculo);
            return alertas.Count > 0;
        }

        /**
         * Verifica se existe um alerta em aberto para a carga
         */
        protected bool ExisteAlertaEmAberto(Dominio.Entidades.Embarcador.Cargas.Carga Carga)
        {
            if (Carga == null) return false;
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = ObterAlertasEmAberto(Carga);
            return alertas.Count > 0;
        }

        /**
         * Verifica se existe um alerta em aberto para a carga
         */
        protected bool ExisteAlertaEmAberto(Dominio.Entidades.Embarcador.Cargas.Carga Carga = null, Dominio.Entidades.Veiculo Veiculo = null)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = ObterAlertas(Carga, Veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto);
            return alertas.Count > 0;
        }

        /**
         * Consulta alertas por carga e/ou veículo
         */
        protected List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> ObterAlertas(Dominio.Entidades.Embarcador.Cargas.Carga Carga = null, Dominio.Entidades.Veiculo Veiculo = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Todos)
        {
            if (Carga == null && Veiculo == null) return new List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(this.unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAlertaMonitor filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAlertaMonitor();
            filtroPesquisa.Carga = Carga;
            filtroPesquisa.Veiculo = Veiculo;
            filtroPesquisa.Status = status;
            filtroPesquisa.TipoAlerta = this.TipoAlerta;
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = repAlerta.Consultar(filtroPesquisa);
            return alertas;
        }

        /**
         * Busca posições de um veículo em um período a partir da base de dados
         */
        protected List<Dominio.Entidades.Embarcador.Logistica.Posicao> ObterUltimasPosicoes(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual PosicaoAtual, int minutos, Dominio.Entidades.Embarcador.Cargas.Carga Carga = null, Dominio.Entidades.Veiculo Veiculo = null)
        {

            List<Dominio.Entidades.Embarcador.Logistica.Posicao> ultimasPosicoes = new List<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            if (Carga != null || Veiculo != null)
            {

                // Busca as últimas posições do veíclulo dentro do período estipulado para o evento
                int codigoVeiculo = (Carga?.Veiculo != null) ? Carga.Veiculo.Codigo : Veiculo.Codigo;
                if (codigoVeiculo > 0)
                {
                    DateTime dataInicial = PosicaoAtual.DataVeiculo.AddMinutes(-minutos);
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    ultimasPosicoes = repPosicao.BuscarPorVeiculoDataInicialeFinal(codigoVeiculo, dataInicial, PosicaoAtual.DataVeiculo);
                }
            }

            return ultimasPosicoes;

        }

        /**
         * Entre uma lista de posições, extrai as posições de um determinado veículo
         */
        protected List<Dominio.Entidades.Embarcador.Logistica.Posicao> ObterUltimasPosicoes(List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes, DateTime dataInicial, DateTime dataFinal)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesVeiculo = new List<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].DataVeiculo >= dataInicial && posicoes[i].DataVeiculo <= dataFinal)
                {
                    posicoesVeiculo.Add(posicoes[i]);
                }
            }
            return posicoesVeiculo;
        }

        /**
         * Verifica se a posição atual indica que está no cliente
         */
        protected bool EstaNoCliente(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<double> codigosClientesAlvo, Dominio.Entidades.Cliente cliente)
        {
            int total = codigosClientesAlvo?.Count ?? 0;
            if ((monitoramentoProcessarEvento?.EmAlvoPosicao ?? false) == true && total > 0 && cliente != null)
            {
                for (int i = 0; i < total; i++)
                {
                    if (cliente.CPF_CNPJ == codigosClientesAlvo[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Busca a data de início de entrega prevista
         */
        protected DateTime? BuscaDataInicioEntregaPrevista(Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = BuscaCargaEntrega(cliente, entregas);
            return (entrega != null) ? entrega.DataPrevista : null;
        }

        protected DateTime? BuscarDataEntradaNoAlvo(Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = BuscaCargaEntrega(cliente, entregas);
            return BuscarDataEntradaNoAlvo(cliente, entrega);
        }

        protected DateTime? BuscarDataEntradaNoAlvo(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            if (cliente != null && entrega != null)
            {
                Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciaCliente = repPermanenciaCliente.BuscarPorCargaEntrega(entrega);
                if (permanenciaCliente != null && permanenciaCliente.Count > 0)
                {
                    return permanenciaCliente.Last().DataInicio;
                }
            }
            return null;
        }

        protected DateTime? BuscarDataSaidaDoAlvo(Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = BuscaCargaEntrega(cliente, entregas);
            return BuscarDataEntradaNoAlvo(cliente, entrega);
        }

        protected DateTime? BuscarDataSaidaDoAlvo(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            if (cliente != null && entrega != null)
            {
                Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciaCliente = repPermanenciaCliente.BuscarPorCargaEntrega(entrega);
                if (permanenciaCliente != null && permanenciaCliente.Count > 0)
                {
                    return permanenciaCliente.Last().DataFim;
                }
            }
            return null;
        }

        protected Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscaCargaEntrega(Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            int total = entregas?.Count ?? 0;
            if (cliente != null && total > 0)
            {
                for (int i = 0; i < total; i++)
                {
                    if (cliente.CPF_CNPJ == entregas[i].Cliente?.CPF_CNPJ)
                    {
                        return entregas[i];
                    }
                }
            }
            return null;
        }

        /**
         * Busca a data de descarregamento programada
         */
        protected DateTime? BuscaDataJanelaDescarregamentoNoCliente(Dominio.Entidades.Cliente cliente, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (cliente != null && cargaJanelasDescarregamento != null)
            {
                int totalCargaJanelasDescarregamento = cargaJanelasDescarregamento.Count;

                for (int i = 0; i < totalCargaJanelasDescarregamento; i++)
                {
                    if ((cargaJanelasDescarregamento[i].CentroDescarregamento != null) && (cliente.Codigo == cargaJanelasDescarregamento[i].CentroDescarregamento.Destinatario.Codigo))
                        return cargaJanelasDescarregamento[i].InicioDescarregamento;
                }
            }

            return null;
        }

        protected DateTime? BuscarDataMonitoramentoEvento(
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData monitoramentoEventoData,
            Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento,
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento,
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega,
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento,
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento
        )
        {
            DateTime? data = null;
            switch (monitoramentoEventoData)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.DataAtual:
                    data = DateTime.Now;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.PosicaoVeiculo:
                    data = monitoramentoProcessarEvento.DataVeiculoPosicao;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.EntradaCliente:
                    data = BuscarDataEntradaNoAlvo(entrega.Cliente, entrega);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.SaidaCliente:
                    data = BuscarDataSaidaDoAlvo(entrega.Cliente, entrega);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.JanelaCarregamento:
                    data = cargaJanelaCarregamento.DataCarregamentoProgramada;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.JanelaDescarregamento:
                    data = BuscaDataJanelaDescarregamentoNoCliente(entrega.Cliente, cargaJanelasDescarregamento);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.EntregaPedidoPrevista:
                    if (monitoramento != null && monitoramento.Carga != null && entrega != null && entrega.Cliente != null)
                    {
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        data = repCargaPedido.BuscarMaiorPrevisaoEntrega(monitoramento.Carga.Codigo, entrega.Cliente.Codigo);
                    }
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.InicioEntregaPrevista:
                    data = entrega.DataPrevista;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.FinalEntregaPrevista:
                    data = entrega.DataFimPrevista;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.InicioEntregaReprogramada:
                    data = entrega.DataReprogramada;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.FinalEntregaReprogramada:
                    data = entrega.DataReprogramada.Value.AddMinutes(entrega.TempoDescargaEmMinutos); //entrega.DataEntregaReprogramada.Value?.AddMinutes(); //+ entrega.DataEntregaReprogramada;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.InicioEntregaRealizada:
                    data = entrega.DataInicio;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.FinalEntregaRealizada:
                    data = entrega.DataFim;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.DataAgendamentodeEntrega:
                    if (monitoramento != null && monitoramento.Carga != null && entrega != null && entrega.Cliente != null)
                        data = entrega.DataAgendamento;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.ETAPrimeiraColeta:
                    if (monitoramento != null && monitoramento.Carga != null && entrega != null && entrega.Cliente != null)
                        data = entrega.DataReprogramada;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.DataCarregamento:
                    data = entrega.Carga.DataCarregamentoCarga;
                    break;
            }
            return data;
        }

        protected List<double> ExtrairCodigosClientesColetaEntregaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            List<double> codigosClientesColetaEntregaCarga = new List<double>();

            // Cliente origem da carga
            Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, carga);
            if (clienteOrigem != null) codigosClientesColetaEntregaCarga.Add(clienteOrigem.CPF_CNPJ);
            int total = entregas?.Count() ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (entregas[i].Cliente != null && (clienteOrigem == null || entregas[i].Cliente.CPF_CNPJ != clienteOrigem.CPF_CNPJ))
                {
                    codigosClientesColetaEntregaCarga.Add(entregas[i].Cliente.CPF_CNPJ);
                }
            }
            return codigosClientesColetaEntregaCarga;
        }

        protected Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ExtraiPosicaoAnterior(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor)
        {
            // Percorre em ordem inversa, das posições mais atuais para as mais antigas
            int ultima = posicoesObjetoValor.Count - 1;
            for (int i = ultima; i >= 0; i--)
            {
                if (posicoesObjetoValor[i].DataVeiculo < monitoramentoProcessarEvento.DataVeiculoPosicao)
                {
                    return posicoesObjetoValor[i];
                }
            }
            return null;
        }

        protected List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ExtrairPosicoesAposAlertaFechadoAtePosicao(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoPeriodo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            // Identifica o último alerta fechado
            DateTime dataInicio = DateTime.MinValue;
            Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor alerta = ObterUltimoAlertaDaLista(alertas, monitoramentoProcessarEvento.CodigoCarga, monitoramentoProcessarEvento.CodigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado);
            if (alerta != null && alerta.DataFim != null)
            {
                dataInicio = alerta.DataFim.Value;
            }

            // Filtra o histórico de posições para considerar apenas posições a partir da data final do último alerta fechado
            int total = posicoesObjetoValor.Count;
            for (int i = 0; i < total; i++)
            {
                if (posicoesObjetoValor[i].DataVeiculo > dataInicio && posicoesObjetoValor[i].DataVeiculo <= monitoramentoProcessarEvento.DataVeiculoPosicao.Value)
                {
                    posicoesDoPeriodo.Add(posicoesObjetoValor[i]);
                }
            }

            return posicoesDoPeriodo;
        }

        protected bool EmAlvoEntrega(List<double> codigosClientesAlvo, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            return entregas.Exists(entrega => codigosClientesAlvo.Contains(entrega.Cliente?.CPF_CNPJ ?? 0));
        }
        #endregion

        #region Métodos privados

        /**
        * Busca a configuração de monitoramento de evento e suas configurações
        */
        private void CarregarListaMonitoramentoEvento()
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(this.unitOfWork);
            this.ListaMonitoramentoEvento = repMonitoramentoEvento.BuscarAtivosPorTipo(this.TipoAlerta);
        }

        private bool NaoProcessarParaPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.CodigoVeiculo != null && monitoramento != null && monitoramento.Carga != null)
            {
                if (this.MonitoramentoEvento.NaoGerarParaPreCarga && monitoramento.Carga.CargaDePreCarga)
                    return true;
            }

            return false;
        }

        private bool VerificaStatusViagemMonitoramento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (
                monitoramentoProcessarEvento != null &&
                monitoramentoProcessarEvento.DataVeiculoPosicao != null &&
                monitoramentoProcessarEvento.CodigoVeiculo != null &&
                monitoramento != null &&
                this.MonitoramentoEvento.VerificarStatusViagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.Todos &&
                this.MonitoramentoEvento.VerificarStatusViagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.NaoVerificar
            )
            {

                var monitoramentoEventoStatusViagem = (
                    from statusViagem in this.MonitoramentoEvento.StatusViagem
                    where statusViagem.MonitoramentoEvento.Ativo
                    select new { statusViagem.MonitoramentoStatusViagem }
                ).ToList();
                int total = monitoramentoEventoStatusViagem.Count;
                if (total > 0)
                {

                    if (this.MonitoramentoEvento.VerificarStatusViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.EstarComStatusViagem ||
                        this.MonitoramentoEvento.VerificarStatusViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.NaoEstarComStatusViagem
                    )
                    {
                        switch (this.MonitoramentoEvento.VerificarStatusViagem)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.EstarComStatusViagem:
                                if (monitoramento.StatusViagem != null)
                                {
                                    for (int i = 0; i < total; i++)
                                    {
                                        if (monitoramento.StatusViagem.TipoRegra == monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                break;

                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.NaoEstarComStatusViagem:
                                bool naoEstaComStatusViagem = true;
                                if (monitoramento.StatusViagem != null)
                                {
                                    for (int i = 0; i < total; i++)
                                    {
                                        if (monitoramento.StatusViagem.TipoRegra == monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra)
                                        {
                                            naoEstaComStatusViagem = false;
                                            break;
                                        }
                                    }
                                }
                                return naoEstaComStatusViagem;
                        }
                    }
                    else
                    {
                        if (monitoramento.StatusViagem != null)
                        {
                            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramento);
                            int totalHistorico = historicosStatusViagem.Count;

                            switch (this.MonitoramentoEvento.VerificarStatusViagem)
                            {
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.HaverPeloMenosUmStatusViagem:
                                    for (int i = 0; i < total; i++)
                                    {
                                        for (int j = 0; j < totalHistorico; j++)
                                        {
                                            if (monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra == historicosStatusViagem[j].StatusViagem.TipoRegra)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    break;

                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.HaverTodosStatusViagem:
                                    for (int i = 0; i < total; i++)
                                    {
                                        bool haStatusViagem = false;
                                        for (int j = 0; j < totalHistorico; j++)
                                        {
                                            if (monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra == historicosStatusViagem[j].StatusViagem.TipoRegra)
                                            {
                                                haStatusViagem = true;
                                                break;
                                            }
                                        }
                                        if (!haStatusViagem) return false;
                                    }
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }

        private bool VerificaTipoDeCargaMonitoramento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (
                monitoramentoProcessarEvento != null &&
                monitoramentoProcessarEvento.DataVeiculoPosicao != null &&
                monitoramentoProcessarEvento.CodigoVeiculo != null &&
                monitoramento != null &&
                monitoramento.Carga != null &&
                monitoramento.Carga.TipoDeCarga != null &&
                this.MonitoramentoEvento.VerificarTipoDeCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.Todos &&
                this.MonitoramentoEvento.VerificarTipoDeCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.NaoVerificar
            )
            {

                var monitoramentoEventoTipoDeCarga = (
                    from obj in this.MonitoramentoEvento.TipoDeCarga
                    where obj.MonitoramentoEvento.Ativo
                    select new { obj.TipoDeCarga }
                ).ToList();
                int total = monitoramentoEventoTipoDeCarga.Count;
                if (total > 0)
                {
                    switch (this.MonitoramentoEvento.VerificarTipoDeCarga)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.Algum:
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoDeCarga.Codigo == monitoramentoEventoTipoDeCarga[i].TipoDeCarga.Codigo)
                                {
                                    return true;
                                }
                            }
                            return false;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.Nenhum:
                            bool naoEstaComStatusViagem = true;
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoDeCarga.Codigo == monitoramentoEventoTipoDeCarga[i].TipoDeCarga.Codigo)
                                {
                                    naoEstaComStatusViagem = false;
                                    break;
                                }
                            }
                            return naoEstaComStatusViagem;
                    }
                }
            }

            return true;
        }

        private bool VerificaTipoDeOperacaoMonitoramento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (
                monitoramentoProcessarEvento != null &&
                monitoramentoProcessarEvento.DataVeiculoPosicao != null &&
                monitoramentoProcessarEvento.CodigoVeiculo != null &&
                monitoramento != null &&
                monitoramento.Carga != null &&
                monitoramento.Carga.TipoOperacao != null &&
                this.MonitoramentoEvento.VerificarTipoDeOperacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.Todos &&
                this.MonitoramentoEvento.VerificarTipoDeOperacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.NaoVerificar
            )
            {

                var monitoramentoEventoTipoDeOperacao = (
                    from obj in this.MonitoramentoEvento.TipoDeOperacao
                    where obj.MonitoramentoEvento.Ativo
                    select new { obj.TipoDeOperacao }
                ).ToList();
                int total = monitoramentoEventoTipoDeOperacao.Count;
                if (total > 0)
                {
                    switch (this.MonitoramentoEvento.VerificarTipoDeOperacao)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.Algum:
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoOperacao.Codigo == monitoramentoEventoTipoDeOperacao[i].TipoDeOperacao.Codigo)
                                {
                                    return true;
                                }
                            }
                            return false;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.Nenhum:
                            bool naoEstaComStatusViagem = true;
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoOperacao.Codigo == monitoramentoEventoTipoDeOperacao[i].TipoDeOperacao.Codigo)
                                {
                                    naoEstaComStatusViagem = false;
                                    break;
                                }
                            }
                            return naoEstaComStatusViagem;
                    }
                }
            }

            return true;
        }

        private Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarProximaEntregaPendente(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas)
        {
            int total = entregas?.Count() ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (
                    entregas[i].Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente ||
                    entregas[i].Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida ||
                    entregas[i].Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue)
                {
                    return entregas[i];
                }
            }
            return null;
        }

        private void CriarAlertaAcompanhamentoCarga(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta)
        {
            if (this.MonitoramentoEvento.GerarAlertaAcompanhamentoCarga)
            {
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
                Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga repAlertaAcompanhamentoCarga = new Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga(this.unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga alertaAcompanhamentoCarga = new Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga
                {
                    Carga = alerta.Carga,
                    AlertaMonitor = alerta,
                    CargaEvento = null,
                    AlertaTratado = false,
                    DataCadastro = DateTime.Now,
                    DataEvento = alerta.Data,
                    CargaEntrega = alerta.CargaEntrega
                };

                repAlertaAcompanhamentoCarga.Inserir(alertaAcompanhamentoCarga);
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(alerta.Carga);
            }

        }

        private Dominio.Entidades.Embarcador.Chamados.Chamado CriarAtendimentoCarga(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta)
        {
            if (this.MonitoramentoEvento.GerarAtendimento && alerta.Carga != null && alerta.MonitoramentoEvento != null && alerta.MonitoramentoEvento.MotivoChamado != null)
            {
                Servicos.Embarcador.Chamado.Chamado srvChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado();

                objChamado.AtendimentoRegistradoPeloMotorista = false;
                objChamado.Empresa = alerta.Carga.Empresa;
                objChamado.Cliente = alerta.CargaEntrega != null ? alerta.CargaEntrega.Cliente : repCliente.BuscarDestinatariosCarga(alerta.Carga.Codigo).FirstOrDefault();
                objChamado.MotivoChamado = alerta.MonitoramentoEvento.MotivoChamado;
                objChamado.Carga = alerta.Carga;
                objChamado.CargaEntrega = alerta.CargaEntrega;

                if (objChamado.CargaEntrega == null && (objChamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao || objChamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga))
                    return null;

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp
                };

                Dominio.Entidades.Usuario Usuario = alerta.Carga.Motoristas?.FirstOrDefault();

                if (Usuario == null)
                    Usuario = alerta.Carga.Operador;

                // Verifica se já não tem um chamado aberto para essa rejeição e não cria, para evitar duplicatas
                var possuiChamadoAberto = repChamado.ContemChamadoMesmaCargaEntregaMotivoSituacao(objChamado.Carga.Codigo, objChamado.CargaEntrega?.Codigo ?? 0, objChamado.MotivoChamado.Codigo, SituacaoChamado.Aberto);
                if (!possuiChamadoAberto && Usuario != null)
                {
                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objChamado, Usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, auditado, unitOfWork);

                    if (chamado != null)
                        Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);

                    return chamado;
                }

            }

            return null;
        }

        private void GerarIntegracaoEvento(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta)
        {
            if ((!alerta.MonitoramentoEvento?.IntegrarEvento) ?? true) return;

            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventoIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao(this.unitOfWork);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento repositorioCargaEntregaEvento = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento(this.unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao repMonitoramentoEventoTipoIntegracao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao(this.unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(this.unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(this.unitOfWork);

            List<TipoIntegracao> enumTiposIntegracao = repMonitoramentoEventoTipoIntegracao.BuscarTipoIntegracaoPorMonitoramentoEvento(alerta.MonitoramentoEvento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarPorTipos(enumTiposIntegracao);
            if (tiposIntegracao.Count == 0) return;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = alerta.Carga ?? new Dominio.Entidades.Embarcador.Cargas.Carga { Codigo = monitoramentoProcessarEvento?.CodigoCarga ?? 0 };
            if (carga.Codigo == 0) return;

            cargaEntrega = alerta.CargaEntrega ?? cargaEntrega;

            EventoColetaEntrega eventoColetaEntrega = EventoColetaEntrega.Intercorrencia;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas;
            if (cargaEntrega != null)
            {
                TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega = cargaEntrega.Coleta ? TipoAplicacaoColetaEntrega.Coleta : TipoAplicacaoColetaEntrega.Entrega;
                configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEventoETipoAplicacao(eventoColetaEntrega, tipoAplicacaoColetaEntrega);
            }
            else
                configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(eventoColetaEntrega);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntregaPedido = configuracaoOcorrenciaEntregas.FirstOrDefault();
            if (configuracaoOcorrenciaEntregaPedido == null) return;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento()
            {
                DataOcorrencia = DateTime.Now,
                TipoDeOcorrencia = configuracaoOcorrenciaEntregaPedido.TipoDeOcorrencia,
                Carga = carga,
                CargaEntrega = cargaEntrega,
                EventoColetaEntrega = eventoColetaEntrega,
                Latitude = alerta.Latitude,
                Longitude = alerta.Longitude,
                DataPosicao = alerta.Data,
                Origem = OrigemSituacaoEntrega.AlertaMonitoramento
            };
            repositorioCargaEntregaEvento.Inserir(cargaEntregaEvento);

            alerta.CargaEntregaEvento = cargaEntregaEvento;
            repAlerta.Atualizar(alerta);

            servicoCargaEntregaEventoIntegracao.GerarIntegracoes(cargaEntregaEvento, tiposIntegracao);
        }
        #endregion

    }
}