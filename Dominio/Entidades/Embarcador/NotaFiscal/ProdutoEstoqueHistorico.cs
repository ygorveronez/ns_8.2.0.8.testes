using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_ESTOQUE_HISTORICO", EntityName = "ProdutoEstoqueHistorico", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico", NameType = typeof(ProdutoEstoqueHistorico))]
    public class ProdutoEstoqueHistorico : EntidadeBase, IEquatable<ProdutoEstoqueHistorico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PEH_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PEH_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoMovimento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoMovimento Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PEH_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "PEH_DOCUMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "PEH_TIPO_DOCUMENTO", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Custo", Column = "PEH_CUSTO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal Custo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEstoque", Column = "PRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEstoque ProdutoEstoque { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString() + " - " + Produto.Descricao; }
        }

        public virtual bool Equals(ProdutoEstoqueHistorico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
