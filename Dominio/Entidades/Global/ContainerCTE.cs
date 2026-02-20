using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_CONTAINER", EntityName = "ContainerCTE", Name = "Dominio.Entidades.ContainerCTE", NameType = typeof(ContainerCTE))]
    public class ContainerCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CER_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevista", Column = "CER_DATAPREVENT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lacre1", Column = "CER_LACRE1", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Lacre1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lacre2", Column = "CER_LACRE2", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Lacre2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lacre3", Column = "CER_LACRE3", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Lacre3 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CTE_CONTAINER_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CER_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeContainerDocumento", Column = "CCD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBrutoContainer", Formula = @"(ISNULL((SELECT SUM(X.NF_PESO) FROM T_CTE_CONTAINER_DOCUMENTO CCD JOIN T_XML_NOTA_FISCAL X ON X.NF_CHAVE = CCD.CCD_CHAVE WHERE CCD.CER_CODIGO = CER_CODIGO), 0) + ISNULL((SELECT TOP(1) CONT.CTR_TARA FROM T_CONTAINER CONT WHERE CONT.CTR_CODIGO = CTR_CODIGO), 0))", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal PesoBrutoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoCubadoContainer", Formula = @"(ISNULL((SELECT SUM(X.NF_PESO_CUBADO) FROM T_CTE_CONTAINER_DOCUMENTO CCD JOIN T_XML_NOTA_FISCAL X ON X.NF_CHAVE = CCD.CCD_CHAVE WHERE CCD.CER_CODIGO = CER_CODIGO), 0) + ISNULL((SELECT TOP(1) CONT.CTR_TARA FROM T_CONTAINER CONT WHERE CONT.CTR_CODIGO = CTR_CODIGO), 0))", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal PesoCubadoContainer { get; set; }
    }
}
