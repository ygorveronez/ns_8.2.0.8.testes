using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class AverbacaoSenig : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig>
    {
        public AverbacaoSenig(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig BuscarPorApolice(int apolice)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoSenig>();

            var result = from obj in query where obj.ApoliceSeguro.Codigo == apolice select obj;

            return result.FirstOrDefault();
        }
    }
}
