using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.Pacote
{
    public class PacoteIdentificador
    {
        #region Atributos

        private static readonly Lazy<PacoteIdentificador> _pacoteIdentificador = new Lazy<PacoteIdentificador>(() => new PacoteIdentificador());
        private readonly object _lockIdentificador = new object();

        #endregion Atributos

        #region Construtores

        private PacoteIdentificador() { }

        public static PacoteIdentificador GetInstance()
        {
            return _pacoteIdentificador.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Task<bool> ExistePacoteIgualAsync(Repositorio.UnitOfWork unitOfWork, string identificador, CancellationToken cancellationToken)
        {
            lock (_lockIdentificador)
            {
                Repositorio.Embarcador.Cargas.Pacote repositorioPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork, cancellationToken);

                return repositorioPacote.ExistePacoteIgualAsync(identificador, cancellationToken);
            }
        }

        #endregion Métodos Públicos
    }
}
