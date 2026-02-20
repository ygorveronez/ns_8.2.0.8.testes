using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_OCORRENCIA", EntityName = "OcorrenciaDeNFSe", Name = "Dominio.Entidades.OcorrenciaDeNFSe", NameType = typeof(OcorrenciaDeNFSe))]
    public class OcorrenciaDeNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ONS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeCadastro", Column = "ONS_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDeCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDaOcorrencia", Column = "ONS_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ONS_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

    }
}
