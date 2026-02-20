using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDetalhamentoColetaEntregaProduto
    {
        public int? CodProduto { get; set; }
        public string NCMProduto { get; set; }
        public decimal? Valor { get; set; }
        public List<RevisaoSMDetalhamentoColetaEntregaProdutoDocumento> Documentos { get; set; }
    }
}
