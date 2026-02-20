namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_EXTRATTA_VALE_PEDAGIO", EntityName = "IntegracaoExtrattaValePedagio", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio", NameType = typeof(IntegracaoExtrattaValePedagio))]

    public class IntegracaoExtrattaValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIX_URL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIX_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJAplicacao", Column = "CIX_CNPJ_APLICACAO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJAplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CIX_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaExtratta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaExtratta TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FornecedorParceiro", Column = "CIX_FORNECEDOR_PARCEIRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FornecedorPedagioExtratta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FornecedorPedagioExtratta FornecedorParceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FORNECEDOR_VP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração Extratta"; }
        }
    }
}

