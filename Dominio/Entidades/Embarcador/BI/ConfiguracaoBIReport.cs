using System;

namespace Dominio.Entidades.Embarcador.BI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BI_CONFIGURACAO_REPORT", EntityName = "ConfiguracaoBIReport", Name = "Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport", NameType = typeof(ConfiguracaoBIReport))]
    public class ConfiguracaoBIReport : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_REPORTID", TypeType = typeof(string), NotNull = true, Length = 50)]
        public virtual string ReportID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_WORKSPACEID", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string WorkspaceID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_CODIGO_FORMULARIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_TOKEN", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_TOKEN_AUTENTICATION", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string TokenAutentication { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_TOKEN_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TokenId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIR_TOKEN_EXPIRATION", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? TokenExpiration { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ReportID;
            }
        }

    }
}
