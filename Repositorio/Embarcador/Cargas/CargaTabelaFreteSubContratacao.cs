using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaTabelaFreteSubContratacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao>
    {
        public CargaTabelaFreteSubContratacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.FirstOrDefault();
        }
    }
}
