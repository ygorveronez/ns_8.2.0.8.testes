using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_TIPO", EntityName = "PagamentoMotoristaTipo", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo", NameType = typeof(PagamentoMotoristaTipo))]
    public class PagamentoMotoristaTipo : EntidadeBase, IEquatable<PagamentoMotoristaTipo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PMT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PMT_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarRateio", Column = "PMT_REALIZAR_RATEIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarRateio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesaFinanceira", Column = "TID_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.Despesa.TipoDespesaFinanceira TipoDespesa { get; set; }

        /// <summary>
        /// Usado para o campo Tipo da Efetivação para a PAMCARD
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "PMT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        /// <summary>
        /// Usado para o campo Tipo da Parcela para a PAMCARD
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTipo", Column = "PMT_CODIGO_INTEGRACAO_TIPO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoPagamentoMotorista", Column = "PMT_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracaoPagamentoMotorista), NotNull = true)]
        public virtual TipoIntegracaoPagamentoMotorista TipoIntegracaoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PMT_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirCancelamento", Column = "PMT_NAO_PERMITIR_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirMultiplosPagamentosAbertosParaMesmoMotorista", Column = "PMT_PERMITIR_MULTIPLOS_PAGAMENTOS_ABERTOS_PARA_MESMO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMultiplosPagamentosAbertosParaMesmoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoAutomatico", Column = "PMT_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_LANCAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_LANCAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoReversaoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTituloPagar", Column = "PMT_TITULO_PAGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloPagar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TITULO_PAGAR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoTituloPagar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PessoaSeraInformadaGeracaoPagamento", Column = "PMT_PESSOA_SERA_INFORMADA_GERACAO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PessoaSeraInformadaGeracaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamentoMotorista", Column = "PMT_TIPO_PAGAMENTO", TypeType = typeof(TipoPagamentoMotorista), NotNull = false)]
        public virtual TipoPagamentoMotorista TipoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTarifaAutomatica", Column = "PMT_GERAR_TARIFA_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTarifaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualTarifa", Column = "PMT_PERCENTUAL_TARIFA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualTarifa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TARIFA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoTarifa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_TARIFA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoReversaoTarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAssociarTipoPagamentoNoAcertoDeViagem", Column = "PMT_NAO_ASSOCIAR_TIPO_PAGAMENTO_NO_ACERTO_DE_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAssociarTipoPagamentoNoAcertoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoEntradaFixaMotorista", Column = "PMT_GERAR_MOVIMENTO_ENTRADA_FIXA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoEntradaFixaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaPagamentoMotorista", Column = "PMT_FORMA_PAGAMENTO", TypeType = typeof(FormaPagamentoMotorista), NotNull = false)]
        public virtual FormaPagamentoMotorista FormaPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmail", Column = "PMT_ASSUNTO_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string AssuntoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmail", Column = "PMT_CORPO_EMAIL", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CorpoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentoPagamentoMotorista", Column = "PMT_TIPO_MOVIMENTO_PAGAMENTO_MOTORISTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade? TipoMovimentoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTituloAPagarAoMotorista", Column = "PMT_GERAR_TITULO_A_PAGAR_AO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAPagarAoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TITULO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoTituloMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesabilitarAlteracaoDosPlanosDeContas", Column = "PMT_DESABILITAR_ALTERACAO_DOS_PLANOS_DE_CONTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarAlteracaoDosPlanosDeContas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada", Column = "PMT_HABILITAR_APROVACAO_AUTOMATICA_CASO_OPERADOR_SEJA_IGUAL_DA_ALCADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirLancarPagamentoContendoAcertoEmAndamento", Column = "PMT_PERMITIR_LANCAR_PAGAMENTO_CONTENDO_ACERTO_EM_ANDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLancarPagamentoContendoAcertoEmAndamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo", Column = "PMT_UTILIZAR_ESTE_PAGAMENTO_PARA_GERACAO_PAGAMENTO_VALOR_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirLancarComDataRetroativa", Column = "PMT_PERMITIR_LANCAR_COM_DATA_RETROATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirLancarComDataRetroativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirGerarPagamentoMotoristaTerceiro", Column = "PMT_NAO_PERMITIR_GERAR_PAGAMENTO_MOTORISTA_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarPagamentoMotoristaTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarMovimentoFinanceiroPelaDataPagamento", Column = "PMT_REALIZAR_MOVIMENTO_FINANCEIRO_PELA_DATA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarMovimentoFinanceiroPelaDataPagamento { get; set; }

        /// <summary>
        /// Usado para o campo Tipo da Efetivação para a PAMCARD para Adiantamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoEfetivacaoAdiantamento", Column = "PMT_CODIGO_INTEGRACAO_EFETIVACAO_ADIANTAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoEfetivacaoAdiantamento { get; set; }

        /// <summary>
        /// Usado para o campo Tipo da Parcela para a PAMCARD para Adiantamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTipoParcelaAdiantamento", Column = "PMT_CODIGO_INTEGRACAO_TIPO_PARCELA_ADIANTAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoTipoParcelaAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoImportacao", Column = "PMT_CODIGO_INTEGRACAO_IMPORTACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoImportacao { get; set; }

        [Obsolete("Propriedade duplicada, utilizar a PermitirLancarPagamentoContendoAcertoEmAndamento")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirGerarPagamentoSemAcertoEmAberto", Column = "PMT_NAO_PERMITIR_GERAR_PAGAMENTO_SEM_ACERTO_EM_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarPagamentoSemAcertoEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPendenciaAoMotorista", Column = "PMT_GERAR_PENDENCIA_AO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPendenciaAoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReterImpostoPagamentoMotorista", Column = "PMT_RETER_IMPOSTO_PAGAMENTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterImpostoPagamentoMotorista { get; set; }


        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string ObterTipoIntegracao
        {
            get { return TipoIntegracaoPagamentoMotorista.ObterDescricao(); }
        }

        public virtual string DescricaoFormaPagamentoMotorista
        {
            get { return FormaPagamentoMotorista.ObterDescricao(); }
        }

        public virtual bool Equals(PagamentoMotoristaTipo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}