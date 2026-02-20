using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraGNRE : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE>
    {
        public ConfiguracaoFinanceiraGNRE(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE BuscarPrimeiroRegistro()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE>();

            return query.FirstOrDefault();
        }
    }
}
