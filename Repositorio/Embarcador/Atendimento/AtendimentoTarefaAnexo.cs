using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Atendimento
{
    public class AtendimentoTarefaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo>
    {
        public AtendimentoTarefaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
