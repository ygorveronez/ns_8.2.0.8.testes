using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class ModalTransporte : RepositorioBase<Dominio.Entidades.ModalTransporte>, Dominio.Interfaces.Repositorios.ModalTransporte
    {
        public ModalTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ModalTransporte(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.ModalTransporte BuscarPorNumero(string numeroModal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModalTransporte>();
            var result = from obj in query where obj.Numero.Contains(numeroModal) select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.ModalTransporte> BuscarPorNumeroAsync(string numeroModal, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModalTransporte>();
            var result = from obj in query where obj.Numero.Contains(numeroModal) select obj;

            return result.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
