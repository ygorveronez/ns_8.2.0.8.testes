namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_TERCEIRO", EntityName = "TipoTerceiro", Name = "Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro", NameType = typeof(TipoTerceiro))]
    public class TipoTerceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TPT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TPT_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado", Column = "TPT_ADICIONAR_PERCENTUAL_ABASTECIMENTO_ADIANTAMENTO_CARTAO_NAO_INFORMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EspecificarConfiguracaoContratoFreteTipoTerceiro", Column = "TPT_ESPECIFICAR_CONFIGURACAO_CONTRATO_FRETE_TIPO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EspecificarConfiguracaoContratoFreteTipoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_CONFIGURACAO_PERCENTUAL_ADIANTAMENTO_CONTRATO_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete? ConfiguracaoPercentualAdiantamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_CONFIGURACAO_PERCENTUAL_ABASTECIMENTO_CONTRATO_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete? ConfiguracaoPercentualAbastecimentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_CONFIGURACAO_DIAS_VENCIMENTO_ADIANTAMENTO_CONTRATO_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete? ConfiguracaoDiasVencimentoAdiantamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_CONFIGURACAO_DIAS_VENCIMENTO_SALDO_CONTRATO_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTerceiroConfiguracaoContratoFrete? ConfiguracaoDiasVencimentoSaldoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_PERCENTUAL_ADIANTAMENTO_FRETES_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAdiantamentoFretesTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_PERCENTUAL_ABASTECIMENTO_FRETES_TERCEIRO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAbastecimentoFretesTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_DIAS_VENCIMENTO_ADIANTAMENTO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasVencimentoAdiantamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_DIAS_VENCIMENTO_SALDO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasVencimentoSaldoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPT_RETER_IMPOSTOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ReterImpostosContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return Situacao ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo; }
        }
    }
}
