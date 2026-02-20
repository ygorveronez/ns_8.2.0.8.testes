using System;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class NotaFiscalReferencia
    {
        public int Codigo { get; set; }
        public Dominio.Enumeradores.TipoDocumentoReferenciaNFe TipoDocumento { get; set; }
        public string Chave { get; set; }
        public string UF { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string CNPJEmitente { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string CPFEmitente { get; set; }
        public string Modelo { get; set; }
        public string IEEmitente { get; set; }
        public string NumeroECF { get; set; }
        public string COO { get; set; }
    }
}
