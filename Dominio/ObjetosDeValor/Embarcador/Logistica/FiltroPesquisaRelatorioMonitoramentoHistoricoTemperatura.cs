using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura
    {
        public DateTime DataCriacaoCargaInicial { get; set; }
        public DateTime DataCriacaoCargaFinal { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public bool DuranteMonitoramento { get; set; }
        public string NumeroCarga { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa ForaFaixa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus StatusMonitoramento { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoFaixaTemperatura { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa EntregasRealizadas { get; set; }
        public List <int> CodigosFiliais { get; set; }
        public List <double> CodigosRecebedores { get; set; }
        public List<int> CodigosStatusViagem { get; set; }

    }
}
