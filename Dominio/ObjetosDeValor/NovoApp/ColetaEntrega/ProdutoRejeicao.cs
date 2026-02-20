namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    /// <summary>
    /// Entidade que representa um produto devolvido no novo App.
    /// </summary>
    public class ProdutoRejeicao
    {
        public int ProtocoloCargaEntregaProduto { get; set; }
        public decimal QuantidadeDevolucao { get; set; }

        /// <summary>
        /// Esse método existe para compatibilidade com serviços antigos. Com sorte, algum dia podemos deletar isso e usar
        /// apenas essa entidade.
        /// </summary>
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto ConverterParaProdutoMobileAntigo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto
            {
                Protocolo = this.ProtocoloCargaEntregaProduto,
                QuantidadeDevolucao = this.QuantidadeDevolucao,
            };
        }
    }

    
}
