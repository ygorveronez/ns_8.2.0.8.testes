namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public sealed class FiltroPesquisaConfiguracaoNFSe
    {
        public int Empresa { get; set; }
        public int LocalidadePrestacao { get; set; }
        public int Servico { get; set; }
        public int TipoOperacao { get; set; }
        public string NBS { get; set; }
    }
}