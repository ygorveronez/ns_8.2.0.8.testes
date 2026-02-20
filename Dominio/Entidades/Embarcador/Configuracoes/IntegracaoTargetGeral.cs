namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_TARGET_GERAL", EntityName = "IntegracaoTargetGeral", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral", NameType = typeof(IntegracaoTargetGeral))]
    public class IntegracaoTargetGeral : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_POSSUI_INTEGRACAO_EMPRESA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracaoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLEmpresa", Column = "CIT_URL_EMPRESA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URLEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioEmpresa", Column = "CIT_USUARIO_EMPRESA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaEmpresa", Column = "CIT_SENHA_EMPRESA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaEmpresa { get; set; }
    }
}
