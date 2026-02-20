using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class AverbacaoBradesco : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco>
    {
        public AverbacaoBradesco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco BuscarPorApolice(int apolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco>();

            var result = from obj in query where obj.ApoliceSeguro.Codigo == apolice select obj;

            return result.FirstOrDefault();
        }
    }
}
