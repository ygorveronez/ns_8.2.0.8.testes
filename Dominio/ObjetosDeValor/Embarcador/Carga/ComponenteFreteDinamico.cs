namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class ComponenteFreteDinamico
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }
        public Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }
        public Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }
        public string OutraDescricaoCTe { get; set; }
        public Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }
        public decimal ValorComponente { get; set; }
        public decimal ValorComponenteComICMSIncluso { get; set; }
        public decimal Percentual { get; set; }
        public bool IncluirBaseCalculoImposto { get; set; }
        public bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }
        public bool ComponenteFilialEmissora { get; set; }
        public bool DescontarValorTotalAReceber { get; set; }
        public bool AcrescentaValorTotalAReceber { get; set; }
        public bool NaoSomarValorTotalAReceber { get; set; }
        public bool DescontarDoValorAReceberValorComponente { get; set; }
        public bool DescontarDoValorAReceberOICMSDoComponente { get; set; }
        public decimal ValorICMSComponenteDestacado { get; set; }
        public bool NaoSomarValorTotalPrestacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }
        public decimal? ValorTotalMoeda { get; set; }
        public decimal? ValorCotacaoMoeda { get; set; }

        public string DescricaoComponente
        {
            get
            {
                switch (this.TipoComponenteFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA:
                        return "DESCARGA";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS:
                        return "ICMS";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM:
                        return "AD VALOREM";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS:
                        return "OUTROS";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO:
                        return "PEDAGIO";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.FRETE:
                        return "ADICIONAL";
                    default:
                        return "";
                }
            }
        }
    }
}
