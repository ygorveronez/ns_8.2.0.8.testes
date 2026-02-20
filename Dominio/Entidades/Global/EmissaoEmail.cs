using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMISSAO_EMAIL", EntityName = "EmissaoEmail", Name = "Dominio.Entidades.EmissaoEmail", NameType = typeof(EmissaoEmail))]
    public class EmissaoEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "EME_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "EME_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumento), NotNull = false)]
        public virtual Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }
    }
}
