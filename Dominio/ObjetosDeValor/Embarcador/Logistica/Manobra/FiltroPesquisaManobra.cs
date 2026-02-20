using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaManobra
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public List<Enumeradores.SituacaoManobra> Situacoes { get; set; }
    }
}
