using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoPosicaoFrotaRastreamento
    {
        public int Codigo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus MonitoramentoStatus { get; set; }
        public DateTime? MonitoramentoDataInicio { get; set; }
        public DateTime? MonitoramentoDataFim { get; set; }
        public int CargaCodigo { get; set; }
        public string CargaCodigoEmbarcador { get; set; }
        public double CargaCodigoClienteOrigem { get; set; }
        public int VeiculoCodigo { get; set; }
        public string VeiculoPlaca { get; set; }
        public string VeiculoNumeroEquipamentoRastreador { get; set; }
        public int EmpresaCodigo { get; set; }
        public string EmpresaCNPJ { get; set; }
        public string EmpresaNomeFantasia { get; set; }
        public string VeiculosVinculados { get; set; }
    }
}
