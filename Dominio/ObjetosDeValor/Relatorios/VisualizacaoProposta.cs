using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class VisualizacaoProposta
    {
        public DateTime Data { get; set; }

        public string ClienteCPFCNPJ { get; set; }
        public string ClienteNome { get; set; }
        public string Cliente
        {
            get
            {
                string cpfcnpj = this.ClienteCPFCNPJ.Length == 14 ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.ClienteCPFCNPJ) : String.Format(@"{0:000\.000\.000\-00}", this.ClienteCPFCNPJ);
                return this.ClienteNome + " - " + cpfcnpj;
            }
        }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public string Nome { get; set; }

        public string TipoColeta { get; set; }

        public Dominio.Enumeradores.ModalProposta ModalProposta { get; set; }
        
        public decimal Peso { get; set; }

        public string TipoVeiculo { get; set; }

        public string TipoCarga { get; set; }

        public int Volumes { get; set; }

        public string Dimensoes { get; set; }

        public string TipoCarroceria { get; set; }

        public string Rastreador { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string ClienteOrigem { get; set; }

        public string ClienteDestino { get; set; }

        public string Observacoes { get; set; }

        public decimal ValorMercadoria { get; set; }

        public string UnidadeMonetaria { get; set; }

        public virtual string DescricaoTipoVeiculo
        {
            get
            {
                switch (this.TipoVeiculo)
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
                    case "07":
                        return "VUC";
                    case "08":
                        return "3/4";
                    case "09":
                        return "Carreta";
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
                switch (this.TipoCarroceria)
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

        public string DescricaoModalProposta
        {
            get
            {
                switch (this.ModalProposta)
                {
                    case Enumeradores.ModalProposta.Rodoviario:
                        return "Rodoviário";
                    case Enumeradores.ModalProposta.Rodoaereo:
                        return "Rodoaereo";
                    case Enumeradores.ModalProposta.Outros:
                        return "Outros";
                    default:
                        return "";
                }
            }
        }
    }
}