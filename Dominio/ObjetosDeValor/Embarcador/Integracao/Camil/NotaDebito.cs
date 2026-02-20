using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Camil
{
    public class NotaDebito
    {
        public string CodigoEstabelecimento { get; set; }
        public string SerieTitulo { get; set; }
        public string NumeroTitulo { get; set; }
        public string EspecieTitulo { get; set; }
        public string CodigoFornecedor { get; set; }
        public long ProtocoloIntegracao { get; set; }
        public decimal? ValorTotalTitulo { get; set; }
        public string DataTransacao { get; set; }
        public string DataEmissao { get; set; }
        public string DataVencimento { get; set; }
        public string Observacao { get; set; }
        public List<Documento> Documento { get; set; }
    }
}
