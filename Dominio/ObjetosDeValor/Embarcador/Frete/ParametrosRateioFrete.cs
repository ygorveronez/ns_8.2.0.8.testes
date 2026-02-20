using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParametrosRateioFrete
    {
        public decimal ValorNotasFiscais { get; set; }
        public int Volumes { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal PesoCubado { get; set; }
        public decimal PesoPaletizado { get; set; }
        public decimal MetrosCubicos { get; set; }
        public int QuantidadeNotasFiscais { get; set; }
        public Dominio.Entidades.Embarcador.Rateio.RateioFormula FormulaRateio { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> Componentes { get; set; }
    }
}
