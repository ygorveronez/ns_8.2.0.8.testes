using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMarfrig : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig>
    {
        public IntegracaoMarfrig(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig>();

            return consultaIntegracao.FirstOrDefault();
        }
    }
}
