namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_MERCADO_LIVRE", EntityName = "IntegracaoMercadoLivre", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre", NameType = typeof(IntegracaoMercadoLivre))]
    public class IntegracaoMercadoLivre : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CML_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CML_URL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CML_ID", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string ID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CML_SECRET_KEY", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string SecretKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CML_NAO_ATUALIZAR_DADOS_PESSOA_EM_IMPORTACOES_DE_NOTAS_FISCAIS_OU_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimparComposicaoCargaRetiradaRotaFacility", Column = "CML_LIMPAR_COMPOSICAO_CARGA_ROTA_FACILITY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LimparComposicaoCargaRetiradaRotaFacility { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Mercado Livre";
            }
        }
    }
}
