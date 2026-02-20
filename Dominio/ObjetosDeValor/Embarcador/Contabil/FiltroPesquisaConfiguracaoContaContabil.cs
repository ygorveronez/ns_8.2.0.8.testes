namespace Dominio.ObjetosDeValor.Embarcador.Contabil
{
    public class FiltroPesquisaConfiguracaoContaContabil
	{
		public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }
		public double CPFCNPJRemetente { get; set; }
		public double CPFCNPJDestinatario { get; set; }
        public double CPFCNPJTomador { get; set; }
		public int CodigoGrupoRemetente { get; set; }
		public int CodigoGrupoDestinatario { get; set; }
		public int CodigoGrupoTomador { get; set; }
		public int CodigoTipoOperacao { get; set; }
		public int CodigoEmpresa { get; set; }
		public int CodigoModeloDocumento { get; set; }
		public int CodigoTipoOcorrencia { get; set; }
		public int CodigoCategoriaDestinatario { get; set; }
		public int CodigoCategoriaRemetente { get; set; }
		public int CodigoCategoriaTomador { get; set; }
        public int CodigoCanalEntrega { get; set; }
		public int CodigoCanalVenda { get; set; }
		public int CodigoTipoDT { get; set; }
    }
}
