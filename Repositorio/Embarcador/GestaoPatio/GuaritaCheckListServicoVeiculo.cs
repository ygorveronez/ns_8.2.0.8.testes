using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaCheckListServicoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo>
    {
        public GuaritaCheckListServicoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo> BuscarPorGuaritaCheckList(int codigoGuaritaCheckList)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo>();

            query = query.Where(o => o.GuaritaCheckList.Codigo == codigoGuaritaCheckList);

            return query.ToList();
        }
    }
}
