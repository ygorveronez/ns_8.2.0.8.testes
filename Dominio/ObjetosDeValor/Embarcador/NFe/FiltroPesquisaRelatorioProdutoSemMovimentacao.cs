using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class FiltroPesquisaRelatorioProdutoSemMovimentacao
    {
        public int Produto { get; set; }
        public int GrupoProduto { get; set; }
        public int Empresa { get; set; }
        public bool EstoqueDiferenteZero { get; set; }
        public Dominio.Enumeradores.TipoAmbiente? TipoAmbiente { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
    }
}
