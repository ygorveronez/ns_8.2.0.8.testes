using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaLeilao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaLeilao>
    {
        public CargaLeilao(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Cargas.CargaLeilao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLeilao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLeilao BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLeilao>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLeilao BuscarPorLeilao(int leilao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLeilao>();
            var resut = from obj in query where obj.Leilao.Codigo == leilao select obj;
            return resut.FirstOrDefault();
        }
    }
}
