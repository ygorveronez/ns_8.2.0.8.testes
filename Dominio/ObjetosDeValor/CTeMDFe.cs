using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class CTeMDFe
    {
        public int Codigo { get; set; }

        public string UFDescarregamento { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal PesoTotal { get; set; }

        public string Numero { get; set; }

        public string DataEmissao { get; set; }

        public string LocalidadeInicioPrestacao { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal PesoKgTotal { get; set; }

        public List<Dominio.ObjetosDeValor.MDFeProdutosPerigosos> ProdutosPerigosos { get; set; }
    }
}
