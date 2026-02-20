using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class RouteMapRequestAPI
    {
        public int distanciaKM { get; set; }
        public string UFOrigem { get; set; }
        public string UFDestino { get; set; }
        public List<Passagem> UFPassagens { get; set; }
        public bool valido { get; set; }
        public string mensagem { get; set; }

    }
}
