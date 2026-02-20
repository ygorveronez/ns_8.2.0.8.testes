namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_FROGGR", EntityName = "IntegracaoFroggr", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFroggr", NameType = typeof(IntegracaoFroggr))]
    public class IntegracaoFroggr : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoFroggr", Column = "CIF_POSSUI_INTEGRACAO_FROGGR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoFroggr { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_URL_INTEGRACAO_FROGGR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoFroggr { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_USUARIO_INTEGRACAO_FROGGR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioIntegracaoFroggr { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_SENHA_INTEGRACAO_FROGGR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaIntegracaoFroggr { get; set; }
    }
}