using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_COMPONENTES_FRETE", EntityName = "CargaPedidoComponentesFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete", NameType = typeof(CargaPedidoComponentesFrete))]
    public class CargaPedidoComponentesFrete : Frete.ComponenteFreteBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "CCF_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public override decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "CCF_PERCENTUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDescricaoCTe", Column = "CPE_OUTRA_DESCRICAO_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string OutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "CCF_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_UTILIZAR_FORMULA_RATEIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarFormulaRateioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "CCF_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "CCF_ACRESCENTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "CCF_NAO_SOMAR_VALOR_TOTA_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberValorComponente", Column = "CCF_DESCONTAR_DO_VALOR_A_RECEBER_VALOR_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberOICMSDoComponente", Column = "CCF_DESCONTAR_DO_VALOR_A_RECEBER_ICMS_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberOICMSDoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSComponenteDestacado", Column = "CCF_VALOR_ICMS_COMPONENTE_DESTACADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSComponenteDestacado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalPrestacao", Column = "CCF_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "CCF_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public override bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirIntegralmenteContratoFreteTerceiro", Column = "CCF_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponenteFilialEmissora", Column = "CCF_COMPONENTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "CCF_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public override Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_POR_QUANTIDADE_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PorQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_TIPO_CALCULO_QUANTIDADE_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? TipoCalculoQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_RATEIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_QUANTIDADE_TOTAL_DOCUMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTotalDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_VALOR_COMPONENTE_COM_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComponenteComICMSIncluso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteFreteLiquido", Column = "CCF_DESCONTAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteFreteLiquido { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete)this.MemberwiseClone();
        }
        public virtual string DescricaoComponente
        {
            get
            {
                return TipoComponenteFrete.ObterDescricao();
            }
        }

        public virtual Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico ConvertarParaComponenteDinamico()
        {
            return new ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico
            {
                TipoComponenteFrete = this.TipoComponenteFrete,
                ComponenteFrete = this.ComponenteFrete,
                ValorComponente = this.ValorComponente,
                IncluirBaseCalculoImposto = this.IncluirBaseCalculoICMS,
                ModeloDocumentoFiscal = this.ModeloDocumentoFiscal,
                RateioFormula = this.RateioFormula,
                OutraDescricaoCTe = this.OutraDescricaoCTe,
                Percentual = this.Percentual,
                DescontarValorTotalAReceber = this.DescontarValorTotalAReceber,
                ComponenteFilialEmissora = this.ComponenteFilialEmissora,
                AcrescentaValorTotalAReceber = this.AcrescentaValorTotalAReceber,
                NaoSomarValorTotalAReceber = this.NaoSomarValorTotalAReceber,
                DescontarDoValorAReceberValorComponente = this.DescontarDoValorAReceberValorComponente,
                DescontarDoValorAReceberOICMSDoComponente = this.DescontarDoValorAReceberOICMSDoComponente,
                ValorICMSComponenteDestacado = this.ValorICMSComponenteDestacado,
                NaoSomarValorTotalPrestacao = this.NaoSomarValorTotalPrestacao,
                TipoValor = this.TipoValor,
                Moeda = this.Moeda,
                ValorCotacaoMoeda = this.ValorCotacaoMoeda,
                ValorTotalMoeda = this.ValorTotalMoeda,
                IncluirIntegralmenteContratoFreteTerceiro = this.IncluirIntegralmenteContratoFreteTerceiro,
                ValorComponenteComICMSIncluso = ValorComponenteComICMSIncluso
            };
        }

    }
}
