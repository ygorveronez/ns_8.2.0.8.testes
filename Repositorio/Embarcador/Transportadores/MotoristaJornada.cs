using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public sealed class MotoristaJornada : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada>
    {
        #region Construtores

        public MotoristaJornada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada BuscarAtualPorMotorista(int codigoMotorista)
        {
            var consultaMotoristaJornada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaJornada>()
                .Where(o => o.Motorista.Codigo == codigoMotorista && o.DataFinal == null)
                .OrderByDescending(o => o.DataInicial);

            return consultaMotoristaJornada.FirstOrDefault();
        }

        #endregion
    }
}
