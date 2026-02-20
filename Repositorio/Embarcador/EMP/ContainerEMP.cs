using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.EMP
{
    public class ContainerEMP : RepositorioBase<Dominio.Entidades.Embarcador.EMP.ContainerEMP>
    {
        public ContainerEMP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.EMP.ContainerEMP BuscarPorCodigoContainer(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.EMP.ContainerEMP>();
            var result = query.Where(obj => obj.CodigoContainer == codigo);
            return result.FirstOrDefault();
        }

        public bool ExistePorNumeroOS(string numeroOS)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.EMP.ContainerEMP>();
            var result = query.Where(obj => obj.NumeroProgramacao == numeroOS);
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.EMP.ContainerEMP BuscarPendentePorNumeroOS(string numeroOS)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.EMP.ContainerEMP>()
                        .Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusContainerEMP.Pendente &&
                               obj.NumeroProgramacao == numeroOS);

            return query.FirstOrDefault();
        }
               
    }
}
