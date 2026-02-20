using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FiltroPesquisaOcorrenciaPatio
    {
        public int CodigoCentroCarregamento { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public List<Enumeradores.SituacaoOcorrenciaPatio> Situacoes { get; set; }

        public Enumeradores.TipoLancamento? TipoLancamento { get; set; }
    }
}
