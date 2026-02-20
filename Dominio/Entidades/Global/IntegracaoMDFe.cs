using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_MDFE", EntityName = "IntegracaoMDFe", Name = "Dominio.Entidades.IntegracaoMDFe", NameType = typeof(IntegracaoMDFe))]
    public class IntegracaoMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "IMD_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "IMD_ARQUIVO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaCarga", Column = "IMD_NUMERO_CARGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "IMD_NUMERO_UNIDADE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "IMD_STATUS", TypeType = typeof(Enumeradores.StatusIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusIntegracao Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArquivo", Column = "IMD_TIPO_ARQUIVO", TypeType = typeof(Enumeradores.TipoArquivoIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoArquivoIntegracao TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "IMD_TIPO", TypeType = typeof(Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoIntegracaoMDFe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEncerramento", Column = "IMD_DATA_ENCERRAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouCargaEmbarcador", Column = "IMD_GEROU_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouCargaEmbarcador { get; set; }
    }
}
