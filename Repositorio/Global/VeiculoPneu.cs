using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class VeiculoPneu : RepositorioBase<Dominio.Entidades.VeiculoPneu>
    {
        #region Construtores

        public VeiculoPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.VeiculoPneu BuscarPorCodigo(int codigo)
        {
            var veiculoPneu = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoPneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return veiculoPneu;
        }

        public Dominio.Entidades.VeiculoPneu BuscarPorEixoPneu(int codigoEixoPneu, int codigoVeiculo)
        {
            var veiculoPneu = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoPneu>()
                .Where(o => o.EixoPneu.Codigo == codigoEixoPneu && o.Veiculo.Codigo == codigoVeiculo)
                .FirstOrDefault();

            return veiculoPneu;
        }

        #endregion
    }
}
