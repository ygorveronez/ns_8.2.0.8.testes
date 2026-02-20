namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaTipoRetornoCarga
    {
        public int CodigoTipoOperacao { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }

        public Enumeradores.RetornoCargaTipo? Tipo { get; set; }
    }
}
