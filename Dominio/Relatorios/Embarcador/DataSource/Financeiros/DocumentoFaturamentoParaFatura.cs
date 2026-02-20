using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class DocumentoFaturamentoParaFatura
    {
        public int Codigo { get; set; }
        public int CodigoCarga { get; set; }
        public string Numero { get; set; }
        public int Serie { get; set; }
        public TipoDocumentoFaturamento TipoDocumento { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataEmissao { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTE { get; set; }
        public string AbreviacaoModeloDocumentoFiscal { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public MoedaCotacaoBancoCentral Moeda { get; set; }
        public decimal ValorDocumento { get; set; }
        public decimal ValorAFaturar { get; set; }
        public string Motorista { get; set; }
        public string NotaFiscal { get; set; }
        public string CST { get; set; }
        public string Container { get; set; }

        #region Propriedades com Regras

        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (TipoDocumento)
                {
                    case TipoDocumentoFaturamento.CTe:
                        return !string.IsNullOrWhiteSpace(AbreviacaoModeloDocumentoFiscal) ? AbreviacaoModeloDocumentoFiscal : "CT-e";
                    case TipoDocumentoFaturamento.Carga:
                        return "Carga";
                    default:
                        return "";
                }
            }
        }

        #endregion
    }
}
