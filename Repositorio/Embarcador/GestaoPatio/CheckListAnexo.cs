using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListAnexo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo>
    {
        #region Construtores

        public CheckListAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CheckListAnexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo> BuscarPorCheckList(int codigoCheckList)
        {
            var consultaCheckList = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCheckList);

            return consultaCheckList.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo>> BuscarPorCheckListAsync(int codigoCheckList)
        {
            var consultaCheckList = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigoCheckList);

            return consultaCheckList.ToListAsync(CancellationToken);
        }

        #endregion
    }
}
