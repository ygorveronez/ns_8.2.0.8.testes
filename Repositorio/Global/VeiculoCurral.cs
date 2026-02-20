using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class VeiculoCurral : RepositorioBase<Dominio.Entidades.VeiculoCurral>
    {
        #region Construtores

        public VeiculoCurral(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public List<Dominio.Entidades.VeiculoCurral> BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCurral>()
                .Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            return query.ToList();
        }

    }
}
