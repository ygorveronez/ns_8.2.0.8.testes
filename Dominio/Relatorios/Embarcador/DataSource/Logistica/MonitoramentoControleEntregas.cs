using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class MonitoramentoControleEntregas
    {
        #region Propriedades
        public string Pedido { get; set; }
        public int Carga { get; set; }
        public string CargaCodigoEmbarcador { get; set; }
        public string Cliente { get; set; }
        public double CodCliente { get; set; }
        public DateTime DataMonitoramento { get; set; }
        public bool Coleta { get; set; }
        public int CodEntrega { get; set; }
        public string NotasFiscais { get; set; }
        public int Transp { get; set; }
        public string NomeTransportador { get; set; }
        public string NomeFantasiaTransportador { get; set; }
        public string PlacaVeiculo { get; set; }
        public int LocalizacaoCliente { get; set; }
        public DateTime DataEntradaRaio { get; set; }
        public DateTime DataSaidaRaio { get; set; }
        public DateTime DataCarregamento { get; set; }
        public DateTime DataEntregaPrevista { get; set; }
        public int TempoRaio { get; set; }
        public int CodMonitoramento { get; set; }
        public decimal KmOrigemDestino { get; set; }
        public int CodVeiculo { get; set; }
        public int TempoViagem { get; set; }
        public int TotalTemperaturasRecebidas { get; set; }
        public int TotalTemperaturasDentroFaixa { get; set; }
        public DateTime DataInicioEventoSemSinal { get; set; }
        public DateTime DataFimEventoSemSinal { get; set; }
        public DateTime DataPosicaoAtual { get; set; }
        public string TipoOperacao { get; set; }

        #endregion

        #region Propriedades com Regras


        public string TipoEntrega
        {
            get { return Coleta ? "Coleta" : "Entrega"; }
        }

        public string TempoLocalFormatado
        {
            get { return ConverterTempo(TempoRaio); }
        }

        public string TempoTransito
        {
            get { return ConverterTempo(TempoViagem); }
        }

        public string Status
        {
            //TODO: mais q 5 horas.
            get { return TempoRaio > 300 ? "Atrasado" : "No Prazo"; }
        }

        public string DistanciaOrigemDestino
        {
            //TODO: mais q 5 horas.
            get { return KmOrigemDestino > 0 ? KmOrigemDestino.ToString("N2") + " km" : ""; }
        }


        private string ConverterTempo(int minutos)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            TimeSpan time = TimeSpan.FromMinutes(minutos);
            return time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");

            //DateTime dateTime = Convert.ToDateTime(time.ToString(), culture);
            //return dateTime.ToString("hh:mm:ss");
        }


        public string DataMonitoramentoFormatada
        {
            get { return DataMonitoramento != DateTime.MinValue ? DataMonitoramento.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
        public string DataSaidaRaioFormatada
        {
            get { return DataSaidaRaio != DateTime.MinValue ? DataSaidaRaio.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
        public string DataEntradaRaioFormatada
        {
            get { return DataEntradaRaio != DateTime.MinValue ? DataEntradaRaio.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
        public string DataEntregaPrevistaFormatada
        {
            get { return DataEntregaPrevista != DateTime.MinValue ? DataEntregaPrevista.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }
        public string PedidoFormatado
        {
            get { return !string.IsNullOrEmpty(Pedido) ? (Pedido.Contains('_') ? Pedido.Split('_')[1].ToString() : Pedido) : Pedido; }
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

        public string PercentualSinal
        {
            get
            {
                if (DataPosicaoAtual != DateTime.MinValue && TempoViagem > 0)
                {
                    if (DataInicioEventoSemSinal != DateTime.MinValue)
                    {
                        //teve eventos de sem sinal.
                        TimeSpan tempoSemSinal = DataInicioEventoSemSinal - DataFimEventoSemSinal;

                        int percentual = (tempoSemSinal.Minutes * 100) / TempoViagem;
                        return decimal.Round(percentual, 2) + "%";
                    }
                    else
                        return "100%";
                }
                else
                    return "";
            }
        }

        #endregion
    }
}
