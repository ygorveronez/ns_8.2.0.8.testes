using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga
{
    public class Request
    {
        public string login { get; set; }
        public string senha { get; set; }
        public List<Frete> Fretes { get; set; }
    }
}
