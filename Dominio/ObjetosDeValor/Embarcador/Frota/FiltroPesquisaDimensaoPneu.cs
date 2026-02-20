namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaDimensaoPneu
    {
        public string Aplicacao { get; set; }

        public int CodigoEmpresa { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }
    }
}
