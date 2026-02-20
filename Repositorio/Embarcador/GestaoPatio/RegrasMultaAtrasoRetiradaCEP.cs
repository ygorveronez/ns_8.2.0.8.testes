using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class RegrasMultaAtrasoRetiradaCEP : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP>
    {
        #region Construtores

        public RegrasMultaAtrasoRetiradaCEP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP> BuscarPorRegrasMultaAtrasoRetiradaCEP(int codigoRegrasMultaAtrasoRetiradaCEP)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP>();

            query = query.Where(o => o.RegrasMultaAtrasoRetirada.Codigo == codigoRegrasMultaAtrasoRetiradaCEP);

            return query.ToList();
        }

        #endregion
    }
}
