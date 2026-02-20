
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_FATURA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoFatura", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura", NameType = typeof(ConfiguracaoTipoOperacaoFatura))]
    public class ConfiguracaoTipoOperacaoFatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CODIGO_GERAR_TITULO_AUTOMATICAMENTE_COM_ADIANTAMENTO_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAutomaticamenteComAdiantamentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CODIGO_PERCENTUAL_ADIANTAMENTO_TITULO_AUTOMATICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdiantamentoTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CODIGO_PRAZO_ADIANTAMENTO_EM_DIAS_TITULO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoAdiantamentoEmDiasTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CODIGO_PERCENTUAL_SALDO_TITULO_AUTOMATICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSaldoTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CODIGO_PRAZO_SALDO_EM_DIAS_TITULO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoSaldoEmDiasTituloAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteFinalDeSemana", Column = "TOF_PERMITE_FINAL_SEMANA_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteFinalDeSemana { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPrazoFaturamento", Column = "TOF_TIPO_PRAZO_FATURAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? TipoPrazoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_FORMA_GERACAO_TITULO_FATURA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura? FormaGeracaoTituloFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasDePrazoFatura", Column = "TOF_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeCanhotoFisico", Column = "TOF_EXIGE_CANHOTO_FISICO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExigeCanhotoFisico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_ARMAZENA_CANHOTO_FISICO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ArmazenaCanhotoFisicoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteOcorrenciasFinalizadoras", Column = "TOF_SOMENTE_OCORRENCIA_FINALIZADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SomenteOcorrenciasFinalizadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturarSomenteOcorrenciasFinalizadoras", Column = "TOF_FATURAR_SOMENTE_OCORRENCIA_FINALIZADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FaturarSomenteOcorrenciasFinalizadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarFaturaAteReceberCanhotos", Column = "TOF_NAO_GERAR_FATURA_ATE_RECEBER_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoGerarFaturaAteReceberCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "TOF_BANCO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "TOF_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "TOF_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "TOF_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "TOF_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTomadorFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFatura", Column = "TOF_OBSERVACAO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.TipoPagamentoRecebimento FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTituloPorDocumentoFiscal", Column = "TOF_GERAR_TITULO_POR_DOCUMENTO_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloPorDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_ENVIAR_BOLETO_POR_EMAIL_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletoPorEmailAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_ENVIAR_DOCUMENTACAO_FATURAMENTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentacaoFaturamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_GERAR_TITULO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_GERAR_FATURA_AUTOMATICA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturaAutomaticaCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_GERAR_FATURAMENTO_A_VISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoAVista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmailFatura", Column = "TOF_ASSUNTO_EMAIL_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string AssuntoEmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmailFatura", Column = "TOF_CORPO_EMAIL_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CorpoEmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_GERAR_BOLETO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarBoletoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_ENVIAR_ARQUIVOS_DESCOMPACTADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarArquivosDescompactados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEmailFaturaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvioFatura", Column = "TOF_TIPO_ENVIO_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura? TipoEnvioFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoFatura", Column = "TOF_TIPO_AGRUPAMENTO_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura? TipoAgrupamentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "TOF_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasSemanaFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERACAO_DIA_SEMANA_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "TOF_DIA_SEMANA_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> DiasSemanaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasMesFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OPERACAO_DIA_MES_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "TOF_DIA_MES_FATURA", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> DiasMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_EMAIL_ENVIO_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoEnvioDocumentacao", Column = "TOF_TIPO_AGRUPAMENTO_ENVIO_DOCUMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao? TipoAgrupamentoEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_ASSUNTO_EMAIL_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssuntoDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CORPO_EMAIL_DOCUMENTACAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CorpoEmailDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailFatura", Column = "TOF_EMAIL_FATURA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_HABILITAR_PERIODO_VENCIMENTO_ESPECIFICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarPeriodoVencimentoEspecifico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacao", Column = "TOF_FORMA_ENVIO_DOCUMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao? FormaEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_EMAIL_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoEnvioDocumentacaoPorta", Column = "TOF_TIPO_AGRUPAMENTO_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao? TipoAgrupamentoEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_ASSUNTO_EMAIL_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssuntoDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_CORPO_EMAIL_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CorpoEmailDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_GERAR_FATURAMENTO_MULTIPLA_PARCELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoMultiplaParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_QUANTIDADE_PARCELAS_FATURAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string QuantidadeParcelasFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_AVISO_VENCIMETO_HABILITAR_CONFIGURACAO_PERSONALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoHabilitarConfiguracaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvisoVencimetoQunatidadeDias", Column = "TOF_AVISO_VENCIMETO_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int AvisoVencimetoQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_AVISO_VENCIMETO_ENVIAR_DIARIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoEnviarDiariamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaHabilitarConfiguracaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaQunatidadeDias", Column = "TOF_COBRANCA_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int CobrancaQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_AVISO_VENCIMETO_NAO_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoNaoEnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_COBRANCA_NAO_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaNaoEnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacaoPorta", Column = "TOF_FORMA_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao? FormaEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOF_NAO_VALIDAR_POSSUIR_ACORDO_FATURAMENTO_AVANCO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarPossuiAcordoFaturamentoAvancoCarga { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Fatura"; }
        }
    }
}