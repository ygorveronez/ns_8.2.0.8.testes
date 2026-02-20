using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaRelatorioHistoricoVeiculo
    {
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoReboque { get; set; }
        public int CodigoEquipamento { get; set; }
        public bool VisualizarVinculosSubRelatorio { get; set; }
        public DateTime DataHoraVinculoInicialHistoricoVeiculo { get; set; }
        public DateTime DataHoraVinculoFinalHistoricoVeiculo { get; set; }
        public DateTime DataInicialVinculoCentroResultado { get; set; }
        public DateTime DataFinalVinculoCentroResultado { get; set; }
    }
}
