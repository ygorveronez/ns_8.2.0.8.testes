using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class TempoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>
    {
        public TempoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.TempoCarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.TempoCarregamento BuscarPorModelo(int modelo, int centro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();

            var result = from obj in query where obj.ModeloVeicular.Codigo == modelo && obj.CentroCarregamento.Codigo == centro select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.TempoCarregamento BuscarModeloVeicularCarga(int codigoModeloVeicularCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>();

            var result = from obj in query where obj.ModeloVeicular.Codigo == codigoModeloVeicularCarga select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TempoCarregamento>()
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return query.ToList();
        }
    }
}
