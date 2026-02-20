namespace Dominio.Relatorios.Embarcador.DataSource.Produtos
{
    public class ProdutoEtiquetaCodigoBarras
    {
        public string Codigo { get; set; }

        public string Descricao { get; set; }        

        public string CodigoBarrasExtenso { get; set; }

        public byte[] CodigoBarrasImagem { get; set; }

        public string GuidFotoProduto { get; set; }

        public string DirArquivoFotoProduto { get; set; }
    }
}