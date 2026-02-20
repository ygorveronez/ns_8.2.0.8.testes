using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoReversaHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico>
    {
        #region Construtores

        public FilaCarregamentoVeiculoReversaHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico> BuscarPorFilaCarregamentoVeiculoReversa(int codigo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico>()
                .Where(o => o.FilaCarregamentoVeiculoReversa.Codigo == codigo);

            return consultaHistorico.ToList();
        }

        #endregion
    }
}
