using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Monitoramento
    {
        private string DATE_MASK = "dd/MM/yyyy";
        private string DATE_HOUR_MASK = "dd/MM/yyyy HH:mm:ss";

        public int Codigo { get; set; }
        public int Carga { get; set; }
        public DateTime? Data { get; set; }
        public string DataFormatada { get { return Data?.ToString(DATE_MASK) ?? ""; } }
        public DateTime? DataInicioMonitoramento { get; set; }
        public string DataInicioMonitoramentoFormatada { get { return DataInicioMonitoramento?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataFimMonitoramento { get; set; }
        public string DataFimMonitoramentoFormatada { get { return DataFimMonitoramento?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string CargaEmbarcador { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public DateTime? DataCriacaoCarga { get; set; }
        public string DataCriacaoCargaFormatada { get { return DataCriacaoCarga?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataPrevisaoTerminoCarga { get; set; }
        public DateTime? DataInicioViagem { get; set; }
        public string DataInicioViagemFormatada { get { return DataInicioViagem?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataInicioViagemPrevista { get; set; }
        public DateTime? DataCarregamentoCarga { get; set; }
        public DateTime? DataInicioCarregamentoJanela { get; set; }
        public DateTime? DataPrevisaoChegada { get; set; }
        public DateTime? DataPrevisaoChegadaPlanta { get; set; }
        public string DataPrevisaoChegadaPlantaFormatada { get { return DataPrevisaoChegadaPlanta?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string RazaoSocialTransportador { get; set; }
        public string NomeFantasiaTransportador { get; set; }
        public string CodigoFilial { get; set; }
        public string Filial { get; set; }
        public string Motoristas { get; set; }
        public string CPFMotoristas { get; set; }
        public int Veiculo { get; set; }
        public string Tracao { get; set; }
        public virtual string Veiculos { get { return !string.IsNullOrEmpty(Reboques) ? Tracao + " - " + Reboques : Tracao; } }
        public string Reboques { get; set; }
        public int PossuiContratoFrete { get; set; }
        public string Posicao { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Velocidade { get; set; }
        public string Distancia { get; set; }
        public decimal DistanciaPrevista { get; set; }
        public virtual string DistanciaPrevistaFormatada { get { return String.Format("{0:n0}Km", DistanciaPrevista); } }
        public decimal DistanciaRealizada { get; set; }
        public decimal? DistanciaAteOrigem { get; set; }
        public decimal? DistanciaAteDestino { get; set; }
        public decimal? DistanciaTotal { get; set; }
        public int Ignicao { get; set; }
        public decimal? PesoTotalCarga { get; set; }
        public decimal? Temperatura { get; set; }
        public decimal? TemperaturaMonitoramento { get; set; }
        public decimal? NivelGPS { get; set; }
        public string DescricaoFaixaTemperatura { get; set; }
        public decimal? TemperaturaFaixaInicial { get; set; }
        public decimal? TemperaturaFaixaFinal { get; set; }
        public string IDEquipamento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus Status { get; set; }
        public string StatusViagem { get; set; }
        public string CorStatusViagem { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra TiporRegraViagem { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega TendenciaProximaParada { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega TendenciaColeta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega TendenciaEntrega { get; set; }
        public string TipoOperacao { get; set; }
        public string GrupoTipoOperacao { get; set; }
        public int CodigoGrupoTipoOperacao { get; set; }

        public DateTime DataUltimaCarga { get; set; }
        public DateTime DataPosicaoAtual { get; set; }
        public string DataPosicaoAtualFormatada { get { return GetDataFormatada(DataPosicaoAtual); } }
        public virtual decimal PercentualViagem { get; set; }
        public string Destinos { get; set; }
        public string DestinosPontoPassagem { get; set; }
        public string CodigoIntegracaoDestino { get; set; }
        public string Recebedor { get; set; }
        public string Expedidor { get; set; }
        public string CategoriasAlvos { get; set; }
        public int CodigoProximaEntrega { get; set; }
        public DateTime? DataEntregaReprogramadaProximaEntrega { get; set; }
        public string DataEntregaReprogramadaProximaEntregaFormatada
        {
            get
            {
                if (Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && TotalEntregas > 0 && EntregaAtual <= TotalEntregas)
                    return DataEntregaReprogramadaProximaEntrega?.ToString(DATE_HOUR_MASK);
                else
                    return "";
            }
        }
        public DateTime? DataEntregaPlanejadaProximaEntrega { get; set; }
        public string DataEntregaPlanejadaProximaEntregaFormatada { get { return DataEntregaPlanejadaProximaEntrega?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string ProximoDestino { get; set; }
        public int TotalColetas { get; set; }
        public int ColetaAtual { get; set; }
        public int TotalEntregas { get; set; }
        public int TotalEntregasEntregues { get; set; }
        public int TotalEntregasRejeitadas { get; set; }
        public int TotalEntregasAderencia { get; set; }
        public string CidadeDestino { get; set; }
        public int TotalEntregasNoRaio { get; set; }
        public int EntregaAtual { get; set; }
        public string Pedidos { get; set; }
        public decimal? ValorTotalNFe { get; set; }
        public DateTime? DataPrevisaoEntregaPedido { get; set; }
        public string DataPrevisaoEntregaPedidoFormatada { get { return DataPrevisaoEntregaPedido?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataPrevisaoDescargaJanela { get; set; }
        public string DataPrevisaoDescargaJanelaFormatada { get { return DataPrevisaoDescargaJanela?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public double RowNum { get; set; }
        private DateTime DataReagendamento { get; set; }
        public int TotalTemperaturasRecebidas { get; set; }
        public int TotalTemperaturasDentroFaixa { get; set; }
        public string NumeroEquipamentoRastreador { get; set; }
        public string NomeResponsavelVeiculo { get; set; }
        public string CPFResponsavelVeiculo { get; set; }
        public string CentroResultado { get; set; }
        public string FuncionarioResponsavel { get; set; }
        public string FronteiraRotaFrete { get; set; }
        public string NotasFiscais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador TecnologiaRastreador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora Gerenciadora { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracaoTecnologiaRastreador { get; set; }
        public string NumeroProtocoloIntegracaoCarga { get; set; }
        public string VersaoAppMotorista { get; set; }
        public string NumeroPedidoEmbarcadorSumarizado { get; set; }
        public DateTime DataInicioStatusAtual { get; set; }
        public DateTime DataFimStatusAtual { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroFrota { get; set; }
        public string TipoCarga { get; set; }
        public int Produtos { get; set; }
        public DateTime DataRealEntrega { get; set; }
        public string Subcontratado { get; set; }
        private DateTime? DataChegadaColeta { get; set; }
        public string DataChegadaColetaFormatada { get { return DataChegadaColeta?.ToString(DATE_HOUR_MASK) ?? ""; } }
        private DateTime? DataSaidaColeta { get; set; }
        public string DataSaidaColetaFormatada { get { return DataSaidaColeta?.ToString(DATE_HOUR_MASK) ?? ""; } }

        public string TempoStatusDescricao
        {
            get
            {
                if (Status == Enumeradores.MonitoramentoStatus.Iniciado)
                {
                    if (DataFimStatusAtual != DateTime.MinValue)
                        return FormatarTempo((DataFimStatusAtual - DataInicioStatusAtual));
                    else
                        return FormatarTempo((DateTime.Now - DataInicioStatusAtual));
                }
                else
                    return "";

            }
        }

        public int TempoStatusEmMinutos
        {
            get
            {
                if (Status == Enumeradores.MonitoramentoStatus.Iniciado)
                {
                    DateTime dataFim = (DataFimStatusAtual != DateTime.MinValue) ? DataFimStatusAtual : DateTime.Now;
                    return (int)(dataFim - DataInicioStatusAtual).TotalMinutes;
                }
                return 0;
            }
        }


        public string NomeRastreador
        {
            get
            {
                return Enumeradores.EnumTecnologiaRastreadorHelper.ObterDescricao(TecnologiaRastreador);
            }
        }

        public string Gerenciador
        {
            get
            {
                return Enumeradores.EnumTecnologiaGerenciadoraHelper.ObterDescricao(Gerenciadora);
            }
        }

        private string _observacao;
        public string Observacao
        {
            get
            {
                return string.IsNullOrEmpty(_observacao) ? string.Empty : _observacao;
            }
            set
            {
                _observacao = value;
            }
        }

        public string TemperaturaDentroFaixa
        {
            get
            {
                if (TotalTemperaturasRecebidas > 0)
                {
                    int percentual = (TotalTemperaturasDentroFaixa * 100) / TotalTemperaturasRecebidas;
                    return decimal.Round(percentual, 2) + "%";
                }
                else
                    return "";

            }
        }

        public string StatusIgnicao
        {
            get
            {
                if (Ignicao == 1)
                    return "Ligado";
                else
                    return "Desligado";
            }
        }

        public string DistanciaRotaPrevistaRealizada
        {
            get
            {
                if (DistanciaPrevista > 0 && DistanciaRealizada > 0)
                {
                    return string.Concat(decimal.Round(DistanciaPrevista, 2)).ToString() + "/" + (decimal.Round(DistanciaRealizada, 2).ToString());
                }
                else
                    return "";

            }
        }

        public string AderenciaSequencia
        {
            get
            {
                if (TotalEntregasEntregues > 0)
                {
                    int percentual = (TotalEntregasAderencia * 100) / TotalEntregasEntregues;
                    return decimal.Round(percentual, 2) + "%";
                }
                else
                    return "";

            }
        }

        public string AderenciaRaio
        {
            get
            {
                if (TotalEntregasEntregues > 0)
                {
                    int percentual = (TotalEntregasNoRaio * 100) / TotalEntregasEntregues;
                    return decimal.Round(percentual, 2) + "%";
                }
                else
                    return "";

            }
        }

        public string QuantidadeEntregas
        {
            get
            {
                if (Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && TotalEntregas > 0)
                    return TotalEntregas + "/" + TotalEntregasEntregues + "/" + TotalEntregasRejeitadas;
                else
                    return "";
            }
        }

        public string NumeroEXP { get; set; }
        public bool Critico { get; set; }
        public string ClienteOrigem { get; set; }
        public string CidadeOrigem { get; set; }
        public int TotalAlertas { get; set; }
        public int TotalAlertasTratados { get; set; }
        public int TotalAlertaTratativaEspecifica { get; set; }
        public string Alertas { get; set; }
        public string GrupoTipoOperacaoCor { get; set; }
        public int GrupoStatusViagemCodigo { get; set; }
        public string GrupoStatusViagemDescricao { get; set; }
        public string GrupoStatusViagemCor { get; set; }
        public int DiasUteisPrazoTransportador { get; set; }
        public bool UsarGrupoDeTipoDeOperacaoNoMonitoramento { get; set; }
        public int PossuiAlertaEmAberto { get; set; }
        public string Cor
        {
            get
            {
                if (UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                    return !string.IsNullOrEmpty(GrupoTipoOperacaoCor) ? GrupoTipoOperacaoCor : "";
                else
                    return !string.IsNullOrEmpty(GrupoStatusViagemCor) ? GrupoStatusViagemCor : "";
            }
        }
        public string Ordens { get; set; }
        public DateTime? DataSaidaOrigem { get; set; }
        public string DataSaidaOrigemFormatada { get { return DataSaidaOrigem?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataChegadaDestino { get; set; }
        public string DataChegadaDestinoFormatada { get { return DataChegadaDestino?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public DateTime? DataProgramadaColeta { get; set; }
        public string DataProgramadaColetaFormatada { get { return DataProgramadaColeta?.ToString(DATE_HOUR_MASK) ?? ""; } }
        public string ResponsavelVeiculo
        {
            get { return !string.IsNullOrWhiteSpace(NomeResponsavelVeiculo) ? $"{NomeResponsavelVeiculo} ({CPFResponsavelVeiculo.ObterCpfOuCnpjFormatado()})" : string.Empty; }
        }

        public string LocalTracking { get; set; }

        public DateTime? PrevisaoFimViagem { get; set; }

        public string PrevisaoFimViagemFormatada { get { return PrevisaoFimViagem?.ToString(DATE_HOUR_MASK) ?? ""; } }

        public DateTime? PrevisaoStopTranking { get; set; }
        public DateTime? DataPrimeiraPosicao { get; set; }
        public string DataPrimeiraPosicaoFormatada { get { return this.DataPrimeiraPosicao?.ToString(DATE_HOUR_MASK) ?? ""; } }

        public string PrevisaoStopTrankingFormatada { get { return PrevisaoStopTranking.HasValue ? PrevisaoStopTranking.Value.ToString(DATE_HOUR_MASK) ?? "" : ""; } }

        public DateTime? PrevisaoSaidaDestino { get; set; }

        public string PrevisaoSaidaDestinoFormatada { get { return PrevisaoSaidaDestino.HasValue ? PrevisaoSaidaDestino.Value.ToString(DATE_HOUR_MASK) ?? "" : ""; } }

        public DateTime? PrevisaoTerminoViagem { get; set; }

        public string PrevisaoTerminoViagemFormatada { get { return PrevisaoTerminoViagem.HasValue ? PrevisaoTerminoViagem.Value.ToString(DATE_HOUR_MASK) ?? "" : ""; } }
        public string TipoTrecho { get; set; }
        public int CodigoLocalRaiosProximidade { get; set; }
        public int CodigoRaioProximidade { get; set; }
        public string DescricaoRaioProximidade { get; set; }
        public int RaioRaioProximidade { get; set; }
        public string DescricaoLocalRaiosProximidade { get; set; }
        public string CorLocalRaiosProximidade { get; set; }

        public string DadosAlerta
        {
            set
            {
                if (value != null)
                {
                    DadosAlertas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dados>>(value);
                    if (DadosAlertas != null && DadosAlertas.Count > 0)
                    {
                        AlertaMonitoramento = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false).OrderByDescending(x => x.Alerta.DataAlerta).Select(x => x.Alerta.AlertaMonitoramento).FirstOrDefault();
                        MonitoramentoEvento = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.MonitoramentoEvento != null).Select(x => x.Alerta.MonitoramentoEvento).FirstOrDefault();
                    }
                }
            }
        }
        public List<Dados> DadosAlertas { get; set; }
        private AlertaMonitoramento AlertaMonitoramento { get; set; }
        private MonitoramentoEvento MonitoramentoEvento { get; set; }

        public virtual string DescricaoUltimoAlertaMonitoramento { get { return TipoAlertaHelper.ObterDescricao(AlertaMonitoramento?.Tipo ?? TipoAlerta.SemAlerta); } }
        public virtual string CorUltimoAlertaMonitoramento { get { return MonitoramentoEvento?.Cor ?? string.Empty; } }
        public virtual AlertaMonitorStatus StatusUltimoAlertaMonitoramento { get { return AlertaMonitoramento?.Status ?? AlertaMonitorStatus.EmAberto; } }
        public virtual string DescricaoStatusUltimoAlertaMonitoramento { get { return AlertaMonitoramento?.Status.ObterDescricao() ?? string.Empty; } }
        public virtual string CorStatusUltimoAlertaMonitoramento { get { return AlertaMonitorStatusHelper.ObterCorStatus(AlertaMonitoramento?.Status ?? AlertaMonitorStatus.Todos); } }
        public virtual Enumeradores.TipoAlerta TipoUltimoAlertaMonitoramento { get { return AlertaMonitoramento?.Tipo ?? Enumeradores.TipoAlerta.SemAlerta; } }

        public virtual string RetornoIntegracaoSM { get; set; }
        public virtual string SituacaoIntegracaoSM { get; set; }
        public virtual string SituacaoIntegracaoSMRetorno
        {
            get
            {
                if (string.IsNullOrEmpty(this.SituacaoIntegracaoSM))
                    return string.Empty;

                string[] situacaoIntegracaoSMRetorno = this.SituacaoIntegracaoSM?.Split(',');

                return string.Join(", ", (from situacao in situacaoIntegracaoSMRetorno select SituacaoIntegracaoHelper.ObterDescricao((SituacaoIntegracao)situacao?.ToInt())));
            }
        }
        public virtual string IconeUltimoAlertaExibirTela
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    AlertaCarga alertaCargaAtendimento = BuscaUltimoAlertaCargaPorTipo(DadosAlertas, new List<Enumeradores.TipoAlertaCarga>() { Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda, Enumeradores.TipoAlertaCarga.AtendimentoIniciado });
                    if (alertaCargaAtendimento != null)
                        return "Content/TorreControle/Icones/alertas/atendimento.svg";

                    if (AlertaMonitoramento != null)
                    {
                        switch (AlertaMonitoramento.Tipo)
                        {
                            case Enumeradores.TipoAlerta.ParadaNaoProgramada:
                                return "Content/TorreControle/Icones/alertas/parada-problema.svg";

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
                                return "Content/TorreControle/Icones/alertas/parada-problema.svg";

                            case Enumeradores.TipoAlerta.InicioDeViagem:
                                return "Content/TorreControle/Icones/alertas/play-blue.svg";

                            case Enumeradores.TipoAlerta.FimDeViagem:
                                return "Content/TorreControle/AcompanhamentoCarga/assets/icons/final-flag.svg";

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

                            case Enumeradores.TipoAlerta.PermanenciaNoPontoApoio:
                                return "Content/TorreControle/Icones/alertas/permanencia-ponto-apoio.svg";

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

        public AlertaCarga BuscaUltimoAlertaCargaPorTipo(List<Dados> dadosAlertas, List<Enumeradores.TipoAlertaCarga> tipos)
        {
            return dadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false && tipos.Contains(x.Alerta?.AlertaCarga?.Tipo ?? (Enumeradores.TipoAlertaCarga)0)).OrderByDescending(x => x.Alerta.DataAlerta).Select(x => x.Alerta.AlertaCarga).FirstOrDefault();
        }

        public virtual int CodigoUltimoAlerta
        {
            get
            {
                if (DadosAlertas != null && DadosAlertas.Count > 0)
                {
                    Alerta alerta = DadosAlertas.Where(x => x.Alerta != null && x.Alerta.AlertaTratado == false).OrderByDescending(x => x.Alerta.DataAlerta).Select(x => x.Alerta).FirstOrDefault();
                    if (alerta?.AlertaMonitoramento != null)
                        return alerta.AlertaMonitoramento.Codigo;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public string UltimaOcorrencia { get; set; }

        public TipoCobrancaMultimodal TipoModalTransporte { get; set; }

        public string ModalTransporte => TipoCobrancaMultimodalHelper.ObterDescricao(TipoModalTransporte);

        public string DataReagendamentoFormatada
        {
            get { return DataReagendamento != DateTime.MinValue ? DataReagendamento.ToString(DATE_HOUR_MASK) : string.Empty; }
        }

        public string AlertasAbertos
        {
            get { return string.Empty; }
        }

        public DateTime? DataAgendamentoPedido { get; set; }

        public string DataAgendamentoPedidoFormatada { get { return DataAgendamentoPedido.HasValue ? DataAgendamentoPedido.Value.ToString(DATE_HOUR_MASK) ?? "" : ""; } }

        public DateTime? DataCarregamentoPedido { get; set; }

        public string DataCarregamentoPedidoFormatada { get { return DataCarregamentoPedido.HasValue ? DataCarregamentoPedido.Value.ToString(DATE_HOUR_MASK) ?? "" : ""; } }

        public string MatrizComplementar { get; set; }

        public string EscritorioVendasComplementar { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public string CanalVenda { get; set; }

        public string DistanciaPercorrida { get; set; }

        public DateTime? DataCarregamento { get; set; }

        public string NumeroRastreador { get; set; }

        public string TendenciaProximaParadaDescricao
        {
            get
            {
                return TendenciaProximaParada == TendenciaEntrega.Nenhum ? "-" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(TendenciaProximaParada);
            }
        }
        public string TendenciaColetaDescricao
        {
            get
            {
                return TendenciaColeta == TendenciaEntrega.Nenhum ? "-" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(TendenciaColeta);
            }
        }
        public string TendenciaEntregaDescricao
        {
            get
            {
                return TendenciaEntrega == TendenciaEntrega.Nenhum ? "-" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(TendenciaEntrega);
            }
        }

        public string Coletas
        {
            get
            {
                return (Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && TotalColetas > 0 && ColetaAtual <= TotalColetas) ? ColetaAtual + "/" + TotalColetas : "";
            }
        }

        public string EntregasDescricao
        {
            get
            {
                return (Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado && TotalEntregas > 0 && EntregaAtual <= TotalEntregas) ? EntregaAtual + "/" + TotalEntregas : "";
            }
        }

        public string PossuiContratoFreteDescricao
        {
            get
            {
                return PossuiContratoFrete > 0 ? "Sim" : "Não";
            }
        }

        public string TemperaturaDescricao
        {
            get
            {
                return (Temperatura ?? TemperaturaMonitoramento).ToString();
            }
        }

        public string FaixaTemperaturaDescricao
        {
            get
            {
                return (DescricaoFaixaTemperatura != null && TemperaturaFaixaInicial != null && TemperaturaFaixaFinal != null) ? DescricaoFaixaTemperatura + $" ({TemperaturaFaixaInicial} a {TemperaturaFaixaFinal})" : "";
            }
        }

        public string ControleDeTemperatura
        {
            get
            {
                return (TemperaturaFaixaInicial != null && TemperaturaFaixaFinal != null) ? ObterControleTemperatura() : "Sem controle";
            }
        }

        public string LatitudeFormatada
        {
            get
            {
                return Latitude != 0 ? Latitude.ToString("f6") : "";
            }
        }

        public string LongitudeFormatada
        {
            get
            {
                return Longitude != 0 ? Longitude.ToString("f6") : "";
            }
        }

        public int PercentualViagemFormatado
        {
            get
            {
                return (int)PercentualViagem;
            }
        }

        public string DistanciaAteDestinoFormatada
        {
            get
            {
                return (DistanciaAteDestino != 0 && DistanciaAteDestino != null) ? DistanciaAteDestino?.ToString("f1") : "0Km";
            }
        }

        public string DistanciaPercorridaFormatada
        {
            get
            {
                return (DistanciaRealizada != 0) ? String.Format("{0:n0}Km", DistanciaRealizada) : "0Km";
            }
        }
        public DataBaseCalculoPrevisaoControleEntrega DataBaseCalculoPrevisaoControleEntrega { get; set; }

        public string DataCarregamentoFormatada
        {
            get
            {
                DateTime? data = DataCarregamentoCarga;

                if (data.HasValue)
                    return data.Value.ToString(DATE_HOUR_MASK);
                else
                    return string.Empty;
            }
        }

        public string DataCarregamentoFormatadaConfiguracao
        {
            get
            {
                DateTime? data;
                switch (DataBaseCalculoPrevisaoControleEntrega)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                        data = DataPrevisaoTerminoCarga;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                        data = DataInicioViagemPrevista;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                        data = DataCarregamentoCarga;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                        data = DataInicioCarregamentoJanela;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                    default:
                        data = DataCriacaoCarga;
                        break;
                }
                if (data.HasValue)
                    return data.Value.ToString(DATE_HOUR_MASK);
                else
                    return string.Empty;
            }
        }

        public virtual int DistanciaMaximaRotaCurta { get; set; }
        public string ClassificacaoRota
        {
            get
            {
                if (!DistanciaTotal.HasValue || DistanciaMaximaRotaCurta <= 0)
                    return string.Empty;
                else
                    return DistanciaTotal.Value <= DistanciaMaximaRotaCurta ? "Rota Curta" : "Rota Longa";
            }
        }

        public string CorTendenciaEntrega
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterCorTendenciaEntrega(TendenciaEntrega);
            }
        }

        public string LocalTrackingDescricao
        {
            get
            {
                return !string.IsNullOrEmpty(LocalTracking) ? LocalTracking : "";
            }
        }

        public string DescricaoSituacaoCarga
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterDescricao(SituacaoCarga);
            }
        }

        public string CorSituacaoCarga
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterCorMonitoramento(SituacaoCarga);
            }
        }
        public virtual int TempoSemPosicaoParaVeiculoPerderSinal { get; set; }

        public string Rastreador
        {
            get
            {
                return (DataPosicaoAtual != null && (DateTime.Now - DataPosicaoAtual).TotalMinutes <= TempoSemPosicaoParaVeiculoPerderSinal).ToString();
            }
        }


        public int StatusRastreador
        {
            get
            {
                return DataPosicaoAtual == null || DataPosicaoAtual == DateTime.MinValue ? 1 : (DataPosicaoAtual != null && (DateTime.Now - DataPosicaoAtual).TotalMinutes <= TempoSemPosicaoParaVeiculoPerderSinal) ? 3 : 4;
            }
        }

        public int RastreadorOnlineOffline
        {
            get
            {
                return DataPosicaoAtual == null || DataPosicaoAtual == DateTime.MinValue ? 1 : (DataPosicaoAtual != null && (DateTime.Now - DataPosicaoAtual).TotalMinutes <= TempoSemPosicaoParaVeiculoPerderSinal) ? 3 : 4;
            }
        }

        public string CorSemaforo
        {
            get
            {
                string verde = "78bf78";
                string amarelo = "fbe56d";
                string vermelho = "f2695b";
                string cor = verde;

                if (TotalAlertas > 0)
                {
                    if (TotalAlertas == TotalAlertasTratados)
                    {
                        if (TiporRegraViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando)
                            cor = vermelho;

                        if (TotalAlertaTratativaEspecifica > 0)
                            cor = amarelo;
                    }
                    else if (TotalAlertaTratativaEspecifica > 0)
                        cor = amarelo;
                    else
                        cor = vermelho;
                }

                if (TiporRegraViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando)
                    cor = vermelho;

                return cor;
            }
        }
        public string StatusViagemDescricao
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegraHelper.ObterDescricao(TiporRegraViagem);
            }
        }
        public string StatusDescricao
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusHelper.ObterDescricao(Status);
            }
        }

        public string Mesoregiao { get; set; }
        public string Regiao { get; set; }
        public bool? Parqueada { get; set; }
        public string ParqueadaDescricao
        {
            get
            {
                return Parqueada.HasValue ? Parqueada.Value ? "Sim" : "Não" : string.Empty;
            }
        }

        public string Vendedor { get; set; }

        public string Supervisor { get; set; }
        public int TempoPermitidoPermanenciaEmCarregamento { get; set; }
        public int TempoPermitidoPermanenciaNoCliente { get; set; }

        public List<MonitoramentoCargaEntrega> Entregas { get; set; }
        public string DadosEntregas
        {
            set
            {
                if (value != null)
                {
                    Entregas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MonitoramentoCargaEntrega>>(value);
                    if (Entregas?.Count > 0)
                    {
                        ProximaColeta = Entregas.OrderBy(entrega => entrega.OrdemPrevista).FirstOrDefault(entrega => entrega.Coleta && entrega.Situacao != SituacaoEntrega.Entregue);
                        ProximaEntrega = Entregas.OrderBy(entrega => entrega.OrdemPrevista).FirstOrDefault(entrega => !entrega.Coleta && entrega.Situacao != SituacaoEntrega.Entregue);
                        UltimaColeta = Entregas.OrderBy(entrega => entrega.OrdemPrevista).FirstOrDefault(entrega => entrega.Coleta && entrega.Situacao == SituacaoEntrega.Entregue);
                        UltimaEntrega = Entregas.OrderBy(entrega => entrega.OrdemPrevista).FirstOrDefault(entrega => entrega.Coleta && entrega.Situacao == SituacaoEntrega.Entregue);
                    }
                }
            }
        }

        public MonitoramentoCargaEntrega ProximaColeta { get; set; }
        public MonitoramentoCargaEntrega ProximaEntrega { get; set; }
        public MonitoramentoCargaEntrega UltimaColeta { get; set; }
        public MonitoramentoCargaEntrega UltimaEntrega { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo? DataRealizacaoDoEvento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo? DataPrevistaDoEvento { get; set; }
        public bool? UltimaColetaRealizadaNoPrazo { get; set; }
        public virtual string PrazoColetaDescricao
        {
            get
            {
                return UltimaColetaRealizadaNoPrazo.HasValue ? (UltimaColetaRealizadaNoPrazo.Value ? "Sim" : "Não") : "-";
            }
        }
        public bool? UltimaEntregaRealizadaNoPrazo { get; set; }
        public virtual string PrazoEntregaDescricao
        {
            get
            {
                return UltimaEntregaRealizadaNoPrazo.HasValue ? (UltimaEntregaRealizadaNoPrazo.Value ? "Sim" : "Não") : "-";
            }
        }

        public DateTime? DataAgendamentoParada { get; set; }

        public string DataAgendamentoParadaFormatada { get { return DataAgendamentoParada.HasValue ? DataAgendamentoParada.Value.ToString(DATE_HOUR_MASK) ?? "" : ""; } }

        #region Métodos privados
        private string GetDataFormatada(DateTime data)
        {
            if (data != DateTime.MinValue)
                return data.ToString(DATE_HOUR_MASK);
            return "";
        }

        private static string FormatarTempo(TimeSpan tempo)
        {
            string formato = String.Empty;
            if (tempo.Days > 0)
            {
                formato = $"{tempo.Days}d";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }

        private string ObterControleTemperatura()
        {
            if (Temperatura != null)
            {
                if (Temperatura >= TemperaturaFaixaInicial && Temperatura <= TemperaturaFaixaFinal)
                    return "Na faixa";
                else
                    return "Fora da faixa";
            }
            else if (TemperaturaMonitoramento != null)
            {
                if (TemperaturaMonitoramento >= TemperaturaFaixaInicial && TemperaturaMonitoramento <= TemperaturaFaixaFinal)
                    return "Na faixa";
                else
                    return "Fora da faixa";
            }
            else
                return "Fora da faixa";
        }

        #endregion
    }
}