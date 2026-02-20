using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class VeiculoEstepe : RepositorioBase<Dominio.Entidades.VeiculoEstepe>
    {
        #region Construtores

        public VeiculoEstepe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.VeiculoEstepe BuscarPorCodigo(int codigo)
        {
            var consultaveiculoEstepe = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoEstepe>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return consultaveiculoEstepe;
        }

        public Dominio.Entidades.VeiculoEstepe BuscarPorEstepe(int codigoEstepe, int codigoVeiculo)
        {
            var consultaveiculoEstepe = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoEstepe>()
                .Where(o => o.Estepe.Codigo == codigoEstepe);

            if (codigoVeiculo > 0)
                consultaveiculoEstepe = consultaveiculoEstepe.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            return consultaveiculoEstepe.FirstOrDefault();
        }

        #endregion
    }
}
