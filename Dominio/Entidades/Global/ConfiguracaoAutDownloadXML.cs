using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_AUT_DOWNLOAD_XML", EntityName = "ConfiguracaoAutDownloadXML", Name = "Dominio.Entidades.ConfiguracaoAutDownloadXML", NameType = typeof(ConfiguracaoAutDownloadXML))]

    public class ConfiguracaoAutDownloadXML : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "save-update")]
        public virtual ConfiguracaoEmpresa Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJCPF", Column = "CNPJ_CPF", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJCPF { get; set; }

        public virtual string CNPJCPF_Formatado
        {
            get
            {
                if(this.CNPJCPF.Length == 14)
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJCPF);
                else
                    return String.Format(@"{000\.000\.000\-00}", this.CNPJCPF);
            }
        }
    }
}
