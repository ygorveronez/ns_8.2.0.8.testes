namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MARILAN", EntityName = "IntegracaoMarilan", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarilan", NameType = typeof(IntegracaoMarilan))]
    public class IntegracaoMarilan : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoMarilan", Column = "CIM_POSSUI_INTEGRACAO_MARILAN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMarilan { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLMarilan", Column = "CIM_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLMarilan { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioMarilan", Column = "CIM_USUARIO_MARILAN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioMarilan { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaMarilan", Column = "CIM_SENHA_MARILAN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaMarilan { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLMarilanChamadoOcorrencia", Column = "CIM_URL_AUTENTICACAO_CHAMADO_OCORRENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLMarilanChamadoOcorrencia { get; set; }
    }
}