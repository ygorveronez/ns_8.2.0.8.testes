namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaParametroOcorrencia
    {
        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoParametroOcorrencia? Tipo { get; set; }

        public bool FiltrarParametrosPeriodo { get; set; }
    }
}
