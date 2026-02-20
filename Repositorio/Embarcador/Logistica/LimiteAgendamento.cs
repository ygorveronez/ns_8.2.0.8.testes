using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class LimiteAgendamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento>
    {
        public LimiteAgendamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        
        public Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento BuscarPorGrupoPessoaCentroDescarregamento(int codigoGrupoPessoa, int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.LimiteAgendamento>();

            query = query.Where(obj => obj.GrupoPessoa.Codigo == codigoGrupoPessoa);
            query = query.Where(obj => obj.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return query.FirstOrDefault();
        }
    }
}
