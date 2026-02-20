using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSIC : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC>
    {
        public IntegracaoSIC(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC>();
            return consultaIntegracao.FirstOrDefault();
        }
    }
}
