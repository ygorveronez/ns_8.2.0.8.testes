namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaRegrasInclusaoICMS
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public double CodigoPessoa { get; set; }

        public int CodigoTipoOperacao { get; set; }
        public bool? Situacao { get; set; }
    }
}
