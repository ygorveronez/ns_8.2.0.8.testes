using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    /// <summary>
    /// Entidade que representa um pedido no momento que uma coleta/entrega é confirmada
    /// no novo app.
    /// </summary>
    public class PedidoConfirmacao
    {
        public int Codigo { get; set; }

        public List<Dominio.ObjetosDeValor.NovoApp.ColetaEntrega.ProdutoConfirmacao> Produtos { get; set; }

        /// <summary>
        /// Esse método existe para compatibilidade com serviços antigos. Com sorte, algum dia podemos deletar isso e usar
        /// apenas essa entidade.
        /// </summary>
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido ConverterParaPedidoMobileAntigo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido
            {
                Codigo = this.Codigo,
                Produtos = (from o in this.Produtos select o.ConverterParaProdutoMobileAntigo()).ToList(),
            };
        }
    }

    
}
