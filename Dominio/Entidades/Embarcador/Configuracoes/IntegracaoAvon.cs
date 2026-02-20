namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_AVON", EntityName = "IntegracaoAvon", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon", NameType = typeof(IntegracaoAvon))]
    public class IntegracaoAvon : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_TOKEN_PRODUCAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenProducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_TOKEN_HOMOLOGACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIA_ENTERPRISE_ID", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string EnterpriseID { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Empresa.Descricao;
            }
        }
    }
}
