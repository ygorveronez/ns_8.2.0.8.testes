using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaCapacidadeDescarregamentoAdicional
    {
        public int CodigoCentroDescarregamento { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }
    }
}
