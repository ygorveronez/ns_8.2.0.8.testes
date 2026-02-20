namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_BBC", EntityName = "IntegracaoBBC", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC", NameType = typeof(IntegracaoBBC))]
    public class IntegracaoBBC : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_URL", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoViagem", Column = "COB_POSSUI_INTEGRACAO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_URL_VIAGEM", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_CNPJ_EMPRESA_VIAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CnpjEmpresaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_SENHA_VIAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_CLIENT_SECRET", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientSecret { get; set; }
    }
}
