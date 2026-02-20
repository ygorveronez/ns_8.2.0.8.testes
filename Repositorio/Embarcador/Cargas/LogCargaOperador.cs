using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class LogCargaOperador: RepositorioBase<Dominio.Entidades.Embarcador.Cargas.LogCargaOperador>
    {
        public LogCargaOperador(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public LogCargaOperador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

    }
}
