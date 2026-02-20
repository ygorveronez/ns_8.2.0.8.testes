using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    /// <summary>
    /// Entidade que representa um produto no momento que uma coleta/entrega é confirmada
    /// no novo app.
    /// </summary>
    public class ProdutoConfirmacao
    {
        public string Codigo { get; set; }

        public List<DivisaoProdutoConfirmacao> Divisoes { get; set; }

        public decimal Quantidade { get; set; }

        public decimal Temperatura { get; set; }

        public int? QuantidadeCaixasVaziasRealizada { get; set; }

        public int MotivoTemperatura { get; set; }

        public int? ImunoRealizado { get; set; }

        public int? QuantidadePorCaixaRealizada { get; set; }

        /// <summary>
        /// Esse método existe para compatibilidade com serviços antigos. Com sorte, algum dia podemos deletar isso e usar
        /// apenas essa entidade.
        /// </summary>
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto ConverterParaProdutoMobileAntigo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto
            {
                Codigo = this.Codigo,
                ProdutoDivisoesCapacidade = (from o in this.Divisoes select o.ConverterParaProdutoDivisaoCapacidadeAntigo()).ToList(),
                Quantidade = this.Quantidade,
                Temperatura = this.Temperatura,
                QuantidadeCaixasVaziasRealizada = this.QuantidadeCaixasVaziasRealizada,
                motivoTemperatura = this.MotivoTemperatura,
                ImunoRealizado = this.ImunoRealizado,
                QuantidadePorCaixaRealizada = this.QuantidadePorCaixaRealizada,
            };
        }
    }

    
}
