namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    /// <summary>
    /// Entidade que representa a divisão de capacidade de um produto no momento que uma coleta/entrega é confirmada
    /// no novo app.
    /// </summary>
    public class DivisaoProdutoConfirmacao
    {
        public int Codigo { get; set; }

        public decimal Quantidade { get; set; }

        public decimal QuantidadePlanejada { get; set; }

        /// <summary>
        /// Esse método existe para compatibilidade com serviços antigos. Com sorte, algum dia podemos deletar isso e usar
        /// apenas essa entidade.
        /// </summary>
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade ConverterParaProdutoDivisaoCapacidadeAntigo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade
            {
                DivisaoCapacidadeModeloVeicular = new Embarcador.Carga.DivisaoCapacidadeModeloVeicular
                {
                    Codigo = this.Codigo
                },
                Quantidade = this.Quantidade,
                QuantidadePlanejada = this.QuantidadePlanejada,
            };
        }
    }

    
}
