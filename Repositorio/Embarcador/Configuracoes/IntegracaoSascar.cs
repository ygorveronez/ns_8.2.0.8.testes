using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSascar : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSascar>
    {
        public IntegracaoSascar(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSascar Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSascar>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }
    }
}
