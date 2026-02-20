using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioMonitoramentoTempoVeiculo
    {
        public DateTime DataInicioEntregaInicial { get; set; }
        public DateTime DataInicioEntregaFinal { get; set; }
        public DateTime DataEntregaInicial { get; set; }
        public DateTime DataEntregaFinal { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosCarga { get; set; }
        public List<double> CodigosClienteEntrega { get; set; }
        public List<double> Recebedores { get; set; }
        public int CodigoOrigem { get; set; }
        public int CodigoDestino { get; set; }
    }
}
