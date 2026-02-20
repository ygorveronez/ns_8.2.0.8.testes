using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class AceiteLote : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.AceiteLote>
    {
        public AceiteLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.AceiteLote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.AceiteLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Avarias.AceiteLote BuscarUltimoAceite(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.AceiteLote>();
            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            result.OrderBy("Codigo descending");

            return result.FirstOrDefault();
        }
    }
}