using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaCheckListAnexo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo>
    {
        public GuaritaCheckListAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo> BuscarPorCheckList(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo>();
            var result = from obj in query where obj.GuaritaCheckList.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
