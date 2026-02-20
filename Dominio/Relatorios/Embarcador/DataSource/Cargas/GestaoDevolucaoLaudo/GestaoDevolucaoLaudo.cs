using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDevolucaoLaudo
{
    public class GestaoDevolucaoLaudo
    {
        public long CodigoLaudo { get; set; }
        public string FilialCargaOrigem { get; set; }
        public string FilialCidadeEstado { get; set; }
        public string FilialCNPJ { get; set; }
        public string FilialCEP { get; set; }
        public string FilialIE { get; set; }
        public string NumeroDTCarga { get; set; }
        public long NumeroLaudoCarga { get; set; }
        public string ResponsavelCarga { get; set; }
        public string DataCriacao { get; set; }
        public string HoraCriacao { get; set; }
        public string PlacaVeiculoCarga { get; set; }
        public string UsuarioCarga { get; set; }
        public string TransportadorCarga { get; set; }
    }
}
