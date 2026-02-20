using System.Linq;

namespace Repositorio
{
    public class PropostaConfiguracao : RepositorioBase<Dominio.Entidades.PropostaConfiguracao>
    {
        public PropostaConfiguracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PropostaConfiguracao BuscaPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PropostaConfiguracao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

    }
}
