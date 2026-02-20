using System.Linq;

namespace Repositorio
{
    public class ErroSefaz : RepositorioBase<Dominio.Entidades.ErroSefaz>, Dominio.Interfaces.Repositorios.ErroSefaz
    {
        public ErroSefaz(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ErroSefaz BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ErroSefaz>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ErroSefaz BuscarPorCodigoDoErro(int codigo, Dominio.Enumeradores.TipoErroSefaz tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ErroSefaz>();
            var result = from obj in query where obj.CodigoDoErro == codigo && obj.Status.Equals("A") && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }
    }
}
