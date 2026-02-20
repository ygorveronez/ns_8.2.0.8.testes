using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraGNRERegistro : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro>
    {
        public ConfiguracaoFinanceiraGNRERegistro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> BuscarPorConfiguracao(int codigoConfiguracaoFinanceiraGNRE)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro>();

            query = query.Where(o => o.ConfiguracaoFinanceiraGNRE.Codigo == codigoConfiguracaoFinanceiraGNRE);

            return query.Fetch(o => o.CFOP)
                        .Fetch(o => o.Estado)
                        .Fetch(o => o.Pessoa)
                        .Fetch(o => o.TipoMovimento).ThenFetch(o => o.PlanoDeContaCredito)
                        .Fetch(o => o.TipoMovimento).ThenFetch(o => o.PlanoDeContaDebito)
                        .ToList();
        }
    }
}
