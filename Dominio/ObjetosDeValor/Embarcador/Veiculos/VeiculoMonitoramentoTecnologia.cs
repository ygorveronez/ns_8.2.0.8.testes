using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class VeiculoMonitoramentoTecnologia
    {

        #region Métodos públicos

        public string Placa { get; set; }
        public string Tipo { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoRodado { get; set; }
        public string TipoCarroceria { get; set; }
        public string ModeloVeiculoDescricao { get; set; }
        public int Ano { get; set; }
        public string Transportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string Tecnologia { get; set; }
        public string Rastreador { get; set; }
        public string Terminal { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataPosicaoAtual { get; set; }
        public string DataPosicaoAtualFormatada { get { return getDataFormatada(DataPosicaoAtual); } }
        public int MonitoramentosFinalizados { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case "P":
                        return "Próprio";
                    case "T":
                        return "Terceiro";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoVeiculo
        {
            get
            {
                switch (TipoVeiculo)
                {
                    case "0":
                        return "Tração";
                    case "1":
                        return "Reboque";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoRodado
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

        public virtual string DescricaoTipoCarroceria
        {
            get
            {
                switch (TipoCarroceria)
                {
                    case "00":
                        return "Não Aplicado";
                    case "01":
                        return "Aberta";
                    case "02":
                        return "Fechada/Baú";
                    case "03":
                        return "Granel";
                    case "04":
                        return "Porta Container";
                    case "05":
                        return "Sider";
                    default:
                        return "";
                }
            }
        }

        #endregion

        #region Métodos privados

        private string getDataFormatada(DateTime? data = null)
        {
            if (data != null && data != DateTime.MinValue)
                return data?.ToString("dd/MM/yyyy HH:mm");
            return "";
        }

        #endregion

    }
}
