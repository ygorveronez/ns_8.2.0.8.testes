using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMoniloc : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc>
    {
        public IntegracaoMoniloc(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc>();
            return consultaIntegracao.FirstOrDefault();
        }
    }
}
