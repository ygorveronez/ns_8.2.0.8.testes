namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SAP_ST", EntityName = "IntegracaoSAPST", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST", NameType = typeof(IntegracaoSAPST))]
    public class IntegracaoSAPST : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoSAPST", Column = "CIS_POSSUI_INTEGRACAO_SAP_ST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSAPST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_CRIAR_ATENDIMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCriarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_CANCELAMENTO_ST", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string URLCancelamentoST { get; set; }
    }
}