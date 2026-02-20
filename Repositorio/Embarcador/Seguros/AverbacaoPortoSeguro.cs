using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class AverbacaoPortoSeguro : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro>
    {
        public AverbacaoPortoSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro BuscarPorApolice(int apolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro>();

            var result = from obj in query where obj.ApoliceSeguro.Codigo == apolice select obj;

            return result.FirstOrDefault();
        }
    }
}
