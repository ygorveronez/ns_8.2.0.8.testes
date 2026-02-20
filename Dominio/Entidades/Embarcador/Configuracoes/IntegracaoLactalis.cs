namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_LACTALIS", EntityName = "IntegracaoLactalis", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLactalis", NameType = typeof(IntegracaoLactalis))]
    public class IntegracaoLactalis : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_URL_INTEGRACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_URL_AUTENTICACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_USUARIO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_SENHA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Senha { get; set; }

    }
}
