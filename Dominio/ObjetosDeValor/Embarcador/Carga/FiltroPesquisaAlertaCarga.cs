namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaAlertaCarga
    {
        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoAlertaCarga? TipoAlerta { get; set; }
    }
}
