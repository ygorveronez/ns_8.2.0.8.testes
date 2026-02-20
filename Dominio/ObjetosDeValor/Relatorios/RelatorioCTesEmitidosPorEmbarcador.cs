using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesEmitidosPorEmbarcador
    {
        public int Codigo { get; set; }

        public int Numero { get; set; }

        public int QuantidadeCTe { get; set; }

        public int Serie { get; set; }

        public string Log { get; set; }

        public string Status { get; set; }

        public string AbreviacaoModeloDocumentoFiscal { get; set; }
        
        public DateTime DataEmissao { get; set; }

        public DateTime DataAutorizacao { get; set; }

        public string CPFCNPJRemetente { get; set; }

        public string Remetente { get; set; }

        public string UFRemetente { get; set; }

        public string CPFCNPJDestinatario { get; set; }

        public int CFOP { get; set; }

        public string Destinatario { get; set; }

        public string UFDestinatario { get; set; }

        public string NumeroNotaFiscal { get; set; }

        public string ChaveNotaFiscal { get; set; }

        public string SerieNotaFiscal {
            get
            {
                string chave = (this.ChaveNotaFiscal ?? string.Empty);
                return chave.Length == 44 ? chave.Substring(22, 3) : string.Empty;
            }
        }

        public decimal ValorAReceber { get; set; }

        public string CNPJTransportador { get; set; }

        public string Transportador { get; set; }

        public string UFTransportador { get; set; }

        public string Observacao { get; set; }

        public string PlacaVeiculo { get; set; }

        public int Carga { get; set; }

        public string CNF { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorAdicional { get; set; }

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
                return this.DataEmissao != null ? this.DataEmissao.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

        public string DescricaoDataAutorizacao
        {
            get
            {
                return this.DataAutorizacao != null ? this.DataAutorizacao.ToString("dd/MM/yyyy hh:ss") : "";
            }
        }

        public string CodIntegracao { get; set; }

        public string CPFCNPJTomador { get; set; }

        public string Tomador { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public string Contrato { get; set; }

    }
}