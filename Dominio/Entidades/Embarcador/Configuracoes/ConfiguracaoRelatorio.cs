namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_RELATORIO", EntityName = "ConfiguracaoRelatorio", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoRelatorio", NameType = typeof(ConfiguracaoRelatorio))]
    public class ConfiguracaoRelatorio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRE_SERVICO_GERACAO_RELATORIO_HABILITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ServicoGeracaoRelatorioHabilitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRE_QUANTIDADE_RELATORIOS_PARALELO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeRelatoriosParalelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRE_UTILIZA_AUTOMACAO_ENVIO_RELATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaAutomacaoEnvioRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRE_QUANTIDADE_AUTOMACAO_ENVIO_RELATORIO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAutomacaoEnvioRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRE_EXIBIR_TODAS_CARGAS_NO_RELATORIO_DE_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTodasCargasNoRelatorioDeValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRE_RETORNAR_DESTINATARIO_NFE_QUANDO_TIPO_FOR_NFSE_NO_RELATORIO_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes { get; set; }
        public virtual string Descricao
        {
            get { return "Configuração dos relatórios"; }
        }
    }
}
