using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class SeparacaoMercadoriaAvancar
    {
        public int Codigo { get; set; }

        public int CodigoResponsavelCarregamento { get; set; }

        public int NumeroCarregadores { get; set; }

        public List<SeparacaoMercadoriaResponsavelSeparacao> ResponsaveisSeparacao { get; set; }
    }
}
