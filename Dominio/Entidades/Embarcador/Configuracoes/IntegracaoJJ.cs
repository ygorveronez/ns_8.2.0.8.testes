namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_JJ", EntityName = "IntegracaoJJ", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ", NameType = typeof(IntegracaoJJ))]
    public class IntegracaoJJ : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIJ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIJ_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoAtendimento", Column = "CIJ_URL_INTEGRACAO_ATENDIMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIJ_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIJ_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }
    }
}
