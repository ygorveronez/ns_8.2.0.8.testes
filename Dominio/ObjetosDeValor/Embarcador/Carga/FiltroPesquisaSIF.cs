namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaSIF
    {
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CodigoSIF { get; set; }
    }
}
