using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoRepom : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom>
    {
        public IntegracaoRepom(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }
    }
}
