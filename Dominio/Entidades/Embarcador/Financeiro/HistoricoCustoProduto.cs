using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_HISTORICO_CUSTO", EntityName = "HistoricoCustoProduto", Name = "Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto", NameType = typeof(HistoricoCustoProduto))]
    public class HistoricoCustoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PHC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoCustoAnterior", Column = "PHC_ULTIMO_CUSTO_ANTERIOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal UltimoCustoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoMedioAnterior", Column = "PHC_CUSTO_MEDIO_ANTERIOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal CustoMedioAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoMedioAnteriorEmpresa", Column = "PHC_CUSTO_MEDIO_ANTERIOR_EMPRESA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal CustoMedioAnteriorEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoUltimoCusto", Column = "PHC_ULTIMO_CUSTO_NOVO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal NovoUltimoCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoCustoMedio", Column = "PHC_CUSTO_MEDIO_NOVO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal NovoCustoMedio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoCustoMedioEmpresa", Column = "PHC_CUSTO_MEDIO_NOVO_EMPRESA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal NovoCustoMedioEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCompra", Column = "PHC_QUANTIDADE_COMPRA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEstoqueAnterior", Column = "PHC_QUANTIDADE_ESTOQUE_ANTERIOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeEstoqueAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "PHC_DATA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaItem", Column = "TDI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaItem DocumentoEntradaItem { get; set; }

        public virtual bool Equals(HistoricoCustoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }        
    }
}
