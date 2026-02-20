namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaContratoPrestacaoServicoAprovacao
    {
        public int CodigoUsuario { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoContratoPrestacaoServico? Situacao { get; set; }
    }
}
