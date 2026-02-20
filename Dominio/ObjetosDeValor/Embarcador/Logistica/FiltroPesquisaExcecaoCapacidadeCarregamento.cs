using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaExcecaoCapacidadeCarregamento
    {
        public int CodigoCentroCarregamento { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }
    }
}
