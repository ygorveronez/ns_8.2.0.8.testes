using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaRelatorioHistoricoMotorista
    {
        public int CodigoMotorista { get; set; }
        public DateTime DataHoraVinculoInicialHistoricoMotorista { get; set; }
        public DateTime DataHoraVinculoFinalHistoricoMotorista { get; set; }
        public DateTime DataInicialVinculoCentroResultado { get; set; }
        public DateTime DataFinalVinculoCentroResultado { get; set; }
    }
}
