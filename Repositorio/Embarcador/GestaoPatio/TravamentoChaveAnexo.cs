using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class TravamentoChaveAnexo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAnexo>
    {
        #region Construtores

        public TravamentoChaveAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAnexo> BuscarPorTravamentoChave(int codigoTravamentoChave)
        {
            var consultaTravamentoChave = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoTravamentoChave);

            return consultaTravamentoChave.ToList();
        }

        #endregion
    }
}
