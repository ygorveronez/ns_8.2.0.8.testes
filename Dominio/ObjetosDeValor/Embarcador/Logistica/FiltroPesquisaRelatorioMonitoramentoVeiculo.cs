using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioMonitoramentoVeiculo
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> CodigosVeiculo { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoTransportador { get; set; }
        public List<int> CodigosContratoFrete { get; set; }
        public bool ApenasMonitoramentosFinalizados { get; set; }
    }
}
