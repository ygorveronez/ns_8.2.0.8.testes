using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODDOCFISCAL", EntityName = "ModeloDocumentoFiscal", Name = "Dominio.Entidades.ModeloDocumentoFiscal", NameType = typeof(ModeloDocumentoFiscal))]
    public class ModeloDocumentoFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOD_CODIGO")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MOD_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MOD_NUM", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MOD_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Editavel", Column = "MOD_EDITAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Editavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_UTILIZAR_NUMERACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_UTILIZAR_NUMERACAO_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeracaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Abreviacao", Column = "MOD_ABREVIACAO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Abreviacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Especie", Column = "MOD_ESPECIE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Especie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Relatorio", Column = "MOD_RELATORIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Relatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_RELATORIO_POSSUI_LOGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RelatorioPossuiLogo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_RELATORIO_TARJA_MENSAGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RelatorioTarjaMensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MOD_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_NAO_GERAR_FATURAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoGerarFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_NAO_GERAR_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoAutomatico", Column = "MOD_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomatico { get; set; }

        /// <summary>
        /// Determina o tipo de documento que deve ser emitido
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoEmissao", Column = "MOD_TIPO_DOCUMENTO_EMISSAO", TypeType = typeof(Dominio.Enumeradores.TipoDocumento), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoDocumento TipoDocumentoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbarDocumento", Column = "MOD_AVERBAR_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarISSAutomaticamente", Column = "MOD_GERAR_ISS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarISSAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoAnulacaoNotaAnulacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiferenciarMovimentosParaImpostos", Column = "MOD_DIFERENCIAR_MOVIMENTOS_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentosParaImpostos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IMPOSTO_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoImpostoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IMPOSTO_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoImpostoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IMPOSTO_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoImpostoAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IMPOSTO_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_GERAR_MOVIMENTO_AUTOMATICO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomaticoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO_ENTRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUsoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_ENTRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DIFERENCIAR_MOVIMENTOS_VALOR_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentosParaValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_GERAR_MOVIMENTO_BASE_ST_RETIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoBaseSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_BASE_ST_RETIDO_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoBaseSTRetidoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_BASE_ST_RETIDO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoBaseSTRetidoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_GERAR_MOVIMENTO_VALOR_ST_RETIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoValorSTRetido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_ST_RETIDO_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorSTRetidoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_ST_RETIDO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorSTRetidoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Series", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_DOCUMENTO_FISCAL_EMPRESA_SERIE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmpresaSerie", Column = "ESE_CODIGO")]
        public virtual ICollection<EmpresaSerie> Series { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_TIPO_DOCUMENTO_CREDITO_DEBITO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito TipoDocumentoCreditoDebito { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DIFERENCIAR_MOVIMENTOS_PIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentosParaPIS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_PIS_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoPISEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_PIS_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoPISCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_PIS_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoPISAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_PIS_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DIFERENCIAR_MOVIMENTOS_COFINS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentosParaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_COFINS_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCOFINSEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_COFINS_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCOFINSCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_COFINS_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCOFINSAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_COFINS_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DIFERENCIAR_MOVIMENTOS_IR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentosParaIR { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IR_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoIREmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IR_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoIRCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IR_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoIRAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_IR_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DIFERENCIAR_MOVIMENTOS_CSLL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentosParaCSLL { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_CSLL_EMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCSLLEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_CSLL_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCSLLCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_CSLL_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCSLLAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_CSLL_ANULACAO_NOTA_ANULACAO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoComMoedaEstrangeira", Column = "MOD_DOCUMENTO_COM_MOEDA_ESTRANGEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoComMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "MOD_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_CALCULAR_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularImpostos { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO_PROPRIA_NACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissaoPropriaNacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO_PROPRIA_NACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamentoPropriaNacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO_PROPRIA_INTERNACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissaoPropriaInternacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO_PROPRIA_INTERNACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamentoPropriaInternacional { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO_AGREGADO_NACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissaoAgregadoNacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO_AGREGADO_NACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamentoAgregadoNacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO_AGREGADO_INTERNACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissaoAgregadoInternacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO_AGREGADO_INTERNACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamentoAgregadoInternacional { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO_TERCEIRO_NACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissaoTerceiroNacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO_TERCEIRO_NACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamentoTerceiroNacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_EMISSAO_TERCEIRO_INTERNACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoEmissaoTerceiroInternacional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO_CANCELAMENTO_TERCEIRO_INTERNACIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquidoCancelamentoTerceiroInternacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DOC_TIPO_CRT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoTipoCRT { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_DESCONTAR_VALOR_DESSE_DOCUMENTO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorDesseDocumentoFatura { get; set; }

        [Obsolete("Será removido, não utilizado.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_TIPO_DOCUMENTO_MODELO_DE_IMPRESSAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoModeloImpressao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoModeloImpressao TipoDocumentoModeloImpressao { get; set; }

        public virtual string TipoDocumentoCreditoDebitoDescricao
        {
            get
            {
                switch (this.TipoDocumentoCreditoDebito)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Credito:
                        return "Crédito";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito:
                        return "Débito";
                    default:
                        return "";
                }
            }
        }
    }
}
