using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoPagbem : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem>
    {
        public IntegracaoPagbem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }
    }
}
