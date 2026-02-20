using System;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VENDA_DIRETA_ITEM", EntityName = "VendaDiretaItem", Name = "Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem", NameType = typeof(VendaDiretaItem))]
    public class VendaDiretaItem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "VDI_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "VDI_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "VDI_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "VDI_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VendaDireta", Column = "VED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual VendaDireta VendaDireta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaPrecoVenda", Column = "TPV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaPrecoVenda TabelaPrecoVenda { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Produto?.Descricao ?? this.Servico?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(VendaDiretaItem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
