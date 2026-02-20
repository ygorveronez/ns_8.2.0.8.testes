namespace Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_PESSOA_FATURA", DynamicUpdate = true, EntityName = "ConfiguracaoPessoaFatura", Name = "Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura", NameType = typeof(ConfiguracaoPessoaFatura))]
    public class ConfiguracaoPessoaFatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_GERAR_TITULO_AUTOMATICAMENTE_COM_ADIANTAMENTO_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAutomaticamenteComAdiantamentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_PERCENTUAL_ADIANTAMENTO_TITULO_AUTOMATICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdiantamentoTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_PRAZO_ADIANTAMENTO_EM_DIAS_TITULO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoAdiantamentoEmDiasTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_PERCENTUAL_SALDO_TITULO_AUTOMATICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSaldoTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_PRAZO_SALDO_EM_DIAS_TITULO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoSaldoEmDiasTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_EFETUAR_IMPRESSAO_DA_TAXA_DE_MOEDA_ESTRANGEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EfetuarImpressaoDaTaxaDeMoedaEstrangeira { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Fatura"; }
        }
    }
}
