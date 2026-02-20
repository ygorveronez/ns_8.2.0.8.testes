using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class ProdutoEmbarcador
    {
        public int Codigo { get; set; }

        public List<ProdutoEmbarcadorFilial> Filiais { get; set; }

        public List<ProdutoEmbarcadorFornecedor> Fornecedores { get; set; }
    }
}
