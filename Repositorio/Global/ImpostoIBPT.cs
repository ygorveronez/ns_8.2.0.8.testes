using System.Linq;

namespace Repositorio
{
    public class ImpostoIBPT : RepositorioBase<Dominio.Entidades.ImpostoIBPT>, Dominio.Interfaces.Repositorios.ImpostoIBPT
    {
        public ImpostoIBPT(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ImpostoIBPT BuscarPorEstado(string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpostoIBPT>();

            var result = from obj in query where obj.Estado.Sigla.Equals(uf) select obj;

            return result.FirstOrDefault();
        }
    }
}
