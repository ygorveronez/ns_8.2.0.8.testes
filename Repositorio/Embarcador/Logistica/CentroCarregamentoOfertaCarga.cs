using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoOfertaCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga>
    {
        #region Construtores

        public CentroCarregamentoOfertaCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        
        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga> BuscarListaDeletarPorCentroCarregamento(int codigoCentroCarregamento, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga>();
            
            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            query = query.Where(o => !codigos.Contains(o.Codigo));
            
            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga>();
            
            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            
            return query
                .ToList();
        }
        #endregion
    }
}
