using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAcompanhamentoFilaCarregamentoReversa
    {
        public List<int> CodigosCentroCarregamento { get; set; }

        public bool? LojaProximidade { get; set; }
    }
}
