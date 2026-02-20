using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_PRODUTO", DynamicUpdate = true, EntityName = "CTeProduto", Name = "Dominio.Entidades.Embarcador.Cargas.CTeProduto", NameType = typeof(CTeProduto))]
    public class CTeProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CTeProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        public virtual bool Equals(CTeProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
