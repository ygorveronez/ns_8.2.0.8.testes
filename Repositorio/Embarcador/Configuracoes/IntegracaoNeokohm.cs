using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoNeokohm : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm>
    {
        public IntegracaoNeokohm(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm>();
            return consultaIntegracao.FirstOrDefault();
        }
    }
}
