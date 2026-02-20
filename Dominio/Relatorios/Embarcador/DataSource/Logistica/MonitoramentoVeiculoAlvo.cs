using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoVeiculoAlvo
    {
        private string OberDataFormatada(DateTime data)
        {
            if (data != DateTime.MinValue)
                return data.ToString("dd/MM/yyyy HH:mm");

            return "";
        }


        public int Codigo { get; set; }
        public DateTime DataDaRota { get; set; }

        public string DataDaRotaFormatada
        {
            get { return OberDataFormatada(DataDaRota); }
        }

        public int Rota { get; set; }
        public int SM { get; set; }
        public string PlacaVeiculo { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoBau { get; set; }
        public string CD { get; set; }
        public string UFOrigem { get; set; }
        public string Loja { get; set; }
        public string UFDestino { get; set; }
        public string Regiao { get; set; }
        public DateTime DataSaidaCD { get; set; }

        public string DataSaidaCDFormatada
        {
            get { return OberDataFormatada(DataSaidaCD); }
        }


        public DateTime DataEntradaLoja { get; set; }

        public string DataEntradaLojaFormatada
        {
            get { return OberDataFormatada(DataEntradaLoja); }
        }

        public DateTime DataSaidaLoja { get; set; }

        public string DataSaidaLojaFormatada
        {
            get { return OberDataFormatada(DataSaidaLoja); }
        }
        public string Permanencia
        {
            get
            {
                if (DataEntradaLoja != null)
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
    }
}
