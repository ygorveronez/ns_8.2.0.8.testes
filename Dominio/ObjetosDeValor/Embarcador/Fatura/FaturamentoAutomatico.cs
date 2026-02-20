using System;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class FaturamentoAutomatico
    {
        public int Codigo { get; set; }
        public int Viagem { get; set; }
        public int Porto { get; set; }
        public int Terminal { get; set; }
        public int QuantidadeHoras { get; set; }
        public DateTime DataAFaturar { get; set; }
        public string Fuso { get; set; }
        public DateTime DataSaidaNavio { get; set; }
    }
}
