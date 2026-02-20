using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class PagamentoSumarizada
    {
        public int Codigo { get; set; }
        public int CodigoCTe { get; set; }
        public string ChaveCTeComplementado { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Dominio.Enumeradores.TipoDocumento? TipoDocumentoEmissao { get; set; }
        public Dominio.Enumeradores.TipoCTE? TipoCTe { get; set; }
        public int? CargaPagamento { get; set; }
        public int? ModeloDocumentoFiscal { get; set; }
        public int? LancamentoNFSManual { get; set; }
        public int? OcorrenciaPagamento { get; set; }
        public int? TipoOcorrencia { get; set; }
        public int? Fechamento { get; set; }
        public double Tomador { get; set; }
        public int? CategoriaTomador { get; set; }
        public int Empresa { get; set; }
        public bool ProvisaoPorNotaFiscal { get; set; }
        public int Filial { get; set; }
        public int? GrupoTomador { get; set; }
        public int? TipoCarga { get; set; }
        public int? TipoOperacao { get; set; }
        public int? RotaFrete { get; set; }
        public double Remetente { get; set; }
        public int? Origem { get; set; }
        public int? GrupoRemetente { get; set; }
        public int? CategoriaRemetente { get; set; }
        public double Destinatario { get; set; }
        public int? GrupoDestinatario { get; set; }
        public int? CategoriaDestinatario { get; set; }
        public double? Expedidor { get; set; }
        public double? Recebedor { get; set; }
    }
}
