namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAreaVeiculo
    {
        public int CodigoCentroCarregamento { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoAreaVeiculo? Tipo { get; set; }
    }
}
