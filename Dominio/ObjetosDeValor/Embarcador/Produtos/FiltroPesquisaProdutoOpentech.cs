using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class FiltroPesquisaProdutoOpentech
    {
        public int CodigoOperacao { get; set; }
        public string UfDestino { get; set; }
        public int CodigoApolice { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }
        public List<int> TipoCarga { get; set; }
    }
}
