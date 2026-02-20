using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaControleTransferenciaPallet
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public IList<int> ListaCodigoFilial { get; set; }
        public IList<int> ListaCodigoSetor { get; set; }
        public IList<int> ListaCodigoTurno { get; set; }
        public Enumeradores.SituacaoTransferenciaPallet Situacao { get; set; }
    }
}
