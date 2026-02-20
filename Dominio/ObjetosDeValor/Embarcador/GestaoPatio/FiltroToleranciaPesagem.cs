using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class FiltroToleranciaPesagem
    {
        public SituacaoToleranciaPesagem Situacao { get; set; }
        public List<int> CodigosFiliais { get; set; }
        public List<int> CodigosModeloVeicular { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public int Codigo { get; set; }
    }
}
