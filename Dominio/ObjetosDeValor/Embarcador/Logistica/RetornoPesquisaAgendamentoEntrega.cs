using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class RetornoPesquisaAgendamentoEntrega
    {
        public string Codigo
        {
            get
            {
                if (string.IsNullOrEmpty(CodigoValue) && CodigoCargaEntrega > 0)
                    CodigoValue = CodigoCargaEntrega.ToString();
                if (string.IsNullOrEmpty(CodigoValue))
                    CodigoValue = Guid.NewGuid().ToString();
                return CodigoValue;
            }
        }
        public string CodigoValue { get; set; }

        public int CodigoCargaPedido { get; set; }

        public int CodigoCargaEntrega { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoPedido { get; set; }
        
        public string CodigoPedidoAgrupado { get; set; }

        public string Carga { get; set; }

        public string Cliente { get; set; }

        public string CodigoIntegracaoCliente { get; set; }

        public double CPFCNPJCliente { get; set; }

        public string Destino { get; set; }

        public string UFDestino { get; set; }

        public string TipoOperacao { get; set; }

        public string Transportador { get; set; }

        public bool ExigeAgendarEntregas { get; set; }

        public bool PermiteAgendarComViagemIniciada { get; set; }

        public DateTime DataCarregamentoInicial { get; set; }

        public DateTime DataCarregamentoFinal { get; set; }

        public DateTime DataSugestaoEntrega { get; set; }

        public DateTime DataAgendamento { get; set; }

        public DateTime DataPrevisaoEntrega { get; set; }

        public string ObservacaoReagendamento { get; set; }

        public SituacaoCarga SituacaoViagem { get; set; }

        public string NFe { get; set; }

        public SituacaoAgendamentoEntregaPedido SituacaoAgendamento { get; set; }

        public bool PermiteAgendarEntregaSomenteAposInicioViagem { get; set; }

        public SituacaoCarga SituacaoCarga { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime DataCriacaoPedido { get; set; }

        public int NumeroNotasFiscaisDoPedido { get; set; }

        public int QuantidadePedidosDaEntrega { get; set; }

        public bool PermitirAgendarDescargaAposDataEntregaSugerida { get; set; }

        public int QuantidadeVolumes { get; set; }

        public decimal QuantidadeMetrosCubicos { get; set; }

        public string SituacaoNotaFiscal { get; set; }

        public DateTime UltimaConsultaTransportador { get; set; }

        public bool ExigeAgendamento { get; set; }

        public string ObservacaoPedido { get; set; }

        public DateTime DataLiberadoAgendamento { get; set; }

        public string UsuarioAgendamento { get; set; }

        public DateTime DataUsuarioAssumiuAgendamento { get; set; }

        public string FormaAgendamento { get; set; }

        public string TelefoneCliente { get; set; }

        public string LinkAgendamento { get; set; }

        public string EmailAgendamento { get; set; }

        public DateTime DataTelaAgendamento { get; set; }

        public bool ObrigaCarga { get; set; }

        public DateTime DataPrevisaoSaida { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string NumeroOrdemPedido { get; set; }
        public string TempoAgendamento { get; set; }


        public string DescricaoExigeAgendamento
        {
            get
            {
                return ExigeAgendamento ? "Sim" : "Não";
            }
        }

        public string SituacaoAgendamentoDescricao
        {
            get
            {
                return SituacaoAgendamento.ObterDescricao();
            }
        }

        public string SituacaoViagemDescricao
        {
            get
            {
                return SituacaoViagem.ObterDescricao();
            }
        }

        public string ClienteDecricao
        {
            get
            {
                return $"{Cliente} - ({CPFCNPJCliente.ToString().ObterCnpjFormatado()})";
            }
        }

        public string QuantidadeMetrosCubicosFormatado
        {
            get
            {
                return QuantidadeMetrosCubicos.ToString("n4");
            }
        }

        public bool PermiteAgendarEntrega
        {
            get
            {
                return !PermiteAgendarEntregaSomenteAposInicioViagem || (DataCarregamentoFinal != DateTime.MinValue && DataCarregamentoFinal < DateTime.Now);
            }
        }

        public string DataSugestaoEntregaFormatada
        {
            get
            {
                return DataSugestaoEntrega != DateTime.MinValue ? DataSugestaoEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataCriacaoPedidoFormatada
        {
            get
            {
                return DataCriacaoPedido != DateTime.MinValue ? DataCriacaoPedido.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataCarregamentoInicialFormatada
        {
            get
            {
                return DataCarregamentoInicial != DateTime.MinValue ? DataCarregamentoInicial.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataCarregamentoFinalFormatada
        {
            get
            {
                return DataCarregamentoFinal != DateTime.MinValue ? DataCarregamentoFinal.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataAgendamentoFormatada
        {
            get
            {
                return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get
            {
                return DataPrevisaoEntrega != DateTime.MinValue ? DataPrevisaoEntrega.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataPrevisaoSaidaFormatada
        {
            get
            {
                return DataPrevisaoSaida != DateTime.MinValue ? DataPrevisaoSaida.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string StatusRecebimentoNotaFiscal
        {
            get
            {
                return NumeroNotasFiscaisDoPedido > 0 ? $"{NumeroNotasFiscaisDoPedido} nota(s) recebida(s)" : $"Nenhuma nota recebida";
            }
        }

        public string StatusRecebimentoNotaFiscalAgrupado
        {
            get
            {
                return $"{NumeroNotasFiscaisDoPedido} nota(s) / {QuantidadePedidosDaEntrega} pedido(s)";
            }
        }

        public string DT_RowColor
        {
            get
            {
                string cor;
                if (!ObrigaCarga)
                {
                    if (SituacaoAgendamento != SituacaoAgendamentoEntregaPedido.Agendado)
                    {
                        double horas = (DateTime.Now - DataTelaAgendamento).TotalHours;
                        cor = horas <= 5d ? "#00ff7e" : horas <= 10d ? "#FFDF00" : "#d94848";
                    }
                    else
                        cor = "";
                }
                else
                    cor = !PermiteAgendarEntrega ? "#c8e8ff" : DataSugestaoEntrega == DateTime.MinValue && PermitirAgendarDescargaAposDataEntregaSugerida ? "#ddd855" : "#ffffff";

                return cor;
            }
        }

        public string DT_FontColor
        {
            get
            {
                return !ExigeAgendarEntregas ? "#1cb512" : (PermiteAgendarComViagemIniciada ? "#7373ff" : "");
            }
        }

        public int Distancia { get; set; }

        public DateTime DataEntregaReprogramada { get; set; }

        public DateTime DataInicioViagem { get; set; }

        public DateTime DataEntregaPrevista { get; set; }

        public DateTime DataCriacaoCarga { get; set; }

        public DateTime DataCarregamento { get; set; }

        public DateTime DataInicioViagemPrevista { get; set; }

        public DateTime DataTerminoCarga { get; set; }

        public int PrevisaoEntregaVelocidadeMedia { get; set; }

        public string CargaJanelaCarregamento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega DataBaseCalculoPrevisaoControleEntrega { get; set; }

        public DateTime InicioCarregamento { get; set; }

        public DateTime? DataeHoraEnvioEmailAgendamento { get; set; }

        public string IconeEmailEnviado { get { return string.Empty; } }

        public string TempoPercurso
        {
            get
            {
                double velocidadeKmH = PrevisaoEntregaVelocidadeMedia > 0 ? PrevisaoEntregaVelocidadeMedia : 50d;

                double tempoDeslocamentoEmMinutos = ((Distancia > 0) && (velocidadeKmH > 0)) ? (((Distancia / 1000) / velocidadeKmH) * 60).RoundUp(0) : 0d;

                TimeSpan tempodeslocamento = TimeSpan.FromMinutes(tempoDeslocamentoEmMinutos);

                return $"{tempodeslocamento.Days}D {tempodeslocamento.Hours}Hrs {tempodeslocamento.Minutes}Min";
            }
        }

        public string TempoChegarNaEntrega
        {
            get
            {
                TimeSpan? resultado = null;

                if (DataInicioViagem != DateTime.MinValue)
                    resultado = (DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada : DataEntregaPrevista) - DataInicioViagem;
                else
                {
                    switch (DataBaseCalculoPrevisaoControleEntrega)
                    {
                        case DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                            resultado = (DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada : DataEntregaPrevista) - DataCriacaoCarga;
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                            resultado = (DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada : DataEntregaPrevista) - DataTerminoCarga;
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                            resultado = (DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada : DataEntregaPrevista) - (DataInicioViagemPrevista != DateTime.MinValue ? DataInicioViagemPrevista : DataTerminoCarga != DateTime.MinValue ? DataTerminoCarga : DataCriacaoCarga);
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                            resultado = (DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada : DataEntregaPrevista) - (DataCarregamento != DateTime.MinValue ? DataCarregamento : DataInicioViagemPrevista);
                            break;
                        case DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                            resultado = (DataEntregaReprogramada != DateTime.MinValue ? DataEntregaReprogramada : DataEntregaPrevista) - InicioCarregamento;
                            break;
                    }
                }

                return $"{resultado?.Days ?? 0}D {resultado?.Hours ?? 0}Hrs {resultado?.Minutes ?? 0}Min";
            }
        }

        public TipoAgendamentoEntrega TipoAgendamentoEntrega { get; set; }
        public string DescricaoTipoAgendamentoEntrega
        {
            get
            {
                return TipoAgendamentoEntrega.ObterDescricao();
            }
        }

        public string DataCriacaoCargaFormatada
        {
            get
            {
                return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public DateTime DataHoraSugestaoReagendamento { get; set; }
        public string DataHoraSugestaoReagendamentoFormatado
        {
            get
            {
                return DataHoraSugestaoReagendamento != DateTime.MinValue ? DataHoraSugestaoReagendamento.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public string DataCriacaoPedidoAgrupado { get; set; }
        
        public string ObservacaoPedidoAgrupado { get; set; }
        public bool ExigeSenhaAgendamento { get; set; }
        public string SenhaEntregaAgendamento { get; set; }
    }
}
