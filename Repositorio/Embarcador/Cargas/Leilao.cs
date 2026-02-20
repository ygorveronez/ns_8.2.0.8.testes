using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class Leilao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Leilao>
    {
        public Leilao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.Leilao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Leilao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

    }
}
