using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Canhotos
{
    public class InconsistenciaDigitacaoCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto>
    {
        #region Construtores

        public InconsistenciaDigitacaoCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public int BuscarProximoNumero(int codigoCanhoto)
        {
            var consultaInconsistenciaDigitacaoCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto>()
                .Where(o => o.Canhoto.Codigo == codigoCanhoto);

            int? ultimoNumero = consultaInconsistenciaDigitacaoCanhoto.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        #endregion
    }
}
