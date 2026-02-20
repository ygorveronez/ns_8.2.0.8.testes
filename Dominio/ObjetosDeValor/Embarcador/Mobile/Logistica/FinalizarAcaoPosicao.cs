using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica
{
    public sealed class FinalizarAcaoPosicao
    {
        public bool Finalizada { get; set; }

        public List<AreaVeiculoPosicao> Locais { get; set; }
    }
}
