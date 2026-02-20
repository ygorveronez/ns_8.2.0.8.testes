using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira
{
    public sealed class Pedido
    {
        public int CodigoIntegracao { get; set; }
        public int IdProposta { get; set; }
        public int IdLote { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
