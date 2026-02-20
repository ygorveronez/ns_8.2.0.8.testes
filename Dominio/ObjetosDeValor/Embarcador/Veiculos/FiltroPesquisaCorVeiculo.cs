namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaCorVeiculo
    {
        public string Descricao  { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
        public string CodigoIntegracao { get; set; }
    }
}
