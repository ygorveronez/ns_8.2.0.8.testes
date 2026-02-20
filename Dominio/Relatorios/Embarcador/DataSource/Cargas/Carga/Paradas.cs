using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class Paradas
    {
        #region Propriedades
        public int Codigo { get; set; }

        public string Filial { get; set; }

        public string Carga { get; set; }

        public string TipoOperacao { get; set; }

        public DateTime DataCriacaoCarga { get; set; }

        public string Transportador { get; set; }

        public string Motoristas { get; set; }

        public string Placas { get; set; }

        public int SituacaoEntrega { get; set; }
        public int OrigemSituacaoEntrega { get; set; }

        public int OrdemPrevista { get; set; }

        public int OrdemExecutada { get; set; }

        public int Aderencia { get; set; }

        public double CPFCNPJCliente { get; set; }

        public string TipoCliente { get; set; }

        public string Cliente { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string CEP { get; set; }

        public DateTime DataChegadaCliente { get; set; }

        public DateTime DataSaidaCliente { get; set; }

        public DateTime DataEntrega { get; set; }

        public string NotasFiscais { get; set; }

        public string Pedidos { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public decimal PesoBruto { get; set; }

        public decimal Quantidades { get; set; }

        public decimal Volumes { get; set; }

        private DateTime DataHoraAvaliacao { get; set; }

        public int ResultadoAvaliacao { get; set; }

        public string MotivoAvaliacao { get; set; }

        public string ObservacaoAvaliacao { get; set; }

        private TipoArea TipoArea { get; set; }
        private int RaioCliente { get; set; }
        private string LatitudeCliente { get; set; }
        private string LongitudeCliente { get; set; }
        private decimal LatitudeEntregaFinalizada { get; set; }
        private decimal LongitudeEntregaFinalizada { get; set; }
        public bool TipoParada { get; set; }
        private bool RealizadaNoPrazo { get; set; }
        public decimal ValorTotalNotas { get; set; }
        public string CodigoIntegracaoCliente { get; set; }

        public decimal KMPlanejado { get; set; }

        public decimal KMRealizado { get; set; }

        private DateTime DataInicioViagem { get; set; }

        private DateTime DataConfirmacaoChegada { get; set; }

        private DateTime DataInicioCarregamento { get; set; }

        private DateTime DataTerminoCarregamento { get; set; }

        private DateTime DataInicioDescarga { get; set; }

        private DateTime DataTerminoDescarga { get; set; }

        private DateTime DataFimViagem { get; set; }
        private DateTime DataCarregamento { get; set; }

        public string ConfirmacaoViaApp { get; set; }
        private bool EncerramentoManualViagem { get; set; }
        public DateTime PrevisaoEntregaCliente { get; set; }
        public DateTime PrevisaoEntregaTransportador { get; set; }
        public bool DesconsiderarHorarioPrazo { get; set; }
        public DateTime AgendamentoEntregaTransportador { get; set; }
        public DateTime AgendamentoEntregaCliente { get; set; }
        public DateTime DataPrimeiroEspelhamento { get; set; }
        public DateTime DataUltimoEspelhamento { get; set; }
        public string UsuarioFinalizador { get; set; }
        public string ModeloVeicular { get; set; }

        public double CPFCNPJRemetente { get; set; }

        public string ClienteRemetente { get; set; }

        public string EnderecoRemetente { get; set; }

        public string BairroRemetente { get; set; }

        public string CidadeRemetente { get; set; }

        public string EstadoRemetente { get; set; }

        public string CEPRemetente { get; set; }
        public int RaioMedioViagem { get; set; }
        public string QuantidadeAnimais { get; set; }
        public string QuantidadeMortalidade { get; set; }
        public DateTime DataInicioMonitoramento { get; set; }
        public DateTime DataFimMonitoramento { get; set; }
        private decimal PercentualViagemMonitoramento { get; set; }
        private string LongitudeUltimaPosicao { get; set; }
        private string LatitudeUltimaPosicao { get; set; }
        public string Rota { get; set; }
        public int OrigemSituacaoEntregaFinalizada { get; set; }
        public string MotivoFimMonitoramento { get; set; }
        public string Transbordo { get; set; }
        public DateTime DataConfirmacaoEntrega { get; set; }
        public DateTime DataColeta { get; set; }
        public string MotivoRetificacao { get; set; }
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus MonitoramentoStatus { get; set; }

        public string DescricaoMonitoramentoStatus
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusHelper.ObterDescricao(this.MonitoramentoStatus); }
        }

        public string ProximaCargaProgramada { get; set; }

        private readonly string _padraoData = "dd/MM/yyyy HH:mm:ss";
        #endregion

        #region Propriedades com Regras
        public string MotivoFimMonitoramentoDescricao => !string.IsNullOrWhiteSpace(MotivoFimMonitoramento) ? MotivoFinalizacaoMonitoramentoHelper.ObterDescricao((MotivoFinalizacaoMonitoramento)Convert.ToInt32(MotivoFimMonitoramento)) : string.Empty;

        public string OrigemMonitoramentoDescricao => OrigemSituacaoEntregaFinalizada > 0 ? OrigemSituacaoEntregaHelper.ObterDescricao((OrigemSituacaoEntrega)OrigemSituacaoEntregaFinalizada) : string.Empty;
        public string PercentualViagemMonitoramentoDescricao => PercentualViagemMonitoramento > 0 ? PercentualViagemMonitoramento.ToString("n2") : string.Empty;
        public string LongitudeUltimaPosicaoDescricao => !string.IsNullOrWhiteSpace(LongitudeUltimaPosicao) ? LongitudeUltimaPosicao.ToString() : string.Empty;
        public string LatitudeUltimaPosicaoDescricao => !string.IsNullOrWhiteSpace(LatitudeUltimaPosicao) ? LatitudeUltimaPosicao.ToString() : string.Empty;

        public string EntregaForaDoRaioFormatada
        {
            get
            {
                if ((string.IsNullOrWhiteSpace(LatitudeCliente)) || (string.IsNullOrWhiteSpace(LongitudeCliente)) || this.LatitudeEntregaFinalizada == 0 || this.LongitudeEntregaFinalizada == 0)
                    return string.Empty;
                else
                {
                    if (TipoArea == TipoArea.Raio)
                    {
                        double distancia = CalcularDistancia(Double.Parse(LatitudeCliente), Double.Parse(LongitudeCliente), (double)LatitudeEntregaFinalizada, (double)LongitudeEntregaFinalizada);
                        int raio = (RaioCliente > 0) ? this.RaioCliente : 300;
                        if (distancia > raio)
                            return "Sim";
                        else
                            return "Não";
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public string TempoPermanencia
        {
            get
            {
                TimeSpan tempoPermanecia = DataSaidaCliente - DataChegadaCliente;

                if (tempoPermanecia.TotalSeconds <= 0)
                    return "";

                return $"{((int)tempoPermanecia.TotalHours).ToString().PadLeft(2, '0')}h:{tempoPermanecia.Minutes.ToString().PadLeft(2, '0')}m";
            }
        }

        public string SituacaoEntregaDescricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterDescricao((Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega)SituacaoEntrega); }
        }

        public string OrigemSituacaoEntregaDescricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntregaHelper.ObterDescricao((Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega)OrigemSituacaoEntrega); }
        }

        public string CPFCNPJClienteFormatado
        {
            get { return CPFCNPJCliente.ToString().ObterCpfOuCnpjFormatado(TipoCliente); }
        }

        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString(_padraoData) : string.Empty; }
        }

        public string DataChegadaClienteFormatada
        {
            get { return DataChegadaCliente != DateTime.MinValue ? DataChegadaCliente.ToString(_padraoData) : string.Empty; }
        }

        public string DataSaidaClienteFormatada
        {
            get { return DataSaidaCliente != DateTime.MinValue ? DataSaidaCliente.ToString(_padraoData) : string.Empty; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString(_padraoData) : string.Empty; }
        }

        public string DataHoraAvaliacaoFormatada
        {
            get { return DataHoraAvaliacao != DateTime.MinValue ? DataHoraAvaliacao.ToString(_padraoData) : string.Empty; }
        }

        public string TipoParadaFormatada
        {
            get
            {
                return TipoParada ? "Coleta" : "Entrega";
            }
        }

        public string RealizadaNoPrazoFormatada
        {
            get
            {
                return RealizadaNoPrazo ? "Sim" : "Não";
            }
        }

        public string DataInicioViagemFormatada
        {
            get { return DataInicioViagem != DateTime.MinValue ? DataInicioViagem.ToString(_padraoData) : string.Empty; }
        }

        public string DataConfirmacaoChegadaFormatada
        {
            get { return DataConfirmacaoChegada != DateTime.MinValue ? DataConfirmacaoChegada.ToString(_padraoData) : string.Empty; }
        }

        public string DataInicioCarregamentoFormatada
        {
            get { return DataInicioCarregamento != DateTime.MinValue ? DataInicioCarregamento.ToString(_padraoData) : string.Empty; }
        }

        public string DataTerminoCarregamentoFormatada
        {
            get { return DataTerminoCarregamento != DateTime.MinValue ? DataTerminoCarregamento.ToString(_padraoData) : string.Empty; }
        }

        public string DataInicioDescargaFormatada
        {
            get { return DataInicioDescarga != DateTime.MinValue ? DataInicioDescarga.ToString(_padraoData) : string.Empty; }
        }

        public string DataTerminoDescargaFormatada
        {
            get { return DataTerminoDescarga != DateTime.MinValue ? DataTerminoDescarga.ToString(_padraoData) : string.Empty; }
        }

        public string DataFimViagemFormatada
        {
            get { return DataFimViagem != DateTime.MinValue ? DataFimViagem.ToString(_padraoData) : string.Empty; }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString(_padraoData) : string.Empty; }
        }

        public string DataConfirmacaoEntregaFormatada
        {
            get { return DataConfirmacaoEntrega != DateTime.MinValue ? DataConfirmacaoEntrega.ToString(_padraoData) : string.Empty; }
        }

        public string EncerramentoManualViagemFormatado
        {
            get { return EncerramentoManualViagem ? "Sim" : "Não"; }
        }
        public string PrazoEntregaCliente
        {
            get
            {
                string ret = string.Empty;
                DateTime dataBase = (DesconsiderarHorarioPrazo && AgendamentoEntregaCliente == DateTime.MinValue) ? DataEntrega.Date : DataEntrega;
                if (PrevisaoEntregaCliente != DateTime.MinValue && dataBase != DateTime.MinValue)
                    ret = dataBase <= (DesconsiderarHorarioPrazo && AgendamentoEntregaCliente == DateTime.MinValue ? PrevisaoEntregaCliente.Date : PrevisaoEntregaCliente) ? "No Prazo" : "Fora do Prazo";
                return ret;
            }
        }
        public string PrazoEntregaTransportador
        {
            get
            {
                string ret = string.Empty;
                DateTime dataBase = (DesconsiderarHorarioPrazo && AgendamentoEntregaTransportador == DateTime.MinValue) ? DataEntrega.Date : DataEntrega;
                if (PrevisaoEntregaTransportador != DateTime.MinValue && dataBase != DateTime.MinValue)
                    ret = dataBase <= (DesconsiderarHorarioPrazo && AgendamentoEntregaTransportador == DateTime.MinValue ? PrevisaoEntregaTransportador.Date : PrevisaoEntregaTransportador) ? "No Prazo" : "Fora do Prazo";
                return ret;
            }
        }

        public string CPFCNPJRemetenteFormatado
        {
            get { return CPFCNPJRemetente.ToString().ObterCpfOuCnpjFormatado(TipoCliente); }
        }

        public string DataInicioMonitoramentoFormatada
        {
            get { return DataInicioMonitoramento != DateTime.MinValue ? DataInicioMonitoramento.ToString(_padraoData) : string.Empty; }
        }

        public string DataFimMonitoramentoFormatada
        {
            get { return DataFimMonitoramento != DateTime.MinValue ? DataFimMonitoramento.ToString(_padraoData) : string.Empty; }
        }

        public string DataPrimeiroEspelhamentoFormatada
        {
            get { return DataPrimeiroEspelhamento != DateTime.MinValue ? DataPrimeiroEspelhamento.ToString(_padraoData) : string.Empty; }
        }

        public string DataUltimoEspelhamentoFormatada
        {
            get { return DataUltimoEspelhamento != DateTime.MinValue ? DataUltimoEspelhamento.ToString(_padraoData) : string.Empty; }
        }

        public string AgendamentoEntregaClienteFormatada
        {
            get { return AgendamentoEntregaCliente != DateTime.MinValue ? AgendamentoEntregaCliente.ToString(_padraoData) : string.Empty; }
        }

        public string DataColetaFormatada
        {
            get { return DataColeta != DateTime.MinValue ? DataColeta.ToString(_padraoData) : string.Empty; }
        }

        #endregion


        #region Métodos Privados

        private static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1 == lat2 && lon1 == lon2)
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2deg(dist);
                dist = dist * 60 * 1.1515;
                dist = dist * 1.609344 * 1000;//Metros
                return dist;
            }
        }
        private static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        private static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        #endregion
    }
}
