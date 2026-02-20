using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Camil
{
    public class Provisao
    {
        public long ProtocoloIntegracao { get; set; }
        public string CodigoEstabelecimento { get; set; }
        public string SerieTitulo { get; set; }
        public string NaturezaOperacao { get; set; }
        public string NumeroTitulo { get; set; }
        public string EspecieTitulo { get; set; }
        public string CodigoFornecedor { get; set; }
        public decimal ValorTotalTitulo { get; set; }
        public string DataTransacao { get; set; }
        public string DataEmissao { get; set; }
        public string DataVencimento { get; set; }
        public string TipoOperacao { get; set; }
        public List<NotaFiscal> NotaFiscal { get; set; }
        public List<Imposto> Imposto { get; set; }
    }
}
