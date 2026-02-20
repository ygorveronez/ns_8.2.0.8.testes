using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoControleEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega>
    {
        public ConfiguracaoControleEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ConfiguracaoControleEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega ObterConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega> query = ObterConsulta();
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega> Consultar(string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega> query = ObterConsulta();
            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega> query = ObterConsulta();
            return query.Count();
        }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega> ObterConsulta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega>();
            return query;
        }

        #endregion
    }
}
