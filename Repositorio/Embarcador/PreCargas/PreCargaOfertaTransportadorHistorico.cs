using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaOfertaTransportadorHistorico : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico>
    {
        #region Construtores

        public PreCargaOfertaTransportadorHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico> BuscarPorOfertaTransportador(int codigo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico>()
                .Where(obj => obj.PreCargaOfertaTransportador.Codigo == codigo);

            return consultaHistorico.ToList();
        }

        #endregion
    }
}
