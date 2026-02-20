using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public sealed class FiltroPesquisaRelatorioProduto
    {
        public string CodigoNCM { get; set; }
        public string CodigoCEST { get; set; }
        public string CodigoBarrasEAN { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoGrupo { get; set; }
        public int CodigoMarca { get; set; }
        public int CodigoLocalArmazenamento { get; set; }
        public int CodigoGrupoImposto { get; set; }
        public int CodigoEmpresa { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public CategoriaProduto? CategoriaProduto { get; set; }
    }
}
