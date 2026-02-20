namespace Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga
{
    public sealed class FiltroPesquisaConfiguracaoProgramacaoCarga
    {
        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoTipoCarga { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa SituacaoAtivo { get; set; }
    }
}
