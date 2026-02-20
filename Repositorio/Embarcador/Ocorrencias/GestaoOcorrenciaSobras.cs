using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class GestaoOcorrenciaSobras : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras>
    {
        public GestaoOcorrenciaSobras(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras>()
                .Where(o => o.CargaEntrega.Codigo == codigoCargaEntrega);
            return query.ToList();
        }

    }
}
