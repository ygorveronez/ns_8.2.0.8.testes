using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class HistoricoSituacaoCargaJanelaDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento>
    {
        #region Construtores

        public HistoricoSituacaoCargaJanelaDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion
    }
}
