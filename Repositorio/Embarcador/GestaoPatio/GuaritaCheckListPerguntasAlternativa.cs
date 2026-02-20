using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaCheckListPerguntasAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa>
    {
        public GuaritaCheckListPerguntasAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa> BuscarPorGuarita(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa>();
            var result = from obj in query where obj.GuaritaCheckListPerguntas.GuaritaCheckList.Guarita.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa BuscarPorCodigoEPergunta(int pergunta, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa>();
            var result = from obj in query where obj.Codigo == codigo && obj.GuaritaCheckListPerguntas.Codigo == pergunta select obj;
            return result.FirstOrDefault();
        }
    }
}
