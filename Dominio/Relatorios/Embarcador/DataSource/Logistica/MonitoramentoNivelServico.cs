using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoNivelServico
    {
        #region Propriedades

        public int Codigo { get; set; }
        public DateTime DataDaCarga { get; set; }

        public string DataDaCargaFormatada
        {
            get { return ObterDataFormatada(DataDaCarga); }
        }

        public int Rota { get; set; }
        public int SM { get; set; }
        public string Carga { get; set; }
        public string PlacaVeiculo { get; set; }
        public string ModeloVeicular { get; set; }
        public string TipoRodado { get; set; }

        public string DescricaoTipoRodado
        {
            get
            {
                switch (TipoRodado)
                {
                    case "00":
                        return "Não Aplicado";
                    case "01":
                        return "Truck";
                    case "02":
                        return "Toco";
                    case "03":
                        return "Cavalo";
                    case "04":
                        return "Van";
                    case "05":
                        return "Utilitário";
                    case "06":
                        return "Outros";
                    default:
                        return "";
                }
            }
        }

        public string CD { get; set; }
        public string CDCodigo { get; set; }
        public string CDDescricao { get { return CDCodigo + " - " + CD; } }
        public string UFOrigem { get; set; }
        public string Loja { get; set; }
        public string LojaCodigo { get; set; }
        public string LojaDescricao { get { return LojaCodigo + " - " + Loja; } }
        public string UFDestino { get; set; }
        public string Regiao { get; set; }
        public DateTime DataSaidaCD { get; set; }

        public string DataSaidaCDFormatada
        {
            get { return ObterDataFormatada(DataSaidaCD); }
        }

        public DateTime DataEntradaLoja { get; set; }

        public string DataEntradaLojaFormatada
        {
            get { return ObterDataFormatada(DataEntradaLoja); }
        }

        public DateTime DataSaidaLoja { get; set; }

        public string DataSaidaLojaFormatada
        {
            get { return ObterDataFormatada(DataSaidaLoja); }
        }

        public string Permanencia
        {
            get
            {
                if (DataEntradaLoja != DateTime.MinValue)
                {
                    DateTime dataIni = DataEntradaLoja;
                    DateTime dataFim = DataSaidaLoja != DateTime.MinValue ? DataSaidaLoja : DateTime.Now;
                    TimeSpan data = dataFim.Subtract(dataIni);

                    return string.Format("{0}:{1}:{2}", data.TotalHours.ToString("00"), data.Minutes.ToString("00"), data.Seconds.ToString("00"));
                }

                return "";
            }
        }

        public int PedidoCliente { get; set; }

        public string TipoDeTransporte { get; set; }

        public string Transportador { get; set; }

        public int NFS { get; set; }

        public string TipoCarga { get; set; }

        public string NomeMotorista { get; set; }

        public string JanelaDescarga { get; set; }

        public string NumeroTransporte { get; set; }

        private DateTime DataConfirmacaoDocumento { get; set; }

        public string DataConfirmacaoDocumentoFormatada
        {
            get { return ObterDataFormatada(DataConfirmacaoDocumento); }
        }

        #endregion

        #region Métodos Privados

        private string ObterDataFormatada(DateTime data)
        {
            return data != DateTime.MinValue ? data.ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }

        #endregion
    }
}
