using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_CONTAINER_DOCUMENTO", EntityName = "CTeContainerDocumento", Name = "Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento", NameType = typeof(CTeContainerDocumento))]
    public class CTeContainerDocumento : EntidadeBase, IEquatable<CTeContainerDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "CCD_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumentoCTe), NotNull = false)]
        public virtual Enumeradores.TipoDocumentoCTe TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "CCD_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CCD_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "CCD_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedidaRateada", Column = "CCD_UNIDADE_MEDIDA_RATEADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal UnidadeMedidaRateada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerCTE", Column = "CER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerCTE ContainerCTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentosCTE", Column = "NFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.DocumentosCTE DocumentosCTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        public virtual bool Equals(CTeContainerDocumento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
