using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRoteirizacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao>
    {
        public CargaRoteirizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.FirstOrDefault();
        }
        
    }
}
