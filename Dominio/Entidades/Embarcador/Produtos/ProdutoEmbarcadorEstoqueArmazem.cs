namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM", EntityName = "ProdutoEmbarcadorEstoqueArmazem", Name = "Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem", NameType = typeof(ProdutoEmbarcadorEstoqueArmazem))]
    public class ProdutoEmbarcadorEstoqueArmazem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialArmazem", Column = "FIA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.FilialArmazem Armazem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEA_ESTOQUE_DISPONIVEL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal EstoqueDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEA_ESTOQUE_SESSAO_ROTERIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal EstoqueSessaoRoterizacao { get; set; }
    }
}