using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTeSemDuplicata
    {
        public int Codigo { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }
        
        public string Status { get; set; }

        public DateTime? DataEmissao { get; set; }

        public DateTime? DataAutorizacao { get; set; }

        public string CPFCNPJRemetente { get; set; }

        public string Remetente { get; set; }

        public string CPFCNPJDestinatario { get; set; }

        public string Destinatario { get; set; }
        
        public string Observacao { get; set; }

        public string PlacaVeiculo { get; set; }

        public string Motorista { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal ValorFrete { get; set; }
        
        public decimal ValorAReceber { get; set; }

        public string ChaveCTe { get; set; }

        public string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Autorizado";
                    case "I":
                        return "Inutilizado";
                    case "C":
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

        public string DescricaoDataEmissao
        {
            get
            {
                return this.DataEmissao.HasValue ? this.DataEmissao.Value.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

        public string DescricaoDataAutorizacao
        {
            get
            {
                return this.DataAutorizacao.HasValue ? this.DataAutorizacao.Value.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

    }
}
