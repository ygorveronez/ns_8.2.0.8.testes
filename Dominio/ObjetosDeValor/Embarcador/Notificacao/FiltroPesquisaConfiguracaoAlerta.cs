namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public sealed class FiltroPesquisaConfiguracaoAlerta
    {
        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoConfiguracaoAlerta? Tipo { get; set; }
    }
}
