namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_LOGRISK", EntityName = "IntegracaoLogRisk", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk", NameType = typeof(IntegracaoLogRisk))]
    public class IntegracaoLogRisk : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_CNPJCliente", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CNPJCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIL_TAG_DOMINIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Dominio { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração LogRisk";
            }
        }

    }
}
