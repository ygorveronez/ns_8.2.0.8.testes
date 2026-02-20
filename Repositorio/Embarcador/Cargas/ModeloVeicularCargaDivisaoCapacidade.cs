using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class ModeloVeicularCargaDivisaoCapacidade : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>
    {
        public ModeloVeicularCargaDivisaoCapacidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> BuscarPorModeloVeicularCarga(int codigoModeloVeicularCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade>();

            query = query.Where(o => o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

            return query.ToList();
        }
    }
}
