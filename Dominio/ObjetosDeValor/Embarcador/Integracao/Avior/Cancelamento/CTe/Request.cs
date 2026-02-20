using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe
{
    public class Request
    {
        public string login { get; set; }
        public string senha { get; set; }
        public List<Cte> Ctes { get; set; }
    }
}
