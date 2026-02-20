using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_COMPONENTE_FRETE", EntityName = "ComponenteFreteTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete", NameType = typeof(ComponenteFreteTabelaFrete))]
    public class ComponenteFreteTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_INCLUIR_BASE_CALCULO_ICMS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_IGNORAR_COMPONENTE_QUANDO_VEICULO_POSSUI_TAG_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IgnorarComponenteQuandoVeiculoPossuiTagValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteTabelaFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_CALCULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoComponenteTabelaFrete? TipoCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_FORMULA_RATEIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarFormulaRateioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_COMPONENTE_COMPARADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteComparado { get; set; }

        [Obsolete("Utilizar a propriedade TipoCalculo!")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_PERCENTUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarPercentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_PERCENTUAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPercentualComponenteTabelaFrete? TipoPercentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PERCENTUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_INCLUIR_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IncluirBaseCalculo { get; set; }

        [Obsolete("Utilizar a propriedade TipoCalculo!")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_CALCULO_PESO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPesoTabelaFreteComponenteFrete? TipoCalculoPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_CALCULO_CUBAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCubagemTabelaFreteComponenteFrete? TipoCalculoCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? Cubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_EXCEDENTE_KG", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorExcedentePorKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_INFORMADO_NA_TABELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValorInformadoNaTabela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_CALCULO_DESSE_COMPONENTE_NA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarCalculoDesseComponenteNaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_UNICAO_PARA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValorUnicoParaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_FORMULA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_MINIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_ENTREGA_MINIMA", TypeType = typeof(int), NotNull = false)]
        public virtual int? EntregaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALIDAR_VALOR_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidarValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_MERCADORIA_MINIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMercadoriaMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_MERCADORIA_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMercadoriaMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALIDAR_PESO_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidarPesoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PESO_MERCADORIA_MINIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? PesoMercadoriaMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PESO_MERCADORIA_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? PesoMercadoriaMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALIDAR_DIMENSOES_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidarDimensoesMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_ALTURA_MERCADORIA_MINIMA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? AlturaMercadoriaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_ALTURA_MERCADORIA_MAXIMA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? AlturaMercadoriaMaxima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_LARGURA_MERCADORIA_MINIMA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? LarguraMercadoriaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_LARGURA_MERCADORIA_MAXIMA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? LarguraMercadoriaMaxima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_COMPRIMENTO_MERCADORIA_MINIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ComprimentoMercadoriaMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_COMPRIMENTO_MERCADORIA_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ComprimentoMercadoriaMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VOLUME_MERCADORIA_MINIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? VolumeMercadoriaMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VOLUME_MERCADORIA_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? VolumeMercadoriaMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_DIAS_ESPECIFICOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarDiasEspecificos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_SEGUNDA_FEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SegundaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TERCA_FEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? TercaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_QUARTA_FEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? QuartaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_QUINTA_FEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? QuintaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_SEXTA_FEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SextaFeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_SABADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Sabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_DOMINGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Domingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_FERIADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Feriados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_PERIODO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarPeriodoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_HORA_COLETA_INICIAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraColetaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_HORA_COLETA_FINAL", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraColetaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_ESCOLTA_ARMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EscoltaArmada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Reentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_RASTREADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Rastreado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_DESPACHO_TRANSITO_ADUANEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DespachoTransitoAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_RESTRICAO_TRAFEGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RestricaoTrafego { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_GERENCIAMENTO_RISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerenciamentoRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_MULTIPLICAR_POR_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarPorAjudante { get; set; }

        [Obsolete("Utilizar a propriedade TipoCalculo!")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_UTILIZAR_QUANTIDADE_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_DOCUMENTO_QUANTIDADE_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? TipoDocumentoQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalRestringirQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TEMPO_MULTIPLICAR_POR_HORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarPorHoraTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TEMPO_UTILIZAR_ARREDONDAMENTO_HORAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarArredondamentoHorasTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TEMPO_MINUTOS_ARREDONDAMENTO_HORAS", TypeType = typeof(int), NotNull = false)]
        public virtual int? MinutosArredondamentoHorasTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TEMPO_POSSUI_HORAS_MINIMAS_COBRANCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiHorasMinimasCobrancaTempo { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TEMPO_HORAS_MINIMAS_COBRANCA", TypeType = typeof(int), NotNull = false)]
        //public virtual int? HorasMinimasCobrancaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TEMPO_HORAS_MINIMAS_COBRANCA_TEMPO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HorasMinimasCobrancaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_VIAGEM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoViagemComponenteTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoViagemComponenteTabelaFrete? TipoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_MULTIPLICAR_POR_DESLOCAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarPorDeslocamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_MULTIPLICAR_POR_DIARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarPorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_MULTIPLICAR_POR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarPorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_SOMENTE_COM_DATA_PREVISAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SomenteComDataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_EXCEDENTE_VOLUME", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorExcedentePorVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_TIPO_CALCULO_VOLUME", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoVolumeTabelaFreteComponenteFrete? TipoCalculoVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int? Volume { get; set; }

        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "Tempos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_COMPONENTE_FRETE_TEMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ComponenteFreteTabelaFreteTempo", Column = "TFT_CODIGO")]
        public virtual IList<ComponenteFreteTabelaFreteTempo> Tempos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ComponenteFrete.Descricao;
            }
        }
    }
}
