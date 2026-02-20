using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaCapacidadeCarregamentoAdicional
    {
        public int CodigoCentroCarregamento { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }
    }
}
