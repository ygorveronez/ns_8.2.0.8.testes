namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_AVERBACAO_CLIENTES", EntityName = "ConfiguracaoAverbacaoClientes", Name = "Dominio.Entidades.ConfiguracaoAverbacaoClientes", NameType = typeof(ConfiguracaoAverbacaoClientes))]
    public class ConfiguracaoAverbacaoClientes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "save-update")]
        public virtual ConfiguracaoEmpresa Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "CAC_TIPO_CLIENTE", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoraAverbacao", Column = "CAC_INTEGRADORA_AVERBACAO", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraAverbacao? IntegradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAverbacao", Column = "CAC_CODIGO_AVERBACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAverbacao", Column = "CAC_USUARIO_AVERBACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaAverbacao", Column = "CAC_SENHA_AVERBACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenAverbacao", Column = "CAC_TOKEN_AVERBACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaizCNPJ", Column = "CAC_RAIZ_CNPJ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RaizCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAverbar", Column = "CAC_NAO_AVERBAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAverbar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTeAverbacao", Column = "CAC_AVERBA_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTEAverbacao TipoCTeAverbacao { get; set; }
    }
}
