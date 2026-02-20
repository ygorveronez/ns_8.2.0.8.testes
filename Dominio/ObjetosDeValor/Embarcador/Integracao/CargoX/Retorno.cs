using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX
{
    public class Retorno
    {
        public string status { get; set; }
        public List<RetornoDetalhes> details { get; set; }
    }
}
