using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoVeiculoVinculoCentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado>
    {
        public HistoricoVeiculoVinculoCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado BuscarPorVinculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado>();
            var result = from obj in query where obj.HistoricoVeiculoVinculo.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

    }
}
