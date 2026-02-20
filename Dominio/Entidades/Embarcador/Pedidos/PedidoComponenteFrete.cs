using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_COMPONENTES_FRETE", EntityName = "PedidoComponenteFrete", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete", NameType = typeof(PedidoComponenteFrete))]
    public class PedidoComponenteFrete : Frete.ComponenteFreteBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "PCF_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public override decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "PCF_PERCENTUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDescricaoCTe", Column = "CPE_OUTRA_DESCRICAO_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string OutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "PCF_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "PCF_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "PCF_ACRESCENTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "PCF_NAO_SOMAR_VALOR_TOTA_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberValorComponente", Column = "PCF_DESCONTAR_DO_VALOR_A_RECEBER_VALOR_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberOICMSDoComponente", Column = "PCF_DESCONTAR_DO_VALOR_A_RECEBER_ICMS_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberOICMSDoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSComponenteDestacado", Column = "PCF_VALOR_ICMS_COMPONENTE_DESTACADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSComponenteDestacado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCF_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "PCF_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public override bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirIntegralmenteContratoFreteTerceiro", Column = "PCF_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponenteFilialEmissora", Column = "PCF_COMPONENTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "PCF_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public override Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCF_POR_QUANTIDADE_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PorQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCF_TIPO_CALCULO_QUANTIDADE_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? TipoCalculoQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_RATEIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCF_QUANTIDADE_TOTAL_DOCUMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTotalDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteFreteLiquido", Column = "PCF_DESCONTAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteFreteLiquido { get; set; }


        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete)this.MemberwiseClone();
        }
        public virtual string DescricaoComponente
        {
            get
            {
                return TipoComponenteFrete.ObterDescricao();
            }
        }

        public virtual string Descricao
        {
            get
            {
                return ComponenteFrete?.Descricao ?? string.Empty;
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
                IncluirIntegralmenteContratoFreteTerceiro = this.IncluirIntegralmenteContratoFreteTerceiro
            };
        }

    }
}
