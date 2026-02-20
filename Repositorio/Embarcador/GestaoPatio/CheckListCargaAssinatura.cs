using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListCargaAssinatura : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura>
    {
        public CheckListCargaAssinatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura> BuscarPorCheckList(int codigoCheckList)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura>()
                .Where(o => o.CheckList.Codigo == codigoCheckList);

            return consultaChecklist.ToList();
        }
    }
}
