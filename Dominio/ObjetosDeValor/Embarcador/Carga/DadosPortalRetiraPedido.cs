using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class DadosPortalRetiraPedido
    {

        public int Codigo { get; set; }

        public List<DadosPortalRetiraPedidoProduto> Produtos { get; set; }
    }
}
