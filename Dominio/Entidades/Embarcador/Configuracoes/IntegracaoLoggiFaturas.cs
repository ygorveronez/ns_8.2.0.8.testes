namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_LOGGI_FATURAS", EntityName = "IntegracaoLoggiFaturas", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas", NameType = typeof(IntegracaoLoggiFaturas))]
    public class IntegracaoLoggiFaturas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIL_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIL_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIL_SENHA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Senha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMaterial", Column = "CIL_NUMERO_MATERIAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroMaterial { get; set; }
    }
}
