using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class CancelamentoPagamentoSumarizada
    {
        public int Codigo { get; set; }
        public int CodigoCTe { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Dominio.Enumeradores.TipoDocumento? TipoDocumentoEmissao { get; set; }
        public int? CargaPagamento { get; set; }
        public int? OcorrenciaPagamento { get; set; }
        public double Tomador { get; set; }
        public int Empresa { get; set; }
        public int? GrupoTomador { get; set; }
        public int? TipoOperacao { get; set; }
        public double Remetente { get; set; }
        public int? GrupoRemetente { get; set; }
        public double Destinatario { get; set; }
        public int? GrupoDestinatario { get; set; }
    }
}
