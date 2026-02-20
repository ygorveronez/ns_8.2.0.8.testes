using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa>
    {
        public CheckListAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarOpcoesNaoPesentesNaLista(int checklist, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa>();
            var result = from obj in query
                         where
                            obj.CheckListOpcoes.Codigo == checklist
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa BuscarPorCheckListEOpcao(int checklist, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa>();
            var result = from obj in query
                         where
                            obj.CheckListOpcoes.Codigo == checklist
                            && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa> BuscarPorCheckListOpcao(int checklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa>();
            var result = from obj in query
                         where
                            obj.CheckListOpcoes.Codigo == checklist
                         select obj;

            return result.ToList();
        }
    }
}
