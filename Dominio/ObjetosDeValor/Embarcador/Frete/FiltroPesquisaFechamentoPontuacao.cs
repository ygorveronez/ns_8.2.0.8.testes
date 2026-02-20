namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaFechamentoPontuacao
    {
        public int Ano { get; set; }

        public Enumeradores.Mes? Mes { get; set; }

        public int Numero { get; set; }

        public Enumeradores.SituacaoFechamentoPontuacao? Situacao { get; set; }
    }
}
