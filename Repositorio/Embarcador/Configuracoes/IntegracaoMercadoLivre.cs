using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMercadoLivre : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre>
    {
        public IntegracaoMercadoLivre(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre BuscarPrimeiroRegistro()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre>();

            return query.FirstOrDefault();
        }
    }
}
