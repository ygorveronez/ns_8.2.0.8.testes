using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fechamento
{
    public class Fechamento
    {
        public decimal ContratoMensal { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public int KmConsumoMes { get; set; }

        public int KmConsumoPeriodo { get; set; }

        public int QuantidadeCavalos { get; set; }

        public int TotalKmMes { get { return KmConsumoMes; } }

        public string Transportador { get; set; }

        public int TotalFranquiaKmMes { get; set; }

        public decimal TotalValorCTe { get; set; }

        public decimal ValorKmExcedido { get; set; }

        public int KmExcedido
        {
            get { return (KmConsumoMes > TotalFranquiaKmMes) ? KmConsumoMes - TotalFranquiaKmMes : 0; }
        }

        public int KmMesPorCavalo
        {
            get { return (this.QuantidadeCavalos > 0) ? (int)(this.KmConsumoMes / this.QuantidadeCavalos) : 0; }
        }

        public int KmRestante
        {
            get { return (KmConsumoMes < TotalFranquiaKmMes) ? TotalFranquiaKmMes - KmConsumoMes : 0; }
        }

        public decimal ValorCTePorKm
        {
            get { return (KmConsumoPeriodo > 0) ? TotalValorCTe / KmConsumoPeriodo : 0m; }
        }
    }
}
