using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaControleCarregamento
    {
        public int CodigoCentroCarregamento { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public List<Enumeradores.SituacaoControleCarregamento> Situacoes { get; set; }
    }
}
