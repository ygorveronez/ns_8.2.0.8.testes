namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_CALISTO", EntityName = "IntegracaoCalisto", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto", NameType = typeof(IntegracaoCalisto))]
    public class IntegracaoCalisto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_URL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIC_POSSUI_INTEGRACAO_CALISTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_URL_CONTABILIZACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string URLContabilizacao { get; set; }
    }
}
