using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ProdutoComissao : IEquatable<ProdutoComissao>
    {
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public string Codigo { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Peso { get; set; }
        public bool PagoPorTonelada { get; set; }
        public decimal ValorLimitePercentual { get; set; }
        public decimal ValorToneladaEntregue { get; set; }

        public bool Equals(ProdutoComissao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

 
}
