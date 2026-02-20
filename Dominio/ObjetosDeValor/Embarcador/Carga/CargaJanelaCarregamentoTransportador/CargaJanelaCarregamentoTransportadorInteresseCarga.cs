using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador
{
    public sealed class CargaJanelaCarregamentoTransportadorInteresseCarga
    {
        public int Codigo { get; set; }

        public int CodigoCarga { get; set; }

        public double CodigoTransportador { get; set; }

        public string CnpjTransportador { get; set; }

        public string NomeTransportador { get; set; }

        public DateTime? HorarioCarregamento { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorComponentesFrete { get; set; }

        public decimal ValorTotalFrete { get; set; }

        public DateTime? DataLance { get; set; }
        public DateTime? DataInicioCarregamento { get; set; }

        public string Veiculo { get; set; }

        public string ModeloVeicular { get; set; }

        public DateTime? DataInteresse { get; set; }

        public string UltimaPosicaoVeiculo { get; set; }

        public List<CargaJanelaCarregamentoTransportadorInteresseCargaVinculada> CargasVinculadas { get; set; } = new List<CargaJanelaCarregamentoTransportadorInteresseCargaVinculada>();
    }
}
