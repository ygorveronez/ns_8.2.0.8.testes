using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENVIO_FTP", EntityName = "EnvioFTP", Name = "Dominio.Entidades.EnvioFTP", NameType = typeof(EnvioFTP))]
    public class EnvioFTP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "EFT_TIPO", TypeType = typeof(Enumeradores.TipoArquivoFTP), NotNull = false)]
        public virtual Enumeradores.TipoArquivoFTP Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFiltro", Column = "EFT_DATA_FILTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFiltro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "EFT_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "EFT_MENSAGEM", Type = "StringClob", NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "EFT_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusEnvioFTP), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusEnvioFTP Status { get; set; }
    }
}
