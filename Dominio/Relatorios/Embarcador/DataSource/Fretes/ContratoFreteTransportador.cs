using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ContratoFreteTransportador
    {
        public string Numero { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Transportador { get; set; }
        public string Situacao { get; set; }
        public string Observacao { get; set; }


        public int FranquiaTotalPorCavalo { get; set; }
        public int FranquiaTotalKM { get; set; }
        public decimal FranquiaContratoMensal { get; set; }
        public decimal FranquiaValorKM { get; set; }
        public decimal FranquiaValorKmExcedente { get; set; }
        public decimal KMConsumido { get; set; }
    }
}
