using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class Carga
    {
        #region propriedades privadas
        private string _formatDataHora = "dd-MM-yyyy HH:mm:ss";

        private static readonly string _caminhoImagem = "../../../../img/controle-entrega/";


        #endregion

        #region Propriedades Dados Carga
        public int CodigoCarga { get; set; }
        public int Veiculo { get; set; }
        public string CargaEmbarcador { get; set; }
        public bool CargaTransbordo { get; set; }
        public bool CargaFechada { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public virtual string DataCriacaoCargaFormatada { get { return DataCriacaoCarga.ToString(_formatDataHora); } }
        public DateTime? DataPrevisaoTerminoCarga { get; set; }
        public virtual string DataPrevisaoTerminoCargaFormatada { get { return DataPrevisaoTerminoCarga?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataInicioViagem { get; set; }
        public virtual string DataInicioViagemFormatada { get { return DataInicioViagem?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataInicioViagemPrevista { get; set; }
        public virtual string DataInicioViagemPrevistaFormatada { get { return DataInicioViagemPrevista?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataCarregamentoCarga { get; set; }
        public virtual string DataCarregamentoCargaFormatada { get { return DataCarregamentoCarga?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataFimViagem { get; set; }
        public virtual string DataFimViagemFormatada { get { return DataFimViagem?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataFimViagemPrevista { get; set; }
        public virtual string DataFimViagemPrevistaFormatada { get { return DataFimViagemPrevista?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataPrevisaoChegadaPlanta { get; set; }
        public DateTime? DataPreViagemInicio { get; set; }
        public virtual string DataPreViagemInicioFormatada { get { return DataPreViagemInicio?.ToString(_formatDataHora) ?? ""; } }
        public DateTime? DataPreViagemFim { get; set; }
        public virtual string DataPreViagemFimFormatada { get { return DataPreViagemFim?.ToString(_formatDataHora) ?? ""; } }
        public virtual string DataPrevisaoChegadaPlantaFormatada { get { return Monitoramento != null && EmDeslocamentoPlanta(Monitoramento[0]) && DataPrevisaoChegadaPlanta.HasValue ? DataPrevisaoChegadaPlanta.Value.ToString(_formatDataHora) : ""; } }
        public virtual bool ChegadaPlantaAtraso { get { return Monitoramento != null && EmDeslocamentoPlanta(Monitoramento[0]) && DataPrevisaoChegadaPlanta.HasValue && DateTime.Now > DataPrevisaoChegadaPlanta.Value; } }
        public virtual string TempoAtrasoChegadaPlanta { get { return Monitoramento != null && EmDeslocamentoPlanta(Monitoramento[0]) && DataPrevisaoChegadaPlanta.HasValue && DateTime.Now > DataPrevisaoChegadaPlanta.Value ? FormatarTempo(TimeSpan.FromMinutes((int)(DataPrevisaoChegadaPlanta.Value - DateTime.Now).TotalMinutes)) : ""; } }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public virtual bool CargaCancelada { get { return (SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada); } }
        public int CodigoAnalistaResponsavel { get; set; }
        public string NomeAnalistaResponsavel { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public string TipoOperacao { get; set; }
        public string StatusLoger { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }
        public decimal PesoTotalCarga { get; set; }
        public string PesoBrutoCarga { get { return PesoTotalCarga.ToString("n2") ?? ""; } }
        public string Origens { get; set; }
        public string Destinos { get; set; }
        public decimal DistanciaPlanejada { get; set; }
        public int CodigoTransportador { get; set; }
        public string NomeTransportador { get; set; }
        public string CodigoFilial { get; set; }
        public string Filial { get; set; }
        public string VeiculoTracao { get; set; }
        public string Reboques { get; set; }
        public virtual string Veiculos { get { return !string.IsNullOrEmpty(Reboques) ? VeiculoTracao + " - " + Reboques : VeiculoTracao; } }
        public string NumeroFrotaReboques { get; set; }
        public string Motoristas { get; set; }
        public string NumeroMotorista { get; set; }
        public DateTime DataAlerta { get; set; }
        public int PossuiAlertaAberto { get; set; }
        public int PossuiAlertaEmTratativa { get; set; }
        public bool CargaFixadaControleCargas { get; set; }
        public string Rastreador { get; set; }
        public EnumTecnologiaRastreador CodigoRastreador { get; set; }
        public string NomeRastreador => EnumTecnologiaRastreadorHelper.ObterDescricao(CodigoRastreador);
        public string VersaoAPP { get; set; }
        public virtual string DescricaoRastreador { get { return !string.IsNullOrWhiteSpace(NomeRastreador) ? NomeRastreador : Rastreador; } }
        public int ChamadosPendentes { get; set; }
        public int ChamadosEmTratativa { get; set; }
        public int ChamadosAtrasados { get; set; }
        public int ChamadosConcluidos { get; set; }

        public int GrupoTipoOperacaoCodigo { get; set; }
        public string GrupoTipoOperacaoDescricao { get; set; }
        public string GrupoTipoOperacaoCor { get; set; }
        public bool MonitoramentoCargaCritica { get { return Monitoramento?.Count > 0 ? Monitoramento.FirstOrDefault().MonitoramentoCritico : false; } }
        public string AnotacoesCard { get; set; }
        public int TempoCarregamento { get; set; }
        public int MaiorPrioridadeAlerta { get; set; }

        public DateTime? DataAgendamentoPedido { get; set; }

        public string DataAgendamentoPedidoFormatada { get { return DataAgendamentoPedido.HasValue ? DataAgendamentoPedido.Value.ToString(_formatDataHora) ?? "" : ""; } }

        public DateTime? DataCarregamentoPedido { get; set; }

        public string DataCarregamentoPedidoFormatada { get { return DataCarregamentoPedido.HasValue ? DataCarregamentoPedido.Value.ToString(_formatDataHora) ?? "" : ""; } }

        public string MatrizComplementar { get; set; }

        public string EscritorioVendasComplementar { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public string CanalVenda { get; set; }

        public TipoCobrancaMultimodal TipoModalTransporte { get; set; }

        public string ModalTransporte => TipoCobrancaMultimodalHelper.ObterDescricao(TipoModalTransporte);

        public bool InicioDeViagemNoRaio { get; set; }

        public bool InicioViagemForaRaio { get { return DataInicioViagem.HasValue ? !InicioDeViagemNoRaio ? true : false : false; } }

        public string ImagemForaRaio { get { return _caminhoImagem + "fora-raio.png"; } }

        public int UtilizaAppTrizy { get; set; }
        #endregion

        #region Dados Monitoramento       
        public string DadosMonitoramento { set { if (value != null) Monitoramento = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Monitoramento>>(value); } }
        public List<Monitoramento> Monitoramento { get; set; }

        public virtual int CodigoMonitoramento { get { return Monitoramento?.Count > 0 ? Monitoramento[0].CodigoMonitoramento : 0; } }
        public virtual bool PossuiMonitoramento { get { return Monitoramento != null && Monitoramento.Count > 0; } }
        public virtual string DescricaoStatusViagem { get { return Monitoramento?.Count > 0 ? Monitoramento[0].StatusViagem : ""; } }
        public virtual string StatusMonitoramento { get { return Monitoramento?.Count > 0 ? ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusHelper.ObterDescricao(Monitoramento[0].StatusMonitoramento) : ""; } }
        public virtual string PercentualViagem { get { return Monitoramento?.Count > 0 ? (int)Monitoramento[0].PercentualViagem + "%" : "0%"; } }
        public virtual string ProximoDestino { get { return Entregas != null ? Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.Cliente[0].Nome).FirstOrDefault() : ""; } }
        public virtual string ProximaCidadeDestino { get { return Entregas != null ? Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.Cliente[0].DescricaoCidade).FirstOrDefault() : ""; } }
        public virtual string DataPrevistaProximaEntrega
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(StatusLoger) && StatusLoger == "Ativo")
                    return "";

                return Entregas != null ? Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.DataEntregaPrevistaFormatada).FirstOrDefault() : "";
            }
        }
        public virtual string DataReprogramadaProximaEntrega
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(StatusLoger) && StatusLoger == "Ativo")
                    return "";

                return Entregas != null ? Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.DataEntregaReprogramadaFormatada).FirstOrDefault() : "";
            }
        }
        public virtual bool ProximaEntregaAtraso { get { return Entregas != null ? (DateTime.Now > Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.DataEntregaPrevista).FirstOrDefault()) : false; } }
        public virtual string TempoAtrasoProximaEntrega
        {
            get
            {
                if (Entregas != null && DateTime.Now > Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.DataEntregaPrevista).FirstOrDefault())
                    return FormatarTempo(TimeSpan.FromMinutes((int)(Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.DataEntregaPrevista).FirstOrDefault() - DateTime.Now).TotalMinutes));
                else
                    return "";
            }
        }

        public virtual string TendenciaProximaEntrega
        {
            get
            {
                if (Entregas == null) return "";

                var tendenciaNaoEntregue = Entregas
                    .Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                && !x.Coleta
                                && x.TendendiaEntrega != Enumeradores.TendenciaEntrega.Nenhum)
                    .OrderBy(x => x.OrdemPrevista)
                    .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.TendendiaEntrega))
                    .FirstOrDefault();

                var tendenciaEntregue = Entregas
                    .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                && x.TendendiaEntrega != Enumeradores.TendenciaEntrega.Nenhum
                                && !x.Coleta)
                    .OrderByDescending(x => x.OrdemPrevista)
                    .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.TendendiaEntrega))
                    .FirstOrDefault();

                return tendenciaNaoEntregue ?? tendenciaEntregue ?? "";
            }
        }

        public virtual string TendenciaProximaColeta
        {
            get
            {
                if (Entregas == null) return "";

                var tendenciaNaoEntregue = Entregas
                    .Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                && x.Coleta
                                && x.TendendiaEntrega != Enumeradores.TendenciaEntrega.Nenhum)
                    .OrderBy(x => x.OrdemPrevista)
                    .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.TendendiaEntrega))
                    .FirstOrDefault();

                var tendenciaEntregue = Entregas
                    .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                && x.TendendiaEntrega != Enumeradores.TendenciaEntrega.Nenhum
                                && x.Coleta)
                    .OrderByDescending(x => x.OrdemPrevista)
                    .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.TendendiaEntrega))
                    .FirstOrDefault();

                return tendenciaNaoEntregue ?? tendenciaEntregue ?? "";
            }
        }

        public virtual string UltimaEntregaRealizadaNoPrazoDescricao
        {
            get
            {
                var realizadaNoPrazo = Entregas != null ? Entregas
                    .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                    && !x.Coleta)
                    .OrderByDescending(x => x.OrdemRealizada)
                    .Select(x => x.RealizadaNoPrazo ? "Sim" : "Não").FirstOrDefault() : "";

                return realizadaNoPrazo;
            }
        }

        public virtual string UltimaColetaRealizadaNoPrazoDescricao
        {
            get
            {
                var realizadaNoPrazo = Entregas != null ? Entregas
                    .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                    && x.Coleta)
                    .OrderByDescending(x => x.OrdemRealizada)
                    .Select(x => x.RealizadaNoPrazo ? "Sim" : "Não").FirstOrDefault() : "";

                return realizadaNoPrazo;
            }
        }

        public virtual bool ProximaEntregaIsColeta { get { return Entregas != null ? (Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x?.Coleta).FirstOrDefault() ?? false) : false; } }

        public virtual int LeadTimeTransportadorProximaEntrega { get { return Entregas != null ? Entregas.Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue).Select(x => x.Cliente[0].Pedido?.Where(ped => ped?.DiasPrazoTransportador != 0 && ped?.DiasPrazoTransportador != null)?.FirstOrDefault()?.DiasPrazoTransportador)?.FirstOrDefault() ?? 0 : 0; } }

        public virtual string DistanciaAteDestino { get { return Monitoramento != null ? String.Format("{0:n0}Km", Monitoramento.Select(x => x.DistanciaAteDestino).FirstOrDefault()) : "N/A"; } }
        public virtual string DistanciaPrevista { get { return DistanciaPlanejada > 0 ? String.Format("{0:n0}Km", DistanciaPlanejada) : Monitoramento != null ? String.Format("{0:n0}Km", Monitoramento.Select(x => x.DistanciaPrevista).FirstOrDefault()) : "N/A"; } }
        public virtual string DistanciaRealizada { get { return Monitoramento != null ? String.Format("{0:n0}Km", Monitoramento.Select(x => x.DistanciaRealizada).FirstOrDefault()) : "N/A"; } }

        public virtual int NivelGPS
        {
            get
            {
                if (Monitoramento != null && Monitoramento[0].NivelGPS > 0)
                {
                    return (int)Monitoramento[0].NivelGPS;
                }
                else
                    return 0;
            }
        }

        public virtual int NivelBateria
        {
            get
            {
                if (Monitoramento != null)
                {
                    double nivel = Monitoramento[0].NivelBateria;

                    if (nivel < 1 && nivel > 0)//por algum motivo esta vindo 0.98, 0.35, 0.40...
                        nivel = nivel * 100;

                    return (int)nivel;
                }
                else
                    return 0;
            }
        }

        public virtual bool ExibirDescricaoAlerta
        {
            get
            {
                Alerta alerta = DadosAlertas?.OrderByDescending(x => x.Alerta?.MonitoramentoEvento?.PrioridadeAlerta).ThenBy(x => x.Alerta?.DataAlerta).Select(x => x.Alerta).FirstOrDefault();
                if (alerta != null)
                {
                    return alerta.MonitoramentoEvento?.ExibirDescricaoAlerta ?? false;
                }
                else
                    return false;
            }
        }
        public virtual bool ExibirDataeHoraGeracaoAlerta
        {
            get
            {
                Alerta alerta = DadosAlertas?.OrderByDescending(x => x.Alerta?.MonitoramentoEvento?.PrioridadeAlerta).ThenBy(x => x.Alerta?.DataAlerta).Select(x => x.Alerta).FirstOrDefault();
                if (alerta != null)
                {
                    return alerta.MonitoramentoEvento?.ExibirDataeHoraGeracaoAlerta ?? false;
                }
                else
                    return false;
            }
        }

        public virtual string DataUltimaPosicao
        {
            get
            {
                if (Monitoramento != null && Monitoramento[0].DataUltimaPosicao != null)
                {
                    return Monitoramento[0].DataUltimaPosicao.ToString(_formatDataHora);
                }
                else
                    return "";
            }
        }

        public virtual double Latitude
        {
            get
            {
                if (Monitoramento != null && Monitoramento[0].Latitude.HasValue)
                {
                    return Monitoramento[0].Latitude.Value;
                }
                else
                    return 0D;
            }
        }

        public virtual double Longitude
        {
            get
            {
                if (Monitoramento != null && Monitoramento[0].Longitude.HasValue)
                {
                    return Monitoramento[0].Longitude.Value;
                }
                else
                    return 0D;
            }
        }


        public virtual bool onLine { get; set; }
        public virtual bool PossuiPosicao
        {
            get
            {
                if (Monitoramento != null && Monitoramento[0].DataUltimaPosicao != null)
                {
                    DateTime? dataPosicao = Monitoramento[0]?.DataUltimaPosicao;

                    return dataPosicao != DateTime.MinValue && dataPosicao != DateTime.MaxValue;
                }
                else
                    return false;
            }
        }

        public int CodigoCargaEspelhadaComMonitoramentoAtivo { get; set; }
        public virtual bool PossuiMonitoramentoAtivoProVeiculoEmOutraCarga { get; set; }

        #endregion

        #region Dados Entregas

        public string DadosEntregas
        {
            set
            {
                if (value != null)
                    Entregas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entrega>>(value);
            }
        }
        public List<Entrega> Entregas { get; set; }
        public string PercentualCargaEntregue
        {
            get
            {
                if (Entregas != null)
                {
                    int percentual = (Entregas.Where(x => x.Situacao == Enumeradores.SituacaoEntrega.Entregue || x.Situacao == Enumeradores.SituacaoEntrega.Reentergue).Count() * 100) / Entregas.Count();
                    return decimal.Round(percentual, 2) + "%";
                }
                else
                    return "";
            }
        }
        public virtual int TotalDeEntregas { get { return Entregas != null ? Entregas.Where(x => !x.Coleta).Count() : 0; } }
        public virtual string PesoTotalNF { get { return Entregas != null ? Entregas.Sum(e => e.Cliente?.Sum(c => c?.NotaFiscal != null ? c.NotaFiscal.Sum(n => n?.Peso) : 0))?.ToString("n2") : ""; } }
        public virtual string ValorTotalNF { get { return Entregas != null ? Entregas.Sum(e => e.Cliente?.Sum(c => c?.NotaFiscal != null ? c.NotaFiscal.Sum(n => n?.Valor) : 0))?.ToString("n2") : ""; } }
        public virtual int TotalDeEntregasEntregues { get { return Entregas != null ? Entregas.Where(x => !x.Coleta && x.Situacao == Enumeradores.SituacaoEntrega.Entregue || x.Situacao == Enumeradores.SituacaoEntrega.Reentergue).Count() : 0; } }
        public virtual int TotalDeColetas { get { return Entregas != null ? Entregas.Where(x => x.Coleta).Count() : 0; } }
        public virtual int TotalDeColetasColetadas { get { return Entregas != null ? Entregas.Where(x => x.Coleta && x.Situacao == Enumeradores.SituacaoEntrega.Entregue || x.Situacao == Enumeradores.SituacaoEntrega.Reentergue).Count() : 0; } }
        public virtual string ConclusaoColetas
        {
            get
            {
                if (Entregas != null)
                {
                    return Entregas.Where(x => x.Coleta && x.Situacao == Enumeradores.SituacaoEntrega.Entregue).Count().ToString() + "/" + Entregas.Where(x => x.Coleta).Count().ToString();
                }
                else
                    return "";
            }
        }
        public virtual string ConclusaoEntregas
        {
            get
            {
                if (Entregas != null)
                {
                    return Entregas.Where(x => !x.Coleta && x.Situacao == Enumeradores.SituacaoEntrega.Entregue).Count().ToString() + "/" + Entregas.Where(x => !x.Coleta).Count().ToString();
                }
                else
                    return "";
            }
        }
        public virtual string ConclusaoViagem
        {
            get
            {
                if (Entregas != null)
                {
                    return Entregas.Where(x => x.Situacao == Enumeradores.SituacaoEntrega.Entregue || x.Situacao == Enumeradores.SituacaoEntrega.Reentergue).Count().ToString() + "/" + Entregas.Count().ToString();
                }
                else
                    return "";
            }
        }

        #endregion

        #region Dados Alertas

        public string DadosAlerta { set { if (value != null) DadosAlertas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dados>>(value); } }
        public List<Dados> DadosAlertas { get; set; }
        public virtual bool PossuiAlerta { get { return DadosAlertas != null && DadosAlertas.Count > 0; } }
        public virtual bool PossuiAlertaExibirTela
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    return DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false).Any();
                }
                else
                {
                    return false;
                }
            }
        }
        public virtual string DataUltimoAlerta
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    DateTime data = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .ThenBy(x => x.Alerta.AlertaCarga?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta.DataAlerta).FirstOrDefault();

                    return data != DateTime.MinValue ? data.ToString(_formatDataHora) : "";
                }
                else
                    return "";
            }
        }

        public virtual DateTime? DateTimeUltimoAlerta
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    DateTime data = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .ThenBy(x => x.Alerta.AlertaCarga?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta.DataAlerta).FirstOrDefault();
                    return data;
                }
                else
                    return null;
            }
        }

        public virtual Enumeradores.TipoAlertaCarga? TipoUltimoAlertaCarga
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    AlertaCarga alertaCargaAtendimento = BuscaUltimoAlertaCargaPorTipo(DadosAlertas, new List<Enumeradores.TipoAlertaCarga>() { Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, Enumeradores.TipoAlertaCarga.AtendimentoIniciado });
                    if (alertaCargaAtendimento != null)
                        return alertaCargaAtendimento.Tipo;

                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaCarga?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta).FirstOrDefault();

                    if (alerta?.AlertaCarga != null)
                        return alerta.AlertaCarga.Tipo;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        public virtual Enumeradores.TipoAlerta? TipoUltimoAlertaMonitoramento
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta).ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta).FirstOrDefault();
                    if (alerta?.AlertaMonitoramento != null)
                        return alerta.AlertaMonitoramento.Tipo;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        public virtual int CodigoUltimoAlerta
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    AlertaCarga alertaCargaAtendimento = BuscaUltimoAlertaCargaPorTipo(DadosAlertas, new List<Enumeradores.TipoAlertaCarga>() { Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, Enumeradores.TipoAlertaCarga.AtendimentoIniciado });
                    if (alertaCargaAtendimento != null)
                        return alertaCargaAtendimento.Codigo;

                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false).OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .ThenBy(x => x.Alerta.AlertaCarga?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta).FirstOrDefault();

                    if (alerta?.AlertaCarga != null)
                        return alerta.AlertaCarga.Codigo;
                    else if (alerta?.AlertaMonitoramento != null)
                        return alerta.AlertaMonitoramento.Codigo;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public virtual Enumeradores.AlertaMonitorStatus? StatusUltimoAlertaMonitoramento
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta).FirstOrDefault();

                    if (alerta?.AlertaMonitoramento != null)
                        return alerta.AlertaMonitoramento.Status;
                    else
                        return null;
                }
                else
                    return null;
            }
        }


        public virtual string DescricaoStatusUltimoAlertaMonitoramento
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta).FirstOrDefault();

                    if (alerta?.AlertaMonitoramento != null)
                        return alerta.AlertaMonitoramento.Status.ObterDescricao();
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        public virtual int QuantidadeAlertasCarga
        {
            get
            {
                int alertas = DadosAlertas?.Count() ?? 0;

                return alertas;
            }
        }

        public virtual string NomeResponsavelUltimoAlertaMonitoramento
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false).OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(alerta?.AlertaMonitoramento?.Funcionario))
                        return alerta.AlertaMonitoramento.Funcionario;
                    else if (alerta?.DataAlerta != null && alerta?.DataAlerta != DateTime.MinValue)
                        return getDataFormatadaExtenso(alerta.DataAlerta);
                    else
                        return "-";
                }
                else
                    return "-";
            }
        }

        public virtual string DescricaoTipoUltimoAlertaExibirTela
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    AlertaCarga alertaCargaAtendimento = BuscaUltimoAlertaCargaPorTipo(DadosAlertas, new List<Enumeradores.TipoAlertaCarga>() { Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, Enumeradores.TipoAlertaCarga.AtendimentoIniciado });
                    if (alertaCargaAtendimento != null)
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterDescricao(alertaCargaAtendimento.Tipo);

                    AlertaMonitoramento alertaMonitoramento = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta.AlertaMonitoramento).FirstOrDefault();

                    if (alertaMonitoramento != null)
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(alertaMonitoramento.Tipo);

                    AlertaCarga alertaCarga = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaCarga?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta.AlertaCarga).FirstOrDefault();
                    if (alertaCarga != null)
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterDescricao(alertaCarga.Tipo);

                    return "";
                }
                else
                    return "";
            }
        }

        public virtual bool EhAlertaParada { get; private set; }

        public virtual string IconeUltimoAlertaExibirTela
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    AlertaCarga alertaCargaAtendimento = BuscaUltimoAlertaCargaPorTipo(DadosAlertas, new List<Enumeradores.TipoAlertaCarga>() { Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, Enumeradores.TipoAlertaCarga.AtendimentoIniciado });
                    if (alertaCargaAtendimento != null)
                        return "Content/TorreControle/Icones/alertas/atendimento.svg";

                    AlertaMonitoramento alertaMonitoramento = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaMonitoramento?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta.AlertaMonitoramento).FirstOrDefault();

                    if (alertaMonitoramento != null)
                    {
                        switch (alertaMonitoramento.Tipo)
                        {
                            case Enumeradores.TipoAlerta.ParadaNaoProgramada:
                                EhAlertaParada = true;
                                return "Content/TorreControle/Icones/alertas/parada-excessiva.svg";

                            case Enumeradores.TipoAlerta.TemperaturaForaDaFaixa:
                            case Enumeradores.TipoAlerta.SensorTemperaturaComProblema:
                                return "Content/TorreControle/Icones/alertas/temperatura.svg";

                            case Enumeradores.TipoAlerta.VelocidadeExcedida:
                                return "Content/TorreControle/Icones/alertas/velocidade.svg";

                            case Enumeradores.TipoAlerta.AtrasoNaEntrega:
                            case Enumeradores.TipoAlerta.AtrasoNaDescarga:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlerta.AtrasoNoCarregamento:
                            case Enumeradores.TipoAlerta.AtrasoNaLiberacao:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlerta.SemSinal:
                            case Enumeradores.TipoAlerta.PerdaDeSinal:
                                return "Content/TorreControle/Icones/alertas/sem-sinal.svg";

                            case Enumeradores.TipoAlerta.DesvioDeRota:
                                return "Content/TorreControle/Icones/alertas/desvio-rota.svg";

                            case Enumeradores.TipoAlerta.ParadaEmAreaDeRisco:
                            case Enumeradores.TipoAlerta.ParadaExcessiva:
                                EhAlertaParada = true;
                                return "Content/TorreControle/Icones/alertas/parada-excessiva.svg";

                            case Enumeradores.TipoAlerta.InicioDeViagem:
                                return "Content/TorreControle/Icones/alertas/play-blue.svg";

                            case Enumeradores.TipoAlerta.FimDeViagem:
                                return "Content/TorreControle/Icones/alertas/fim-viagem-black.svg";

                            case Enumeradores.TipoAlerta.InicioEntrega:
                                return "Content/TorreControle/Icones/alertas/inicio-entrega.svg";

                            case Enumeradores.TipoAlerta.FimEntrega:
                                return "Content/TorreControle/Icones/alertas/fim-entrega.svg";

                            case Enumeradores.TipoAlerta.Pernoite:
                                return "Content/TorreControle/Icones/alertas/pernoite.svg";

                            case Enumeradores.TipoAlerta.DirecaoContinuaExcessiva:
                            case Enumeradores.TipoAlerta.DirecaoSemDescanso:
                                return "Content/TorreControle/Icones/alertas/direcao-problema.svg";

                            case Enumeradores.TipoAlerta.Almoco:
                                return "Content/TorreControle/Icones/alertas/almoco.svg";

                            case Enumeradores.TipoAlerta.Espera:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlerta.Repouso:
                                return "Content/TorreControle/Icones/alertas/repouso.svg";

                            case Enumeradores.TipoAlerta.Abastecimento:
                                return "Content/TorreControle/Icones/alertas/abastecimento.svg";

                            case Enumeradores.TipoAlerta.ChegadaNoRaio:
                                return "Content/TorreControle/Icones/alertas/chegada-raio-entrega.svg";

                            case Enumeradores.TipoAlerta.ChegadaNoRaioEntrega:
                                return "Content/TorreControle/Icones/alertas/chegada-raio-entrega.svg";

                            case Enumeradores.TipoAlerta.ConcentracaoDeVeiculosNoRaio:
                            case Enumeradores.TipoAlerta.PermanenciaNoRaio:
                                return "Content/TorreControle/Icones/alertas/permanencia-raio.svg";

                            case Enumeradores.TipoAlerta.PermanenciaNoRaioEntrega:
                                return "Content/TorreControle/Icones/alertas/permanencia-raio.svg";

                            case Enumeradores.TipoAlerta.ForaDoPrazo:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlerta.InicioViagemSemDocumentacao:
                                return "Content/TorreControle/Icones/alertas/inicio-viagem-problema.svg";

                            case Enumeradores.TipoAlerta.SensorDesengate:
                                return "Content/TorreControle/Icones/alertas/desengate.svg";

                            case Enumeradores.TipoAlerta.AusenciaDeInicioDeViagem:
                                return "Content/TorreControle/Icones/alertas/inicio-viagem-problema.svg";

                            case Enumeradores.TipoAlerta.PossivelAtrasoNaOrigem:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlerta.AlertaTendenciaEntregaAdiantada:
                                return "Content/TorreControle/Icones/alertas/tendencia-adiantamento.svg";

                            case Enumeradores.TipoAlerta.AlertaTendenciaEntregaAtrasada:
                                return "Content/TorreControle/Icones/alertas/tendencia-atraso.svg";

                            case Enumeradores.TipoAlerta.AlertaTendenciaEntregaPoucoAtrasada:
                                return "Content/TorreControle/Icones/alertas/tendencia-atraso.svg";

                            case Enumeradores.TipoAlerta.PermanenciaNoPontoApoio:
                                return "Content/TorreControle/Icones/alertas/permanencia-ponto-apoio.svg";

                            default:
                                return "Content/TorreControle/AcompanhamentoCarga/assets/icons/default.png";
                        }
                    }

                    AlertaCarga alertaCarga = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false)
                        .OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta)
                        .ThenBy(x => x.Alerta.DataAlerta)
                        .ThenBy(x => x.Alerta.AlertaCarga?.Status == AlertaMonitorStatus.EmAberto)
                        .Select(x => x.Alerta.AlertaCarga).FirstOrDefault();
                    if (alertaCarga != null)
                    {
                        switch (alertaCarga.Tipo)
                        {
                            case Enumeradores.TipoAlertaCarga.InicioViagem:
                                return "Content/TorreControle/Icones/alertas/play-blue.svg";

                            case Enumeradores.TipoAlertaCarga.FimViagem:
                                return "Content/TorreControle/Icones/alertas/fim-viagem-black.svg";

                            case Enumeradores.TipoAlertaCarga.ConfirmacaoColetaEntrega:
                                return "Content/TorreControle/Icones/gerais/icons-ok.svg";

                            case Enumeradores.TipoAlertaCarga.AtrasoInicioViagem:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlertaCarga.AtrasoColetaDescarga:
                                return "Content/TorreControle/Icones/alertas/espera.svg";

                            case Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda:
                            case Enumeradores.TipoAlertaCarga.AtendimentoIniciado:
                                return "Content/TorreControle/Icones/alertas/atendimento.svg";

                            case Enumeradores.TipoAlertaCarga.CargaSemTransportador:
                            case Enumeradores.TipoAlertaCarga.CagraSemVeiculo:
                                return "Content/TorreControle/Icones/alertas/carga-problema.png";

                            case Enumeradores.TipoAlertaCarga.VeiculoComInsumos:
                            case Enumeradores.TipoAlertaCarga.VeiculoNaoMonitorado:
                                return "Content/TorreControle/Icones/alertas/veiculo.svg";

                            case Enumeradores.TipoAlertaCarga.ValidacaoGerenciadoraRisco:
                                return "Content/TorreControle/Icones/alertas/perigo.svg";

                            case Enumeradores.TipoAlertaCarga.AntecedenciaGrade:
                                return "Content/TorreControle/Icones/alertas/antecedencia.svg";

                            default:
                                return "Content/TorreControle/AcompanhamentoCarga/assets/icons/default.png";
                        }
                    }

                    return "";
                }
                else
                    return "";
            }
        }

        public virtual bool PossuiAlertaTemperaturaForaFaixa { get { return DadosAlertas != null && DadosAlertas.Where(x => x.Alerta != null).Any(x => x.Alerta.AlertaMonitoramento?.Tipo == Enumeradores.TipoAlerta.TemperaturaForaDaFaixa); } }
        public virtual bool PossuiAlertaVelocidade { get { return DadosAlertas != null && DadosAlertas.Where(x => x.Alerta != null).Any(x => x.Alerta.AlertaMonitoramento?.Tipo == Enumeradores.TipoAlerta.VelocidadeExcedida); } }
        public virtual bool PossuiAlertaParadaExcessiva { get { return DadosAlertas != null && DadosAlertas.Where(x => x.Alerta != null).Any(x => x.Alerta.AlertaMonitoramento?.Tipo == Enumeradores.TipoAlerta.ParadaExcessiva); } }
        public virtual bool PossuiAlertaParadaNaoProgramada { get { return DadosAlertas != null && DadosAlertas.Where(x => x.Alerta != null).Any(x => x.Alerta.AlertaMonitoramento?.Tipo == Enumeradores.TipoAlerta.ParadaNaoProgramada); } }
        public virtual bool PossuiAlertaInicioViagemSemDocumentacao { get { return DadosAlertas != null && DadosAlertas.Where(x => x.Alerta != null).Any(x => x.Alerta.AlertaMonitoramento?.Tipo == Enumeradores.TipoAlerta.InicioViagemSemDocumentacao); } }

        public virtual string DataPrevisaoInicioViagem
        {
            get
            {
                if (DataCarregamentoCarga != null)
                {
                    DateTime data = (DateTime)(DataCarregamentoCarga?.AddMinutes(TempoCarregamento));
                    return data.ToString(_formatDataHora) ?? string.Empty;
                }
                else
                    return string.Empty;
            }
        }
        #endregion

        public static string FormatarTempo(TimeSpan tempo)
        {
            string formato = String.Empty;
            if (tempo.Days > 0 || tempo.Days < 0)
            {
                if (tempo.Days < 0)
                    formato = $"{tempo.Days * -1}d ";
                else
                    formato = $"{tempo.Days}d ";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }

        public static bool EmDeslocamentoPlanta(Monitoramento monitoramento)
        {
            if (monitoramento.TipoRegraViagem == Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta || monitoramento.TipoRegraViagem == Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento)
                return true;
            else
                return false;
        }

        public virtual string Cor { get; set; }
        public virtual string BuscarCor(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configTMS)
        {
            if (configTMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                return !string.IsNullOrEmpty(GrupoTipoOperacaoCor) ? GrupoTipoOperacaoCor : "";
            else
            {
                if (Monitoramento != null && !string.IsNullOrEmpty(Monitoramento[0].GrupoStatusViagemCor))
                    return Monitoramento[0].GrupoStatusViagemCor;
                else
                    return "";
            }

            //return "";
        }

        public virtual bool OnlineStatus(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configTMS)
        {
            if (Monitoramento != null && Monitoramento[0].DataUltimaPosicao != null)
            {
                DateTime? dataPosicao = Monitoramento[0].DataUltimaPosicao;

                return (dataPosicao != null && (DateTime.Now - dataPosicao).Value.TotalMinutes <= configTMS.TempoSemPosicaoParaVeiculoPerderSinal);
            }
            else
                return false;
        }

        public AlertaCarga BuscaUltimoAlertaCargaPorTipo(List<Dados> dadosAlertas, List<Enumeradores.TipoAlertaCarga> tipos)
        {
            return dadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false && tipos.Contains(x.Alerta?.AlertaCarga?.Tipo ?? (Enumeradores.TipoAlertaCarga)0)).OrderByDescending(x => x.Alerta.MonitoramentoEvento?.PrioridadeAlerta).ThenBy(x => x.Alerta.DataAlerta).Select(x => x.Alerta.AlertaCarga).FirstOrDefault();
        }

        public virtual bool ExibirNivelBateria { get; set; }

        public virtual bool PedidoEmOutrasCargas { get; set; }

        public bool HabilitarPreViagemTrizy { get; set; }

        private string getDataFormatadaExtenso(DateTime data)
        {
            if (data != DateTime.MinValue)
            {
                TimeSpan ts = DateTime.Now - data;

                const int second = 1;
                const int minute = 60 * second;
                const int hour = 60 * minute;
                const int day = 24 * hour;
                const int month = 30 * day;
                double delta = Math.Abs(ts.TotalSeconds);
                if (delta < 1 * minute) return "Há " + (ts.Seconds == 1 ? "um segundo" : ts.Seconds + " segundos");
                if (delta < 2 * minute) return "Há um minuto";
                if (delta < 45 * minute) return "Há " + ts.Minutes + " minutos";
                if (delta < 90 * minute) return "Há uma hora";
                if (delta < 24 * hour) return "Há " + ts.Hours + " horas";
                if (delta < 48 * hour) return "Ontem";
                if (delta < 30 * day) return "Há " + ts.Days + " dias";
                if (delta < 12 * month)
                {
                    var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return "Há " + (months <= 1 ? "um mês" : months + " meses");
                }
                else
                {
                    var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return "Há " + (years <= 1 ? "um ano" : years + " anos");
                }
            }
            else
                return "";
        }

        public string Mesoregiao { get; set; }

        public string Regiao { get; set; }

    }
}
