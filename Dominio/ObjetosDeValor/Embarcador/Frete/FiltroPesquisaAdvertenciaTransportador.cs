using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaAdvertenciaTransportador
    {
        public int CodigoMotivo { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }
    }
}
