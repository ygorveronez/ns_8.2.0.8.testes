namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaMotivoRejeicaoAlteracaoPedido
    {
        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.TipoMotivoRejeicaoAlteracaoPedido Tipo { get; set; }
    }
}
