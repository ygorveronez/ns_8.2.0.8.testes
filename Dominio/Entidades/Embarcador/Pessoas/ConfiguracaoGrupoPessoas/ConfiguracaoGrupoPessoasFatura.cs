namespace Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_GRUPO_PESSOAS_FATURA", DynamicUpdate = true, EntityName = "ConfiguracaoGrupoPessoasFatura", Name = "Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura", NameType = typeof(ConfiguracaoGrupoPessoasFatura))]
    public class ConfiguracaoGrupoPessoasFatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_CODIGO_GERAR_TITULO_AUTOMATICAMENTE_COM_ADIANTAMENTO_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAutomaticamenteComAdiantamentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_CODIGO_PERCENTUAL_ADIANTAMENTO_TITULO_AUTOMATICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdiantamentoTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_CODIGO_PRAZO_ADIANTAMENTO_EM_DIAS_TITULO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoAdiantamentoEmDiasTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_CODIGO_PERCENTUAL_SALDO_TITULO_AUTOMATICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSaldoTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_CODIGO_PRAZO_SALDO_EM_DIAS_TITULO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoSaldoEmDiasTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGF_EFETUAR_IMPRESSAO_DA_TAXA_DE_MOEDA_ESTRANGEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EfetuarImpressaoDaTaxaDeMoedaEstrangeira { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Fatura"; }
        }
    }
}
