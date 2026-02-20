using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_DOCUMENTO_CTE", EntityName = "CargaPedidoDocumentoCTe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe", NameType = typeof(CargaPedidoDocumentoCTe))]

    public class CargaPedidoDocumentoCTe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>
    {
        public CargaPedidoDocumentoCTe()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO_DOCUMENTO_CTE_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedidoDocumentoCTeComponenteFrete", Column = "PCC_CODIGO")]
        public virtual ICollection<CargaPedidoDocumentoCTeComponenteFrete> Componentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CDC_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.CargaPedido?.Carga?.CodigoCargaEmbarcador ?? string.Empty) + " - " + (this.CTe?.Descricao ?? string.Empty);
            }
        }

        public virtual bool Equals(CargaPedidoDocumentoCTe other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe)this.MemberwiseClone();
        }
    }
}
