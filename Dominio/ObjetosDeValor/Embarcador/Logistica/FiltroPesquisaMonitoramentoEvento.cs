namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaMonitoramentoEvento
    {
        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoMonitoramentoEvento? Tipo { get; set; }
    }
}
