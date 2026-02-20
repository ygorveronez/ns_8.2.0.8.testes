using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_XML_NOTA_FISCAL_COMPONENTES_FRETE", EntityName = "PedidoXMLNotaFiscalComponenteFrete", Name = "Dominio.Entidades.Embarcador.Cargas.Pedidos.PedidoXMLNotaFiscalComponenteFrete", NameType = typeof(PedidoXMLNotaFiscalComponenteFrete))]
    public class PedidoXMLNotaFiscalComponenteFrete : Frete.ComponenteFreteBase, IEquatable<PedidoXMLNotaFiscalComponenteFrete>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "NFC_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public override decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "NFC_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "NFC_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "NFC_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "NFC_ACRESCENTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "NFC_NAO_SOMAR_VALOR_TOTA_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberValorComponente", Column = "NFC_DESCONTAR_DO_VALOR_A_RECEBER_VALOR_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberOICMSDoComponente", Column = "NFC_DESCONTAR_DO_VALOR_A_RECEBER_ICMS_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberOICMSDoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSComponenteDestacado", Column = "NFC_VALOR_ICMS_COMPONENTE_DESTACADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSComponenteDestacado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalPrestacao", Column = "NFC_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "CCF_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public override Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDescricaoCTe", Column = "NFC_OUTRA_DESCRICAO_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string OutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "CCF_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public override bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirIntegralmenteContratoFreteTerceiro", Column = "CCF_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponenteFilialEmissora", Column = "CCF_COMPONENTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_POR_QUANTIDADE_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PorQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_TIPO_CALCULO_QUANTIDADE_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete? TipoCalculoQuantidadeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_RATEIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_VALOR_COMPONENTE_COM_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComponenteComICMSIncluso { get; set; }

        public virtual string DescricaoComponente
        {
            get
            {
                return TipoComponenteFrete.ObterDescricao();
            }
        }

        public virtual bool Equals(PedidoXMLNotaFiscalComponenteFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico ConvertarParaComponenteDinamico()
        {
            return new ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico
            {
                TipoComponenteFrete = this.TipoComponenteFrete,
                ComponenteFrete = this.ComponenteFrete,
                ValorComponente = this.ValorComponente,
                ModeloDocumentoFiscal = this.ModeloDocumentoFiscal,
                RateioFormula = this.RateioFormula,
                OutraDescricaoCTe = this.OutraDescricaoCTe,
                IncluirBaseCalculoImposto = this.IncluirBaseCalculoICMS,
                ComponenteFilialEmissora = this.ComponenteFilialEmissora,
                Percentual = this.Percentual,
                TipoValor = this.TipoValor,
                DescontarValorTotalAReceber = this.DescontarValorTotalAReceber,
                AcrescentaValorTotalAReceber = this.AcrescentaValorTotalAReceber,
                NaoSomarValorTotalAReceber = this.NaoSomarValorTotalAReceber,
                DescontarDoValorAReceberValorComponente = this.DescontarDoValorAReceberValorComponente,
                DescontarDoValorAReceberOICMSDoComponente = this.DescontarDoValorAReceberOICMSDoComponente,
                ValorICMSComponenteDestacado = this.ValorICMSComponenteDestacado,
                NaoSomarValorTotalPrestacao = this.NaoSomarValorTotalPrestacao,
                Moeda = this.Moeda,
                ValorCotacaoMoeda = this.ValorCotacaoMoeda,
                ValorTotalMoeda = this.ValorTotalMoeda,
                IncluirIntegralmenteContratoFreteTerceiro = this.IncluirIntegralmenteContratoFreteTerceiro,
                ValorComponenteComICMSIncluso = ValorComponenteComICMSIncluso
            };
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete)this.MemberwiseClone();
        }
    }
}
