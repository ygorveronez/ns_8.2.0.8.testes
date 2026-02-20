using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO", EntityName = "DocumentoEntradaItemAbastecimento", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento", NameType = typeof(DocumentoEntradaItemAbastecimento))]
    public class DocumentoEntradaItemAbastecimento : EntidadeBase, IEquatable<DocumentoEntradaItemAbastecimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Abastecimento Abastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaItem", Column = "TDI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaItem DocumentoEntradaItem { get; set; }

        public virtual bool Equals(DocumentoEntradaItemAbastecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
