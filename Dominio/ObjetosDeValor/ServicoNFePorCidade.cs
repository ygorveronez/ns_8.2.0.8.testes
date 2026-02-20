namespace Dominio.ObjetosDeValor
{
    public class ServicoNFsePorCidade
    {
        public int CodigoCidade { get; set; }
        public string DescricaoCidade { get; set; }

        public int CodigoNatureza { get; set; }
        public string DescricaoNatureza { get; set; }

        public int CodigoServico { get; set; }
        public string DescricaoServico { get; set; }

        public bool Excluir { get; set; }
    }
}
