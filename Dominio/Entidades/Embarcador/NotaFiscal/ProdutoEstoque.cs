using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_ESTOQUE", EntityName = "ProdutoEstoque", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque", NameType = typeof(ProdutoEstoque))]
    public class ProdutoEstoque : EntidadeBase, IEquatable<ProdutoEstoque>
    {
        public ProdutoEstoque() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PRE_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PRE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_CUSTO_MEDIO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal CustoMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_ULTIMO_CUSTO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal UltimoCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_ESTOQUE_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal EstoqueMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_ESTOQUE_MAXIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal EstoqueMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEstoqueReservada", Column = "PRE_QUANTIDADE_ESTOQUE_RESERVADA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeEstoqueReservada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString() + " - " + Produto.Descricao; }
        }

        public virtual bool Equals(ProdutoEstoque other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
