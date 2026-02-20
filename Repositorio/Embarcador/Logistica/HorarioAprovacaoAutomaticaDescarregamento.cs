using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class HorarioAprovacaoAutomaticaDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento>
    {
        #region Construtores

        public HorarioAprovacaoAutomaticaDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        
        public List<Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.HorarioAprovacaoAutomaticaDescarregamento>()
                .Where(obj => obj.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            
            return query.ToList();
        }

        #endregion
    }
}
