using System.Linq;

namespace Repositorio
{
    public class SolicitacaoAlteracaoStatusEmpresa : RepositorioBase<Dominio.Entidades.SolicitacaoAlteracaoStatusEmpresa>, Dominio.Interfaces.Repositorios.SolicitacaoAlteracaoStatusEmpresa
    {
        public SolicitacaoAlteracaoStatusEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.SolicitacaoAlteracaoStatusEmpresa BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SolicitacaoAlteracaoStatusEmpresa>();
            var result = from obj in query where obj.Codigo == codigo && obj.EmpresaSolicitante.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }
    }
}
