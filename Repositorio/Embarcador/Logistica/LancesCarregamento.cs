using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class LancesCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento>
    {
        #region Construtores

        public LancesCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        
        public List<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento> BuscarListaDeletarPorCentroCarregamento(int codigoCentroCarregamento, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento>();
            
            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            query = query.Where(o => !codigos.Contains(o.Codigo));
            
            return query
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento>();
            
            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            
            return query
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.LancesCarregamento BuscarPorNumeroLance(int rodada, int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.LancesCarregamento>();

            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento && rodada >= o.NumeroLanceDe && rodada <= o.NumeroLanceAte);

            return query
                .FirstOrDefault();
        }

        #endregion
    }
}
