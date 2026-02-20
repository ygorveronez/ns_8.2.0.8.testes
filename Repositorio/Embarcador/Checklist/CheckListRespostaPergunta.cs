using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Checklist
{
    public class CheckListRespostaPergunta : RepositorioBase<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta>
    {

        public CheckListRespostaPergunta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta> BuscarPorCheckList(int checkList)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta>();
            var result = from obj in query where obj.CheckListResposta.Codigo == checkList select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta> BuscarPerguntasObrigatoriasPorCheckList(int codigoChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta>();
            query = query.Where(obj => obj.CheckListResposta.Codigo == codigoChecklist && obj.Obrigatorio == true);

            return query.ToList();
        }
    }
}
