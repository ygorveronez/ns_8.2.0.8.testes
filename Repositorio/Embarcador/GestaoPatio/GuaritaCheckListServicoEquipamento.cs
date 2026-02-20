using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaCheckListServicoEquipamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento>
    {
        public GuaritaCheckListServicoEquipamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento> BuscarPorGuaritaCheckList(int codigoGuaritaCheckList)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento>();

            query = query.Where(o => o.GuaritaCheckList.Codigo == codigoGuaritaCheckList);

            return query.ToList();
        }

        #endregion
    }
}
