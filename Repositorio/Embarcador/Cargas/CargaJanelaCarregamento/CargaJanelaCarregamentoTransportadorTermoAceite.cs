using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportadorTermoAceite : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorTermoAceite(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite> BuscarPorJanelasTransportador(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite>()
                .Where(obj => codigos.Contains(obj.CargaJanelaCarregamentoTransportador.Codigo));
            
            return query
                .Fetch(obj => obj.CargaJanelaCarregamentoTransportador)
                .ToList();
        }
       
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite> BuscarPorCargas(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite>()
                .Where(obj => codigos.Contains(obj.Carga.Codigo));
            
            return query
                .ToList();
        }

        public bool ExistePorCargaJanelaCarregamentoTransportador(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite>()
                .Where(obj => obj.CargaJanelaCarregamentoTransportador.Codigo == codigo);
            
            return query.Count() > 0;
        }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Públicos

        #endregion
    }
}
