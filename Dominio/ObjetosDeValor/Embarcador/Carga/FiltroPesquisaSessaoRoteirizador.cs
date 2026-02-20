namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaSessaoRoteirizador
    {
        public Enumeradores.SituacaoSessaoRoteirizador Situacao { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoUsuario { get; set; }

        public int NumeroSessao { get; set; }

    }
}
