namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_AMBIPAR_VALE_PEDAGIO", EntityName = "IntegracaoAmbiparValePedagio", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio", NameType = typeof(IntegracaoAmbiparValePedagio))]

    public class IntegracaoAmbiparValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CAM_URL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CAM_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CAM_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FORNECEDOR_VP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração Ambipar"; }
        }
    }
}

