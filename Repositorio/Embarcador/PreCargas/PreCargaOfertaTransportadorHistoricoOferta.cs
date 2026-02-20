using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaOfertaTransportadorHistoricoOferta : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta>
    {
        #region Construtores

        public PreCargaOfertaTransportadorHistoricoOferta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta> BuscarPorOfertaTransportadorHistorico(int codigo)
        {
            var consultaHistoricoOferta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta>()
                .Where(obj => obj.PreCargaOfertaTransportadorHistorico.Codigo == codigo);

            return consultaHistoricoOferta
                .Fetch(o => o.Empresa)
                .ToList();
        }

        #endregion
    }
}
