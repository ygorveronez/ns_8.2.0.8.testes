namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_DADOS", EntityName = "DadosCliente", Name = "Dominio.Entidades.DadosCliente", NameType = typeof(DadosCliente))]
    public class DadosCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        #region Dados Bancários

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "DCL_CONTA_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "DCL_CONTA_AGENCIA_DIGITO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "DCL_CONTA_NUMERO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoConta", Column = "DCL_CONTA_TIPO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoConta), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoConta? TipoConta { get; set; }

        #endregion

        #region Dados CIOT

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCartao", Column = "DCL_NUMERO_CARTAO", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string NumeroCartao { get; set; }

        #endregion

        #region Dados Diversos

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencaoICMSST", Column = "DCL_PERCENTUAL_RETENCAO_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencaoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "DCL_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailStatus", Column = "DCL_EMAIL_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string EmailStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PIS", Column = "DCL_PIS", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string PIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "DCL_NOMEFANTASIA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string NomeFantasia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TITULO_PAGAR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.TipoMovimento TipoMovimentoTituloPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCL_ARMAZENA_NOTAS_PARA_GERAR_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenaNotasParaGerarPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCL_NAO_AVERBAR_QUANDO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAverbarQuandoTerceiro { get; set; }

        #endregion

        public virtual string Descricao
        {
            get
            {
                string descricao = "DadosClientes:" + Cliente.Descricao;

                return descricao;
            }
        }
    }
}
