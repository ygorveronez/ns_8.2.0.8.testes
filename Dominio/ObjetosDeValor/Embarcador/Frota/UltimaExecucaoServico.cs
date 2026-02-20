using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class UltimaExecucaoServico
    {
        public int CodigoServico { get; set; }
        public string DescricaoServico { get; set; }
        public int CodigoOrdemServico { get; set; }
        public int CodigoManutencaoOrdemServico { get; set; }
        public DateTime DataUltimaExecucao { get; set; }
        public string Placa { get; set; }
        public int QuilometragemUltimaExecucao { get; set; }
        public int QuilometragemAtual { get; set; }
        public int ValidadeQuilometrosServico { get; set; }
        public int ToleranciaQuilometrosServico { get; set; }

        public int HorimetroUltimaExecucao { get; set; }
        public int HorimetroAtual { get; set; }
        public int ValidadeHorimetroServico { get; set; }
        public int ToleranciaHorimetroServico { get; set; }

        public int ValidadeDiasServico { get; set; }
        public int ToleranciaDiasServico { get; set; }
        public int TempoEstimado { get; set; }
        public Embarcador.Enumeradores.TipoServicoVeiculo TipoServico { get; set; }
        public long RowNumber { get; set; }
    }
}
