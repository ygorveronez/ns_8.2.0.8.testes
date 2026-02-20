using System.Linq;

namespace Repositorio
{
    public class ProprietarioVeiculoCTe : RepositorioBase<Dominio.Entidades.ProprietarioVeiculoCTe>, Dominio.Interfaces.Repositorios.ProprietarioVeiculoCTe
    {
        public ProprietarioVeiculoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ProprietarioVeiculoCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ProprietarioVeiculoCTe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

    }
}
