using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaControleAvariaPallet
    {
        public System.DateTime? DataInicial { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public bool ExibirQuantidadesAvariadas { get; set; }

        public IList<int> ListaCodigoFilial { get; set; }

        public IList<int> ListaCodigoMotivoAvaria { get; set; }

        public IList<int> ListaCodigoSetor { get; set; }

        public IList<int> ListaCodigoTransportador { get; set; }

        public Enumeradores.SituacaoAvariaPallet Situacao { get; set; }
    }
}
