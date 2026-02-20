using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ManobraHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ManobraHistorico>
    {
        #region Construtores

        public ManobraHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.ManobraHistorico> BuscarPorManobra(int codigoManobra)
        {
            var consultaManobraHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraHistorico>()
                .Where(o => o.Manobra.Codigo == codigoManobra);

            return consultaManobraHistorico.ToList();
        }

        #endregion
    }
}
