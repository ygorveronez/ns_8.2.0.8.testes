using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesEmitidosComExpedidor
    {
        public int CodigoCTe { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public DateTime DataEmissao { get; set; }

        public string ChaveCTe { get; set; }

        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }

        public string NomeTransportador { get; set; }

        public string CNPJTransportador { get; set; }

        public string UFTransportador { get; set; }

        public string NomeRemetente { get; set; }

        public string CNPJRemetente { get; set; }

        public string UFRemetente { get; set; }

        public string NomeExpedidor { get; set; }

        public string CNPJExpedidor { get; set; }

        public string UFExpedidor { get; set; }

        public string NomeDestinatario { get; set; }

        public string CNPJDestinatario { get; set; }

        public string UFDestinatario { get; set; }

        public string NomeRecebedor { get; set; }

        public string CNPJRecebedor { get; set; }

        public string UFRecebedor { get; set; }

        public string NomeTomador { get; set; }

        public string CNPJTomador { get; set; }

        public string UFTomador { get; set; }

        public string CST { get; set; }
        public decimal PercentualICMS { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal BaseCalculoICMS { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorTotalReceber { get; set; }
        
        public string StatusDescricao
        {
            get
            {
                switch (Status)
                {
                    case "A":
                        return "Autorizados";
                    case "C":
                        return "Cancelados";
                    case "D":
                        return "Denegados";
                    case "S":
                        return "Em Digitação";
                    case "I":
                        return "Inutilizados";
                    case "R":
                        return "Rejeição";
                    case "P":
                        return "Pendentes";
                    case "E":
                        return "Enviados";
                    case "K":
                        return "Em Cancelamento";
                    case "L":
                        return "Em Inutilização";
                    default:
                        return "";
                }
            }
        }

        public string Status;
    }
}
