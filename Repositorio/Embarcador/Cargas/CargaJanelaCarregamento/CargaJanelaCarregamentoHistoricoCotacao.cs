using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoHistoricoCotacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao>
    {
        #region Construtores

        public CargaJanelaCarregamentoHistoricoCotacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao> BuscarPorCargaJanelaCarregamento(int codigo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == codigo);

            return consultaHistorico.ToList();
        }

        public bool ExistePorCargaJanelaCarregamento(int codigo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == codigo);

            return consultaHistorico.Any();
        }

        public List<int> BuscarCodigosCargaJanelaCarregamentoComHistorico(List<int> codigos)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao>()
                .Where(o => codigos.Contains(o.CargaJanelaCarregamento.Codigo));

            return consultaHistorico.Select(o => o.CargaJanelaCarregamento.Codigo).ToList();
        }

        #endregion
    }
}
