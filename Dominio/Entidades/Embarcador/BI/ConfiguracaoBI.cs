namespace Dominio.Entidades.Embarcador.BI
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BI_CONFIGURACAO", EntityName = "ConfiguracaoBI", Name = "Dominio.Entidades.Embarcador.BI.ConfiguracaoBI", NameType = typeof(ConfiguracaoBI))]
    public class ConfiguracaoBI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "BIC_APPLICATIONID", TypeType = typeof(string), NotNull = true, Length =50)]
        public virtual string ApplicationId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIC_USERNAME", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string UserName { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BIC_PASSWORD", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Password { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ApplicationId;
            }
        }

    }
}
