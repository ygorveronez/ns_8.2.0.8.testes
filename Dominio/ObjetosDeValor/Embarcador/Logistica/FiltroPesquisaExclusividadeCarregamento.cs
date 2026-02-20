using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaExclusividadeCarregamento
    {
        public int CodigoCentroCarregamento { get; set; }

        public int Transportador { get; set; }

        public double Cliente { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }
    }
}

