using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestAtualizarDadosPosicionamento
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCarga { get; set; }
        public List<Dominio.ObjetosDeValor.NovoApp.Comum.CoordenadaApp> coordenadas { get; set; }
    }
}
