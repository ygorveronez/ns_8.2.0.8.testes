using System.Linq;

namespace Repositorio.Embarcador.Atendimento
{
    public class AtendimentoResposta : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta>
    {
        public AtendimentoResposta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
