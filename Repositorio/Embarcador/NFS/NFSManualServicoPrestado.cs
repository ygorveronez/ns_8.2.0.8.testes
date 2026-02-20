using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class NFSManualServicoPrestado : RepositorioBase<Dominio.Entidades.Embarcador.NFS.NFSManualServicoPrestado>
    {
        public NFSManualServicoPrestado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NFS.NFSManualServicoPrestado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualServicoPrestado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
