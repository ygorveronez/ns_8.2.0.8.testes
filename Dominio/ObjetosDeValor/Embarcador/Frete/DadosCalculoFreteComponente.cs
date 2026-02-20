using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class DadosCalculoFreteComponente
    {
        public DadosCalculoFreteComponente()
        {
            ID = Guid.NewGuid().ToString();
        }

        public virtual string ID { get; set; }

        public virtual decimal ValorComponente { get; set; }
        public virtual decimal ValorComponenteMoeda { get; set; }

        public virtual decimal Percentual { get; set; }

        public virtual Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        public virtual bool DescontarValorTotalAReceber { get; set; }

        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        public virtual bool NaoSomarValorTotalAReceber { get; set; }
        public virtual bool DescontarDoValorAReceberValorComponente { get; set; }

        public virtual bool DescontarDoValorAReceberOICMSDoComponente { get; set; }

        public virtual decimal ValorICMSComponenteDestacado { get; set; }

        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        public virtual bool ComponenteComparado { get; set; }

        public virtual bool SomarComponenteFreteLiquido { get; set; }
        
        public virtual bool DescontarComponenteFreteLiquido { get; set; }

        public virtual bool UtilizarFormulaRateioCarga { get; set; }

        public virtual Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        public virtual Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        public virtual Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        public virtual string OutraDescricaoCTe { get; set; }

        public virtual Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        public virtual bool IncluirBaseCalculoICMS { get; set; }

        public virtual bool CalculoPorQuantidadeDocumentos { get; set; }

        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete TipoCalculoQuantidadeDocumentos { get; set; }

        public virtual Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalRateio { get; set; }

        public virtual int QuantidadeTotalDocumentos { get; set; }

        public virtual DadosCalculoFreteComponente Clonar()
        {
            return (DadosCalculoFreteComponente)this.MemberwiseClone();
        }

        public virtual decimal ValorComponenteParaCarga { get; set; }

        public virtual bool ComponentePorCarga { get; set; }

        /// <summary>
        /// Utilizado apenas para controle de rateio.
        /// </summary>
        public virtual decimal ValorTotalRateado { get; set; }

        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }
    }
}
