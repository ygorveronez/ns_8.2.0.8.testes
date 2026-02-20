using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoEFrete : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete>
    {
        public IntegracaoEFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete>();

            return consultaIntegracao.FirstOrDefault();
        }
    }
}
