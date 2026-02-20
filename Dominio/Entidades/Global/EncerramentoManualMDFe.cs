using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENCERRAMENTO_MANUAL_MDFE", EntityName = "EncerramentoManualMDFe", Name = "Dominio.Entidades.EncerramentoManualMDFe", NameType = typeof(EncerramentoManualMDFe))]
    public class EncerramentoManualMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveMDFe", Column = "EMM_CHAVE_MDFE", TypeType = typeof(string), Length = 44, NotNull = true)]
        public virtual string ChaveMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "EMM_PROTOCOLO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraEncerramento", Column = "EMM_DATA_HORA_ENCERRAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHoraEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraEvento", Column = "EMM_DATA_HORA_EVENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHoraEvento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "EMM_LOG", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Log { get; set; }
    }
}
