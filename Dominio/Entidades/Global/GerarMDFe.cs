using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GERAR_MDFE", EntityName = "GerarMDFe", Name = "Dominio.Entidades.GerarMDFe", NameType = typeof(GerarMDFe))]
    public class GerarMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTEs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GERAR_MDFE_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GMD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        public virtual IList<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTEs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MDF_STATUS", TypeType = typeof(Enumeradores.StatusMDFe), NotNull = true)]
        public virtual Enumeradores.StatusMDFe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "GMD_MENSAGEM", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "MDFEs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GERAR_MDFE_MDFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GMD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "MDF_CODIGO")]
        public virtual IList<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MDFEs { get; set; }
    }
}
