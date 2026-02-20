using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX
{
    public class RetornoEmissao
    {
        public string status { get; set; }
        public List<RetornoEmissaoDetalhe> details { get; set; }
    }
}
