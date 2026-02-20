using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class ControleVisitaPessoa : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa>
    {
        public ControleVisitaPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa BuscarPorCPF(string CPF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa>();
            var result = from obj in query where obj.CPF == CPF select obj;
            return result.FirstOrDefault();
        }
    }
}
