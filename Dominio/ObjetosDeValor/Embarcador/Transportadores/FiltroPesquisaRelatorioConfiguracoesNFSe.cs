namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public sealed class FiltroPesquisaRelatorioConfiguracoesNFSe
    {
        public int CodigoTransportador { get; set; }
        public int CodigoLocalidadePrestacaoServico { get; set; }
        public int CodigoServico { get; set; }
        public double CPFCNPJClienteTomador { get; set; }
        public int CodigoGrupoTomador { get; set; }
        public int CodigoLocalidadeTomador { get; set; }
        public int CodigoUFTomador { get; set; }
        public int CodigoTipoOperacao { get; set; }
    }
}
