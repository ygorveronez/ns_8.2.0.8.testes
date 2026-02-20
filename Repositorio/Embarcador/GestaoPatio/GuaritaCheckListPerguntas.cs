using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaCheckListPerguntas : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas>
    {
        public GuaritaCheckListPerguntas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas> BuscarPorGuarita(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas>();
            var result = from obj in query where obj.GuaritaCheckList.Guarita.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas BuscarPorCodigoEGuarita(int guarita, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas>();
            var result = from obj in query where obj.Codigo == codigo && obj.GuaritaCheckList.Codigo == guarita select obj;
            return result.FirstOrDefault();
        }
    }
}
