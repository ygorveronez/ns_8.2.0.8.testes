using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaOcorrenciaDocumentosAgrupados
    {
        public int Codigo { get; set; }
        public string NomeRemetente { get; set; }
        public string CnpjRemetente { get; set; }
        public string CnpjRemetente_Formatado {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CnpjRemetente));
            }
        }
        public string CodigoRemetente { get; set; }
        public string NomeDestinatario { get; set; }
        public string CnpjDestinatario { get; set; }
        public string CnpjDestinatario_Formatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CnpjDestinatario));
            }
        }
        public string CodigoDestinatario { get; set; }
        public string DescricaoModeloDocumento { get; set; }
        public int ModeloDocumento { get; set; }
        public int QuantidadeCargas { get; set; }
        public int QuantidadeDocumentos { get; set; }
        public decimal ValorMercadoria { get; set; }
    }
}
