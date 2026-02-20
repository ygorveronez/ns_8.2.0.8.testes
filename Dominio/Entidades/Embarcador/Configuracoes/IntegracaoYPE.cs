namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_YPE", EntityName = "IntegracaoYPE", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE", NameType = typeof(IntegracaoYPE))]
    public class IntegracaoYPE: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIY_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIY_POSSUI_INTEGRACAO_YPE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIY_URL_INTEGRACAO_YPE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLintegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIY_USUARIO_INTEGRACAO_YPE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIY_SENHA_INTEGRACAO_YPE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIY_URL_INTEGRACAO_OCORRENCIA_YPE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIY_URL_INTEGRACAO_RECEBE_DADOS_LAUDO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLintegracaoRecebeDadosLaudo { get; set; }
    }
}