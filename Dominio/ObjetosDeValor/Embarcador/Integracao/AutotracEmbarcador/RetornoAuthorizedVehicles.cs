using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AutotracEmbarcador
{
    public class RetornoAuthorizedVehicles
    {

        public List<AuthorizedVehicles> Data { get; set; }

        public bool HasNextPage { get; set; }
        public int NumberGridLine { get; set; }

    }
}
