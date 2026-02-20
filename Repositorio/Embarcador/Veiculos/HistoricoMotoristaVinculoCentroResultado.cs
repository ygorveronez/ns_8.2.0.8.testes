using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoMotoristaVinculoCentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado>
    {
        public HistoricoMotoristaVinculoCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado BuscarPorVinculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado>();
            var result = from obj in query where obj.HistoricoMotoristaVinculo.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

    }
}
