using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class AverbacaoATM : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.AverbacaoATM>
    {
        public AverbacaoATM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoATM BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoATM>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoATM BuscarPorApolice(int apolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoATM>();

            var result = from obj in query where obj.ApoliceSeguro.Codigo == apolice select obj;

            return result.FirstOrDefault();
        }
    }
}
